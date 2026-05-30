using Shiko.LessonExerciseProvider.Api.Contracts;
using Shiko.LessonExerciseProvider.Api.Contracts.Admin;

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

    Task<IReadOnlyList<LessonExerciseResponse>> GetAdminLessonExercisesAsync(
        Guid courseId,
        CancellationToken cancellationToken = default);

    Task<LessonExerciseResponse> CreateLessonExerciseAsync(
        Guid courseId,
        CreateLessonExerciseRequest request,
        CancellationToken cancellationToken = default);

    Task<LessonExerciseResponse?> UpdateLessonExerciseAsync(
        Guid courseId,
        Guid lessonExerciseId,
        UpdateLessonExerciseRequest request,
        CancellationToken cancellationToken = default);

    Task<bool> DeleteLessonExerciseAsync(
        Guid courseId,
        Guid lessonExerciseId,
        CancellationToken cancellationToken = default);
}