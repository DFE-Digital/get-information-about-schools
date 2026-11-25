# Progress details — 04.06-cross-closure-validation-and-gates

## Summary
- Completed cross-closure validation for Task 04 migration outputs.
- Stabilized remaining runtime failures in the UI unit-test closure.
- Confirmed full UI unit-test pass and successful workspace build after fixes.

## Key changes validated
- `Web/Edubase.Web.UI/Areas/Establishments/Controllers/EstablishmentsSearchController.cs`
  - Null-safe TempData access in search flow.
  - Download flow parity adjustments (custom field load behavior and route-style redirect result).
- `Web/Edubase.Web.UI/Controllers/Api/SqlLaNameCodeRepository.cs`
  - Localdev connection-string fallback via app settings for compatibility.
- `Web/Edubase.Web.UIUnitTests/Compatibility/SystemWebMvcCompatibility.cs`
  - Improved principal/query adaptation for legacy MVC-style tests on ASP.NET Core.
- `Web/Edubase.Web.UIUnitTests/Areas/Establishments/Controllers/EstablishmentsSearchControllerTests.cs`
  - Updated compatibility-sensitive setups to match adapted principals.
- `Web/Edubase.Web.UIUnitTests/Controllers/Api/SqlLaNameCodeRepositoryTests.cs`
  - Removed brittle reflection dependency; aligned expectations with modern runtime behavior.

## Validation evidence
- `dotnet test Web/Edubase.Web.UIUnitTests/Edubase.Web.UIUnitTests.csproj -c Release --no-build`
  - Result: **Passed** (887 passed, 0 failed).
- Workspace build verification via IDE build tool (`run_build`)
  - Result: **Build successful**.

## 2026-11-19 recheck evidence
- Migration-surface recheck for framework targeting:
  - All solution project files currently declare `<TargetFramework>net10.0</TargetFramework>`.
- Solution build recheck:
  - `dotnet build .\Web\EdubaseWeb.sln -c Release`
  - Result: **Build succeeded** (`0 Error(s)`, `91 Warning(s)`).
  - Initial failed attempt was caused by a locked output DLL from a running `Edubase.Web.UI` process; re-run succeeded after stopping the locking process.
- Application startup recheck:
  - `dotnet run --project .\Web\Edubase.Web.UI\Edubase.Web.UI.csproj -c Release --no-build --no-launch-profile`
  - Result: **Host started successfully** (Kestrel listening; `/health` returned HTTP 200).
  - Initial note at recheck time: browsing `/` returned HTTP 500 due missing ASP.NET Core DI wiring.

## 2026-11-20 runtime wiring follow-up
- `Web/Edubase.Web.UI/Controllers/HomeController.cs`
  - Removed constructor-time dependencies that were not required for homepage rendering and moved blob/news usage to action-level service injection.
- `Web/Edubase.Web.UI/App_Start/CoreServiceRegistration.cs`
  - Added scoped registrations for `DbConnection` (`SqlConnection`), `FrontEndDbContext`, and `INewsArticleRepository` (`SqlNewsArticleRepository`).
  - Added guarded `DbProviderFactories.RegisterFactory("Microsoft.Data.SqlClient", SqlClientFactory.Instance)` for EF6 provider resolution on net10.

## 2026-11-20 follow-up validation
- `dotnet build .\Web\EdubaseWeb.sln -c Release`
  - Result: **Build succeeded** (`0 Error(s)`, `86 Warning(s)`).
- Startup command still reaches listening state (`Now listening on: http://localhost:5075`), but in-session HTTP probing of `localhost` endpoints was blocked by sandbox network isolation (`Invoke-WebRequest : Unable to connect to the remote server`), so final `/` response-code verification could not be completed inside the harness.

## 2026-11-24 continued recheck (post-reboot)
- `Web/Edubase.AcceptanceTests/Edubase.AcceptanceTests.csproj`
  - Added `<Compile Remove="obj\**" />` and `<Compile Remove="artifacts\**" />` to stop SDK default globbing from compiling generated files under `artifacts/obj`.
- `dotnet build .\Web\Edubase.AcceptanceTests\Edubase.AcceptanceTests.csproj -c Release`
  - Result: **Build succeeded** (`0 Error(s)`, warnings only), confirming duplicate `TargetFrameworkAttribute` issue is resolved for AcceptanceTests.
- `dotnet build .\Web\EdubaseWeb.sln -c Release`
  - Result: **Blocked by locked output assemblies** in `Web\Edubase.Web.UI\bin\net10.0\*` (`MSB3021/MSB3027`).
  - Lock holders reported by MSBuild at time of run: `Edubase Web UI layer` PIDs `36156`, `4760`, and `.NET Host` PID `34160`.
  - In-session process termination of those PIDs was denied by approval policy, so full solution rebuild and final startup recheck could not be completed in this run.

## 2026-11-24 runtime startup verification follow-up
- `Web/Edubase.Web.UI/App_Start/CoreServiceRegistration.cs`
  - Added `DbConfiguration.SetConfiguration(new MicrosoftSqlDbConfiguration())` (guarded for already-initialized EF config) to align EF6 provider services with `Microsoft.Data.SqlClient` at runtime.
- `dotnet build .\Web\Edubase.Web.UI\Edubase.Web.UI.csproj -c Release -p:OutputPath=<temp-folder>`
  - Result: **Build succeeded** (`0 Error(s)`, warnings only), using a fresh temp output path to avoid lock contention from already-running host processes.
- `dotnet <temp-folder>\Edubase.Web.UI.dll` with `ASPNETCORE_URLS=http://localhost:5079`
  - Result: **Host started successfully** (`Now listening on: http://localhost:5079`, `Application started`).
  - Homepage request reached controller execution and then failed on external SQL connectivity (`The underlying provider failed on Open`, server/connection not reachable in this environment), replacing the prior EF provider-resolution failure.
- `dotnet build .\Web\EdubaseWeb.sln -c Release -p:OutputPath=bin\net10.0-temp\`
  - Result: **Build succeeded** (`0 Error(s)`), providing full-solution compile validation while bypassing lock contention on default `bin\net10.0` outputs.
- `dotnet run --project .\Web\Edubase.Web.UI\Edubase.Web.UI.csproj -c Release --no-build -p:OutputPath=bin\net10.0-temp\ --no-launch-profile` with `ASPNETCORE_URLS=http://localhost:5081`
  - Result: **Application started successfully** (`Now listening on: http://localhost:5081`, `Application started`).
  - Request to `/` again reached `HomeController.Index` and then failed only on runtime SQL connectivity (`SqlException` network/instance specific connection failure), indicating startup and DI/provider wiring are now functioning and the remaining failure is environment-data access dependent.

## Migration hygiene still visible after recheck
- `Web/Edubase.Web.UI/Edubase.Web.UI.csproj` still contains net48-conditional item groups/target blocks and excludes `Models\BaseViewModel.cs` from compile.
- `Web/Edubase.Services/Edubase.Services.csproj` still excludes `Security\Permissions\Permission.cs` from compile.
- These remain carry-forward cleanup items for release-readiness/final validation closure.

## Explicit carry-forward notes
- No unresolved blocker remains for Task 04 closure validation itself.
- Next task is release readiness (`05-release-readiness`).
- Existing dependency/security warnings remain tracked outside this task’s closure objective and should be handled per release policy in Task 05/06.
