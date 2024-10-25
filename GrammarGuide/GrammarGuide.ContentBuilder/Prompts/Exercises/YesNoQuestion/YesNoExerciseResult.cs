using System.Collections.Generic;
using GrammarGuide.Services.Entities.Exercises;

namespace GrammarGuide.ContentBuilder.Prompts.Exercises.YesNoQuestion
{
    public class YesNoExerciseResult : IExerciseResult
    {
        public string Question { get; set; } = "Write the correct values into empty boxes.";
        public List<YesNoQuestionResult> Questions { get; set; }

        public Exercise ToEntity()
        {
            var ex = new Exercise
            {
                Type = ExerciseType.YesNoQuestion,
                Title = Question,
                Parts = new List<ExercisePart>()
            };
            for (int i = 0; i < Questions.Count; i++)
                ex.Parts.Add(Questions[i].ToEntity(i + 1));

            return ex;
        }
    }

    public class YesNoQuestionResult
    {
        public string Question { get; set; }

        public string Explanation { get; set; }

        public string Answer { get; set; }

        public ExercisePart ToEntity(int index)
        {
            var res = new ExercisePart
            {
                Index = index,
                Question = Question,
                Explanation = Explanation,
                Parts =
                [
                    QuestionPartBuilder.BuildText(Question)
                ]
            };

            bool isCorrectAnswerYes = Answer.ToLower() == "yes";
            res.Parts.Add(QuestionPartBuilder.BuildRadio(["Yes", "No"], isCorrectAnswerYes ? 0 : 1));
            return res;
        }
    }
}