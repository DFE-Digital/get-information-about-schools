## Task
04.05.02.04.02-root-mvc-controller-contracts

## Checkpoint summary (2026-11-11)
This subtask is in progress and partially complete. Multiple root MVC controllers have been migrated and compile-validated under net10 compile slices, with higher-complexity controllers still pending.

## Completed migration work in this subtask
Migrated root MVC controllers:
- `Web/Edubase.Web.UI/Controllers/UnauthorizedController.cs`
- `Web/Edubase.Web.UI/Controllers/TermsofUseController.cs`
- `Web/Edubase.Web.UI/Controllers/GuidanceController.cs`
- `Web/Edubase.Web.UI/Controllers/AmalgamateMergeController.cs`
- `Web/Edubase.Web.UI/Controllers/GlossaryController.cs`
- `Web/Edubase.Web.UI/Controllers/FaqController.cs`
- `Web/Edubase.Web.UI/Controllers/NewsController.cs`
- `Web/Edubase.Web.UI/Controllers/NotificationsController.cs`

Related model/helper updates brought in for compile parity:
- News models (`NewsArticleViewModel`, audit/list models where used)
- Notifications models (`NotificationsBannerViewModel`, `NotificationsTemplateViewModel`, list/audit models)
- Validation/helper dependencies needed by migrated models (`RequireUrlNotification`, `ValidateUrl`, `EnumExtensions`)

Compile-slice wiring updates:
- `Web/Edubase.Web.UI/Edubase.Web.UI.csproj` expanded to include the migrated root controllers and direct model/helper dependencies.

## Migration patterns applied (replication guide)
1. Replace MVC5 usings/contracts with ASP.NET Core MVC equivalents.
2. Convert route metadata from `RoutePrefix` style to class-level `[Route("...")]` and action-level `[HttpGet]/[HttpPost]` templates, preserving route names used by `RedirectToAction`/links.
3. Replace MVC5 `HttpNotFound()` with ASP.NET Core `NotFound()`.
4. Keep workflow semantics in multi-step actions (`GoBack`, action progression, `TempData["ShowSaved"]`, model-state error clearing).
5. Keep JSON behavior through existing shared compatibility path (`EduBaseController` + `JsonCamelCaseResult`), not ad-hoc per-controller serializer changes.
6. Remove MVC5 `[AllowHtml]` usage from migrated view models while preserving existing sanitize/render behavior.
7. For model-state rehydration in ASP.NET Core, use `ModelState.SetModelValue(..., new ValueProviderResult(new StringValues(...), CultureInfo.CurrentCulture))`.

## Validation evidence
- Build command repeatedly used after each slice:
  - `dotnet build C:\copilot\get-information-about-schools\Web\Edubase.Web.UI\Edubase.Web.UI.csproj -v minimal`
- Latest checkpoint result before pause: build success with no errors in active slice.

## Outstanding work in this subtask
Pending root controllers include:
- `AdminController`
- `AccountController`
- `ApprovalsController`
- `ChangeHistoryController`
- `DataQualityController`
- `DownloadsController`
- `HomeController`
- `SearchController`
- `SitemapController`
- `ToolsController`

## Known blockers / cautions
- `AdminController` compile activation remains blocked by unresolved legacy logging dependency path (`AzureTableLogger` / `ILoggingService`) and should be sequenced with scenario logging decisions.
- `DownloadsController` and `ChangeHistoryController` have deeper System.Web coupling (request-context helpers, URI/query helpers, file/stream and redirect flows) and should be migrated in narrow cuts.
- Route-name and redirect-target parity must be checked on each migrated controller to avoid silent behavior regressions.

## Handover note
- The workflow state currently reports this subtask as stale/in-progress from previous session. On resume, assess this checkpoint file and then either complete the task or continue with remaining controllers before completion.