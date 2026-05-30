using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Shiko.LessonExerciseProvider.Api.Data;

namespace Shiko.LessonExerciseProvider.Api.IntegrationTests.TestInfrastructure;

public sealed class LessonExerciseApiFactory : WebApplicationFactory<Program>
{
    private readonly string _connectionString;

    public LessonExerciseApiFactory(string connectionString)
    {
        _connectionString = connectionString;

        Environment.SetEnvironmentVariable(
            "ConnectionStrings__LessonExerciseDatabase",
            connectionString);

        Environment.SetEnvironmentVariable("Jwt__Issuer", "test-issuer");
        Environment.SetEnvironmentVariable("Jwt__Audience", "test-audience");
        Environment.SetEnvironmentVariable(
            "Jwt__SigningKey",
            "test-signing-key-for-integration-tests-123456789");
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<LessonExerciseDbContext>();
            services.RemoveAll<DbContextOptions<LessonExerciseDbContext>>();

            services.AddDbContext<LessonExerciseDbContext>(options =>
            {
                options.UseSqlServer(
                    _connectionString,
                    sqlOptions =>
                    {
                        sqlOptions.MigrationsHistoryTable(
                            "__EFMigrationsHistory",
                            "lesson_exercise");
                    });
            });
        });
    }

    public async Task ApplyMigrationsAsync()
    {
        using var scope = Services.CreateScope();

        var dbContext = scope.ServiceProvider
            .GetRequiredService<LessonExerciseDbContext>();

        await dbContext.Database.MigrateAsync();
    }
}