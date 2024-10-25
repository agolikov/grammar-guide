namespace GrammarGuide.Services;

public abstract class LLmCacheKey
{
    public static string GetGuideKey(string sourceLanguage, string targetLanguage)
    {
        return $"guide_{sourceLanguage}_{targetLanguage}";
    }
    
    public static string GetUnitKey(string sourceLanguage, string targetLanguage, int unitIndex)
    {
        return $"unit_{sourceLanguage}_{targetLanguage}_{unitIndex}";
    }

    public static string GetExerciseKey(string sourceLanguage, string targetLanguage, int unitIndex, int exerciseIndex)
    {
        return $"exercise_{sourceLanguage}_{targetLanguage}_{unitIndex}_{exerciseIndex}";
    }
}