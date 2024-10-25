using System.Collections.Generic;

namespace GrammarGuide.Services.Entities;

public class UnitProgress
{
    public int UnitIndex { get; set; }
    public List<int> CompletedExercises { get; set; }
}