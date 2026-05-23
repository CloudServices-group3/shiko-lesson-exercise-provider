namespace Shiko.LessonExerciseProvider.Api.Models;

public class UserCourseProgress
{
    public Guid Id { get; set; }

    public string UserId { get; set; } = null!;

    public Guid CourseId { get; set; }

    public ICollection<UserCompletedLesson> CompletedLessons { get; set; } = [];
}