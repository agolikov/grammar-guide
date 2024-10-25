using System.Collections.Generic;

namespace GrammarGuide.Services.Entities;

public class GuideProgress
{
    public string GuideId { get; set; }
    public List<int> CompletedUnitGroupsIndexes { get; set; }
    public List<int> CompletedUnitIndexes { get; set; }
    public List<UnitProgress> CompletedExercises { get; set; }
    public List<int> Bookmarks { get; set; }
}