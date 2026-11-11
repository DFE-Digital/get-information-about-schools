# 04.02-services-net10: Migrate Services closure to net10 preserving API contracts

# 04.02-services-net10

## Objective
Retarget `Edubase.Services` and its direct test closure to net10.0 while preserving service contracts and behavior.

## Scope
- Projects: `Edubase.Services`, `Edubase.ServicesUnitTests`
- Dependency updates from Common/Data migration outputs
- Service-layer behavior parity for downstream consumers

## Steps
1. Retarget Services and its test project(s) to net10.0.
2. Resolve package/API compatibility issues introduced by framework move.
3. Preserve service-layer behavior and error semantics used by Web.UI/Texuna.
4. Run targeted build/tests for the Services closure.

## Done when
- Services closure builds warning-free on net10.0.
- Services unit tests pass or have explicit blockers with evidence.
