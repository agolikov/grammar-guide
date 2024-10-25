using System.Collections.Generic;

namespace GrammarGuide.ContentBuilder.Prompts.Guide;

public class UnitGroupLlmResult
{
    public string Title { get; set; }
    public List<UnitLLMResult> Units { get; set; }
}