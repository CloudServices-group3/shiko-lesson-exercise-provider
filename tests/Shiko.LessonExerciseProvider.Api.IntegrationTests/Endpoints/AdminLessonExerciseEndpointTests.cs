using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Shiko.LessonExerciseProvider.Api.Contracts.Admin;
using Shiko.LessonExerciseProvider.Api.IntegrationTests.TestInfrastructure;

namespace Shiko.LessonExerciseProvider.Api.IntegrationTests.Endpoints;

public sealed class AdminLessonExerciseEndpointTests
    : IClassFixture<LessonExerciseIntegrationTestFixture>
{
    private readonly LessonExerciseIntegrationTestFixture _fixture;

    public AdminLessonExerciseEndpointTests(LessonExerciseIntegrationTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task GetLessonExercises_WithoutToken_ReturnsUnauthorized()
    {
        await _fixture.ResetDatabaseAsync();

        var courseId = Guid.NewGuid();

        var response = await _fixture.Client.GetAsync(
            $"/api/admin/courses/{courseId}/lesson-exercises");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetLessonExercises_WithUserToken_ReturnsForbidden()
    {
        await _fixture.ResetDatabaseAsync();

        var courseId = Guid.NewGuid();

        using var request = new HttpRequestMessage(
            HttpMethod.Get,
            $"/api/admin/courses/{courseId}/lesson-exercises");

        request.Headers.Authorization = new AuthenticationHeaderValue(
            "Bearer",
            JwtTokenFactory.CreateUserToken());

        var response = await _fixture.Client.SendAsync(request);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task GetLessonExercises_WithAdminToken_ReturnsOk()
    {
        await _fixture.ResetDatabaseAsync();

        var courseId = Guid.NewGuid();

        using var request = new HttpRequestMessage(
            HttpMethod.Get,
            $"/api/admin/courses/{courseId}/lesson-exercises");

        request.Headers.Authorization = new AuthenticationHeaderValue(
            "Bearer",
            JwtTokenFactory.CreateAdminToken());

        var response = await _fixture.Client.SendAsync(request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task CreateLessonExercise_WithAdminToken_ReturnsCreated()
    {
        await _fixture.ResetDatabaseAsync();

        var courseId = Guid.NewGuid();

        using var request = new HttpRequestMessage(
            HttpMethod.Post,
            $"/api/admin/courses/{courseId}/lesson-exercises");

        request.Headers.Authorization = new AuthenticationHeaderValue(
            "Bearer",
            JwtTokenFactory.CreateAdminToken());

        request.Content = JsonContent.Create(new CreateLessonExerciseRequest(
            "Introduction lesson",
            20,
            1));

        var response = await _fixture.Client.SendAsync(request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var lesson = await response.Content
            .ReadFromJsonAsync<LessonExerciseResponse>();

        Assert.NotNull(lesson);
        Assert.Equal(courseId, lesson.CourseId);
        Assert.Equal("Introduction lesson", lesson.Title);
        Assert.Equal(20, lesson.DurationMinutes);
        Assert.Equal(1, lesson.OrderIndex);
        Assert.False(lesson.IsDeleted);
        Assert.Null(lesson.DeletedAtUtc);
    }
}