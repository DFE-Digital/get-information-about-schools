## Task
04.05.02.04.02-root-mvc-controller-contracts

## Checkpoint summary (2026-11-12)
This subtask is now implementation-complete for all currently actionable root MVC controllers in the compile slice. Remaining root surfaces are explicitly deferred/blocked decisions carried forward.

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
- `Web/Edubase.Web.UI/Controllers/ChangeHistoryController.cs`
- `Web/Edubase.Web.UI/Controllers/DownloadsController.cs`
- `Web/Edubase.Web.UI/Controllers/SitemapController.cs`
- `Web/Edubase.Web.UI/Controllers/ApprovalsController.cs`
- `Web/Edubase.Web.UI/Controllers/DataQualityController.cs`
- `Web/Edubase.Web.UI/Controllers/HomeController.cs`
- `Web/Edubase.Web.UI/Controllers/SearchController.cs`
- `Web/Edubase.Web.UI/Controllers/ToolsController.cs`

Related model/helper updates brought in for compile parity:
- News models (`NewsArticleViewModel`, audit/list models where used)
- Notifications models (`NotificationsBannerViewModel`, `NotificationsTemplateViewModel`, list/audit models)
- Validation/helper dependencies needed by migrated models (`RequireUrlNotification`, `ValidateUrl`, `EnumExtensions`)
- Root-controller support models used by newer slices (`SitemapNode`, `ApprovalsViewModel`, DataQuality models, `HomepageViewModel`)
- Search-slice helper/model dependencies (`SearchViewModel`, `GovernorSearchPayloadViewModel`, disambiguation models, `QueryStringHelper`, `QueryValidator`).
- Tools-slice dependencies (`ToolsViewModel`, tools view models, `CannotBeEmptyAttribute`) and ASP.NET Core helper type migrations (`IHtmlHelper`, `IHtmlContent`, `SelectListItem`).
- DataQuality validator cascade mode updated to FluentValidation non-obsolete API while preserving validation semantics.

Compile-slice wiring updates:
- `Web/Edubase.Web.UI/Edubase.Web.UI.csproj` expanded to include migrated root controllers and direct model/helper dependencies required to compile this slice.

## Migration patterns applied (replication guide)
1. Replace MVC5 usings/contracts with ASP.NET Core MVC equivalents.
2. Convert route metadata from `RoutePrefix` style to class-level `[Route("...")]` and action-level `[HttpGet]/[HttpPost]` templates, preserving route names used by `RedirectToAction`/links.
3. Replace MVC5 `HttpNotFound()`