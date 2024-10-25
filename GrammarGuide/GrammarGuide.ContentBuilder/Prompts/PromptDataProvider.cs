using System;
using System.Collections.Generic;

namespace GrammarGuide.ContentBuilder.Prompts;

public class PromptDataProvider : IPromptDataProvider
{
    private readonly Dictionary<string, string> _personas = new()
    {
        {
            PromptConstants.Persona.Tutor,
            "You are a professional language tutor specialized in teaching {0} language."
        },
        {
            PromptConstants.Persona.Translator,
            "You are a language translator."
        }
    };

    private readonly Dictionary<string, string> _tasks = new()
    {
        {
            PromptConstants.Task.CreateGuide,
            @"Create a 12-week grammar learning guide with 48 units, divided into 4 units per week. 
             Each week should be titled 'Week X: Unit Y-Z' (X for week number, Y for unit number, Z for unit topics).
             Ensure the plan is suitable for beginner and intermediate learners, focusing on accessibility and comprehension."
        },
        {
            PromptConstants.Task.CreateUnit,
            "You need to prepare grammar learning materials for the unit with the title '{0}'."
        },
        {
            PromptConstants.Task.CreatePickAnOptionExercise,
            "Create an exercise to test student knowledge on the provided rule. " +
            "The exercise requires you to pick an option from a list. " +
            "You will be provided with two or more sentences or words, " +
            "and the task is to select the correct one. The question should include unit title."
        },
        {
            PromptConstants.Task.CreatePickAnOrderExercise,
            @"Create an example sentence which demonstrates a grammar rule in action.
             The number of words in the sentence must be more than 4 and less than 8.
 There should be no repeated words in this sentence.
             The delimiter between words in the sentence should be '/'. The topic of the sentence should be common knowledge."
        },
        {
            PromptConstants.Task.EnterCorrectFormExercise,
            "Create an exercise to test student knowledge on the provided rule. " +
            "The exercise provide a single word. The answer for the task is a correct form of the word."
        },
        {
            PromptConstants.Task.TextWithOptionsExercise,
            @"Create an exercise to test student knowledge on the provided rule. 
             The exercise consists of a text {0} sentences and missing words.
             There is one missing word represented as empty in the text.
             The student task is to match empty slot in the sentences with a correct word from the list."
        },
        {
            PromptConstants.Task.TextWithoutOptionsExercise,
            @"Create an exercise to test student knowledge on the provided rule. 
              Each question consist of text with 1 sentence. The student task is to fill empty slots in 
              the sentence with a correct word."
        },
        {
            PromptConstants.Task.YesNoExercise,
            @"Create an exercise to test student knowledge on the provided rule. 
            The exercise consists of general questions with possible answers: Yes or No."
        },
        {
            PromptConstants.Task.GetImagePrompt,
            @"Create a one sentence Google VertexAI prompt for a pencil drawing with transparency in the background. 
             The prompt must shows the process or action from the following text: '{0}'."
        },
        {
            PromptConstants.Task.ValidateExercise,
            "You have an question to answer. Try to answer the question and assess whether the question is valid or not"
        },
        {
            PromptConstants.Task.Translate, "Your task is to translate the following document into {0} language."
        }
    };


    private readonly Dictionary<string, string> _contexts = new()
    {
        {
            PromptConstants.Context.UnitDefinition,
            @"The materials should be divided into rules consist of at least 3 and up to 5 rules.
             The response should include a comprehensive list of these rules with grammar explanations. 
             Ensure each rule is precise and can be distinctly assessed.
             For pronunciation rules, add and highlight placeholders. 
             Make rule definitions as clear as possible.  
             'Content' field is the detailed rule definition.
             'Example' is one sentence the text in {2} language which demonstrates the rule using native language symbols. 
             'ExampleTranslation' is the translation of 'Example' sentence in {1} language."
        },
        {
            PromptConstants.Context.RuleExamination,
            "Question should tests the knowledge for the unit '{0}' and the following grammar rule: '{1}'. "
        },
        {
            PromptConstants.Context.UserPreferences,
            "The exercise content should include the topic which user is interested in." +
            "The topic title is: {0}"
        }
    };

    private readonly Dictionary<string, string> _formats = new()
    {
        {
            PromptConstants.Format.MDJSONFormat, 
            @"The content of the result should be provided in MD format.
            Where important parts are highlighted with bold or italic font.
            Take into account that the result will be a part of json, 
            which means that you should escape all special characters."
        },
        {
            PromptConstants.Format.ExerciseParts,
            "The task must consist of {0} parts. Each part should be a separate question."
        },
        {
            PromptConstants.Format.ContentLanguage,
            "The content must be in {0} language. The tasks must be in {1} language."
        }
    };

    public string GetPersona(string key)
    {
        if (!_personas.TryGetValue(key, out var persona))
            throw new Exception("Persona not found");
        return persona;
    }

    public string GetTask(string key)
    {
        if (!_tasks.TryGetValue(key, out var task))
            throw new Exception("Task not found");
        return task;
    }

    public string GetContext(string key)
    {
        if (!_contexts.TryGetValue(key, out var context))
            throw new Exception("Context not found");
        return context;
    }

    public string GetFormat(string key)
    {
        if (!_formats.TryGetValue(key, out var format))
            throw new Exception("Format not found");
        return format;
    }
}