using System;
using System.Threading.Tasks;
using GrammarGuide.ContentBuilder.LLMClients;
using GrammarGuide.Services.Tools;
using Newtonsoft.Json;

namespace GrammarGuide.ContentBuilder.Prompts;

public class PromptExecutor
{
    private readonly IApiClient _client;
    public PromptExecutor(IApiClient client)
    {
        _client = client;
    }

    public async Task<string> ExecuteTextPromptRaw(string prompt)
    {
        return await _client.TextPrompt(prompt);
    }
    
    public async Task<T> ExecuteTextPrompt<T>(ITextPrompt<T> textPrompt)
    {
        string finalPrompt = textPrompt.BuildPrompt();
        int maxRetries = 3;
        int delay = 1000; // delay in milliseconds

        for (int retry = 0; retry < maxRetries; retry++)
        {
            try
            {
                var result = await ExecuteTextPromptRaw(finalPrompt);
                if (result.StartsWith("```json") || result.StartsWith("```JSON"))
                    result = result.Substring(7, result.Length - 10);

                return JsonConvert.DeserializeObject<T>(result);
            }
            catch (Exception) when (retry < maxRetries - 1)
            {
                // Optionally log the exception here
                await Task.Delay(delay);
            }
        }

        // If all retries fail, throw an exception or handle accordingly
        throw new Exception("Failed to execute text prompt after multiple retries.");
    }
    
    public async Task<byte[]> ExecuteImagePrompt(string prompt, string lang)
    {
        var result = await _client.ImagePrompt(prompt.Cleaned(), lang);
        return result;
    }
}