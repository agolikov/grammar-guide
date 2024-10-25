using System;
using System.Threading.Tasks;
using GrammarGuide.ContentBuilder.Prompts;
using GrammarGuide.ContentBuilder.Prompts.Exercises;
using GrammarGuide.ContentBuilder.Prompts.Exercises.PickAnOption;
using GrammarGuide.ContentBuilder.Prompts.Exercises.PickAnOrder;
using GrammarGuide.ContentBuilder.Prompts.Exercises.TextWithOptions;
using GrammarGuide.ContentBuilder.Prompts.Exercises.YesNoQuestion;
using GrammarGuide.ContentBuilder.Prompts.Guide;
using GrammarGuide.ContentBuilder.Prompts.Structure;
using GrammarGuide.ContentBuilder.Prompts.Translation;
using GrammarGuide.ContentBuilder.Prompts.Unit;
using GrammarGuide.Services.Entities;
using GrammarGuide.Services.Entities.Exercises;

namespace GrammarGuide.ContentBuilder;

public class ContentGenerator(PromptExecutor executor, IPromptDataProvider dataProvider)
{
    public async Task<GuideLlmResult> GenerateReferenceGuide(string destinationLanguage)
    {
        var prompt = new GuideTextPrompt(new PromptBuilder(dataProvider), destinationLanguage);
        return await executor.ExecuteTextPrompt(prompt);
    }

    public async Task<GuideLlmResult> TranslateGuide(GuideLlmResult guide, string destinationLanguage)
    {
        var prompt =
            new TranslateTextPrompt<GuideLlmResult>(new PromptBuilder(dataProvider), destinationLanguage, guide);
        var llmResult = await executor.ExecuteTextPrompt(prompt);
        return llmResult;
    }

    public async Task<UnitRulesLlmResult> GenerateReferenceUnit(string sourceLanguage, string destinationLanguage, string unitTitle)
    {
        var prompt = new UnitTextPrompt(new PromptBuilder(dataProvider), sourceLanguage, destinationLanguage,
            unitTitle);
        
        return await executor.ExecuteTextPrompt(prompt);
    }

    public async Task<byte[]> GenerateImage(string text, string lang)
    {
        var imagePrompt = new ImageTextPrompt(new PromptBuilder(dataProvider), text);
        var result = await executor.ExecuteTextPromptRaw(imagePrompt.BuildPrompt());
        return await executor.ExecuteImagePrompt(result, lang);
    }

    public async Task<UnitRulesLlmResult> TranslateUnit(UnitRulesLlmResult cached, string destinationLanguage)
    {
        var prompt =
            new TranslateTextPrompt<UnitRulesLlmResult>(new PromptBuilder(dataProvider), destinationLanguage, cached);
        return await executor.ExecuteTextPrompt(prompt);
    }

    public async Task<object> GenerateExercise(string sourceLanguage, string destinationLanguage,Unit unit, Rule referenceRule,
        ExerciseType type, int questionsCount)
    {
        IExerciseResult result;
        var builder = new PromptBuilder(dataProvider);
        switch (type)
        {
            case ExerciseType.PickAnOption:
                var pickAnOptionExercisePrompt = new PickAnOptionExercisePrompt(builder, sourceLanguage,
                    destinationLanguage,unit.Title,
                    referenceRule.Content, questionsCount);
                result = await executor.ExecuteTextPrompt(pickAnOptionExercisePrompt);
                break;
            case ExerciseType.PickAnOrder:
                var pickAnOrderExercisePrompt = new PickAnOrderExercisePrompt(builder, sourceLanguage,
                    destinationLanguage, unit.Title,
                    referenceRule.Content, questionsCount);
                result = await executor.ExecuteTextPrompt(pickAnOrderExercisePrompt);
                break;
            case ExerciseType.TextWithOptions:
                var textWithOptionsExercisePrompt = new TextWithOptionsExercisePrompt(builder,
                    sourceLanguage, destinationLanguage,unit.Title,
                    referenceRule.Content, questionsCount);
                result = await executor.ExecuteTextPrompt(textWithOptionsExercisePrompt);
                break;
            case ExerciseType.YesNoQuestion:
                var yesNoExercisePrompt = new YesNoExercisePrompt(builder, sourceLanguage, destinationLanguage,unit.Title,
                    referenceRule.Content, questionsCount);
                result = await executor.ExecuteTextPrompt(yesNoExercisePrompt);
                break;
            default:
                throw new Exception($"Exercise {type} is not implemented.");
        }

        return result;
    }
}