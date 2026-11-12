# 04.05.04-webui-parity-validation: Execute focused Web.UI parity validation and document evidence

## Objective
Validate migrated Web.UI behavior for parity-critical flows and capture evidence/blockers before closing Task 04.05.

## Scope
- Build and runtime checks for Web.UI on net10
- Targeted behavior checks for routes, auth/authorization paths, downloads/exports/reports, redirects, and contract-sensitive endpoints
- Task documentation updates (progress evidence and unresolved blockers)

## Research findings
- Web.UI build surface is currently compile-sliced (`EnableDefaultCompileItems=false`, `RazorCompileOnBuild=false`), so parity validation in this task must combine build evidence with targeted contract pattern checks and explicit blocker capture.
- Views/assets parity hardening from 04.05.03 is now in place for the migrated surface; targeted scans over `Views/**/*.cshtml` and `Areas/**/*.cshtml` show no remaining `System.Web`/`HttpUtility` references.
- Controller/API parity-sensitive surfaces (Establishments/Groups/Governors and migrated API controllers) continue to expose explicit route names/redirect patterns and anti-forgery attributes on migrated form posts.
- Runtime automated parity tests are externally constrained by test-project framework mismatch:
  - `Edubase.Web.UIUnitTests` remains `net48` and cannot currently restore/build against `Edubase.Web.UI` targeting `net10.0` (`NU1201`).

## Steps
1. Run focused build/test/runtime validations for migrated Web.UI feature surfaces.
2. Execute parity checks for contract/file/UI critical journeys covered by this task.
3. Record evidence and any remaining blockers with clear scope/owner.

## Done when
- Validation evidence for 04.05 parity scope is captured and reproducible.
- Remaining gaps are explicitly documented for follow-on tasks instead of implicit fallbacks.
