namespace Shiko.LessonExerciseProvider.Api.Contracts;

public class CourseLessonExercisesResponse
{
    public Guid CourseId { get; set; }

    public int TotalLessons { get; set; }

    public int CompletedLessons { get; set; }

    public int TotalDurationMinutes { get; set; }

    public List<LessonExerciseItemResponse> Lessons { get; set; } = [];
}