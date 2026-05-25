using Microsoft.EntityFrameworkCore;
using Shiko.LessonExerciseProvider.Api.Contracts;
using Shiko.LessonExerciseProvider.Api.Data;
using Shiko.LessonExerciseProvider.Api.Models;

namespace Shiko.LessonExerciseProvider.Api.Services;

public class LessonExerciseService : ILessonExerciseService
{
    private readonly LessonExerciseDbContext _context;

    public LessonExerciseService(LessonExerciseDbContext context)
    {
        _context = context;
    }

    public async Task<CourseLessonExercisesResponse> GetCourseLessonExercisesAsync(
        Guid courseId,
        string userId,
        CancellationToken cancellationToken = default)
    {
        var lessons = await _context.LessonExercises
            .Where(x => x.CourseId == courseId)
            .OrderBy(x => x.OrderIndex)
            .ToListAsync(cancellationToken);

        var progress = await _context.UserCourseProgresses
            .Include(x => x.CompletedLessons)
            .FirstOrDefaultAsync(
                x => x.CourseId == courseId && x.UserId == userId,
                cancellationToken);

        return BuildResponse(courseId, lessons, progress);
    }

    public async Task<CourseLessonExercisesResponse?> CompleteLessonExerciseAsync(
        Guid courseId,
        Guid lessonExerciseId,
        string userId,
        CancellationToken cancellationToken = default)
    {
        var lessonExists = await _context.LessonExercises
            .AnyAsync(
                x => x.Id == lessonExerciseId && x.CourseId == courseId,
                cancellationToken);

        if (!lessonExists)
        {
            return null;
        }

        var progress = await _context.UserCourseProgresses
            .Include(x => x.CompletedLessons)
            .FirstOrDefaultAsync(
                x => x.CourseId == courseId && x.UserId == userId,
                cancellationToken);

        if (progress is null)
        {
            progress = new UserCourseProgress
            {
                Id = Guid.NewGuid(),
                CourseId = courseId,
                UserId = userId
            };

            _context.UserCourseProgresses.Add(progress);
        }

        var alreadyCompleted = progress.CompletedLessons
            .Any(x => x.LessonExerciseId == lessonExerciseId);

        if (!alreadyCompleted)
        {
            progress.CompletedLessons.Add(new UserCompletedLesson
            {
                UserCourseProgressId = progress.Id,
                LessonExerciseId = lessonExerciseId,
                CompletedAtUtc = DateTime.UtcNow
            });

            await _context.SaveChangesAsync(cancellationToken);
        }

        return await GetCourseLessonExercisesAsync(courseId, userId, cancellationToken);
    }

    private static CourseLessonExercisesResponse BuildResponse(
        Guid courseId,
        List<LessonExercise> lessons,
        UserCourseProgress? progress)
    {
        var completedLessons = progress?.CompletedLessons ?? [];

        return new CourseLessonExercisesResponse
        {
            CourseId = courseId,
            TotalLessons = lessons.Count,
            CompletedLessons = completedLessons.Count,
            TotalDurationMinutes = lessons.Sum(x => x.DurationMinutes),
            Lessons = lessons
                .Select(lesson =>
                {
                    var completedLesson = completedLessons
                        .FirstOrDefault(x => x.LessonExerciseId == lesson.Id);

                    return new LessonExerciseItemResponse
                    {
                        Id = lesson.Id,
                        Title = lesson.Title,
                        DurationMinutes = lesson.DurationMinutes,
                        OrderIndex = lesson.OrderIndex,
                        IsCompleted = completedLesson is not null,
                        CompletedAtUtc = completedLesson?.CompletedAtUtc
                    };
                })
                .ToList()
        };
    }
}