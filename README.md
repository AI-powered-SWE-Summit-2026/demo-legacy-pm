# LegacyPM

LegacyPM is a deliberately old-fashioned project and task management sample built with ASP.NET Core MVC, a companion REST API, EF Core 5, and SQL Server LocalDB. It exists as a modernization demo: the app works, but it intentionally carries risky or outdated patterns that a Copilot modernization agent should flag.

## App purpose
- Track projects, milestones, resources, tasks, and time entries.
- Generate cached project reports from the MVC app.
- Expose project, task, and time-entry endpoints from a separate REST API.
- Provide a realistic .NET 5 sample for upgrade and remediation exercises.

## Project layout
- `LegacyPM.sln` - solution entry point.
- `LegacyPM.Web` - MVC UI, Razor views, legacy services, and background worker.
- `LegacyPM.Api` - REST API with inconsistent controller conventions.
- `LegacyPM.Core` - domain models, EF Core 5 `DbContext`, and shared service layer.
- `Database` - LocalDB schema + seed scripts.

## Setup instructions
1. From `D:\Code\SWE Summit\App4-LegacyPM`, run `dotnet restore`.
2. Create the database: `sqlcmd -S "(localdb)\MSSQLLocalDB" -i .\Database\CreateDatabase.sql`.
3. Seed the database: `sqlcmd -S "(localdb)\MSSQLLocalDB" -d LegacyPMDb -i .\Database\SeedData.sql`.
4. Build the solution: `dotnet build .\LegacyPM.sln`.
5. To run with `dotnet run`, install the ASP.NET Core 5 runtime (`Microsoft.AspNetCore.App` 5.0) or publish self-contained; the .NET 10 SDK can build `net5.0`, but this machine does not have the .NET 5 shared runtime installed.

## Intentional legacy patterns
1. **EOL target frameworks** - `LegacyPM.Core\LegacyPM.Core.csproj:1-12`, `LegacyPM.Web\LegacyPM.Web.csproj:1-12`, `LegacyPM.Api\LegacyPM.Api.csproj:1-12`
2. **Old Program + Startup hosting model** - `LegacyPM.Web\Program.cs:7-19`, `LegacyPM.Web\Startup.cs:23-52`, `LegacyPM.Api\Program.cs:7-19`, `LegacyPM.Api\Startup.cs:21-46`
3. **Unsafe `BinaryFormatter` switch + serialization** - `LegacyPM.Web\Program.cs:11`, `LegacyPM.Web\Services\ReportCacheService.cs:19-37`
4. **`DateTime.Now` everywhere instead of `DateTimeOffset.UtcNow`** - `LegacyPM.Core\Services\ProjectService.cs:80-97`, `LegacyPM.Web\Controllers\ReportController.cs:69-91`, `LegacyPM.Web\Views\Home\Index.cshtml:10`, `LegacyPM.Web\BackgroundServices\DeadlineReminderBackgroundService.cs:41`
5. **Blocking async with `Task.Result` / `.Wait()` / `GetAwaiter().GetResult()`** - `LegacyPM.Core\Services\ProjectService.cs:46-52`, `LegacyPM.Core\Services\ProjectService.cs:65-72`, `LegacyPM.Web\Controllers\HomeController.cs:28-47`, `LegacyPM.Web\Controllers\ProjectController.cs:19-25`, `LegacyPM.Web\Controllers\ProjectController.cs:83`, `LegacyPM.Web\Controllers\ReportController.cs:40-59`, `LegacyPM.Web\Services\EmailReminderService.cs:54-55`
6. **`new HttpClient()` anti-pattern** - `LegacyPM.Web\Services\ExternalHttpService.cs:8-17`
7. **Manual thread culture mutation** - `LegacyPM.Web\Services\CultureService.cs:8-22`, `LegacyPM.Web\Services\EmailReminderService.cs:22-25`
8. **Obsolete `SmtpClient` usage** - `LegacyPM.Web\Services\EmailReminderService.cs:43-46`
9. **`ReaderWriterLock` instead of `ReaderWriterLockSlim`** - `LegacyPM.Core\Services\ProjectService.cs:14-15`, `LegacyPM.Core\Services\ProjectService.cs:25-35`, `LegacyPM.Core\Services\ProjectService.cs:131-138`
10. **Nullable reference types intentionally not enabled** - `LegacyPM.Core\LegacyPM.Core.csproj:1-12` (no `<Nullable>enable</Nullable>` entry)
11. **Mixed JSON serializers and no shared options** - `LegacyPM.Api\Controllers\ProjectsApiController.cs:22-25`, `LegacyPM.Api\Controllers\TasksApiController.cs:37-39`, `LegacyPM.Api\Startup.cs:25-30`
12. **EF Core 5 enum conversion patterns** - `LegacyPM.Core\ProjectDbContext.cs:23-33`
13. **CancellationToken not propagated** - `LegacyPM.Web\BackgroundServices\DeadlineReminderBackgroundService.cs:23-42`
14. **Hardcoded culture strings** - `LegacyPM.Web\Services\CultureService.cs:10-17`, `LegacyPM.Web\BackgroundServices\DeadlineReminderBackgroundService.cs:46`
15. **Reflection-based report generation** - `LegacyPM.Web\Controllers\ReportController.cs:65-71`
16. **`[Serializable]` domain model for BinaryFormatter** - `LegacyPM.Core\Models\Report.cs:5-15`
17. **Inconsistent API controller conventions (`[ApiController]`, `Ok`, `Json`)** - `LegacyPM.Api\Controllers\ProjectsApiController.cs:9-25`, `LegacyPM.Api\Controllers\TasksApiController.cs:10-39`, `LegacyPM.Api\Controllers\TimeEntriesApiController.cs:8-32`
18. **Legacy Razor behaviors** - `LegacyPM.Web\Views\Home\Index.cshtml:10`, `LegacyPM.Web\Views\Home\Index.cshtml:61`

