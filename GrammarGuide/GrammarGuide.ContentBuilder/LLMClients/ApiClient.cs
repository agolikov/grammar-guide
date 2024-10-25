using System.Text;
using System.Threading.Tasks;
using DotnetGeminiSDK.Client.Interfaces;
using GrammarGuide.Services.Services;

namespace GrammarGuide.ContentBuilder.LLMClients;

public class ApiClient : IApiClient
{
    private readonly IGeminiClient _geminiClient;
    private readonly IBlobGenerator _blobGenerator;
    public ApiClient(IGeminiClient geminiClient, 
        IBlobGenerator blobGenerator)
    {
        _geminiClient = geminiClient;
        _blobGenerator = blobGenerator;
    }
    
    public async Task<string> TextPrompt(string textPrompt)
    {
        var response = await _geminiClient.TextPrompt(textPrompt);
        StringBuilder sb = new StringBuilder();
        if (response == null) 
            return sb.ToString();
        
        foreach (var part in response.Candidates[0].Content.Parts)
            sb.Append(part.Text);

        return sb.ToString();
    }

    public async Task<byte[]> ImagePrompt(string textPrompt, string lang)
    {
        return await _blobGenerator.GenerateImage(textPrompt, lang);
    }
}
