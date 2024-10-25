namespace GrammarGuide.ContentBuilder.Prompts.Guide;

public class GuideTextPrompt(PromptBuilder builder, string destinationLanguage) : ITextPrompt<GuideLlmResult>
{
    private readonly GuideLlmResult _example =
        new GuideLlmResult
        {
            UnitGroups =
            [
                new UnitGroupLlmResult()
                {
                    Title = "Week 1 Title",
                    Units =
                    [
                        new UnitLLMResult
                        {
                            Title = "Unit Title 1",
                        },

                        new UnitLLMResult
                        {
                            Title = "Unit Title 2"
                        }
                    ]
                },

                new UnitGroupLlmResult
                {
                    Title = "Week 2 Title",
                    Units =
                    [
                        new UnitLLMResult
                        {
                            Title = "Unit Title 3",
                        },

                        new UnitLLMResult
                        {
                            Title = "Unit Title 4",
                        }
                    ]
                }
            ]
        };

    public string BuildPrompt()
    {
        return builder
            .AddPersona(PromptConstants.Persona.Tutor, destinationLanguage)
            .AddTask(PromptConstants.Task.CreateGuide)
            .AddJsonFormatWithOneShotLearningExample(_example)
            .Build();
    }
}