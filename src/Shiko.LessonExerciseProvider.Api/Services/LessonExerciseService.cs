using Microsoft.EntityFrameworkCore;
using Shiko.LessonExerciseProvider.Api.Contracts;
using Shiko.LessonExerciseProvider.Api.Data;
using Shiko.LessonExerciseProvider.Api.Models;
using Shiko.LessonExerciseProvider.Api.Contracts.Admin;

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
            .Where(x => x.CourseId == courseId && !x.IsDeleted)
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
                x => x.Id == lessonExerciseId
                && x.CourseId == courseId
                && !x.IsDeleted,
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

    public async Task<IReadOnlyList<LessonExerciseResponse>> GetAdminLessonExercisesAsync(
    Guid courseId,
    CancellationToken cancellationToken = default)
    {
        var lessons = await _context.LessonExercises
            .Where(x => x.CourseId == courseId && !x.IsDeleted)
            .OrderBy(x => x.OrderIndex)
            .ToListAsync(cancellationToken);

        return lessons
            .Select(ToAdminResponse)
            .ToList();
    }

    public async Task<LessonExerciseResponse> CreateLessonExerciseAsync(
        Guid courseId,
        CreateLessonExerciseRequest request,
        CancellationToken cancellationToken = default)
    {
        var lesson = new LessonExercise
        {
            Id = Guid.NewGuid(),
            CourseId = courseId,
            Title = request.Title.Trim(),
            DurationMinutes = request.DurationMinutes,
            OrderIndex = request.OrderIndex
        };

        _context.LessonExercises.Add(lesson);

        await _context.SaveChangesAsync(cancellationToken);

        return ToAdminResponse(lesson);
    }

    public async Task<LessonExerciseResponse?> UpdateLessonExerciseAsync(
        Guid courseId,
        Guid lessonExerciseId,
        UpdateLessonExerciseRequest request,
        CancellationToken cancellationToken = default)
    {
        var lesson = await _context.LessonExercises
            .FirstOrDefaultAsync(
                x => x.Id == lessonExerciseId
                    && x.CourseId == courseId
                    && !x.IsDeleted,
                cancellationToken);

        if (lesson is null)
        {
            return null;
        }

        lesson.Title = request.Title.Trim();
        lesson.DurationMinutes = request.DurationMinutes;
        lesson.OrderIndex = request.OrderIndex;

        await _context.SaveChangesAsync(cancellationToken);

        return ToAdminResponse(lesson);
    }

    public async Task<bool> DeleteLessonExerciseAsync(
        Guid courseId,
        Guid lessonExerciseId,
        CancellationToken cancellationToken = default)
    {
        var lesson = await _context.LessonExercises
            .FirstOrDefaultAsync(
                x => x.Id == lessonExerciseId
                    && x.CourseId == courseId
                    && !x.IsDeleted,
                cancellationToken);

        if (lesson is null)
        {
            return false;
        }

        lesson.IsDeleted = true;
        lesson.DeletedAtUtc = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
    private static CourseLessonExercisesResponse BuildResponse(
        Guid courseId,
        List<LessonExercise> lessons,
        UserCourseProgress? progress)
    {
        var lessonIds = lessons
            .Select(x => x.Id)
            .ToHashSet();

        var completedLessons = progress?.CompletedLessons
            .Where(x => lessonIds.Contains(x.LessonExerciseId))
            .ToList() ?? [];

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

    private static LessonExerciseResponse ToAdminResponse(LessonExercise lesson)
    {
        return new LessonExerciseResponse(
            lesson.Id,
            lesson.CourseId,
            lesson.Title,
            lesson.DurationMinutes,
            lesson.OrderIndex,
            lesson.IsDeleted,
            lesson.DeletedAtUtc);
    }
}