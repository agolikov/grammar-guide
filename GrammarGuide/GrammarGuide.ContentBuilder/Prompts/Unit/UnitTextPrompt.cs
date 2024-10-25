namespace GrammarGuide.ContentBuilder.Prompts.Unit;

public class UnitTextPrompt : ITextPrompt<UnitRulesLlmResult>
{
    private readonly string _destinationLanguage;
    private readonly string _sourceLanguage;
    private readonly string _unit;
    private readonly PromptBuilder _builder;
    
    private readonly UnitRulesLlmResult _example =
        new UnitRulesLlmResult
        {
            Title = "Spanish Grammar Basics",
            Rules =
            [
                new RuleLLmResult
                {
                    Title = "Subject-Verb Agreement.",
                    Content = "The subject and verb in a sentence must agree in number.",
                    Example = "El perro corre rápido.",
                    ExampleTranslation = "The dog runs fast."
                },

                new RuleLLmResult
                {
                    Title = "Use of Articles.",
                    Content = "Articles are used to define a noun as specific or unspecific.",
                    Example = "Ella compró un libro.",
                    ExampleTranslation = "She bought a book."
                }
            ]
        };

    public UnitTextPrompt(PromptBuilder builder, string sourceLanguage, string destinationLanguage, string unit)
    {
        _sourceLanguage = sourceLanguage;
        _destinationLanguage = destinationLanguage;
        _unit = unit;
        _builder = builder;
    }

    public string BuildPrompt()
    {
        return _builder
            .AddPersona(PromptConstants.Persona.Tutor, _destinationLanguage)
            .AddTask(PromptConstants.Task.CreateUnit, _unit)
            .AddContext(PromptConstants.Context.UnitDefinition, _unit, _sourceLanguage, _destinationLanguage)
            .AddFormat(PromptConstants.Format.MDJSONFormat)
            .AddJsonFormatWithOneShotLearningExample(_example)
            .Build();
    }
}