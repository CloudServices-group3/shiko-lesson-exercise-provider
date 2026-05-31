using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Shiko.LessonExerciseProvider.Api.Models;
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

    [Fact]
    public async Task CreateLessonExercise_WithExistingOrderIndex_MovesExistingLessonsDown()
    {
        await _fixture.ResetDatabaseAsync();

        var courseId = Guid.NewGuid();

        await _fixture.SeedLessonExercisesAsync(
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
                Title = "Second lesson",
                DurationMinutes = 20,
                OrderIndex = 2
            },
            new LessonExercise
            {
                Id = Guid.NewGuid(),
                CourseId = courseId,
                Title = "Third lesson",
                DurationMinutes = 30,
                OrderIndex = 3
            });

        using var createRequest = new HttpRequestMessage(
            HttpMethod.Post,
            $"/api/admin/courses/{courseId}/lesson-exercises");

        createRequest.Headers.Authorization = new AuthenticationHeaderValue(
            "Bearer",
            JwtTokenFactory.CreateAdminToken());

        createRequest.Content = JsonContent.Create(new CreateLessonExerciseRequest(
            "New second lesson",
            15,
            2));

        var createResponse = await _fixture.Client.SendAsync(createRequest);

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        using var getRequest = new HttpRequestMessage(
            HttpMethod.Get,
            $"/api/admin/courses/{courseId}/lesson-exercises");

        getRequest.Headers.Authorization = new AuthenticationHeaderValue(
            "Bearer",
            JwtTokenFactory.CreateAdminToken());

        var getResponse = await _fixture.Client.SendAsync(getRequest);

        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        var lessons = await getResponse.Content
            .ReadFromJsonAsync<List<LessonExerciseResponse>>();

        Assert.NotNull(lessons);

        Assert.Collection(
            lessons.OrderBy(x => x.OrderIndex),
            first =>
            {
                Assert.Equal("First lesson", first.Title);
                Assert.Equal(1, first.OrderIndex);
            },
            second =>
            {
                Assert.Equal("New second lesson", second.Title);
                Assert.Equal(2, second.OrderIndex);
            },
            third =>
            {
                Assert.Equal("Second lesson", third.Title);
                Assert.Equal(3, third.OrderIndex);
            },
            fourth =>
            {
                Assert.Equal("Third lesson", fourth.Title);
                Assert.Equal(4, fourth.OrderIndex);
            });
    }
}