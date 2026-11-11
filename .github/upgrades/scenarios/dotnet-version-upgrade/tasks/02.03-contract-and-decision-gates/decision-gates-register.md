# Decision and Blocker Register (Task 02.03)

## Confirmed decisions (effective constraints for tasks 03/04)

| Topic | Decision | Source of record | Effect on downstream work |
|---|---|---|---|
| SQL migration scope | Ten feature-flagged data areas are expected to complete SQL migration before .NET 10 implementation | `scenario-instructions.md` (Technical Preferences + Key Decisions Log) | Task 04 should treat those areas as SQL-backed and not preserve Azure Table Storage for them |
| Logging + API session recording destination | Application Insights | `scenario-instructions.md` (Technical Preferences + Key Decisions Log) | Task 04 replaces legacy table-backed logging/recording behavior with Application Insights-compatible implementation |
| Admin log viewer | Retire in migration scope | `scenario-instructions.md` (Technical Preferences + Key Decisions Log) | Task 04 should not preserve legacy admin log viewer surface |
| Deployment pattern | Single production cutover only; no production deployment authorized during migration tasks | `scenario-instructions.md` (User Preferences + Execution Constraints) | Tasks 03/04/05 must avoid incremental production routing/deployment changes |
| Commit policy | Manual commits | `scenario-instructions.md` (Source Control) | No auto-commit behavior in execution steps |

## Explicit unresolved gates / blockers

| Topic | Current state | Owner | Evidence required before closure | Blocks |
|---|---|---|---|---|
| OData contract disposition | Deferred decision (preserve/remove/replace not approved yet) | User + migration executor | Runtime verification of active OData contracts/endpoints/consumers and compatibility impact | Task 04 OData package/routing removal/replacement work |
| Acceptance-test environment prerequisites | External blocker remains open | User/environment owners | Valid `devsecrets.gias.config.alwaysignore` access and reachable target environment so acceptance tests can run fully | Full green acceptance baseline/re-validation in Tasks 04 and 06 |

## Guardrails reaffirmed for Task 04

- Do not introduce hidden identity/provider/protocol changes while resolving config/dependency migration work.
- Do not infer OData retirement from static package analysis alone.
- Keep compatibility-first behavior for SOAP/WSDL endpoints, exports/download/report contracts, and UI behavior as already captured in scenario gates.

## Outcome

Decision baseline and blockers are now consolidated for task consumption. No unresolved gate is treated as implicitly approved.