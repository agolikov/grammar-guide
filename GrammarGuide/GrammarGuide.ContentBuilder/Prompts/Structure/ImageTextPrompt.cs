namespace GrammarGuide.ContentBuilder.Prompts.Structure;

public class ImageTextPrompt : ITextPrompt<string>
{
    private readonly PromptBuilder _builder;
    private readonly string _text;

    public ImageTextPrompt(PromptBuilder builder, string text)
    {
        _builder = builder;
        _text = text;
    }

    public string BuildPrompt()
    {
        return _builder
            .AddTask(PromptConstants.Task.GetImagePrompt, _text)
            .Build();
    }
}