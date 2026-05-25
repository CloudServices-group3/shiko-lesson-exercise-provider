# Shiko Lesson Exercise Provider

Small ASP.NET Core Web API provider for lesson exercises and user lesson progress in Shiko LMS.

This provider handles lesson exercises for a course, lesson duration, total lesson count, total lesson duration, and which lessons a logged-in user has completed.

Uses EF Core with Azure SQL. Tables are stored in the lesson_exercise schema because my providers share the same Azure SQL database.

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

- lesson exercises for a course
- lesson duration
- total lesson count
- total lesson duration
- completed lessons per user
- course progress data that other modules can use later

The Course Provider owns course data like title, image and description.

The Lesson Exercise Provider only stores CourseId as an external reference. It does not have a database relationship to the Course Provider.

## Endpoints

### Auth protected endpoints

These require JWT Bearer auth.

GET /api/courses/{courseId}/lesson-exercises/me

PUT /api/courses/{courseId}/lesson-exercises/{lessonExerciseId}/complete

The user id is read from the JWT token. Frontend should not send userId in the request body.

GET returns all lesson exercises for the selected course and marks which lessons the logged-in user has completed.

PUT marks one lesson as completed for the logged-in user. Calling the same endpoint again does not create duplicates.

## Local config

Local development uses SQL Server LocalDB.

The local database connection string is stored in appsettings.Development.json:

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

Tables are stored in the lesson_exercise schema:

- lesson_exercise.LessonExercises
- lesson_exercise.UserCourseProgresses
- lesson_exercise.UserCompletedLessons
- lesson_exercise.__EFMigrationsHistory

Run migrations with:

dotnet ef database update --project .\src\Shiko.LessonExerciseProvider.Api --startup-project .\src\Shiko.LessonExerciseProvider.Api --context LessonExerciseDbContext

## Run locally

Run the API with:

dotnet run --project .\src\Shiko.LessonExerciseProvider.Api --launch-profile https

Scalar opens at:

https://localhost:7123/scalar

## Current test data

For demo/testing, lesson data has been inserted manually for the Digital Marketing course.

CourseId:

bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb

The course currently has 5 lesson exercises with a total duration of 78 minutes.

## Notes

Course Provider currently returns lessonCount and duration, but Lesson Exercise Provider should be the source of truth for lesson count and lesson duration later.

Admin create, update and delete for lessons is not built yet. That belongs to a later admin flow.

The current MVP flow supports reading lessons and marking lessons as completed.

Completed lessons are stored per user and course. If no completed lessons exist for a user, the course can later be treated as not started. If some but not all lessons are completed, it can be treated as in progress. If all lessons are completed, it can be treated as finished.
