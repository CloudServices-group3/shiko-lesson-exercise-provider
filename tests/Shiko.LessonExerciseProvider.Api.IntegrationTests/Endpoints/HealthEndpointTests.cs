using System.Net;
using Shiko.LessonExerciseProvider.Api.IntegrationTests.TestInfrastructure;
using Xunit;

namespace Shiko.LessonExerciseProvider.Api.IntegrationTests.Endpoints;

public sealed class HealthEndpointTests
    : IClassFixture<LessonExerciseIntegrationTestFixture>
{
    private readonly LessonExerciseIntegrationTestFixture _fixture;

    public HealthEndpointTests(LessonExerciseIntegrationTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task Health_ReturnsOk()
    {
        var response = await _fixture.Client.GetAsync("/health");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}