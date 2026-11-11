# .NET 10 Upgrade

## Preferences
- **Flow Mode**: Guided
- **Target Framework**: net10.0
- **Assessment Scope**: Solution, Web\EdubaseWeb.sln, all included projects.
- **Current Stage Limit**: Upgrade options confirmed; generate plan for review. Implementation requires separate explicit approval.

## Source Control
- **Source Branch**: main
- **Working Branch**: copilot/upgrade-dot-net-10
- **Commit Strategy**: Manual
- **Branch Sync**: Disabled
- **Tagging Policy**: Create a tag before implementation starts and another tag at completion of each implementation-plan task.

## Key Decisions Log
- 2026-11-06: User confirmed .NET 10 solution-wide assessment, Guided mode, new upgrade-dotnet-10 branch from clean main, manual commits and disabled automatic branch sync.
- 2026-11-09: User prefers single-step deployment. Detailed cutover, downtime and rollback arrangements remain to be confirmed during planning.
- 2026-11-09: User authorised the planning stage only. Retain Guided mode and obtain approval before implementation.
- 2026-11-09: User confirmed the presented options with selective multi-targeting and selective temporary System.Web Adapters refinements. Confirmation authorises plan generation, not implementation.
- 2026-11-10: User renamed the working branch to copilot/upgrade-dot-net-10 (superseding the initial working branch name above) and committed assessment/planning work as 0fdcd8cb0. Verified the working tree was clean before recording this update. Commits remain manual; implementation still requires explicit approval.
- 2026-11-10: User approved implementation start with task 01 readiness/compatibility, requested branch tagging before implementation and after each task completion, reaffirmed Guided mode and manual commits, and prohibited production deployment.
- 2026-11-10: User confirmed SQL migration scope covers the ten feature-flagged data areas and will complete before .NET 10 implementation (no Azure Table Storage use expected for those areas). Logging/API recording are excluded from SQL migration; application logging direction is ILogger/Application Insights, while admin log viewer and API session recording destination remain unresolved.
- 2026-11-10: User decided to use Application Insights for both application logging and API session recordings, and to retire the current admin log viewer in the .NET 10 migration.
- 2026-11-10: User deferred the OData removal/preservation decision until deeper runtime verification is completed.
- 2026-11-10: User directed acceptance-test environment prerequisites to remain an explicit external blocker until valid secrets and environment access are provided.
- 2026-11-11: User agreed Task 03 should not perform FluentValidation 11.x behavioral MVC5 migration work; keep Task 03 structural and defer FluentValidation behavior migration to Task 04.
- 2026-11-11: User confirmed SQL replacement is expected to be complete in this branch and instructed enforcement of SQL-only wiring now, including removal of associated migration feature flags, while handling logging/API-recording path separately.
- 2026-11-11: User accepted replacing System.Device.Location usage as long as new functionality remains equivalent.
- 2026-11-11: User chose to expand Task 04.02 scope to resolve remaining Services-closure net10 warnings immediately (including package/security warning remediation).
- 2026-11-11: User evaluated non-licensed AutoMapper path and chose to keep AutoMapper at 10.1.1 for now (instead of moving to 14.x), given current net48 compatibility constraints.
- 2026-11-11: User requested adherence to the existing application's coding standards and style conventions, and asked for this preference to be explicitly captured in scenario instructions.
- 2026-11-11: User approved a host-foundation cut for 04.04: produce a runnable ASP.NET Core shell on net10 and temporarily exclude legacy MVC5/System.Web feature code from compile, then restore full feature parity in 04.05.
- 2026-11-11: User requested consistent line endings in edited/generated files to avoid Visual Studio normalization prompts; enforce repository-consistent endings for all future edits.
- 2026-11-11: User requested a pause; stop implementation activity until the user asks to continue.

