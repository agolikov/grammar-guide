using System;

namespace GrammarGuide.Services.Tools;

public static class ArrayTools
{
    public static void Shuffle<T>(T[] array)
    {
        Random rng = new Random();
        int n = array.Length;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            (array[k], array[n]) = (array[n], array[k]);
        }
    }
}