namespace GrammarGuide.Services.Settings;
public class DbSettings
{
    public string UsersCollectionName { get; set; }
    public string GuidesCollectionName { get; set; }
    public string LlmCacheCollectionName { get; set; }
    public string ConnectionString { get; set; }
    public string DatabaseName { get; set; }
}