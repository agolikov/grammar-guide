using System;
using System.Linq;
using System.Threading.Tasks;
using GrammarGuide.ContentBuilder.Services;

namespace GrammarGuide.API.Validation;

public class LanguageValidator(IAppDataService appDataService)
{
    public void ValidateLanguages(string sourceLang, string destLang)
    {
        var langs = appDataService.GetGuideLanguages();
        
        if (langs.Languages.All(l => l.Id != sourceLang.ToLower()))
            throw new Exception("Source language is incorrect.");

        if (langs.Languages.All(l => l.Id != destLang.ToLower()))
            throw new Exception("Source language is incorrect.");
    }
}