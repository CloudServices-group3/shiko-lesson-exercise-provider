using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using Shiko.LessonExerciseProvider.Api.Data;
using Shiko.LessonExerciseProvider.Api.OpenApi;
using Shiko.LessonExerciseProvider.Api.Services;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
    options.AddOperationTransformer<AuthOperationTransformer>();
});

builder.Services.AddControllers();

var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var jwtAudience = builder.Configuration["Jwt:Audience"];
var jwtSigningKey = builder.Configuration["Jwt:SigningKey"];

if (string.IsNullOrWhiteSpace(jwtIssuer))
{
    throw new InvalidOperationException("JWT issuer is not configured.");
}

if (string.IsNullOrWhiteSpace(jwtAudience))
{
    throw new InvalidOperationException("JWT audience is not configured.");
}

if (string.IsNullOrWhiteSpace(jwtSigningKey))
{
    throw new InvalidOperationException("JWT signing key is not configured.");
}

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtIssuer,

            ValidateAudience = true,
            ValidAudience = jwtAudience,

            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSigningKey)),

            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromSeconds(30)
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddDbContext<LessonExerciseDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("LessonExerciseDatabase")));

builder.Services.AddScoped<ILessonExerciseService, LessonExerciseService>();

var app = builder.Build();

app.MapOpenApi();

app.MapScalarApiReference(options =>
{
    options.WithTitle("Shiko Lesson Exercise Provider API");
});

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

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