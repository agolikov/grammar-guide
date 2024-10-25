namespace GrammarGuide.Services.Entities.Exercises;

public enum ExerciseType
{
    /// <summary>
    /// Yes No Question
    /// </summary>
    YesNoQuestion = 0,
    
    /// <summary>
    /// User should pick 1 item. options. Options count can be 2 or 3.
    /// </summary>
    PickAnOption = 1,
    
    /// <summary>
    /// User should place all options into empty spaces
    /// </summary>
    TextWithOptions = 2,
    
    /// <summary>
    /// User should pick a correct word order to build a correct sentence.
    /// </summary>
    PickAnOrder = 4,
}