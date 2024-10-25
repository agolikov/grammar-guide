using System.Collections.Generic;

namespace GrammarGuide.Services.Entities;

public class UnitGroup
{
    public int Index { get; set; }
    public string Title { get; set; }
    public List<Unit> Units { get; set; }
}