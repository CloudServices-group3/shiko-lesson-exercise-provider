using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Shiko.LessonExerciseProvider.Api.Data;
using Shiko.LessonExerciseProvider.Api.Models;
using Testcontainers.MsSql;
using Xunit;

namespace Shiko.LessonExerciseProvider.Api.IntegrationTests.TestInfrastructure;

public sealed class LessonExerciseIntegrationTestFixture : IAsyncLifetime
{
    private readonly MsSqlContainer _sqlServer = new MsSqlBuilder(
        "mcr.microsoft.com/mssql/server:2022-CU14-ubuntu-22.04")
        .Build();

    private LessonExerciseApiFactory? _factory;

    public HttpClient Client { get; private set; } = null!;

    public IServiceProvider Services => _factory!.Services;

    public async Task InitializeAsync()
    {
        await _sqlServer.StartAsync();

        _factory = new LessonExerciseApiFactory(_sqlServer.GetConnectionString());

        await _factory.ApplyMigrationsAsync();

        Client = _factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            BaseAddress = new Uri("https://localhost")
        });
    }

    public async Task DisposeAsync()
    {
        Client?.Dispose();

        _factory?.Dispose();

        await _sqlServer.DisposeAsync();
    }

    public async Task ResetDatabaseAsync()
    {
        using var scope = Services.CreateScope();

        var dbContext = scope.ServiceProvider
            .GetRequiredService<LessonExerciseDbContext>();

        dbContext.UserCompletedLessons.RemoveRange(dbContext.UserCompletedLessons);
        dbContext.UserCourseProgresses.RemoveRange(dbContext.UserCourseProgresses);
        dbContext.LessonExercises.RemoveRange(dbContext.LessonExercises);

        await dbContext.SaveChangesAsync();
    }

    public async Task SeedLessonExercisesAsync(params LessonExercise[] lessonExercises)
    {
        using var scope = Services.CreateScope();

        var dbContext = scope.ServiceProvider
            .GetRequiredService<LessonExerciseDbContext>();

        dbContext.LessonExercises.AddRange(lessonExercises);

        await dbContext.SaveChangesAsync();
    }
}