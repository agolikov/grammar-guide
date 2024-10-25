namespace GrammarGuide.ContentBuilder.Prompts.Exercises.TextWithOptions;

public class TextWithOptionsExercisePrompt(PromptBuilder builder, string sourceLanguage,
    string destinationLanguage, 
    string unitTitle, string rule, int sentences)
    : ITextPrompt<TextWithOptionsExerciseResult>
{
    public string BuildPrompt()
    {
        var example = new TextWithOptionsExerciseResult
        {
            Question = "Insert correct words in a text",
            Questions =
            [
                new TextWithOptionsQuestionResult
                {
                    Question =
                        $"The weather was very {PromptConstants.Separator} yesterday, so we decided to go for a walk in the park." +
                        $"She couldn't {PromptConstants.Separator} the book she wanted at the library. " +
                        $"After a long day at work, he felt extremely {PromptConstants.Separator} and just wanted to rest. " +
                        $"They {PromptConstants.Separator} to meet at the coffee shop at noon.  " +
                        $"The {PromptConstants.Separator} of the movie was so surprising that everyone gasped." +
                        $"The cat is {PromptConstants.Separator} the table.",
                    CorrectOptions = ["sunny", "find", "tired", "agreed", "conclusion", "completes", "on"]
                }
            ]
        };

        return builder
            .AddPersona(PromptConstants.Persona.Tutor, destinationLanguage)
            .AddTask(PromptConstants.Task.TextWithOptionsExercise, sentences.ToString())
            .AddContext(PromptConstants.Context.RuleExamination, unitTitle, rule)
            .AddFormat(PromptConstants.Format.ContentLanguage, sourceLanguage, destinationLanguage)
            .AddFormat(PromptConstants.Format.ExerciseParts, sentences.ToString())
            .AddJsonFormatWithOneShotLearningExample(example)
            .Build();
    }
}