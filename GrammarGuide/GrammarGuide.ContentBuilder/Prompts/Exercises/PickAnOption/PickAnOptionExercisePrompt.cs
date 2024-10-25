namespace GrammarGuide.ContentBuilder.Prompts.Exercises.PickAnOption;

public class PickAnOptionExercisePrompt(
    PromptBuilder builder,
    string sourceLanguage,
    string destinationLanguage,
    string unitTitle,
    string rule,
    int parts) : ITextPrompt<PickAnOptionExerciseResult>
{
    public string BuildPrompt()
    {
        var example = new PickAnOptionExerciseResult
        {
            Question = "Which is right: ",
            Questions =
            [
                new PickAnOptionQuestionResult
                {
                    Question = "Select a correct sentence.",
                    Options = ["Can you pass me the apple?", "Can you pass me the grape?", "Can you pass me the pear"],
                    CorrectOption = "Can you pass me the apple?",
                    Explanation = ""
                }
            ]
        };
        return builder
            .AddPersona(PromptConstants.Persona.Tutor, destinationLanguage)
            .AddContext(PromptConstants.Context.RuleExamination, unitTitle, rule)
            .AddTask(PromptConstants.Task.CreatePickAnOptionExercise)
            .AddFormat(PromptConstants.Format.ExerciseParts, parts.ToString())
            .AddFormat(PromptConstants.Format.ContentLanguage, sourceLanguage, destinationLanguage)
            .AddJsonFormatWithOneShotLearningExample(example)
            .Build();
    }
}