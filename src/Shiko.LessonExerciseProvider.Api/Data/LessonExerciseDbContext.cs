using Microsoft.EntityFrameworkCore;
using Shiko.LessonExerciseProvider.Api.Models;

namespace Shiko.LessonExerciseProvider.Api.Data;

public sealed class LessonExerciseDbContext(
    DbContextOptions<LessonExerciseDbContext> options) : DbContext(options)
{
    public DbSet<LessonExercise> LessonExercises => Set<LessonExercise>();

    public DbSet<UserCourseProgress> UserCourseProgresses => Set<UserCourseProgress>();

    public DbSet<UserCompletedLesson> UserCompletedLessons => Set<UserCompletedLesson>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(LessonExerciseDbContext).Assembly);
    }
}
