using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shiko.LessonExerciseProvider.Api.Models;

namespace Shiko.LessonExerciseProvider.Api.Data.Configurations;

public class LessonExerciseConfiguration : IEntityTypeConfiguration<LessonExercise>
{
    public void Configure(EntityTypeBuilder<LessonExercise> builder)
    {
        builder.ToTable("LessonExercises", "lesson_exercise");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.CourseId)
            .IsRequired();

        builder.Property(x => x.Title)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.DurationMinutes)
            .IsRequired();

        builder.Property(x => x.OrderIndex)
            .IsRequired();

        builder.HasIndex(x => new { x.CourseId, x.OrderIndex });
    }
}