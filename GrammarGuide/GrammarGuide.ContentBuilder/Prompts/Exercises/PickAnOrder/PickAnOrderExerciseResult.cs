using System;
using System.Collections.Generic;
using System.Linq;
using GrammarGuide.Services.Entities.Exercises;

namespace GrammarGuide.ContentBuilder.Prompts.Exercises.PickAnOrder;

public class PickAnOrderExerciseResult : IExerciseResult
{
    public string Question { get; set; } = "Pick the correct order of the words.";
    public List<PickAnOrderQuestionResult> Questions { get; set; }

    public Exercise ToEntity()
    {
        var ex = new Exercise
        {
            Type = ExerciseType.PickAnOrder,
            Title = Question,
            Parts = []
        };
        for (int i = 0; i < Questions.Count; ++i)
            ex.Parts.Add(Questions[i].ToEntity(i + 1));
        return ex;
    }
}

public class PickAnOrderQuestionResult
{
    public string Question { get; set; }

    public ExercisePart ToEntity(int index)
    {
        var output = new ExercisePart
        {
            Index = index,
            Question = Question,
            Parts = []
        };

        var parts = Question
            .ToLower()
            .Replace("/", " ")
            .Replace(".", "")
            .Replace("*", "")
            .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
            .Select(s => s.Trim())
            .ToList();
        
        foreach (var word in parts)
            output.Parts.Add(QuestionPartBuilder.BuildInputOptions(parts, Mapper.Extensions.FindIndex(parts, word)));
        
        return output;
    }
}