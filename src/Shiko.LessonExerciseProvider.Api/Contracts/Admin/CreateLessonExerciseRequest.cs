using System.ComponentModel.DataAnnotations;

namespace Shiko.LessonExerciseProvider.Api.Contracts.Admin;

public sealed record CreateLessonExerciseRequest(
    [param: Required]
    [param: StringLength(200, MinimumLength = 1)]
    string Title,

    [param: Range(1, 600)]
    int DurationMinutes,

    [param: Range(1, int.MaxValue)]
    int OrderIndex
);