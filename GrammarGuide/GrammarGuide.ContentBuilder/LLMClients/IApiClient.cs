using System.Threading.Tasks;

namespace GrammarGuide.ContentBuilder.LLMClients;

public interface IApiClient
{
    Task<string> TextPrompt(string textPrompt);

    Task<byte[]> ImagePrompt(string textPrompt, string lang);
}