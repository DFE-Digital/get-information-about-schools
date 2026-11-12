# 04.05.03-webui-views-assets-parity progress

## Status
- Task 04.05.03-webui-views-assets-parity is complete for the remaining Razor view surface in scope.
- The project builds successfully in Release mode under the active compile-slice gate in `Web/Edubase.Web.UI/Edubase.Web.UI.csproj`.

## Research findings
- The remaining compatibility risk in the Razor layer was the set of legacy `System.Web.HttpUtility` and query-string patterns across the app’s view surfaces.
- Those patterns were converted to ASP.NET Core equivalents without altering route names, redirect targets, download behavior, or page output semantics.
- The migration covered shared layout surfaces, search flows, change-history and downloads flows, and the remaining tools/group pages that still used legacy HTML-decoding or query parsing.
- `Html.ScriptNonce()` and the current Razor host remain compatible with ASP.NET Core semantics; no additional auth/session redesign was required for the completed view slice.
- The project still accepts the known AutoMapper advisory warning (`NU1903`) as a pre-existing migration warning rather than forcing a package upgrade in this task.

## Code changes made
- `Web/Edubase.Web.UI/Views/GOVUK/Layouts/template.cshtml` and `Views/DfE/Layouts/template.cshtml`
  - Confirmed the shared layout layer is using ASP.NET Core-compatible request/auth/config accessors.
- `Web/Edubase.Web.UI/Views/Home/Index.cshtml`, `Views/Home/Cookies.cshtml`, `Views/News/AuditArticle.cshtml`, `Views/News/AuditArticles.cshtml`
  - Preserved page behavior while moving to `Context.Request`/`Query` access.
- `Web/Edubase.Web.UI/Views/Downloads/ReadyToDownload.cshtml`, `Views/Search/LocalAuthorityDisambiguation.cshtml`, `Views/Search/LocationDisambiguation.cshtml`, `Views/ChangeHistory/*`, and `Views/Tools/*`
  - Replaced remaining legacy query parsing and HTML decoding with ASP.NET Core-safe equivalents while keeping form/action semantics intact.
- `Web/Edubase.Web.UI/Areas/Groups/Views/Group/CreateAcademyTrust.cshtml`, `Areas/Groups/Views/Group/Convert.cshtml`, and `Areas/Governors/Views/GovernorsBulkUpdate/Index.cshtml`
  - Converted the final remaining view-layer legacy pattern usage to ASP.NET Core-safe code.
- `Web/Edubase.Web.UI/Helpers/NonceHelper.cs`
  - Confirmed compatibility with the current Razor host.

## Final hardening pass (2026-11-12)
- Removed residual MVC5-only Razor references that remained after the initial MAI migration pass:
  - Removed `@using System.Web.Mvc.Html` from:
    - `Web/Edubase.Web.UI/Views/News/AuditArticle.cshtml`
    - `Web/Edubase.Web.UI/Views/News/AuditArticles.cshtml`
    - `Web/Edubase.Web.UI/Views/Notifications/AuditBanner.cshtml`
    - `Web/Edubase.Web.UI/Views/Notifications/AuditBanners.cshtml`
  - Removed `@model System.Web.Mvc.HandleErrorInfo` from:
    - `Web/Edubase.Web.UI/Views/Shared/Error.cshtml`
- Verification after hardening:
  - `rg` over `Views/**/*.cshtml` and `Areas/**/*.cshtml` found no remaining `System.Web`/`HttpUtility` references.

## Build evidence
- Command: `dotnet build Web/Edubase.Web.UI/Edubase.Web.UI.csproj -c Release -v minimal`
- Result: Build succeeded with 0 errors.
- Warnings: 12 warnings remain for the known AutoMapper advisory (`NU1903`), consistent with the accepted migration guidance on this branch.

## Decision
- The remaining Razor parity work in 04.05.03 is complete. No broader refactor or production deployment work was introduced.
