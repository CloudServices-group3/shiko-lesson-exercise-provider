using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shiko.LessonExerciseProvider.Api.Models;

namespace Shiko.LessonExerciseProvider.Api.Data.Configurations;

public class UserCompletedLessonConfiguration : IEntityTypeConfiguration<UserCompletedLesson>
{
    public void Configure(EntityTypeBuilder<UserCompletedLesson> builder)
    {
        builder.ToTable("UserCompletedLessons", "lesson_exercise");

        builder.HasKey(x => new { x.UserCourseProgressId, x.LessonExerciseId });

        builder.Property(x => x.CompletedAtUtc)
            .IsRequired();

        builder.HasOne(x => x.UserCourseProgress)
            .WithMany(x => x.CompletedLessons)
            .HasForeignKey(x => x.UserCourseProgressId);

        builder.HasOne(x => x.LessonExercise)
            .WithMany()
            .HasForeignKey(x => x.LessonExerciseId);
    }
}