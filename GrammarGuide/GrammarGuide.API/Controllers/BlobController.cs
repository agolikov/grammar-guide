using System;
using System.Threading.Tasks;
using GrammarGuide.Services.Services;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace GrammarGuide.API.Controllers;

[ApiController]
[Route("blob")]
[EnableCors("AllowAnyOrigin")]
public class BlobController(IBlobService blobService) : Controller
{
    [HttpGet("image")]
    public async Task<IActionResult> GetImage([FromQuery] string blobId)
    {
        try
        {
            var blob = await blobService.GetImageStream(blobId);
            return File(blob, "image/png", "demo.png");
        }
        catch (Exception)
        {
            return NotFound("Image not found.");
        }
    }
    
    [HttpGet("audio")]
    public async Task<IActionResult> GetAudio([FromQuery] string blobId)
    {
        try
        {
            var blob = await blobService.GetAudioStream(blobId);
            return File(blob, "audio/mp3", "audio.mp3");
        }
        catch (Exception)
        {
            return NotFound("Audio not found.");
        }
    }
}