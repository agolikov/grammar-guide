using System.Collections.Generic;
using GrammarGuide.Services.Tools;

namespace GrammarGuide.Services.Entities.Exercises;

public static class QuestionPartBuilder
{
    public static QuestionPart BuildText(string text)
    {
        return new QuestionPart
        {
            Kind = QuestionPartKind.PlainText,
            Text = text.RemoveUnderscores()
        };
    }
    public static QuestionPart BuildRadio(List<string> options, int correctOptionIndex)
    {
        return new QuestionPart
        {
            Kind = QuestionPartKind.Radio,
            Options = options,
            CorrectOptionIndex = correctOptionIndex
        };
    }
   
    public static QuestionPart BuildInputText(string correctValue)
    {
        return new QuestionPart
        {
            Kind = QuestionPartKind.InputText,
            CorrectValue = correctValue
        };
    }
    
    public static QuestionPart BuildInputOptions(List<string> options, int correctIndex)
    {
        return new QuestionPart
        {
            Kind = QuestionPartKind.InputTextWithOptions,
            Options = options,
            CorrectOptionIndex = correctIndex
        };
    }
}