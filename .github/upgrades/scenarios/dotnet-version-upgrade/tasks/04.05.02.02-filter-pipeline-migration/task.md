# 04.05.02.02-filter-pipeline-migration: Migrate API/MVC filter and exception handling pipeline components

## Objective
Migrate legacy MVC5/WebApi filter implementations and registrations used by controller pipelines to ASP.NET Core equivalents.

## Scope
- `Web/Edubase.Web.UI/Filters/ApiExceptionFilter.cs`
- Remaining legacy global/exception filter hooks tied to controller execution
- Registration in Program/App_Start core foundation

## Research findings (2026-11-11)
- Current API exception filter remains WebApi2-specific:
  - `Filters/ApiExceptionFilter.cs` derives from `System.Web.Http.Filters.ExceptionFilterAttribute` and consumes `HttpActionExecutedContext`.
  - Uses `Request.Properties["MS_HttpContext"]` and Autofac resolver to call `ExceptionHandler.Log(...)`.
- Global MVC exception handling remains legacy:
  - `Filters/ExceptionFilter.cs` (`ExceptionHandler`) is MVC5 `IExceptionFilter` + `HttpContextBase`-based.
  - `App_Start/FilterConfig.cs` still registers legacy global filters (`UnauthorizedAccessAttribute`, `ExceptionHandler`) via `GlobalFilterCollection`.
- Startup references to old filters are currently not active in net10 host shell path (because Global.asax is not active), but API parity requires an ASP.NET Core equivalent for API exception mapping now that API controllers compile.
- Existing `Program.cs` already has generic `UseExceptionHandler("/500")`; this subtask should add API-focused filter registration parity in ASP.NET Core MVC options.

## Implementation boundary for this subtask
- Migrate API-facing exception filter behavior to ASP.NET Core and wire into current core filter registration.
- Keep the broader MVC view/error pipeline conversion (`ExceptionHandler` full migration) for subsequent controller/view subtasks where shared dependencies are migrated together.
- Explicitly document any deferred legacy filter surfaces rather than silently dropping behavior.

## Steps
1. Replace WebApi/MVC filter interfaces and context types with ASP.NET Core equivalents.
2. Rewire filter registrations through ASP.NET Core MVC options/pipeline.
3. Preserve existing redirect/error logging/status behavior contracts for controller exceptions.
4. Build and verify migrated filter pipeline compiles and is active.

## Done when
- Filter components compile against ASP.NET Core filter APIs.
- Equivalent global filter behavior is wired through ASP.NET Core pipeline.
