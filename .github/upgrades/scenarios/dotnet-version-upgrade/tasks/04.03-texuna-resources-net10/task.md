# 04.03-texuna-resources-net10: Migrate Texuna/Resources closures and related tests

# 04.03-texuna-resources-net10

## Objective
Retarget Texuna and Resources projects (and dependent tests) to net10.0 while preserving external integration and generated resource behavior.

## Scope
- Projects: `Edubase.Services.Texuna`, `Edubase.Web.Resources`, `Edubase.Services.TexunaUnitTests`, `Edubase.Web.ResourcesUnitTests`
- Java API request/response/auth/header behavior preservation
- Resource naming/localization/output parity

## Steps
1. Convert/retarget remaining legacy test project(s) as needed for net10.0 compatibility.
2. Retarget Texuna/Resources projects to net10.0 and resolve package/API issues.
3. Preserve Java integration contract behavior and resource output behavior.
4. Run targeted build/tests for this closure.

## Done when
- Texuna/Resources closure builds warning-free on net10.0.
- Related tests pass or blockers are explicit and evidenced.
