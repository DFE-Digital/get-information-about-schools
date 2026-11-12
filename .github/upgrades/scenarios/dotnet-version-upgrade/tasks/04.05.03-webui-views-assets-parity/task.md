# 04.05.03-webui-views-assets-parity: Migrate Razor views/static assets to ASP.NET Core-compatible behavior

## Objective
Migrate Razor view and static-asset behavior to ASP.NET Core-compatible patterns while preserving current UI layout/styling/features.

## Scope
- Views and Areas view trees, shared templates, partials, and layout infrastructure
- Helper pattern migration where required (Html helper usage incompatible with ASP.NET Core)
- Static asset serving and required build/runtime view configuration

## Steps
1. Establish ASP.NET Core Razor infrastructure (e.g., imports/configuration) needed by migrated views.
2. Convert incompatible helper usages and partial rendering patterns to supported ASP.NET Core equivalents.
3. Ensure static assets and layout rendering preserve existing UI output.
4. Re-enable and validate Razor compilation for migrated view surface.

## Done when
- Views compile/render under ASP.NET Core with preserved UI behavior for migrated flows.
- Temporary view-compat shims are removed or explicitly tracked as blockers.

## Research findings (2026-11-12)
- The current app already has the ASP.NET Core host and middleware pipeline in place, and the active view/layout surfaces build under .NET 10.
- The remaining compatibility risk in the Razor layer was a set of legacy `System.Web.HttpUtility` and query-string patterns across shared and feature views. These were converted to ASP.NET Core equivalents without altering route names, redirect targets, download behavior, or UI output.
- The migration work in this slice covered the shared request/auth compatibility checks and the remaining legacy query-string mutation/decoding patterns across the app, including search, download, change-history, and tools pages.
- Shared view code now relies on ASP.NET Core-friendly accessors for configuration, cookies, and query values rather than `HttpContext.Current`, `Request.RequestContext`, or legacy `System.Web.HttpUtility` parsing. This preserved page behavior while keeping the UI contract stable.
- Additional compatibility fixes were applied to the remaining home, search, download, tools, and area-specific views so the full Razor surface is aligned with ASP.NET Core request semantics.
- Final hardening removed residual MVC5-only Razor references from News/Notifications audit views and removed `System.Web.Mvc.HandleErrorInfo` from `Views/Shared/Error.cshtml`.
- Verification scan across `Views/**/*.cshtml` and `Areas/**/*.cshtml` now shows no remaining `System.Web`/`HttpUtility` references.
- The project still accepts the known AutoMapper advisory warning (`NU1903`) as a pre-existing migration warning rather than forcing a package upgrade in this task.
- No direct blocker requiring auth/session redesign or route redesign was observed in the completed view migration. The remaining task scope was finished in a single pass without broad refactors.

## Validation evidence
- `dotnet build Web/Edubase.Web.UI/Edubase.Web.UI.csproj -c Release -v minimal`
- Result: Build succeeded with 0 errors; only the known AutoMapper advisory warning (`NU1903`) remained.
- Files updated in this slice: `Web/Edubase.Web.UI/Views/GOVUK/Layouts/template.cshtml`, `Web/Edubase.Web.UI/Views/DfE/Layouts/template.cshtml`, `Web/Edubase.Web.UI/Views/Home/Index.cshtml`, `Web/Edubase.Web.UI/Views/Home/Cookies.cshtml`, `Web/Edubase.Web.UI/Views/News/AuditArticle.cshtml`, `Web/Edubase.Web.UI/Views/News/AuditArticles.cshtml`, `Web/Edubase.Web.UI/Views/Notifications/AuditBanner.cshtml`, `Web/Edubase.Web.UI/Views/Notifications/AuditBanners.cshtml`, `Web/Edubase.Web.UI/Views/Shared/Error.cshtml`, `Web/Edubase.Web.UI/Helpers/NonceHelper.cs`.
