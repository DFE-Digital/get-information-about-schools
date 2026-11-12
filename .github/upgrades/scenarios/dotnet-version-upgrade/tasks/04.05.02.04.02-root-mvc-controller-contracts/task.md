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

## Repository research addendum (2026-11-12)
- Re-checked remaining root controllers still on `System.Web.Mvc`: `AccountController`, `ApprovalsController`, `DataQualityController`, `HomeController`, `SearchController`, `ToolsController` (plus `AdminController` blocked separately).
- Selected next medium slice: `ApprovalsController` + its direct view model dependency `Models/Tools/ApprovalsViewModel.cs`.
- `ApprovalsController` current contract to preserve:
  - Route prefix `Approvals` with index entry behavior and named routes `PendingApprovals` / `PendingApprovalsPost`.
  - POST workflow parity for approve/reject branches, including `TempData["ShowSuccess"]` messages.
  - Anti-forgery validation on POST index action.
- Compile-slice note: `EnableDefaultCompileItems=false` is active; this slice requires explicit compile includes for `Controllers/ApprovalsController.cs` and `Models/Tools/ApprovalsViewModel.cs` in `Edubase.Web.UI.csproj`.
- No unresolved design decisions for this slice; proceed with parity-first migration and targeted project build validation.

## Execution approach for this subtask
1. Start with low-risk controllers to prove route/attribute parity under ASP.NET Core in active compile slice.
2. Expand root controller compile surface incrementally, preserving route names and action signatures.
3. Defer controllers with cookie/request-context and larger behavioral surfaces until their contracts can be migrated and validated without broad regressions.

## Steps
1. Convert controller attributes/usings/action signatures to ASP.NET Core equivalents.
2. Preserve named routes and redirect targets.
3. Preserve cookie/session/form access behavior through ASP.NET Core APIs.
4. Validate compile for this slice.

## Repository research addendum (2026-11-12, continued)
- Completed medium slices in this resume session: `ApprovalsController`, `DataQualityController`, and `HomeController`, with compile-slice inclusion and successful build validation after each slice.
- Auth migration decision captured: defer `AccountController` until explicit ASP.NET Core auth-scheme wiring is confirmed.
- Next highest-impact remaining root controllers for this task are `SearchController` and `ToolsController`; both have broad query-string and contract-sensitive behavior and require narrow parity-preserving cuts.

## Repository research addendum (2026-11-12, search slice)
- `SearchController` migrated with parity-focused route/query-string handling updates for ASP.NET Core request APIs.
- `SearchViewModel` bind aliases were decoupled to stable literal constants (`b`, `d`, `f`) to avoid bringing large cross-area compile dependencies into the active slice; values preserved from existing Establishments search model contract.
- Next remaining executable root-controller scope in this subtask is `ToolsController` (with `AccountController` deferred by decision and `AdminController` blocked).

## Repository research addendum (2026-11-12, tools slice)
- `ToolsController` migrated to ASP.NET Core MVC attributes/usings with parity-preserving route and action behavior, including independent-schools tooling flows.
- Tooling support models were migrated to ASP.NET Core helper abstractions (`IHtmlHelper`, `IHtmlContent`, `SelectListItem`) in the active compile slice.
- Root-controller execution for this task is complete for actionable controllers. Remaining root controllers are now:
  - `AccountController` (deferred by user decision pending auth-scheme wiring confirmation)
  - `AdminController` (blocked by logging dependency path)
