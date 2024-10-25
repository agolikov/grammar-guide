namespace GrammarGuide.ContentBuilder.Prompts.Exercises.YesNoQuestion;

public class YesNoExercisePrompt(
    PromptBuilder builder,
    string sourceLanguage,
    string destinationLanguage,
    string unitTitle,
    string rule,
    int sentences)
    : ITextPrompt<YesNoExerciseResult>
{
    public string BuildPrompt()
    {
        var example = new YesNoExerciseResult
        {
            Question = "Answer to question Yes or No",
            Questions =
            [
                new YesNoQuestionResult
                {
                    Question =
                        $"Is the word 'beautiful' an adjective?",
                    Answer = "Yes",
                    Explanation = "This is correct statement."
                },
                new YesNoQuestionResult
                {
                    Question =
                        $"Is 'he don't like pizza' grammatically correct?",
                    Answer = "No",
                    Explanation = "No, 'he don't like pizza' is not grammatically correct. " +
                                  "The correct form is he doesn't like pizza"
                }
            ]
        };

        return builder
            .AddPersona(PromptConstants.Persona.Tutor, destinationLanguage)
            .AddTask(PromptConstants.Task.YesNoExercise, sentences.ToString())
            .AddContext(PromptConstants.Context.RuleExamination, unitTitle, rule)
            .AddFormat(PromptConstants.Format.ContentLanguage, sourceLanguage, destinationLanguage)
            .AddFormat(PromptConstants.Format.ExerciseParts, sentences.ToString())
            .AddJsonFormatWithOneShotLearningExample(example)
            .Build();
    }
}