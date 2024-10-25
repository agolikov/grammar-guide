using Newtonsoft.Json;

namespace GrammarGuide.ContentBuilder.Prompts.Translation;

public class TranslateTextPrompt<T> : ITextPrompt<T>
{
    private readonly PromptBuilder _builder;
    private readonly string _language;
    private readonly object _obj;
    
    public TranslateTextPrompt(PromptBuilder builder, string language, T obj)
    {
        _builder = builder;
        _language = language;
        _obj = obj;
        _builder = builder;
        _language = language;
        _obj = obj;
    }

    public string BuildPrompt()
    {
        var objString = JsonConvert.SerializeObject(_obj, new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore
        });
        
        return _builder
            .AddPersona(PromptConstants.Persona.Translator)
            .AddTask(PromptConstants.Task.Translate, _language)
            .AddJsonForTranslation(objString)
            .Build();
    }
}