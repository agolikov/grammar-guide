namespace GrammarGuide.Services.Entities;

public class Rule
{
    public int Index { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public string Example { get; set; }
    public string ExampleTranslation { get; set; }
    public string ExampleAudioId { get; set; }
    public string ExampleTranslationAudioId { get; set; }
    public string ImageId { get; set; }
}