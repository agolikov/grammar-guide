namespace GrammarGuide.API.Models;

public class UpdateUserDataRequest
{
    public string UserId { get; set; }
    public string Theme { get; set; }
    public string Name { get; set; }
}