var builder = WebApplication.CreateBuilder(args);


builder.Services.AddOpenApi();

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

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
