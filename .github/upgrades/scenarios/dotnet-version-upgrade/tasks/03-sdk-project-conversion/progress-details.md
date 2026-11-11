# 03-sdk-project-conversion progress details

## Summary
- Converted all nine planned legacy projects to SDK-style format while retaining `net48` target framework.
- Validated converted projects with targeted builds.
- Task is blocked on a Web.UI FluentValidation compatibility decision that exceeds structural-only conversion scope.

## Converted projects
- `Web/Edubase.Common/Edubase.Common.csproj`
- `Web/Edubase.Data/Edubase.Data.csproj`
- `Web/Edubase.Services/Edubase.Services.csproj`
- `Web/Edubase.Web.Resources/Edubase.Web.Resources.csproj`
- `Web/Edubase.Services.Texuna/Edubase.Services.Texuna.csproj`
- `Web/Edubase.Web.UI/Edubase.Web.UI.csproj`
- `Web/Edubase.ServicesUnitTests/Edubase.ServicesUnitTests.csproj`
- `Web/Edubase.DataUnitTests/Edubase.DataUnitTests.csproj`
- `Web/Edubase.CommonUnitTests/Edubase.CommonUnitTests.csproj`

## Validation evidence
- `run_build` succeeded for:
  - Common, Data, Services, Web.Resources, Services.Texuna,
  - ServicesUnitTests, DataUnitTests, CommonUnitTests.
- `Edubase.Web.UI` conversion completed but targeted build remains blocked.
- Additional diagnostics (`dotnet build`) identified the remaining blocker as FluentValidation MVC/API incompatibility under transitive 11.x resolution.

## Conversion-coupled adjustments made
- Resolved NU1605 package downgrade errors in converted projects by aligning direct package references with dependency-closure versions where required:
  - `Web/Edubase.Data/Edubase.Data.csproj`
  - `Web/Edubase.Services/Edubase.Services.csproj`
  - `Web/Edubase.Services.Texuna/Edubase.Services.Texuna.csproj`
  - `Web/Edubase.Web.UI/Edubase.Web.UI.csproj`
- Added `EnableDefaultContentItems=false` in `Web/Edubase.Web.UI/Edubase.Web.UI.csproj` to avoid SDK implicit-content duplication against explicit legacy content includes.

## Blocker
- `Web/Edubase.Web.UI/Edubase.Web.UI.csproj` cannot currently build after conversion because MVC5 validation code uses legacy FluentValidation MVC/attribute APIs that are incompatible with the transitive FluentValidation 11.x dependency chain.
- User-directed resolution path: defer FluentValidation 11.x migration behavior changes to Task 04.

## Notes
- `packages.config` files were removed from converted target projects.
- Remaining `packages.config` in repository (`Web/Edubase.Services.TexunaUnitTests/packages.config`) belongs to a project not in Task 03 conversion scope.