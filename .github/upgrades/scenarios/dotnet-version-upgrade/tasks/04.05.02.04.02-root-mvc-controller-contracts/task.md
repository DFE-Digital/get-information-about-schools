# 04.05.02.04.02-root-mvc-controller-contracts: Migrate root (non-area) MVC controllers preserving route/action contracts

## Objective
Migrate root non-API MVC controllers to ASP.NET Core contracts while preserving route templates, redirect behavior, anti-forgery, and JSON response expectations.

## Scope
- Web/Edubase.Web.UI/Controllers/*.cs (excluding Controllers/Api/*)

## Repository research (2026-11-11)
- Root controller set is large and mixed-complexity (20+ root non-API controllers).
- Shared foundation now migrated in prior subtask:
  - `Controllers/EduBaseController.cs`
  - `Helpers/JsonCamelCaseResult.cs`
- Common MVC5-to-Core migration patterns found across root controllers:
  - `using System.Web.Mvc` in most files
  - `Json(..., JsonRequestBehavior.AllowGet)` call sites
  - `HttpNotFound()` action returns
  - route attributes with `RoutePrefix` + named routes used by redirects
- Higher-complexity root controllers (`HomeController`, `ToolsController`, `DownloadsController`, `ChangeHistoryController`, `NotificationsController`) include additional `System.Web` dependencies (cookies, request context helpers, legacy helper types) and will be handled after initial low-risk root surface migration.
- Low-risk root controllers identified for initial compile-validated slice:
  - `Controllers/UnauthorizedController.cs`
  - `Controllers/TermsofUseController.cs`

## Execution approach for this subtask
1. Start with low-risk controllers to prove route/attribute parity under ASP.NET Core in active compile slice.
2. Expand root controller compile surface incrementally, preserving route names and action signatures.
3. Defer controllers with cookie/request-context and larger behavioral surfaces until their contracts can be migrated and validated without broad regressions.

## Steps
1. Convert controller attributes/usings/action signatures to ASP.NET Core equivalents.
2. Preserve named routes and redirect targets.
3. Preserve cookie/session/form access behavior through ASP.NET Core APIs.
4. Validate compile for this slice.
