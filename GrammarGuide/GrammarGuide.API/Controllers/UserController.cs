using System.Threading.Tasks;
using GrammarGuide.API.Models;
using GrammarGuide.Services.Services;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace GrammarGuide.API.Controllers;

[ApiController]
[Route("user")]
[EnableCors("AllowAnyOrigin")]
public class UserController(IUserService userService) : Controller
{
    [HttpGet("{userId}")]
    public async Task<ActionResult> GetUserProgress(string userId)
    {
        var user = await userService.GetUserProgress(userId);
        return Ok(user);
    }

    [HttpPost]
    public async Task<ActionResult> NewUser(string userName)
    {
        return Ok(await userService.CreateUser(userName));
    }

    [HttpPut("guide/{guideId}/unit/{unitIndex}")]
    public async Task<ActionResult> UpdateUnitCompletion(string guideId, int unitIndex, [FromQuery] bool isCompleted, [FromQuery] string userId)
    {
        return Ok(await userService.UpdateUnitCompletion(guideId, userId, unitIndex, isCompleted));
    }

    [HttpPut("guide/{guideId}/unit-group/{unitIndex}")]
    public async Task<ActionResult> UpdateUnitGroupCompletion(string guideId, int unitIndex, [FromQuery] bool isCompleted,[FromQuery] string userId)
    {
        return Ok(await userService.UpdateUnitGroupCompletion(guideId, userId, unitIndex, isCompleted));
    }

    [HttpPut("user")]
    public async Task<ActionResult> Update([FromBody]UpdateUserDataRequest request)
    {
        return Ok(await userService.Update(request.UserId, request.Theme, request.Name));
    }

    [HttpPut("guide/{guideId}/bookmark/{unitIndex}")]
    public async Task<ActionResult> UpdateBookmark(string guideId, int unitIndex, [FromQuery] bool isAdded,[FromQuery] string userId)
    {
        return Ok(await userService.UpdateBookmark(guideId, userId, unitIndex, isAdded));
    }
    
    [HttpPut("guide/{guideId}/unit/{unitIndex}/exercise/{exerciseIndex}")]
    public async Task<ActionResult> UpdateExerciseCompletion(string guideId, int unitIndex, int exerciseIndex, [FromQuery] bool isCompleted, [FromQuery] string userId)
    {
        return Ok(await userService.UpdateExerciseCompletion(guideId, userId, unitIndex, exerciseIndex, isCompleted));
    }
}