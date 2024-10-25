using GrammarGuide.Services.Entities.Exercises;

namespace GrammarGuide.ContentBuilder.Prompts.Exercises;

public interface IExerciseResult
{
    Exercise ToEntity();
}