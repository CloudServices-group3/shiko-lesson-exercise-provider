using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shiko.LessonExerciseProvider.Api.Contracts;
using Shiko.LessonExerciseProvider.Api.Services;

namespace Shiko.LessonExerciseProvider.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/courses/{courseId:guid}/lesson-exercises")]
public class LessonExercisesController : ControllerBase
{
    private readonly ILessonExerciseService _lessonExerciseService;

    public LessonExercisesController(ILessonExerciseService lessonExerciseService)
    {
        _lessonExerciseService = lessonExerciseService;
    }

    [HttpGet("me")]
    [ProducesResponseType(typeof(CourseLessonExercisesResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<CourseLessonExercisesResponse>> GetMyCourseLessonExercises(
        Guid courseId,
        CancellationToken cancellationToken)
    {
        var userId = GetUserId();

        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized();
        }

        var response = await _lessonExerciseService.GetCourseLessonExercisesAsync(
            courseId,
            userId,
            cancellationToken);

        return Ok(response);
    }

    [HttpPut("{lessonExerciseId:guid}/complete")]
    [ProducesResponseType(typeof(CourseLessonExercisesResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CourseLessonExercisesResponse>> CompleteLessonExercise(
        Guid courseId,
        Guid lessonExerciseId,
        CancellationToken cancellationToken)
    {
        var userId = GetUserId();

        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized();
        }

        var response = await _lessonExerciseService.CompleteLessonExerciseAsync(
            courseId,
            lessonExerciseId,
            userId,
            cancellationToken);

        if (response is null)
        {
            return NotFound();
        }

        return Ok(response);
    }

    private string? GetUserId()
    {
        return User.FindFirstValue("userId")
            ?? User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue("sub");
    }
}