## What the GitHub Copilot modernization agent should detect
- Every project targets unsupported `.NET 5` and still uses separate `Program` / `Startup` bootstrapping.
- `BinaryFormatter`, `[Serializable]`, and the unsafe AppContext switch are still active.
- `DateTime.Now`, thread-level culture mutation, and hardcoded locale strings are spread through the UI, services, and background worker.
- Blocking async calls, `ReaderWriterLock`, `new HttpClient()`, and `SmtpClient` remain in production paths.
- API behavior is inconsistent: mixed `Controller` / `ControllerBase`, mixed serializers, mixed `Ok(...)` / `Json(...)`, and incomplete `[ApiController]` usage.
- EF Core uses older conversion patterns and the codebase does not use nullable reference types.
- The background service accepts a `CancellationToken` but does not pass it into inner async work.

## Expected modernization path
1. Upgrade every project from `net5.0` to `net8.0` or `net10.0`.
2. Replace `Program` + `Startup` with minimal hosting and centralize service registration.
3. Remove `BinaryFormatter`; switch report caching to safe JSON, MessagePack, or protobuf.
4. Replace `DateTime.Now` with `DateTimeOffset.UtcNow` and add request localization middleware.
5. Replace blocking async with full async flows, `IHttpClientFactory`, and proper `CancellationToken` propagation.
6. Replace `SmtpClient` with MailKit (or another supported client) and `ReaderWriterLock` with `ReaderWriterLockSlim` or concurrent collections.
7. Standardize JSON serialization, apply `[ApiController]` consistently, and unify API conventions.
8. Enable nullable reference types, update EF Core, add migrations, and introduce automated tests.

## Validation performed
- `dotnet build .\LegacyPM.sln` succeeded on the installed .NET 10 SDK.
- `CreateDatabase.sql` and `SeedData.sql` were executed successfully against `(localdb)\MSSQLLocalDB`.
- Seed verification confirmed: 4 projects, 5 resources, 15 tasks, 6 milestones, 20 time entries, 3 project members, and 5 notification logs.
