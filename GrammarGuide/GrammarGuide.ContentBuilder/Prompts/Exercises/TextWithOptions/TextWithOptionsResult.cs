using System;
using System.Collections.Generic;
using System.Linq;
using GrammarGuide.Services.Entities.Exercises;

namespace GrammarGuide.ContentBuilder.Prompts.Exercises.TextWithOptions;

public class TextWithOptionsExerciseResult : IExerciseResult
{
    public string Question { get; set; } = "Pick the correct options from list.";
    public List<TextWithOptionsQuestionResult> Questions { get; set; }

    public Exercise ToEntity()
    {
        var ex = new Exercise
        {
            Type = ExerciseType.TextWithOptions,
            Title = Question,
            Parts = new List<ExercisePart>()
        };
        var allOptions = Questions.SelectMany(q => q.CorrectOptions).ToList();
        
        for (int i = 0; i < Questions.Count; i++)
            ex.Parts.Add(Questions[i].ToEntity(i + 1, allOptions));
        
        return ex;
    }
}

public class TextWithOptionsQuestionResult
{
    public string Question { get; set; }
    public string Explanation { get; set; }
    public List<string> CorrectOptions { get; set; }

    public ExercisePart ToEntity(int index, List<string> allOptions)
    {
        int start = 0;
        var res = new ExercisePart
        {
            Index = index,
            Question = Question,
            Explanation = Explanation,
            Parts = []
        };
        int pos;
        int id = 0;
            var randomized = allOptions.Shuffle();
        do
        {
            pos = Question.IndexOf(PromptConstants.Separator, start, StringComparison.Ordinal);
            if (pos == -1)
            {
                if (res.Parts.Count == 0)
                    throw new Exception("Separator not found in array.");
                break;
            }

            if (pos > 0)
                res.Parts.Add(QuestionPartBuilder.BuildText(Question.Substring(start, pos - start)));

            res.Parts.Add(QuestionPartBuilder.BuildInputOptions(randomized, Mapper.Extensions.FindIndex(randomized,
                CorrectOptions[id])));
            id += 1;
            start = pos + 1;
        } while (true);

        if (pos < start)
            res.Parts.Add(QuestionPartBuilder.BuildText(Question.Substring(start)));

        return res;
    }
}