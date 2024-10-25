namespace GrammarGuide.ContentBuilder.Prompts;

public class PromptConstants
{
    public const string Separator = "____";
    public class Persona
    {
        public const string Tutor = "Tutor";
        public const string Translator = "Translator";
    }
    
    public class Context
    {
        public const string UnitDefinition = "UnitDefinition";
        public const string RuleExamination = "RuleExamination";
        public const string UserPreferences = "UserPreferences";
    }
    
    public class Task
    {
        // General
        public const string CreateGuide = "CreateGuide";
        public const string CreateUnit = "CreateUnit";
        
        // Exercises
        public const string CreatePickAnOptionExercise = "CreatePickAnOptionExercise";
        public const string TextWithOptionsExercise = "TextWithOptionsExercise";
        public const string CreatePickAnOrderExercise = "CreatePickAnOrderExercise";
        public const string EnterCorrectFormExercise = "EnterCorrectFormExercise";
        public const string TextWithoutOptionsExercise = "TextWithoutOptionsExercise";
        public const string YesNoExercise = "YesNoExercise";
        
        // Images
        public const string GetImagePrompt = "GetImagePrompt";
        
        //Validation
        public const string ValidateExercise = "ValidateExercise";
        
        //Translate
        public const string Translate = "Translate";
    }
    
    public class Format
    {
        public const string MDJSONFormat = "MDJSONFormat";
        public const string ExerciseParts = "ExerciseParts";
        public const string ContentLanguage = "ContentLanguage";
    }
}