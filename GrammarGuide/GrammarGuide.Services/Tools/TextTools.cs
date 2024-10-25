using System.Text.RegularExpressions;

namespace GrammarGuide.Services.Tools;

public static class TextTools
{
    public static string Cleaned(this string input)
    {
        input = input.Replace("\t", " ").Replace("\n", " ").Replace("\r", " ");
        return Regex.Replace(input, @"[^a-zA-Z0-9\s]", "");
    }

    public static string RemoveUnderscores(this string input)
    {
        return input.Replace("_", "");
    }

    public static string RemoveBracketsContent(this string input)
    {
        return Regex.Replace(input, @"\s*\([^)]*\)", "");
    }
}