using Shiko.LessonExerciseProvider.Api.Contracts;

namespace Shiko.LessonExerciseProvider.Api.Services;

public interface ILessonExerciseService
{
    Task<CourseLessonExercisesResponse> GetCourseLessonExercisesAsync(
        Guid courseId,
        string userId,
        CancellationToken cancellationToken = default);

    Task<CourseLessonExercisesResponse?> CompleteLessonExerciseAsync(
        Guid courseId,
        Guid lessonExerciseId,
        string userId,
        CancellationToken cancellationToken = default);
}