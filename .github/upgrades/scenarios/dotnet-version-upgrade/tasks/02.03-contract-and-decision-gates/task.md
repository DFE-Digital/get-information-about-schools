# 02.03-contract-and-decision-gates: Resolve/record remaining configuration decision gates

# 02.03-contract-and-decision-gates

## Objective
Consolidate decision gates that affect configuration/dependency migration execution and ensure unresolved items are explicit blockers with owner/evidence.

## Scope
- OData contract/usage disposition gate (currently deferred)
- Acceptance-test environment access blocker
- Confirmed decisions from task 01 that constrain config/dependency work (SQL migration assumptions, logging/AppInsights direction)

## Research findings
- Confirmed persisted decisions in `scenario-instructions.md`:
  - SQL migration for the ten feature-flagged data areas is assumed complete before .NET 10 implementation.
  - Logging and API session recording destination is Application Insights.
  - Admin log viewer is to be retired in migration scope.
  - OData disposition remains explicitly deferred pending deeper runtime verification.
  - Acceptance-test environment prerequisites remain an external blocker until secrets/environment access are provided.
- Reviewed task 02 outputs for consistency:
  - `02.01` configuration mapping provides the non-secret configuration baseline.
  - `02.02` dependency conflict review identifies OData/auth/storage clusters and required caution boundaries.
- Confirmed this subtask is documentation/decision-gate consolidation only; no application/runtime behavior changes are in scope.

## Deliverable
- Decision and blocker register authored at:
  - `.github/upgrades/scenarios/dotnet-version-upgrade/tasks/02.03-contract-and-decision-gates/decision-gates-register.md`

## Steps
1. Reconcile task-01 decisions into task-02 outputs so downstream tasks use a single consistent decision baseline.
2. Capture OData verification plan and evidence required before package/routing disposition.
3. Capture acceptance environment blocker details (required secrets/environment access and affected validations).
4. Confirm that no hidden assumptions are made for identity/provider/protocol changes.
5. Publish final decision/blocker register for tasks 03/04 consumption.

## Done when
- Remaining ambiguous decisions are either resolved or explicitly blocked with owner/evidence.
- Downstream tasks have clear constraints and no silent assumptions.
- Decision register aligns with `scenario-instructions.md`.
