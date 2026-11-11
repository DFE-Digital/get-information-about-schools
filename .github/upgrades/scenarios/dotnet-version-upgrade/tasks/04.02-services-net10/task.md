# 04.02-services-net10: Migrate Services closure to net10 preserving API contracts

# 04.02-services-net10

## Objective
Retarget `Edubase.Services` and its direct test closure to net10.0 while preserving service contracts and behavior.

## Scope
- Projects: `Edubase.Services`, `Edubase.ServicesUnitTests`
- Dependency updates from Common/Data migration outputs
- Service-layer behavior parity for downstream consumers

## Research findings (2026-11-11)
- `Web/Edubase.Services/Edubase.Services.csproj` is currently SDK-style but targets `net48`; it references already-retargeted `Edubase.Common` and `Edubase.Data` (`net10.0`), so retargeting Services is required to remove cross-TFM mismatch.
- `Web/Edubase.ServicesUnitTests/Edubase.ServicesUnitTests.csproj` is currently SDK-style targeting `net48` and references `Edubase.Services` + `Edubase.Common`; it needs `net10.0` retargeting to keep the closure aligned.
- Services package surface includes `EntityFramework` 6.5.1, `Microsoft.AspNet.Identity.Core` 2.2.4, `WindowsAzure.Storage` 9.3.3, `AzureTableLogger` 1.1.3, and BCL compatibility packages. These are expected to be the primary compatibility risk area after TFM change.
- ServicesUnitTests currently uses xUnit runner packages but lacks explicit `Microsoft.NET.Test.Sdk` reference; prior net10 test-closure migrations in this branch required explicit test SDK for stable discovery/execution.
- No inherited `TargetFramework` override was found in shared props/targets; TFM is defined directly in each project file.

## Affected files (expected)
- `Web/Edubase.Services/Edubase.Services.csproj`
- `Web/Edubase.ServicesUnitTests/Edubase.ServicesUnitTests.csproj`
- Service C# files under `Web/Edubase.Services/**` only if API compatibility fixes are required by build/test output.

## Validation plan
1. Retarget both Services projects to `net10.0`.
2. Build `Edubase.Services` in isolation and fix compile/warning issues.
3. Build `Edubase.ServicesUnitTests` and run targeted tests.
4. Record blockers explicitly if any dependency/API is not net10-compatible in this closure.

## Steps
1. Retarget Services and its test project(s) to net10.0.
2. Resolve package/API compatibility issues introduced by framework move.
3. Preserve service-layer behavior and error semantics used by Web.UI/Texuna.
4. Run targeted build/tests for the Services closure.

## Done when
- Services closure builds warning-free on net10.0.
- Services unit tests pass or have explicit blockers with evidence.
