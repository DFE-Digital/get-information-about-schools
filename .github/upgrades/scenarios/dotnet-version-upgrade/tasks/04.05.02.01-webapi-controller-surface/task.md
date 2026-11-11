# 04.05.02.01-webapi-controller-surface: Migrate Web API controller surface to ASP.NET Core action patterns

## Objective
Convert Web API 2 controllers to ASP.NET Core controller/action patterns while preserving status-code and JSON response contracts.

## Scope
- Controllers under `Web/Edubase.Web.UI/Controllers/Api/*.cs` and `Areas/Groups/Controllers/GroupApiController.cs`
- ApiController -> ControllerBase migration, action return type changes, route/verb attributes, Request.CreateResponse/ResponseMessage conversions

## Research findings (2026-11-11)
- Active Web API migration files identified:
  - `Controllers/Api/AcademyOpeningsApiController.cs`
  - `Controllers/Api/AmalgamateMergeApiController.cs`
  - `Controllers/Api/ApprovalsApiController.cs`
  - `Controllers/Api/BulkCreateAcademiesApiController.cs`
  - `Controllers/Api/EstablishmentApiController.cs`
  - `Controllers/Api/SearchApiController.cs`
  - `Controllers/Api/SqlDataController.cs`
  - `Controllers/Api/UtilApiController.cs`
  - `Areas/Groups/Controllers/GroupApiController.cs`
- Legacy API patterns confirmed across these files:
  - `ApiController` base type.
  - `IHttpActionResult` and `HttpResponseMessage` return types.
  - `Request.CreateResponse(...)` and `ResponseMessage(...)` response construction.
  - `System.Web.Http` route attributes and route-prefix patterns.
- Compile-slice dependency findings:
  - API controllers rely on local role helper attributes (`Helpers/AuthorizeRolesAttribute.cs`) and role constants (`Helpers/AuthorizedRoles.cs`).
  - `SqlDataController` currently depends on package surfaces that remain net48-conditioned in csproj and is deferred as an explicit follow-on blocker for this task stream.
  - `GroupApiController` migration was applied in source, but integration compile coverage is deferred to the MVC/Area controller parity slice.

## Implementation notes for this subtask
- Keep compile activation narrowly scoped to API controller files + required helper types to limit blast radius.
- Preserve existing route templates and status-code behavior where conversions are mechanical.
- Document explicitly deferred API files rather than silent fallback.

## Steps
1. Migrate controller base classes/usings/attributes to ASP.NET Core equivalents.
2. Convert `IHttpActionResult` and `HttpResponseMessage` return paths to `IActionResult`/`ActionResult<T>` preserving semantics.
3. Preserve endpoint route templates and anti-forgery/authorization behavior where applied.
4. Build and fix compile/runtime-contract blockers in migrated API controllers.

## Done when
- API controllers compile on ASP.NET Core types.
- Existing API status-code/response-shape intent is preserved or explicitly documented as blocker.
