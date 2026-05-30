using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shiko.LessonExerciseProvider.Api.Contracts.Admin;
using Shiko.LessonExerciseProvider.Api.Services;

namespace Shiko.LessonExerciseProvider.Api.Controllers;

[ApiController]
[Authorize(Roles = "Admin")]
[Route("api/admin/courses/{courseId:guid}/lesson-exercises")]
public class AdminLessonExercisesController : ControllerBase
{
    private readonly ILessonExerciseService _lessonExerciseService;

    public AdminLessonExercisesController(ILessonExerciseService lessonExerciseService)
    {
        _lessonExerciseService = lessonExerciseService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<LessonExerciseResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<IReadOnlyList<LessonExerciseResponse>>> GetLessonExercises(
        Guid courseId,
        CancellationToken cancellationToken)
    {
        var response = await _lessonExerciseService.GetAdminLessonExercisesAsync(
            courseId,
            cancellationToken);

        return Ok(response);
    }

    [HttpPost]
    [ProducesResponseType(typeof(LessonExerciseResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<LessonExerciseResponse>> CreateLessonExercise(
        Guid courseId,
        CreateLessonExerciseRequest request,
        CancellationToken cancellationToken)
    {
        var response = await _lessonExerciseService.CreateLessonExerciseAsync(
            courseId,
            request,
            cancellationToken);

        return CreatedAtAction(
            nameof(GetLessonExercises),
            new { courseId },
            response);
    }

    [HttpPut("{lessonExerciseId:guid}")]
    [ProducesResponseType(typeof(LessonExerciseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<LessonExerciseResponse>> UpdateLessonExercise(
        Guid courseId,
        Guid lessonExerciseId,
        UpdateLessonExerciseRequest request,
        CancellationToken cancellationToken)
    {
        var response = await _lessonExerciseService.UpdateLessonExerciseAsync(
            courseId,
            lessonExerciseId,
            request,
            cancellationToken);

        if (response is null)
        {
            return NotFound();
        }

        return Ok(response);
    }

    [HttpDelete("{lessonExerciseId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteLessonExercise(
        Guid courseId,
        Guid lessonExerciseId,
        CancellationToken cancellationToken)
    {
        var deleted = await _lessonExerciseService.DeleteLessonExerciseAsync(
            courseId,
            lessonExerciseId,
            cancellationToken);

        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }
}