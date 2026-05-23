using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Shiko.LessonExerciseProvider.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "lesson_exercise");

            migrationBuilder.CreateTable(
                name: "LessonExercises",
                schema: "lesson_exercise",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CourseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    DurationMinutes = table.Column<int>(type: "int", nullable: false),
                    OrderIndex = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LessonExercises", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserCourseProgresses",
                schema: "lesson_exercise",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    CourseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserCourseProgresses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserCompletedLessons",
                schema: "lesson_exercise",
                columns: table => new
                {
                    UserCourseProgressId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LessonExerciseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompletedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserCompletedLessons", x => new { x.UserCourseProgressId, x.LessonExerciseId });
                    table.ForeignKey(
                        name: "FK_UserCompletedLessons_LessonExercises_LessonExerciseId",
                        column: x => x.LessonExerciseId,
                        principalSchema: "lesson_exercise",
                        principalTable: "LessonExercises",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserCompletedLessons_UserCourseProgresses_UserCourseProgressId",
                        column: x => x.UserCourseProgressId,
                        principalSchema: "lesson_exercise",
                        principalTable: "UserCourseProgresses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LessonExercises_CourseId_OrderIndex",
                schema: "lesson_exercise",
                table: "LessonExercises",
                columns: new[] { "CourseId", "OrderIndex" });

            migrationBuilder.CreateIndex(
                name: "IX_UserCompletedLessons_LessonExerciseId",
                schema: "lesson_exercise",
                table: "UserCompletedLessons",
                column: "LessonExerciseId");

            migrationBuilder.CreateIndex(
                name: "IX_UserCourseProgresses_UserId_CourseId",
                schema: "lesson_exercise",
                table: "UserCourseProgresses",
                columns: new[] { "UserId", "CourseId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserCompletedLessons",
                schema: "lesson_exercise");

            migrationBuilder.DropTable(
                name: "LessonExercises",
                schema: "lesson_exercise");

            migrationBuilder.DropTable(
                name: "UserCourseProgresses",
                schema: "lesson_exercise");
        }
    }
}
