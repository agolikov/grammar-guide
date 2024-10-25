using System.Threading.Tasks;
using GrammarGuide.API.Validation;
using GrammarGuide.ContentBuilder.Services;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace GrammarGuide.API.Controllers;

[ApiController]
[Route("admin")]
[EnableCors("AllowAnyOrigin")]
public class AdminController(IAdminService adminService, LanguageValidator languageValidator)
    : Controller
{
    [HttpDelete("guide/{sourceLanguage}/{destinationLanguage}/unit/{unitIndex}")]
    public async Task<ActionResult> DeleteUnit(string sourceLanguage, string destinationLanguage, int unitIndex)
    {
        languageValidator.ValidateLanguages(sourceLanguage, destinationLanguage);
        await adminService.DeleteUnit(sourceLanguage, destinationLanguage, unitIndex);
        return Ok();
    }

    [HttpDelete("guide/{sourceLanguage}/{destinationLanguage}/unit/{unitIndex}/example-translations")]
    public async Task<ActionResult> DeleteUnitExampleTranslations(string sourceLanguage, string destinationLanguage,
        int unitIndex)
    {
        languageValidator.ValidateLanguages(sourceLanguage, destinationLanguage);
        await adminService.DeleteUnitExampleTranslations(sourceLanguage, destinationLanguage, unitIndex);
        return Ok();
    }

    [HttpDelete("guide/{sourceLanguage}/{destinationLanguage}/unit/{unitIndex}/exercise/{exerciseIndex}")]
    public async Task<ActionResult> DeleteExercise(string sourceLanguage, string destinationLanguage, int unitIndex,
        int exerciseIndex)
    {
        languageValidator.ValidateLanguages(sourceLanguage, destinationLanguage);
        await adminService.DeleteExercise(sourceLanguage, destinationLanguage, unitIndex, exerciseIndex);
        return Ok();
    }

    [HttpDelete("guide/{sourceLanguage}/{destinationLanguage}")]
    public async Task<ActionResult> DeleteGuide(string sourceLanguage, string destinationLanguage)
    {
        languageValidator.ValidateLanguages(sourceLanguage, destinationLanguage);
        await adminService.DeleteGuide(sourceLanguage, destinationLanguage);
        return Ok();
    }

    [HttpPut("guide/{sourceLanguage}/{destinationLanguage}/unit/{unitIndex}/rule/{ruleIndex}/audio")]
    public async Task<ActionResult> UpdateAudioForRule(string sourceLanguage, string destinationLanguage, int unitIndex,
        int ruleIndex)
    {
        languageValidator.ValidateLanguages(sourceLanguage, destinationLanguage);
        await adminService.UpdateAudioForRule(sourceLanguage, destinationLanguage, unitIndex, ruleIndex);
        return Ok();
    }

    [HttpPut(
        "guide/{sourceLanguage}/{destinationLanguage}/unit/{unitIndex}/exercise/{exerciseIndex}/part/{partIndex}/image")]
    public async Task<IActionResult> RegenerateImageForExercisePart(string sourceLanguage, string destinationLanguage,
        int unitIndex, int exerciseIndex, int partIndex)
    {
        languageValidator.ValidateLanguages(sourceLanguage, destinationLanguage);
        await adminService.UpdateImageForExercisePart(sourceLanguage, destinationLanguage, unitIndex, exerciseIndex,
            partIndex);

        return Ok();
    }

    [HttpGet("audio")]
    public async Task<ActionResult> GenerateAudio(string text)
    {
        return Ok(await adminService.GenerateAudio(text));
    }
}