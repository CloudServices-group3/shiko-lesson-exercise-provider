using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Shiko.LessonExerciseProvider.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddSoftDeleteToLessonExercises : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAtUtc",
                schema: "lesson_exercise",
                table: "LessonExercises",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                schema: "lesson_exercise",
                table: "LessonExercises",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeletedAtUtc",
                schema: "lesson_exercise",
                table: "LessonExercises");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                schema: "lesson_exercise",
                table: "LessonExercises");
        }
    }
}
