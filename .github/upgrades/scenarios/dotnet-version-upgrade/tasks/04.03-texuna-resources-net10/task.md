# 04.03-texuna-resources-net10: Migrate Texuna/Resources closures and related tests

# 04.03-texuna-resources-net10

## Objective
Retarget Texuna and Resources projects (and dependent tests) to net10.0 while preserving external integration and generated resource behavior.

## Scope
- Projects: `Edubase.Services.Texuna`, `Edubase.Web.Resources`, `Edubase.Services.TexunaUnitTests`, `Edubase.Web.ResourcesUnitTests`
- Java API request/response/auth/header behavior preservation
- Resource naming/localization/output parity

## Research findings (2026-11-11)
- `Edubase.Services.Texuna.csproj` is SDK-style but still targets `net48` and references already-retargeted `Edubase.Common`/`Edubase.Data`/`Edubase.Services` (`net10.0`), so it must be retargeted for closure consistency.
- `Edubase.Services.Texuna` uses `System.Web` and `Microsoft.AspNet.Identity` only in `Core/HttpClientWrapper.cs` (current-user/context helpers); this is the main expected code-compatibility hotspot for net10.
- `Edubase.Web.Resources.csproj` is SDK-style on `net48` and references `Edubase.Common` + `Edubase.Services` (`net10.0`), so retargeting is required.
- `Edubase.Services.TexunaUnitTests.csproj` is still legacy non-SDK format with packages.config-era references; conversion to SDK-style is required before practical net10 retargeting.
- `Edubase.Services.TexunaUnitTests` currently references `Edubase.Web.UI` to call `IocConfig.CreateJsonMediaTypeFormatter()` in HttpClient wrapper tests. This creates an avoidable coupling to a still-net48 project and should be replaced with local formatter construction in tests.
- `Edubase.Web.ResourcesUnitTests.csproj` is SDK-style net48 and has project references to `Edubase.Web.Resources` plus `Edubase.Services.TexunaUnitTests`; the latter appears unnecessary for the two resource tests and may need removal to keep the closure clean.

## Affected files (expected)
- `Web/Edubase.Services.Texuna/Edubase.Services.Texuna.csproj`
- `Web/Edubase.Services.Texuna/Core/HttpClientWrapper.cs`
- `Web/Edubase.Web.Resources/Edubase.Web.Resources.csproj`
- `Web/Edubase.Services.TexunaUnitTests/Edubase.Services.TexunaUnitTests.csproj`
- `Web/Edubase.Services.TexunaUnitTests/Core/HttpClientWrapperTests.cs`
- `Web/Edubase.Services.TexunaUnitTests/Core/HttpClientWrapperTest.cs`
- `Web/Edubase.Web.ResourcesUnitTests/Edubase.Web.ResourcesUnitTests.csproj`

## Validation plan
1. Convert legacy Texuna unit-test project to SDK-style, then retarget test projects to net10.
2. Retarget Texuna and Resources projects to net10 and resolve package/API errors.
3. Build Texuna/Resources closure projects incrementally and remove warnings in touched closure.
4. Run Texuna and Resources related tests and record blockers only if unresolved.

## Steps
1. Convert/retarget remaining legacy test project(s) as needed for net10.0 compatibility.
2. Retarget Texuna/Resources projects to net10.0 and resolve package/API issues.
3. Preserve Java integration contract behavior and resource output behavior.
4. Run targeted build/tests for this closure.

## Done when
- Texuna/Resources closure builds warning-free on net10.0.
- Related tests pass or blockers are explicit and evidenced.
