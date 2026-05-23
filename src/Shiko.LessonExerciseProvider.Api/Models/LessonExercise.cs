namespace Shiko.LessonExerciseProvider.Api.Models;

public class LessonExercise
{
    public Guid Id { get; set; }

    public Guid CourseId { get; set; }

    public string Title { get; set; } = null!;

    public int DurationMinutes { get; set; }

    public int OrderIndex { get; set; }
}