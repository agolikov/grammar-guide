using System.Collections.Generic;
using GrammarGuide.Services.Entities;
using GrammarGuide.Services.Tools;

namespace GrammarGuide.ContentBuilder.Prompts.Unit;

public class UnitRulesLlmResult
{
    public string Title { get; set; }
    public List<RuleLLmResult> Rules { get; set; }
    
    public List<Rule> ToRulesEntity()
    {
        int rIndex = 0;
        List<Rule> rules = [];
        foreach (var r in Rules)
        {
            rules.Add(new Rule
            {
                Index = rIndex++,
                Title = r.Title,
                Content = r.Content,
                Example = r.Example?.Cleaned().RemoveBracketsContent(),
                ExampleTranslation = r.ExampleTranslation?.Cleaned().RemoveBracketsContent()
            });
        }

        return rules;
    }
}