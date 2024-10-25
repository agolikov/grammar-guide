using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using GrammarGuide.ContentBuilder.Prompts.Exercises;
using GrammarGuide.ContentBuilder.Prompts.Guide;
using GrammarGuide.ContentBuilder.Prompts.Unit;
using GrammarGuide.Services;
using GrammarGuide.Services.Entities;
using GrammarGuide.Services.Entities.Exercises;
using GrammarGuide.Services.Services;
using GrammarGuide.Services.Settings;
using GrammarGuide.Services.Tools;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
namespace GrammarGuide.ContentBuilder.Services;

public class AppDataService(
    ContentGenerator contentGenerator,
    IMongoCollection<Guide> guides,
    ICache cache,
    IBlobService blobService,
    IBlobGenerator blobGenerator,
    IOptions<SupportedLanguages> supportedLanguages) : IAppDataService
{
    private readonly string _flagUrl = "https://flagsapi.com/{0}/flat/64.png";
    private readonly SupportedLanguages _supportedLanguages = supportedLanguages.Value;

    private async Task<Guide> GetGuideInternal(string sourceLanguage, string destinationLanguage)
    {
        var guide = await guides
            .Find(r => r.SourceLanguage == sourceLanguage && r.DestinationLanguage == destinationLanguage)
            .FirstOrDefaultAsync();

        if (guide == null)
            return null;

        foreach (var unit in guide.UnitGroups.SelectMany(unitGroup => unitGroup.Units))
            unit.GuideId = guide.Id;

        return guide;
    }

    public async Task<byte[]> GetFlag(string countryCode)
    {
        var client = new HttpClient();
        string flagUrl = string.Format(_flagUrl, countryCode);
        var data = await client.GetAsync(flagUrl);
        await using var stream = await data.Content.ReadAsStreamAsync();
        using var memoryStream = new MemoryStream();
        await stream.CopyToAsync(memoryStream);
        return memoryStream.ToArray();
    }

    public SupportedLanguages GetGuideLanguages()
    {
        return _supportedLanguages;
    }

    private async Task<Guide> GetReferenceGuide(string destinationLanguage)
    {
        var refGuide = await GetGuideInternal(AppConstants.Languages.English, destinationLanguage);
        if (refGuide != null) 
            return refGuide;
        
        var llmResult = await contentGenerator.GenerateReferenceGuide(destinationLanguage);
        await cache.Set(LLmCacheKey.GetGuideKey(AppConstants.Languages.English, destinationLanguage), llmResult);
        await guides.InsertOneAsync(llmResult.ToEntity(AppConstants.Languages.English, destinationLanguage));
        return refGuide;
    }

    private async Task<Unit> GetReferenceUnit(string destinationLanguage, int unitIndex)
    {
        var refGuide = await GetReferenceGuide(destinationLanguage);
        var unit = refGuide.UnitGroups.SelectMany(t => t.Units).FirstOrDefault(u => u.Index == unitIndex);
        if (unit == null)
            throw new Exception("Unit not found.");

        if (unit.Rules != null)
            return unit;

        var llmResult = await contentGenerator.GenerateReferenceUnit(AppConstants.Languages.English, destinationLanguage, unit.Title);
        await cache.Set(LLmCacheKey.GetUnitKey(AppConstants.Languages.English, destinationLanguage, unitIndex),
            llmResult);

        unit.Rules = llmResult.ToRulesEntity();
        unit.GuideId = refGuide.Id;
        
        foreach (var rule in unit.Rules)
        {
            if (!string.IsNullOrEmpty(rule.Example))
            {
                var audio = await blobGenerator.GenerateAudio(rule.Example, destinationLanguage, null);
                rule.ExampleAudioId = await blobService.SaveAudioBlob(audio);
            }

            if (!string.IsNullOrEmpty(rule.ExampleTranslation))
            {
                var audio = await blobGenerator.GenerateAudio(rule.ExampleTranslation, destinationLanguage, null);
                rule.ExampleTranslationAudioId = await blobService.SaveAudioBlob(audio);
            }
        }
        
        await guides.ReplaceOneAsync(g => refGuide.Id == g.Id, refGuide);
        return unit;
    }

    public async Task<Guide> GetGuide(string sourceLanguage, string destinationLanguage, bool captionsOnly)
    {
        sourceLanguage = sourceLanguage.ToLower();
        destinationLanguage = destinationLanguage.ToLower();
        
        var guide = await GetGuideInternal(sourceLanguage, destinationLanguage);
        if (guide == null)
        {
            var referenceGuide = await GetReferenceGuide(destinationLanguage);
            if (AppConstants.Languages.English == sourceLanguage)
                return referenceGuide;

            var cached =
                await cache.Get<GuideLlmResult>(LLmCacheKey.GetGuideKey(AppConstants.Languages.English,
                    destinationLanguage));

            var guideLlmResult = await contentGenerator.TranslateGuide(cached, sourceLanguage);
            await cache.Set(LLmCacheKey.GetGuideKey(sourceLanguage, destinationLanguage), guideLlmResult);

            guide = guideLlmResult.ToEntity(sourceLanguage, destinationLanguage);
            await guides.InsertOneAsync(guide);
        }

        if (!captionsOnly) 
            return guide;
        
        foreach (var ug in guide.UnitGroups)
        {
            foreach (var g in ug.Units)
            {
                g.Rules = null;
                g.Exercises = null;
            }
        }

        return guide;
    }

    public async Task<Unit> GetUnit(string sourceLanguage, string destinationLanguage, int unitIndex)
    {
        sourceLanguage = sourceLanguage.ToLower();
        destinationLanguage = destinationLanguage.ToLower();

        var guide = await GetGuideInternal(sourceLanguage, destinationLanguage);
        if (guide == null)
            throw new Exception("Not found.");

        var unit = guide.UnitGroups.FirstOrDefault(u => u.Units.Any(x => x.Index == unitIndex))?.Units
            .FirstOrDefault(x => x.Index == unitIndex);

        if (unit == null)
            throw new Exception("Unit not found.");

        if (unit.Rules != null)
            return unit;
        
        var refUnit = await GetReferenceUnit(destinationLanguage, unitIndex);

        var refUnitCached =
            await cache.Get<UnitRulesLlmResult>(LLmCacheKey.GetUnitKey(AppConstants.Languages.English,
                destinationLanguage, unitIndex));

        UnitRulesLlmResult guideLlmResult = refUnitCached;
        if (sourceLanguage != "english") 
            guideLlmResult = await contentGenerator.TranslateUnit(refUnitCached, sourceLanguage);
        
        await cache.Set(LLmCacheKey.GetUnitKey(sourceLanguage, destinationLanguage, unitIndex), guideLlmResult);

        var unitEntity = guideLlmResult.ToRulesEntity();

        foreach (var rule in unitEntity)
        {
            var refRule = refUnit.Rules.FirstOrDefault(r => r.Index == rule.Index);
            rule.Example = refRule.Example;
            rule.ImageId = refRule.ImageId;
            rule.ExampleAudioId = refRule.ExampleAudioId;
            if (!string.IsNullOrEmpty(rule.ExampleTranslation))
            {
                var audio = await blobGenerator.GenerateAudio(rule.ExampleTranslation, sourceLanguage, null);
                var audioId = await blobService.SaveAudioBlob(audio);
                rule.ExampleTranslationAudioId = audioId;
            }
        }

        unit.Rules = unitEntity;
        await guides.ReplaceOneAsync(g => guide.Id == g.Id, guide);
        return unit;
    }

    public async Task<Unit> AddExerciseSet(
        string sourceLanguage,
        string destinationLanguage,
        int unitIndex)
    {
        sourceLanguage = sourceLanguage.ToLower();
        destinationLanguage = destinationLanguage.ToLower();

        var guide = await GetGuide(sourceLanguage, destinationLanguage, false);
        var unit = GetUnitByIndex(guide, unitIndex);

        var ruleExerciseMap = GetRandomizedRuleExerciseMap(unit.Rules.Count);

        foreach (var rule in unit.Rules)
        {
            var type = ruleExerciseMap[rule.Index];
            await AddExercise(sourceLanguage, destinationLanguage, unit, rule, type);
            await guides.ReplaceOneAsync(g => guide.Id == g.Id, guide);
        }

        return unit;
    }

    public async Task<Unit> AddExercise(string sourceLanguage, string destinationLanguage, int unitIndex, int ruleIndex, ExerciseType? type)
    {
        sourceLanguage = sourceLanguage.ToLower();
        destinationLanguage = destinationLanguage.ToLower();
        var guide = await GetGuide(sourceLanguage, destinationLanguage, false);
        var unit = GetUnitByIndex(guide, unitIndex);
        var rule = GetRuleByIndex(unit, ruleIndex);
        
        await AddExercise(sourceLanguage, destinationLanguage, unit, rule, type ?? GetRandomType());
        
        await guides.ReplaceOneAsync(g => guide.Id == g.Id, guide);
        return unit;
    }

    public async Task UpdateImageForRule(string sourceLanguage, string destinationLanguage, int unitIndex, int ruleIndex)
    {
        var guide = await GetGuideInternal(sourceLanguage, destinationLanguage);
        var unit = GetUnitByIndex(guide, unitIndex);
        var rule = GetRuleByIndex(unit, ruleIndex);

        if (!string.IsNullOrEmpty(rule.Example))
        {
            var image = await contentGenerator.GenerateImage(rule.Example, destinationLanguage);
            rule.ImageId = await blobService.SaveImageBlob(image);
        }

        await guides.ReplaceOneAsync(g => guide.Id == g.Id, guide);
    }

    private async Task<Unit> AddExercise(
        string sourceLanguage,
        string destinationLanguage,
        Unit unit, 
        Rule rule,
        ExerciseType exerciseType)
    {
        int questionsCount = 3;
        
        var llmResult = await contentGenerator.GenerateExercise(sourceLanguage, destinationLanguage, unit, rule,
            exerciseType,
            questionsCount);

        var exercise = ((IExerciseResult)llmResult).ToEntity();
        unit.Exercises ??= [];

        exercise.Index = unit.Exercises.Count + 1;
        for (int i = 1; i <= unit.Exercises.Count; ++i)
        {
            if (unit.Exercises[i - 1].Index == i)
                continue;
            exercise.Index = i;
            break;
        }

        exercise.RuleIndex = rule.Index;
        exercise.UnitIndex = unit.Index;
        unit.Exercises.Add(exercise);

        return unit;
    }

    private ExerciseType GetRandomType()
    {
        ExerciseType[] values =
        {
            ExerciseType.PickAnOption,
            ExerciseType.PickAnOrder,
            ExerciseType.TextWithOptions,
            ExerciseType.YesNoQuestion
        };
        ArrayTools.Shuffle(values);
        return values[0];
    }

    private Dictionary<int, ExerciseType> GetRandomizedRuleExerciseMap(int rulesCount)
    {
        ExerciseType[] values =
        {
            ExerciseType.PickAnOption,
            ExerciseType.PickAnOrder,
            ExerciseType.TextWithOptions,
            ExerciseType.YesNoQuestion
        };

        ArrayTools.Shuffle(values);
        var d = new Dictionary<int, ExerciseType>();
        for (int i = 0; i < rulesCount; ++i)
            d.Add(i, values[i % values.Length]);
        
        return d;
    }

    private Unit GetUnitByIndex(Guide guide, int unitIndex)
    {
        var unit = guide.UnitGroups.SelectMany(ug => ug.Units)
            .FirstOrDefault(u => u.Index == unitIndex);
        if (unit == null)
            throw new Exception("Unit not found.");

        return unit;
    }

    private Rule GetRuleByIndex(Unit unit, int ruleIndex)
    {
        var rule = unit.Rules.FirstOrDefault(r => r.Index == ruleIndex);
        if (rule == null)
            throw new Exception("Rule not found.");
        return rule;
    }

    private bool CanAddImageToExercise(Exercise exercise)
    {
        foreach (var part in exercise.Parts)
            if (part.ImageId == null && !string.IsNullOrEmpty(part.Question))
                return true;

        return false;
    }

    private async Task GenerateNewImageForExercise(Exercise exercise, string language)
    {
        if (!CanAddImageToExercise(exercise))
            return;
        
        var r = new Random();
        do
        {
            int imageIndex = r.Next(0, exercise.Parts.Count);
            var part = exercise.Parts[imageIndex];
            if (part.ImageId != null)
                continue;

            var image = await contentGenerator.GenerateImage(part.Question, language);
            var imageId = await blobService.SaveImageBlob(image);
            part.ImageId = imageId;
            break;
        } while (true);
    }
}