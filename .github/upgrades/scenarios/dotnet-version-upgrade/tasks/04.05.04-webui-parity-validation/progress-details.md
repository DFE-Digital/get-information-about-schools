# Progress details — 04.05.04 (Web.UI parity validation)

## Summary
Executed focused parity validation for migrated Web.UI surfaces and captured reproducible evidence plus explicit remaining blocker(s).

## Validation evidence

### 1) Focused Web.UI build
- Command: `run_build` on `Web/Edubase.Web.UI/Edubase.Web.UI.csproj`
- Result: **Build successful**.

### 2) View-layer compatibility parity checks (targeted)
- Command: `rg` over `Web/Edubase.Web.UI/Views/**/*.cshtml` and `Areas/**/*.cshtml` for legacy MVC5/System.Web markers:
  - `@using System.Web.Mvc.Html`
  - `@model System.Web.Mvc.HandleErrorInfo`
  - `System.Web.HttpUtility` / `HttpUtility`
- Result: **No matches found** on the migrated surface.
- Interpretation: the previously remaining legacy view-layer markers have been removed from the active migrated view set.

### 3) Contract-sensitive route/auth pattern checks (targeted)
- Command: `rg` across migrated controller surfaces (`Areas/Establishments`, `Areas/Groups`, `Areas/Governors`, `Controllers/Api`) for:
  - `RedirectToRoute(...)`
  - named route declarations (`Name = "..."`)
  - `ValidateAntiForgeryToken`
- Result: expected parity-sensitive patterns remain present in the migrated controller/API surfaces.
- Interpretation: route/redirect and anti-forgery contract hooks remain explicitly represented in migrated code paths.

### 4) Automated parity test attempt
- Command: `dotnet test Web/Edubase.Web.UIUnitTests/Edubase.Web.UIUnitTests.csproj -c Release`
- Result: **Blocked** with `NU1201`.
- Error: `Edubase.Web.UIUnitTests` targets `net48` and cannot reference `Edubase.Web.UI` targeting `net10.0`.

## Remaining blocker(s)
- **Blocker**: Automated Web.UI parity tests are unavailable until `Web.UIUnitTests` target/reference alignment is migrated.
- **Owner/next step**: Address under migration closure and solution-validation path (`04.06` / `06-solution-validation`), then re-run endpoint/UI parity tests.

## Outcome
- The focused parity checks required by 04.05.04 are captured and reproducible for the migrated Web.UI surface.
- Remaining automated parity gap is explicit and tracked as a framework-alignment blocker, not an implicit fallback.
