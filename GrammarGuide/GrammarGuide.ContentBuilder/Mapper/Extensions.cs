using System;
using System.Collections.Generic;

namespace GrammarGuide.ContentBuilder.Mapper;

public static class Extensions
{
    public static int FindIndex(List<string> list, string value)
    {
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i] == value)
            {
                return i;
            }
        }
        throw new Exception("Value not found in array.");
    }
}