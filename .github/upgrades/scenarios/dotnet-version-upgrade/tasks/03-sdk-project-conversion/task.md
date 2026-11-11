# 03-sdk-project-conversion: Convert legacy project structure without changing target frameworks

Convert the assessment's nine non-SDK projects to SDK style: Common, Data, Services, Services.Texuna, Web.Resources, Web.UI, CommonUnitTests, DataUnitTests and ServicesUnitTests. Convert packages.config to PackageReference as part of this structural work, retaining current TFMs and avoiding unrelated package/API changes. Keep the four existing SDK-style projects intact except where directly necessary for references. Reconcile this inventory against the current branch before execution.

Preserve explicit content/resource inclusions, generated resources, assembly metadata, build/publish assets, test discovery and existing Razor/static-asset tooling. The in-place Framework web project needs project-type-specific conversion research; do not assume a generic class-library conversion preserves its build and publish semantics. If the conversion tool cannot preserve the web project's current-TFM behaviour, stop for a decision rather than silently combining conversion and ASP.NET Core migration. Convert projects sequentially and validate structural changes against the recorded baseline.

## Research findings
- Current solution inventory confirmed (13 projects total).
- Legacy non-SDK projects confirmed for this task (all currently `TargetFrameworkVersion v4.8`):
  - `Web\Edubase.Common\Edubase.Common.csproj`
  - `Web\Edubase.Data\Edubase.Data.csproj`
  - `Web\Edubase.Services\Edubase.Services.csproj`
  - `Web\Edubase.Web.Resources\Edubase.Web.Resources.csproj`
  - `Web\Edubase.Services.Texuna\Edubase.Services.Texuna.csproj`
  - `Web\Edubase.Web.UI\Edubase.Web.UI.csproj`
  - `Web\Edubase.CommonUnitTests\Edubase.CommonUnitTests.csproj`
  - `Web\Edubase.DataUnitTests\Edubase.DataUnitTests.csproj`
  - `Web\Edubase.ServicesUnitTests\Edubase.ServicesUnitTests.csproj`
- Already SDK-style projects to keep intact: `Edubase.AcceptanceTests`, `Edubase.Web.UIUnitTests`, `Edubase.Web.ResourcesUnitTests`, `Edubase.Services.TexunaUnitTests`.
- `packages.config` currently exists in six legacy projects: Common, Data, Services, Services.Texuna, Services.TexunaUnitTests, Web.UI.
- Topological order from dependency graph will drive sequential conversion/build validation.

## Planned execution order (topological)
1. `Web\Edubase.Common\Edubase.Common.csproj`
2. `Web\Edubase.Data\Edubase.Data.csproj`
3. `Web\Edubase.Services\Edubase.Services.csproj`
4. `Web\Edubase.Web.Resources\Edubase.Web.Resources.csproj`
5. `Web\Edubase.Services.Texuna\Edubase.Services.Texuna.csproj`
6. `Web\Edubase.Web.UI\Edubase.Web.UI.csproj`
7. `Web\Edubase.ServicesUnitTests\Edubase.ServicesUnitTests.csproj`
8. `Web\Edubase.DataUnitTests\Edubase.DataUnitTests.csproj`
9. `Web\Edubase.CommonUnitTests\Edubase.CommonUnitTests.csproj`

## Execution outcomes
- Converted to SDK-style successfully:
  - `Edubase.Common`, `Edubase.Data`, `Edubase.Services`, `Edubase.Web.Resources`, `Edubase.Services.Texuna`, `Edubase.Web.UI`, `Edubase.ServicesUnitTests`, `Edubase.DataUnitTests`, `Edubase.CommonUnitTests`.
- Targeted project build validation passed for all converted projects except `Edubase.Web.UI`.
- `packages.config` files were removed from converted projects (remaining `packages.config` is only in non-target `Edubase.Services.TexunaUnitTests`).
- Conversion-coupled package alignment applied where required to resolve NU1605 downgrades in converted dependency chain projects (`Edubase.Data`, `Edubase.Services`, `Edubase.Services.Texuna`, `Edubase.Web.UI`).

## Blocker
- `Edubase.Web.UI` remains build-blocked after conversion due FluentValidation stack incompatibility:
  - Dependency chain now resolves `FluentValidation` 11.x transitively from converted dependencies.
  - MVC5/Web.UI code depends on legacy FluentValidation MVC/attributes APIs (8.x-era packages/usages).
  - Resolving this requires a behavioral package/API migration decision (defer to Task 04 per user direction), not a pure structural conversion tweak.

**Done when**: Applicable projects are SDK-style with PackageReference, their original TFMs remain unchanged, resources/content and test discovery are preserved, and builds/tests show no unexplained regression from the baseline. Any conversion limitation is resolved explicitly before completion.
