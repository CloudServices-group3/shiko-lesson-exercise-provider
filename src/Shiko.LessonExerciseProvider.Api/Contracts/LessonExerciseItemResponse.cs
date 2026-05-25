namespace Shiko.LessonExerciseProvider.Api.Contracts;

public class LessonExerciseItemResponse
{
    public Guid Id { get; set; }

    public string Title { get; set; } = null!;

    public int DurationMinutes { get; set; }

    public int OrderIndex { get; set; }

    public bool IsCompleted { get; set; }

    public DateTime? CompletedAtUtc { get; set; }
}