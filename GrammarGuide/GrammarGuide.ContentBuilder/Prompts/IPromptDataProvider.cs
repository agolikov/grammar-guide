namespace GrammarGuide.ContentBuilder.Prompts;

public interface IPromptDataProvider
{
    string GetPersona(string key);
    string GetTask(string key);
    string GetContext(string key);
    string GetFormat(string key);
}