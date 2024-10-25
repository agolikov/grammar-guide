using System.Collections.Generic;

namespace GrammarGuide.Services.Entities.Exercises;

public class Exercise
{
    public int Index { get; set; }

    public int UnitIndex { get; set; }
    
    public int RuleIndex { get; set; }

    public ExerciseType Type { get; set; }

    public string Title { get; set; }
    
    public List<ExercisePart> Parts { get; set; }
}