## User Preferences
### Technical Preferences
- Upgrade this solution and projects to .NET 10.
- Need to keep backward compatibility with existing clients, eg SOAP endpoints, contracts, exports, downloads, reports.
- UI style, layout, features should all be preserved.
- Do not substitute .NET 11 preview for the requested .NET 10 target.
- Use standard Microsoft.Extensions.Logging ILogger for logging.
- ApiRecorderSessionItemRepository and WebLogItemRepository currently write to Azure Table Storage and will not migrate to SQL.
- The existing application's feature-flagged Table Storage to SQL migration is expected to complete before .NET 10 implementation. If timings overlap, assess temporary Table Storage support complexity and ask the user before choosing. Confirm migration completion and fallback requirements before omitting Table Storage; the two logging repositories require separate treatment.
- SQL migration scope is the ten data areas already represented by SQL repositories and migration flags; assume this SQL migration completes before .NET 10 implementation and these areas should no longer use Azure Table Storage.
- Logging and API recording remain outside SQL migration scope; use Application Insights for both application logging and API session recordings, and retire the current admin log viewer.
- OData disposition is deferred until deeper runtime verification confirms whether active contracts exist.
- Acceptance-test environment access (`devsecrets.gias.config.alwaysignore` + reachable target environment) is an explicit external blocker until provided.
- Deployment preference: migrate in one step, not incrementally; prefer a single production cutover rather than a staged feature-by-feature migration.
- Use temporary library multi-targeting only where keeping existing consumers buildable provides enough value to justify the extra work, not automatically for every library. Evaluate each library and its dependency chain; do not assume successful builds establish runtime compatibility.
- Use System.Web Adapters selectively, not automatically: prefer native ASP.NET Core APIs for straightforward changes; consider temporary shims only for supported, tightly coupled legacy request-context usages where they materially reduce migration risk. Verify actual usage and behaviour with explicit tests and remove temporary shims before completion. Adapters do not make MVC5 run on ASP.NET Core or require side-by-side deployment.
- Enforce SQL-only repository wiring now for the migrated data areas and remove associated migration feature flags; handle logging/API-recording storage path separately.
- Keep AutoMapper at 10.1.1 for now (non-licensed path), acknowledging current advisory exposure until a later migration/licensing decision.
- Follow existing application coding standards, conventions, naming, structure, and style patterns unless an explicit task requires a deliberate deviation.
- Keep line endings consistent with repository/editor conventions for every modified or newly generated file; avoid introducing mixed line endings that trigger normalization prompts.

## Task 04 migration handover notes (for next chat / model switch)
### Architectural decisions already in force
- `04.04` intentionally established a runnable ASP.NET Core host shell first, then deferred full MVC5 surface migration to `04.05`.
- Web.UI migration is being executed using **compile-slice gating** in `Web/Edubase.Web.UI/Edubase.Web.UI.csproj` (small, explicitly included file sets) to keep each slice buildable and debuggable.
- Existing behavior parity is prioritized over syntactic modernization. Route names, redirect targets, JSON casing, and UI output contracts must remain equivalent unless explicitly approved otherwise.

### Controller migration patterns to keep using
- Replace MVC5 controller contracts/usings (`System.Web.Mvc`) with ASP.NET Core (`Microsoft.AspNetCore.Mvc`) and keep existing controller/action names.
- Convert `RoutePrefix` + action routes to class-level `[Route("...")]` plus action-level `[HttpGet]/[HttpPost]` templates while preserving route names (`Name = ...`) used by redirects and links.
- Replace `HttpNotFound()` with `NotFound()`.
- Preserve legacy JSON allow-get semantics through the existing compatibility path (`EduBaseController` + `JsonCamelCaseResult` + compatibility `JsonRequestBehavior` enum), not ad-hoc serializer changes per controller.
- For multi-step admin forms, preserve back/forward workflow semantics (`GoBack`, `Action` stepping, `TempData["ShowSaved"]`, model-state clearing/removal).
- Where model-state value rehydration is required, use ASP.NET Core `ValueProviderResult` with `StringValues` and current culture.
- Remove MVC5 `[AllowHtml]` attributes from migrated view models and preserve equivalent behavior through existing sanitization/rendering logic.

### Task 04.05 controller work completed so far
- Completed subtasks:
  - `04.05.02.01` Web API controller surface
  - `04.05.02.02` filter pipeline migration
  - `04.05.02.03` model-binding/value-provider migration
  - `04.05.02.04.01` shared MVC controller base/helpers
- Root MVC controllers migrated and compile-validated in current slice:
  - `UnauthorizedController`
  - `TermsofUseController`
  - `GuidanceController`
  - `AmalgamateMergeController`
  - `GlossaryController`
  - `FaqController`
  - `NewsController`
  - `NotificationsController`

### Outstanding issues / next execution targets
- `04.05.02.04.02` is partially implemented but still marked stale/in-progress in workflow state; finalize status explicitly when resuming.
- Remaining higher-complexity root controllers are still pending migration in this track (notably `DownloadsController`, `ChangeHistoryController`, `HomeController`, `ToolsController`, `SearchController`, `DataQualityController`, `ApprovalsController`, `AccountController`, `SitemapController`, plus `AdminController`).
- `AdminController` compile activation remains blocked by unresolved legacy logging dependency path (`AzureTableLogger` / `ILoggingService`) and should be coordinated with logging migration decisions already captured in scenario preferences.
- After finishing root controllers, continue planned order: `04.05.02.04.03` (Establishments), `04.05.02.04.04` (Groups/Governors), `04.05.02.04.05` (route contract check), `04.05.02.05` (endpoint contract validation).

### Validation approach to replicate
- After each controller slice, run targeted build:
  - `dotnet build C:\copilot\get-information-about-schools\Web\Edubase.Web.UI\Edubase.Web.UI.csproj -v minimal`
- Keep known warning context explicit in notes:
  - User-approved AutoMapper advisory exposure while pinned at `AutoMapper 10.1.1`.
  - Existing `RequiredDateTimeViewModel` CS0108 warnings currently pre-existing in migrated slice builds.
- No production deployment during migration tasks.
