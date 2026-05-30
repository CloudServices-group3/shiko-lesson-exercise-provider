using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Shiko.LessonExerciseProvider.Api.Contracts;
using Shiko.LessonExerciseProvider.Api.IntegrationTests.TestInfrastructure;
using Shiko.LessonExerciseProvider.Api.Models;
using Xunit;

namespace Shiko.LessonExerciseProvider.Api.IntegrationTests.Endpoints;

public sealed class LessonExerciseEndpointTests
    : IClassFixture<LessonExerciseIntegrationTestFixture>
{
    private readonly LessonExerciseIntegrationTestFixture _fixture;

    public LessonExerciseEndpointTests(LessonExerciseIntegrationTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task GetLessonExercises_WithUserToken_ReturnsActiveLessons()
    {
        await _fixture.ResetDatabaseAsync();

        var courseId = Guid.NewGuid();

        await _fixture.SeedLessonExercisesAsync(
            new LessonExercise
            {
                Id = Guid.NewGuid(),
                CourseId = courseId,
                Title = "Second lesson",
                DurationMinutes = 20,
                OrderIndex = 2
            },
            new LessonExercise
            {
                Id = Guid.NewGuid(),
                CourseId = courseId,
                Title = "First lesson",
                DurationMinutes = 10,
                OrderIndex = 1
            },
            new LessonExercise
            {
                Id = Guid.NewGuid(),
                CourseId = courseId,
                Title = "Deleted lesson",
                DurationMinutes = 30,
                OrderIndex = 3,
                IsDeleted = true,
                DeletedAtUtc = DateTime.UtcNow
            });

        using var request = new HttpRequestMessage(
            HttpMethod.Get,
            $"/api/courses/{courseId}/lesson-exercises/me");

        request.Headers.Authorization = new AuthenticationHeaderValue(
            "Bearer",
            JwtTokenFactory.CreateUserToken());

        var response = await _fixture.Client.SendAsync(request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content
            .ReadFromJsonAsync<CourseLessonExercisesResponse>();

        Assert.NotNull(result);
        Assert.Equal(courseId, result.CourseId);
        Assert.Equal(2, result.TotalLessons);
        Assert.Equal(0, result.CompletedLessons);
        Assert.Equal(30, result.TotalDurationMinutes);

        Assert.Collection(
            result.Lessons,
            first => Assert.Equal("First lesson", first.Title),
            second => Assert.Equal("Second lesson", second.Title));

        Assert.DoesNotContain(
            result.Lessons,
            lesson => lesson.Title == "Deleted lesson");
    }
}