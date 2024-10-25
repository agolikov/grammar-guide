namespace GrammarGuide.Services.Settings;

public class BlobSettings
{
    public string ProjectId { get; set; }
    public string LocationId { get; set; }
    public string PublisherId { get; set; }
    public string ModelId { get; set; }
    public int ImageWidth { get; set; }
    public int ImageHeight { get; set; }
    public string AiPlatformEndpoint { get; set; }
   
    public string BucketName { get; set; }
}