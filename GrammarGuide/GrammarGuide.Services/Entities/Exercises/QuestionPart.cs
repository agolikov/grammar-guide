using System.Collections.Generic;

namespace GrammarGuide.Services.Entities.Exercises;

public class QuestionPart
{
    public QuestionPartKind Kind { get; set; }

    //Text content to display.
    public string Text { get; set; }

    //if options provided, this is the list of them
    public List<string> Options { get; set; }

    //correct option
    public int CorrectOptionIndex { get; set; }

    /// <summary>
    /// if no options are provided, then this is the correct answer.
    /// </summary>
    public string CorrectValue { get; set; }
}