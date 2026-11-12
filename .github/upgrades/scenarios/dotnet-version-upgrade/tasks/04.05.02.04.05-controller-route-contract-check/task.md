# 04.05.02.04.05-controller-route-contract-check: Validate route/action contract parity for migrated MVC/Area controller surfaces

## Objective
Run focused contract checks after MVC/Area migration slices to confirm route names/templates and action result behaviors remain intact.

## Scope
- Migrated controllers under root + Areas for this parent task
- Route mapping dependencies in App_Start/CoreRouteConfig.cs

## Steps
1. Build the Web.UI project with expanded controller compile set.
2. Verify route-name and route-template references used by RedirectToRoute calls.
3. Record any intentional deferred parity gaps as explicit blockers for follow-on validation task.

## Verification notes (2026-11-12)
- Confirmed named route contracts used by migrated Groups/Governors controllers are defined and reachable via attribute routes.
- Restored missing named route attributes on `GovernorController` action surfaces (`GroupEditGovernance`, `EstabEditGovernance`, add/edit/replace governor route names, and replace-chair route name) to preserve RedirectToRoute behavior.
- Confirmed route dictionary usage in search controllers is mapped to ASP.NET Core routing APIs.
- Verified `CoreRouteConfig` endpoint registration still maps controllers and default conventional route.
- `Web.UI` Release build succeeds after route contract parity checks.
