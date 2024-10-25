using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace GrammarGuide.ContentBuilder.Prompts;

public static class PromptBuilderExtensions
{
    public static PromptBuilder AddJsonFormatWithOneShotLearningExample<T>(this PromptBuilder promptBuilder, T example) 
    { 
        promptBuilder.AddCustomContext("The output format must be a valid JSON object. " +
                  "It should not contains characters which can not be in JSON." +
                  "All fields of json must be defined." +
                  "The example of the output is: \n");
        
        promptBuilder.AddCustomContext(JsonConvert.SerializeObject(example, new JsonSerializerSettings
        {
            Formatting = Formatting.None,
            NullValueHandling = NullValueHandling.Ignore
        }));

        return promptBuilder;
    }
    
    public static PromptBuilder AddJsonForTranslation<T>(this PromptBuilder promptBuilder, T example)
    {
        promptBuilder.AddCustomContext("The output format must be a valid JSON object. " +
                                       "It should not contains characters which can not be in JSON." +
                                       "Translate only the values of the json object. Do not translate the property names." +
                                       "The requested json object is: \n");
        
        promptBuilder.AddCustomContext(JsonConvert.SerializeObject(example, new JsonSerializerSettings
        {
            Formatting = Formatting.None,
            NullValueHandling = NullValueHandling.Ignore
        }));

        return promptBuilder;
    }
    
    public static List<T> Shuffle<T>(this List<T> list)
    {
        var random = new Random();
        return list.OrderBy(x => random.Next()).ToList();
    }
}