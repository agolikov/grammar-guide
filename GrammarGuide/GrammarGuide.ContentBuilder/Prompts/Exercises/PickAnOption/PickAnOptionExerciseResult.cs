using System.Collections.Generic;
using GrammarGuide.Services.Entities.Exercises;

namespace GrammarGuide.ContentBuilder.Prompts.Exercises.PickAnOption;

public class PickAnOptionExerciseResult : IExerciseResult
{
    public string Question { get; set; }
    public List<PickAnOptionQuestionResult> Questions { get; set; }

    public Exercise ToEntity()
    {
        var ex = new Exercise
        {
            Type = ExerciseType.PickAnOption,
            Title = Question,
            Parts = new List<ExercisePart>()
        };
        
        for (int i = 0; i < Questions.Count; ++i)
            ex.Parts.Add(Questions[i].ToEntity(i + 1, Questions[i].Options));

        return ex;
    }
}

public class PickAnOptionQuestionResult
{
    public string Question { get; set; }
    public string Explanation { get; set; }
    public List<string> Options { get; set; }
    public string CorrectOption { get; set; }

    public ExercisePart ToEntity(int index, List<string> allOptions)
    {
        return new ExercisePart
        {
            Index = index,
            Question = Question,
            Parts =
            [
                QuestionPartBuilder.BuildText(Question),
                QuestionPartBuilder.BuildRadio(allOptions, Mapper.Extensions.FindIndex(allOptions, CorrectOption))
            ]
        };
    }
}