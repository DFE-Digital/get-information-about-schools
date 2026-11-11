## Task
04.05.01-webui-bootstrap-di-routing

## Summary
Implemented startup/DI/routing foundation extraction for Web.UI on net10 so bootstrap behavior is now driven through ASP.NET Core-compatible App_Start core helpers rather than Program-only inline wiring.

## Changes made
- Updated `Web/Edubase.Web.UI/Edubase.Web.UI.csproj` compile set to include startup foundation files for this subtask:
  - `App_Start/CoreServiceRegistration.cs`
  - `App_Start/CoreFilterConfig.cs`
  - `App_Start/CoreRouteConfig.cs`
  - `App_Start/CoreUnauthorizedAccessExceptionFilter.cs`
- Refactored `Web/Edubase.Web.UI/Program.cs`:
  - Replaced inline service registration with `builder.Services.AddWebUiFoundationServices(builder.Configuration)`.
  - Replaced inline route mapping with `CoreRouteConfig.MapRoutes(app)`.
  - Preserved existing middleware ordering and host-shell security-header logic from Task 04.04.
- Added ASP.NET Core foundation files:
  - `CoreServiceRegistration.cs`: central registration for routing/controllers/session/cookie auth/authorization/antiforgery baseline.
  - `CoreFilterConfig.cs`: ASP.NET Core MVC filter registration entry point.
  - `CoreUnauthorizedAccessExceptionFilter.cs`: ASP.NET Core exception filter equivalent for unauthorized-access redirect behavior.
  - `CoreRouteConfig.cs`: central endpoint route mapping (`MapControllers` + default route).

## Validation
- Build:
  - `dotnet build C:\copilot\get-information-about-schools\Web\Edubase.Web.UI\Edubase.Web.UI.csproj -v minimal`
  - Result: **Success**.
  - Observed warnings: existing `NU1903` AutoMapper advisory warnings (known and user-accepted in scenario decisions).
- Runtime probe:
  - `dotnet run --project ... --urls http://127.0.0.1:5058`
  - Result: blocked in this environment with `Access is denied` when launching generated apphost exe.
  - This appears environment/process-launch related; compile gate for this subtask is still satisfied.

## Notes for follow-on subtasks
- Full controller/API migration remains in `04.05.02`.
- Full view/static-asset parity remains in `04.05.03`.
- Runtime endpoint parity verification remains in `04.05.04`.
