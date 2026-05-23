using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shiko.LessonExerciseProvider.Api.Models;

namespace Shiko.LessonExerciseProvider.Api.Data.Configurations;

public class UserCourseProgressConfiguration : IEntityTypeConfiguration<UserCourseProgress>
{
    public void Configure(EntityTypeBuilder<UserCourseProgress> builder)
    {
        builder.ToTable("UserCourseProgresses", "lesson_exercise");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.UserId)
            .HasMaxLength(450)
            .IsRequired();

        builder.Property(x => x.CourseId)
            .IsRequired();

        builder.HasMany(x => x.CompletedLessons)
            .WithOne(x => x.UserCourseProgress)
            .HasForeignKey(x => x.UserCourseProgressId);

        builder.HasIndex(x => new { x.UserId, x.CourseId })
            .IsUnique();
    }
}