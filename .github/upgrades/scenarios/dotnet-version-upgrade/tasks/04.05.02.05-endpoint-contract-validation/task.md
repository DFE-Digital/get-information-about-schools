# 04.05.02.05-endpoint-contract-validation: Run targeted endpoint contract validation for migrated controller surfaces

## Objective
Validate migrated controller/API endpoint behavior for parity-critical status codes, routing, and response-shape expectations.

## Scope
- Targeted build/runtime validation of migrated controller groups from 04.05.02.01-.04
- Contract-sensitive checks (status codes, JSON shape/casing expectations, redirects, anti-forgery/auth flows)

## Steps
1. Run focused project build and relevant targeted tests/checks for migrated controller surfaces.
2. Execute targeted endpoint checks for high-risk API and MVC routes.
3. Capture parity evidence and remaining explicit blockers.

## Done when
- Validation evidence for controller/API contract behavior is recorded and reproducible.
- Remaining unresolved behavior differences are explicitly documented with next-step owners.
