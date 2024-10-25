using System.Collections.Generic;

namespace GrammarGuide.Services.Entities.Exercises;

public class ExercisePart
{
    public int Index { get; set; }
    public string Question { get; set; }
    public string ImageId { get; set; }
    public string Explanation { get; set; }
    public List<QuestionPart> Parts { get; set; }
}