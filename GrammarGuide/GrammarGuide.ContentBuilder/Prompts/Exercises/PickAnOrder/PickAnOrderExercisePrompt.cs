namespace GrammarGuide.ContentBuilder.Prompts.Exercises.PickAnOrder;

public class PickAnOrderExercisePrompt(
    PromptBuilder builder,
    string sourceLanguage,
    string destinationLanguage,
    string unitTitle,
    string rule,
    int parts)
    : ITextPrompt<PickAnOrderExerciseResult>
{
    public string BuildPrompt()
    {
        var example = new PickAnOrderExerciseResult
        {
            Questions =
            [
                new PickAnOrderQuestionResult
                {
                    Question = "Can you pass me the apple?"
                },
                new PickAnOrderQuestionResult
                {
                    Question = "Can you pass me the apple?"
                },
            ]
        };
        return builder
            .AddPersona(PromptConstants.Persona.Tutor, destinationLanguage)
            .AddContext(PromptConstants.Context.RuleExamination, unitTitle, rule)
            .AddTask(PromptConstants.Task.CreatePickAnOrderExercise)
            .AddFormat(PromptConstants.Format.ExerciseParts, parts.ToString())
            .AddFormat(PromptConstants.Format.ContentLanguage, sourceLanguage, destinationLanguage)
            .AddJsonFormatWithOneShotLearningExample(example)
            .Build();
    }
}