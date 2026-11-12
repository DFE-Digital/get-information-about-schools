# 04.05.02.05-endpoint-contract-validation: Run targeted endpoint contract validation for migrated controller surfaces

## Objective
Validate migrated controller/API endpoint behavior for parity-critical status codes, routing, and response-shape expectations.

## Scope
- Targeted build/runtime validation of migrated controller groups from 04.05.02.01-.04
- Contract-sensitive checks (status codes, JSON shape/casing expectations, redirects, anti-forgery/auth flows)

## Research findings
- Migrated API surfaces in scope:
  - `Web/Edubase.Web.UI/Controllers/Api/AcademyOpeningsApiController.cs`
  - `Web/Edubase.Web.UI/Controllers/Api/AmalgamateMergeApiController.cs`
  - `Web/Edubase.Web.UI/Controllers/Api/ApprovalsApiController.cs`
  - `Web/Edubase.Web.UI/Controllers/Api/BulkCreateAcademiesApiController.cs`
  - `Web/Edubase.Web.UI/Controllers/Api/EstablishmentApiController.cs`
  - `Web/Edubase.Web.UI/Controllers/Api/SearchApiController.cs`
  - `Web/Edubase.Web.UI/Controllers/Api/UtilApiController.cs`
- Migrated MVC/Area surfaces in scope include Establishments, Groups and Governors controllers covered by 04.05.02.04.03-.05.
- High-risk contract points identified for this validation pass:
  - Named-route redirects (`RedirectToRoute(...)`) used heavily by Establishments/Groups/Governors workflows.
  - Anti-forgery + postback endpoints in Establishments/Groups/Governors.
  - API status code contracts (`Ok`, `BadRequest`, `NotFound`) in migrated API controllers.
- Known environment constraint affecting runtime contract verification:
  - `Web/Edubase.Web.UIUnitTests` remains `net48` and currently cannot reference migrated `Web.UI` (`net10.0`), so automated endpoint tests from that project are blocked until test-project migration is aligned.

## Steps
1. Run focused project build and relevant targeted tests/checks for migrated controller surfaces.
2. Execute targeted endpoint checks for high-risk API and MVC routes.
3. Capture parity evidence and remaining explicit blockers.

## Done when
- Validation evidence for controller/API contract behavior is recorded and reproducible.
- Remaining unresolved behavior differences are explicitly documented with next-step owners.
