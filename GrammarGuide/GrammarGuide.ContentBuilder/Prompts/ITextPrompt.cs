namespace GrammarGuide.ContentBuilder.Prompts;

public interface ITextPrompt<out T>
{
    string BuildPrompt();
}