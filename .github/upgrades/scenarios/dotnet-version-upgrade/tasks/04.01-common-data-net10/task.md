# 04.01-common-data-net10: Migrate Common/Data closure to net10 with EF6 preserved

# 04.01-common-data-net10

## Objective
Retarget `Edubase.Common` and `Edubase.Data` to net10.0 while preserving existing behavior and keeping EF6 in Data.

## Scope
- Projects: `Edubase.Common`, `Edubase.Data`, `Edubase.CommonUnitTests`, `Edubase.DataUnitTests`
- Package/API updates required for net10 compatibility
- EF6/provider/spatial/mapping behavior checks in Data

## Research findings
- Current TFMs were `net48` in this closure and are now retargeted to `net10.0`.
- `Edubase.Data` references EF6 (`EntityFramework`) and remains on EF6 per scenario/user constraint.
- User clarified SQL replacement is expected complete in this branch for migrated domains; SQL-only wiring should be enforced now and migration feature flags removed.
- Logging/API-recording table-storage path is explicitly excluded from this SQL cutover and must be handled separately.

## Implementation outcomes
- Retargeted closure projects to `net10.0`:
  - `Edubase.Common`, `Edubase.Data`, `Edubase.CommonUnitTests`, `Edubase.DataUnitTests`.
- Migrated Common runtime dependencies away from `System.Web`/`System.Device` usage in net10-targeted code:
  - URL token and URL encode handling moved to ASP.NET Core-compatible APIs.
  - MIME mapping fallback moved to `FileExtensionContentTypeProvider`.
  - Spatial distance calculation replaced with explicit Haversine implementation.
  - Runtime caching package support added for net10.
- Enforced SQL-only wiring and removed associated feature-flag toggles in Web.UI (`IocConfig` + `Web.config`).
- For Data compile under SQL-only mode, excluded legacy table-storage repository implementation source files for migrated domains from `Edubase.Data.csproj` so they no longer participate in this closure build.
- Preserved separate table-storage path for excluded logging/API-recording components.

## Execution plan
1. Retarget the four closure projects from `net48` to `net10.0`.
2. Resolve package/runtime/compiler breaks in Common/Data first, then unit tests.
3. Keep EF6 in Data and validate the closure builds warning-free.
4. Run Common/Data unit tests and capture blockers explicitly if environment-dependent.

## Steps
1. Retarget project TFMs to net10.0 (use temporary multi-targeting only if explicitly justified).
2. Resolve package/runtime conflicts inline without warning suppression.
3. Fix API breaks in Common/Data while preserving contracts and serialization behavior.
4. Update and run affected unit tests for this closure.

## Done when
- Common/Data closure builds warning-free on net10.0.
- Common/Data unit tests pass or have explicit external blockers.
- EF6 behavior constraints are preserved and evidenced.
