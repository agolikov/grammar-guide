using System.Collections.Generic;
using GrammarGuide.Services.Entities.Exercises;

namespace GrammarGuide.Services.Entities;

public class Unit
{
    public string Title { get; set; }
    public int Index { get; set; }
    public string GuideId { get; set; }
    public List<Rule> Rules { get; set; }
    public List<Exercise> Exercises { get; set; }
}