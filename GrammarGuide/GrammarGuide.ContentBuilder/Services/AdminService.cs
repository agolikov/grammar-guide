using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GrammarGuide.Services;
using GrammarGuide.Services.Entities;
using GrammarGuide.Services.Entities.Exercises;
using GrammarGuide.Services.Services;
using MongoDB.Driver;

namespace GrammarGuide.ContentBuilder.Services;

public class AdminService(
    IBlobService blobService,
    IMongoDatabase database,
    string guidesCollectionName,
    ContentGenerator contentGenerator,
    ICache cache,
    IBlobGenerator blobGenerator)
    : IAdminService
{
    private readonly IMongoCollection<Guide> _guides = database.GetCollection<Guide>(guidesCollectionName);

    private async Task<Guide> GetGuideInternal(string sourceLanguage, string destinationLanguage)
    {
        var guide = await _guides
            .Find(r => r.SourceLanguage == sourceLanguage && r.DestinationLanguage == destinationLanguage)
            .FirstOrDefaultAsync();

        if (guide == null)
            return null;

        foreach (var unitGroup in guide.UnitGroups)
        foreach (var unit in unitGroup.Units)
            unit.GuideId = guide.Id;

        return guide;
    }
    
    public async Task UpdateAudioForRule(string sourceLanguage, string destinationLanguage, int unitIndex, int ruleIndex)
    {
        var guide = await GetGuideInternal(sourceLanguage, destinationLanguage);
        var unit = GetUnitByIndex(guide, unitIndex);
        var rule = GetRuleByIndex(unit, ruleIndex);
        if (!string.IsNullOrEmpty(rule.Example))
        {
            var audio = await blobGenerator.GenerateAudio(rule.Example, destinationLanguage, null);
            rule.ExampleAudioId = await blobService.SaveAudioBlob(audio);
        }

        if (!string.IsNullOrEmpty(rule.ExampleTranslation))
        {
            var audio = await blobGenerator.GenerateAudio(rule.ExampleTranslation, sourceLanguage, null);
            rule.ExampleTranslationAudioId = await blobService.SaveAudioBlob(audio);
        }

        await _guides.ReplaceOneAsync(g => guide.Id == g.Id, guide);
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

        await _guides.ReplaceOneAsync(g => guide.Id == g.Id, guide);
    }

    public async Task UpdateImageForExercisePart(string sourceLanguage, string destinationLanguage, int unitIndex, int exerciseIndex, int partIndex)
    {
        var guide = await GetGuideInternal(sourceLanguage, destinationLanguage);
        var unit = GetUnitByIndex(guide, unitIndex);
        var exercise = GetExerciseByIndex(unit, exerciseIndex);
        var part = GetExercisePartByIndex(exercise, partIndex);

        if (!string.IsNullOrEmpty(part.Question))
        {
            var image = await contentGenerator.GenerateImage(part.Question, destinationLanguage);
            part.ImageId = await blobService.SaveImageBlob(image);
        }

        await _guides.ReplaceOneAsync(g => guide.Id == g.Id, guide);
    }

    public async Task DeleteUnit(string sourceLanguage, string destinationLanguage, int unitIndex)
    {
        var guide = await GetGuideInternal(sourceLanguage, destinationLanguage);
        var unit = GetUnitByIndex(guide, unitIndex);
        unit.Rules = null;
        unit.Exercises = null;
        await _guides.ReplaceOneAsync(g => guide.Id == g.Id, guide);

        await cache.Delete(LLmCacheKey.GetUnitKey(sourceLanguage, destinationLanguage, unitIndex));
    }

    public async Task DeleteUnitExampleTranslations(string sourceLanguage, string destinationLanguage, int unitIndex)
    {
        var guide = await GetGuideInternal(sourceLanguage, destinationLanguage);
        var unit = GetUnitByIndex(guide, unitIndex);
        foreach (var rule in unit.Rules)
        {
            rule.ExampleTranslation = null;
            rule.ExampleTranslationAudioId = null;
        }

        await _guides.ReplaceOneAsync(g => guide.Id == g.Id, guide);
    }

    public async Task DeleteExercise(string sourceLanguage, string destinationLanguage, int unitIndex, int exerciseIndex)
    {
        var guide = await GetGuideInternal(sourceLanguage, destinationLanguage);
        var unit = GetUnitByIndex(guide, unitIndex);
        var exercise = GetExerciseByIndex(unit, exerciseIndex);
        unit.Exercises.Remove(exercise);
        for (int i = 0; i < unit.Exercises.Count; ++i)
            unit.Exercises[i].Index = i + 1;
      
        await _guides.ReplaceOneAsync(g => guide.Id == g.Id, guide);

        await cache.Delete(LLmCacheKey.GetExerciseKey(sourceLanguage, destinationLanguage, unitIndex, exerciseIndex));
    }

    public async Task DeleteGuide(string sourceLanguage, string destinationLanguage)
    {
        var guide = await GetGuideInternal(sourceLanguage, destinationLanguage);
        if (guide != null)
        {
            await _guides.DeleteOneAsync(r => r.Id == guide.Id);

            await cache.Delete(LLmCacheKey.GetGuideKey(sourceLanguage, destinationLanguage));
        }
    }

    public async Task<List<string>> GenerateAudio(string text)
    {
        string[] parts = text.Split('/');
        List<string> result = [];
        foreach (var part in parts)
        {
            var audio = await blobGenerator.GenerateAudio(part, "english","en-US-Studio-O");
            var audioId = await blobService.SaveAudioBlob(audio);
            result.Add(audioId);
        }

        return result;
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
    
    private Exercise GetExerciseByIndex(Unit unit, int exerciseIndex)
    {
        var exercise = unit.Exercises.FirstOrDefault(r => r.Index == exerciseIndex);
        if (exercise == null)
            throw new Exception("Exercise not found.");

        return exercise;
    }
    
    private ExercisePart GetExercisePartByIndex(Exercise exercise, int partIndex)
    {
        var part = exercise.Parts.FirstOrDefault(r => r.Index == partIndex);
        if (part == null)
            throw new Exception("Part not found.");

        return part;
    }
}