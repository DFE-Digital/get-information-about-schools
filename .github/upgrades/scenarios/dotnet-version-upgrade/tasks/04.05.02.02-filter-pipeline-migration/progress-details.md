## Task
04.05.02.02-filter-pipeline-migration

## Summary
Migrated the API exception filter path from legacy WebApi2 filter contracts to ASP.NET Core filter infrastructure and wired it into the active global filter registration used by the net10 host shell.

## Changes made
- Migrated `Web/Edubase.Web.UI/Filters/ApiExceptionFilter.cs`:
  - Replaced `System.Web.Http.Filters.ExceptionFilterAttribute` with ASP.NET Core `IExceptionFilter`.
  - Replaced `HttpActionExecutedContext` response wiring with ASP.NET Core `ObjectResult`/`NotFoundObjectResult`.
  - Replaced Autofac-based runtime resolution dependency for error-code generation with ASP.NET Core logger + trace identifier model.
  - Preserved special-case behavior for `TexunaApiNotFoundException` (`404` with not-found message).
  - Preserved friendly-vs-technical detail behavior via `EnableFriendlyErrorPage` configuration key.
- Updated `Web/Edubase.Web.UI/App_Start/CoreFilterConfig.cs`:
  - Added global registration of `ApiExceptionFilter` alongside `CoreUnauthorizedAccessExceptionFilter`.
- Updated active compile slice in `Web/Edubase.Web.UI/Edubase.Web.UI.csproj`:
  - Included `Filters/ApiExceptionFilter.cs` in net10 compile set for current migration slice.

## Validation
- Build:
  - `dotnet build C:\copilot\get-information-about-schools\Web\Edubase.Web.UI\Edubase.Web.UI.csproj -v minimal`
  - Result: **Success**.
  - Known warnings: existing user-accepted AutoMapper advisory warnings (`NU1903`).

## Notes
- Full migration of legacy MVC `ExceptionHandler` (`Filters/ExceptionFilter.cs`) and `FilterConfig.cs` remains aligned to subsequent MVC/controller-view migration slices where their dependencies are activated.
