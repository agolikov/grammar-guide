using System.Collections.Generic;
using System.Threading.Tasks;

namespace GrammarGuide.ContentBuilder.Services;

public interface IAdminService
{
    Task UpdateAudioForRule(string sourceLanguage, string destinationLanguage, int unitIndex, int ruleIndex);
    Task UpdateImageForRule(string sourceLanguage, string destinationLanguage, int unitIndex, int ruleIndex);

    Task UpdateImageForExercisePart(string sourceLanguage, string destinationLanguage, int unitIndex, int exerciseIndex,
        int partIndex);

    Task DeleteUnit(string sourceLanguage, string destinationLanguage, int unitIndex);
    Task DeleteUnitExampleTranslations(string sourceLanguage, string destinationLanguage, int unitIndex);
    Task DeleteExercise(string sourceLanguage, string destinationLanguage, int unitIndex, int exerciseIndex);
    Task DeleteGuide(string sourceLanguage, string destinationLanguage);

    Task<List<string>> GenerateAudio(string text);
}