# Shiko Lesson Exercise Provider

Small ASP.NET Core Web API provider for lesson exercises and user lesson progress in Shiko LMS.

This provider handles lesson exercises for a course, lesson duration, total lesson count, total lesson duration, and which lessons a logged-in user has completed.

Uses EF Core with Azure SQL. Tables are stored in the `lesson_exercise` schema because my providers share the same Azure SQL database.

Connection strings and JWT settings are stored with user secrets locally and environment variables in Azure.

## Live API

### Base URL

https://shiko-lesson-exercise-provider.azurewebsites.net

The base URL is only the root address for the API. It does not have its own page, so opening it directly can return 404. Use the endpoints below instead.

### Health check

https://shiko-lesson-exercise-provider.azurewebsites.net/health

### Scalar

https://shiko-lesson-exercise-provider.azurewebsites.net/scalar

### OpenAPI JSON

https://shiko-lesson-exercise-provider.azurewebsites.net/openapi/v1.json

## Responsibility

The Lesson Exercise Provider owns:

* lesson exercises for a course
* lesson duration
* total lesson count
* total lesson duration
* completed lessons per user
* course progress data that other modules can use later

The Course Provider owns basic course data like title and image.

The Course Details Provider owns course overview/about text and key points.

The Lesson Exercise Provider only stores `CourseId` as an external reference. It does not have a database relationship to the Course Provider.

## Endpoints

### User endpoints

These require JWT Bearer auth.

GET /api/courses/{courseId}/lesson-exercises/me

PUT /api/courses/{courseId}/lesson-exercises/{lessonExerciseId}/complete

The user id is read from the JWT token. Frontend should not send `userId` in the request body.

GET returns all active lesson exercises for the selected course and marks which lessons the logged-in user has completed.

PUT marks one active lesson as completed for the logged-in user. Calling the same endpoint again does not create duplicates.

Soft-deleted lessons are not returned, counted or possible to complete.

### Admin endpoints

These require JWT Bearer auth with the `Admin` role.

GET /api/admin/courses/{courseId}/lesson-exercises

POST /api/admin/courses/{courseId}/lesson-exercises

PUT /api/admin/courses/{courseId}/lesson-exercises/{lessonExerciseId}

DELETE /api/admin/courses/{courseId}/lesson-exercises/{lessonExerciseId}

Admin endpoints are used to manage lesson exercises for a course.

DELETE uses soft delete. The lesson is hidden from both admin and user lists, and it is no longer counted in course lesson totals. The row stays in the database so existing progress data is not broken.

## Local config

Local development uses SQL Server LocalDB.

The local database connection string is stored in `appsettings.Development.json`:

ConnectionStrings:LessonExerciseDatabase

Set JWT config with user secrets:

dotnet user-secrets set "Jwt:Issuer" "your-issuer" --project .\src\Shiko.LessonExerciseProvider.Api

dotnet user-secrets set "Jwt:Audience" "your-audience" --project .\src\Shiko.LessonExerciseProvider.Api

dotnet user-secrets set "Jwt:SigningKey" "your-signing-key" --project .\src\Shiko.LessonExerciseProvider.Api

## Azure config

Azure Web App uses environment variables and app settings.

The database connection string is stored as:

LessonExerciseDatabase

JWT app settings are stored as:

Jwt__Issuer

Jwt__Audience

Jwt__SigningKey

## Database

Tables are stored in the `lesson_exercise` schema:

* lesson_exercise.LessonExercises
* lesson_exercise.UserCourseProgresses
* lesson_exercise.UserCompletedLessons
* lesson_exercise.__EFMigrationsHistory

Run migrations with:

dotnet ef database update --project .\src\Shiko.LessonExerciseProvider.Api --startup-project .\src\Shiko.LessonExerciseProvider.Api --context LessonExerciseDbContext

## Run locally

Run the API with:

dotnet run --project .\src\Shiko.LessonExerciseProvider.Api --launch-profile https

Scalar opens at:

https://localhost:7123/scalar

## Current test data

For demo/testing, lesson data can be created through the admin endpoints.

Example CourseId used during local testing:

bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb

Admin can create lesson exercises for this course through:

POST /api/admin/courses/bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb/lesson-exercises

Example request body:

{
"title": "Introduction to the course",
"durationMinutes": 20,
"orderIndex": 1
}

## Tested locally

The following flows have been tested locally:

* admin GET returns active lessons for a course
* admin POST creates a lesson exercise
* admin PUT updates a lesson exercise
* admin DELETE soft-deletes a lesson exercise
* user GET returns active lessons and progress
* soft-deleted lessons are not returned or counted
* soft-deleted lessons cannot be completed

Expected security behavior:

* admin endpoints return 401 without a token
* admin endpoints require a JWT token with the Admin role
* user endpoints require a valid JWT token

## Notes

Course Provider currently returns `lessonCount` and `duration`, but Lesson Exercise Provider should be the source of truth for lesson count and lesson duration later.

The current MVP flow supports reading lessons, marking lessons as completed, and managing course lesson exercises through admin endpoints.

Completed lessons are stored per user and course. If no completed lessons exist for a user, the course can later be treated as not started. If some but not all lessons are completed, it can be treated as in progress. If all lessons are completed, it can be treated as finished.

Lessons use soft delete because users may already have progress connected to a lesson. When admin deletes a lesson, it should disappear from the course and stop counting toward progress totals, but existing progress data should not be broken.

A future Course Provider delete/archive flow could publish an event that Lesson Exercise Provider listens to. In that case, Lesson Exercise Provider can soft-delete lesson exercises for that course. This is not implemented now.
