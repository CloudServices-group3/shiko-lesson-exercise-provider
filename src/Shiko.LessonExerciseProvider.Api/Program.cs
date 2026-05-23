using Microsoft.EntityFrameworkCore;
using Shiko.LessonExerciseProvider.Api.Data;
using Shiko.LessonExerciseProvider.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<LessonExerciseDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("LessonExerciseDatabase")));

builder.Services.AddScoped<ILessonExerciseService, LessonExerciseService>();

builder.Services.AddControllers();

builder.Services.AddOpenApi();

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapControllers();

app.MapGet("/health", () =>
    Results.Ok(new
    {
        status = "Healthy",
        service = "Shiko Lesson Exercise Provider",
        utc = DateTime.UtcNow
    }))
    .WithName("HealthCheck")
    .WithTags("Health");

app.Run();
