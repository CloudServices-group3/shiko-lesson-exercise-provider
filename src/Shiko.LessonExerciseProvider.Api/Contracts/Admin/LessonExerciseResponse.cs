namespace Shiko.LessonExerciseProvider.Api.Contracts.Admin;

public sealed record LessonExerciseResponse(
    Guid Id,
    Guid CourseId,
    string Title,
    int DurationMinutes,
    int OrderIndex,
    bool IsDeleted,
    DateTime? DeletedAtUtc
);