# 04.05.02.04.01-shared-controller-base-and-helpers: Migrate shared MVC base controller and helper dependencies used by non-API controllers

## Objective
Migrate shared controller foundation types used across MVC/Area controllers to ASP.NET Core-compatible contracts while preserving JSON and model-state behavior.

## Scope
- Web/Edubase.Web.UI/Controllers/EduBaseController.cs
- Shared helper/result types directly required by EduBaseController behavior
- Compile-slice updates required for this shared foundation

## Repository research (2026-11-11)
- `Controllers/EduBaseController.cs` currently depends on MVC5 `System.Web.Mvc.Controller` and `JsonRequestBehavior.AllowGet`.
- The custom JSON result implementation used by this base controller is `Helpers/JsonCamelCaseResult.cs`, which currently depends on:
  - `System.Web.Mvc.ActionResult`
  - `JsonRequestBehavior`
  - synchronous `ExecuteResult(ControllerContext)` response writing.
- Many root/area controllers still call `Json(..., JsonRequestBehavior.AllowGet)` and/or rely on the base `Json(...)` helper from `EduBaseController`, so preserving a compatible helper surface is required before broader controller migration slices.
- Active compile slice does not yet include `EduBaseController.cs` or `JsonCamelCaseResult.cs`; this subtask will add them once migrated.

## Planned approach
1. Port `EduBaseController` to ASP.NET Core `Controller` while preserving current helper semantics.
2. Port `JsonCamelCaseResult` to ASP.NET Core `ActionResult` + async execution.
3. Preserve existing camelCase serialization intent and GET policy semantics via a local compatibility enum.
4. Add migrated shared files to current compile slice and validate build.

## Steps
1. Replace MVC5 namespaces and deprecated JSON helper signatures with ASP.NET Core equivalents.
2. Preserve AddApiErrorsToModelState behavior and custom JSON result intent.
3. Build and document any remaining dependencies deferred to later subtasks.
