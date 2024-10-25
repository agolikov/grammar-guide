namespace GrammarGuide.Services.Entities.Exercises;

public enum QuestionPartKind
{
    //display only value
    PlainText = 1,

    //display an empty text field, which should be validated by what is in Options array.
    //Options array have only one value - correct answer
    InputText = 2,

    //display an empty text field, which should be validated by what is in Options array
    //Options array have multiple values - but correct one is in CorrectOptionId.
    InputTextWithOptions = 3,
    
    /// <summary>
    /// Display radio button
    /// </summary>
    Radio = 4
}