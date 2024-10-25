using System.Collections.Generic;
using GrammarGuide.Services.Entities;

namespace GrammarGuide.ContentBuilder.Prompts.Guide;

public class GuideLlmResult
{
    public List<UnitGroupLlmResult> UnitGroups { get; set; }

    public GrammarGuide.Services.Entities.Guide ToEntity(string sourceLanguage, string destinationLanguage)
    {
        var guide = new GrammarGuide.Services.Entities.Guide
        {
            SourceLanguage = sourceLanguage,
            DestinationLanguage = destinationLanguage,
            UnitGroups = new List<UnitGroup>()
        };

        int ugIndex = 0;
        int uIndex = 0;
        foreach (var ug in UnitGroups)
        {
            var g = new UnitGroup
            {
                Index = ++ugIndex,
                Title = ug.Title,
                Units = new List<GrammarGuide.Services.Entities.Unit>()
            };

            foreach (var u in ug.Units)
                g.Units.Add(new GrammarGuide.Services.Entities.Unit
                {
                    Index = ++uIndex,
                    Title = u.Title
                });

            guide.UnitGroups.Add(g);
        }

        return guide;
    }
}