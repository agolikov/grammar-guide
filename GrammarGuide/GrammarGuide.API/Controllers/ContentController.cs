using System.IO;
using System.Threading.Tasks;
using GrammarGuide.API.Validation;
using GrammarGuide.ContentBuilder.Services;
using GrammarGuide.Services;
using GrammarGuide.Services.Entities;
using GrammarGuide.Services.Entities.Exercises;
using GrammarGuide.Services.Settings;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace GrammarGuide.API.Controllers;

[ApiController]
[Route("content")]
[EnableCors("AllowAnyOrigin")]
public class ContentController(IAppDataService appDataService, LanguageValidator languageValidator)
    : Controller
{
    [HttpGet("languages")]
    public ActionResult<SupportedLanguages> GetLanguages() => Ok(appDataService.GetGuideLanguages());

    [HttpGet("flag/{countryCode}")]
    public async Task<ActionResult> GetFlag(string countryCode)
    {
        var stream = await appDataService.GetFlag(countryCode);
        var memStream = new MemoryStream(stream);
        memStream.Position = 0;
        return File(memStream, "image/png", $"{countryCode}.png");
    }

    [HttpGet("guide/{sourceLanguage}/{destinationLanguage}/structure")]
    public async Task<ActionResult<Guide>> GetGuide(string sourceLanguage, string destinationLanguage)
    {
        languageValidator.ValidateLanguages(sourceLanguage, destinationLanguage);

        var guide = await appDataService.GetGuide(sourceLanguage, destinationLanguage, true);
        return Ok(guide);
    }

    [HttpGet("guide/{sourceLanguage}/{destinationLanguage}/unit/{unitIndex}")]
    public async Task<ActionResult<Unit>> GetUnit(string sourceLanguage, string destinationLanguage, int unitIndex)
    {
        languageValidator.ValidateLanguages(sourceLanguage, destinationLanguage);
        
        var unit = await appDataService.GetUnit(sourceLanguage, destinationLanguage, unitIndex);
        return Ok(unit);
    }

    [HttpPost("guide/{sourceLanguage}/{destinationLanguage}/unit/{unitIndex}/exercise-set")]
    public async Task<ActionResult<Unit>> AddExerciseSet(string sourceLanguage, string destinationLanguage, int unitIndex)
    {
        languageValidator.ValidateLanguages(sourceLanguage, destinationLanguage);
        return Ok(await appDataService.AddExerciseSet(sourceLanguage, destinationLanguage, unitIndex));
    }
    
    [HttpPost("guide/{sourceLanguage}/{destinationLanguage}/unit/{unitIndex}/rule/{ruleIndex}/exercise")]
    public async Task<ActionResult<Unit>> AddExercise(string sourceLanguage, string destinationLanguage, int unitIndex,
        int ruleIndex,
        [FromQuery] ExerciseType? type)
    {
        languageValidator.ValidateLanguages(sourceLanguage, destinationLanguage);
        return Ok(await appDataService.AddExercise(sourceLanguage, destinationLanguage, unitIndex, ruleIndex, type));
    }
    
    [HttpPost("guide/{sourceLanguage}/{destinationLanguage}/unit/{unitIndex}/rule/{ruleIndex}/image")]
    public async Task<ActionResult> RegenerateImageForRule(string sourceLanguage, string destinationLanguage,
        int unitIndex,
        int ruleIndex)
    {
        languageValidator.ValidateLanguages(sourceLanguage, destinationLanguage);
        await appDataService.UpdateImageForRule(sourceLanguage, destinationLanguage, unitIndex, ruleIndex);
        return Ok();
    }
}