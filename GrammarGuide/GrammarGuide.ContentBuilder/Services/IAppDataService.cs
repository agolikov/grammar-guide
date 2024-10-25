using System.Threading.Tasks;
using GrammarGuide.Services.Entities;
using GrammarGuide.Services.Entities.Exercises;
using GrammarGuide.Services.Settings;

namespace GrammarGuide.ContentBuilder.Services;

public interface IAppDataService
{
    Task<byte[]> GetFlag(string countryCode);
    SupportedLanguages GetGuideLanguages();
    Task<Guide> GetGuide(string sourceLanguage, string destinationLanguage, bool captionsOnly);
    Task<Unit> GetUnit(string sourceLanguage, string destinationLanguage, int unitIndex);
    Task<Unit> AddExerciseSet(string sourceLanguage, string destinationLanguage, int unitIndex);
    Task<Unit> AddExercise(string sourceLanguage, string destinationLanguage, int unitIndex, int ruleIndex,
        ExerciseType? type);
    Task UpdateImageForRule(string sourceLanguage, string destinationLanguage, int unitIndex, int ruleIndex);
}