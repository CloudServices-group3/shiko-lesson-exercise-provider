namespace Shiko.LessonExerciseProvider.Api.Models;

public class UserCompletedLesson
{
    public Guid UserCourseProgressId { get; set; }

    public Guid LessonExerciseId { get; set; }

    public DateTime CompletedAtUtc { get; set; }

    public UserCourseProgress UserCourseProgress { get; set; } = null!;

    public LessonExercise LessonExercise { get; set; } = null!;
}