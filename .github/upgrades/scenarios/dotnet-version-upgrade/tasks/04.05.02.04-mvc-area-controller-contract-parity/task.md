# 04.05.02.04-mvc-area-controller-contract-parity: Migrate MVC/Area controller contract surfaces and route parity dependencies

## Objective
Migrate non-API MVC/Area controllers and contract-sensitive routing surfaces required for behavior parity.

## Scope
- Controllers under `Web/Edubase.Web.UI/Controllers/*.cs` and `Areas/*/Controllers/*.cs` (excluding already-migrated API controllers)
- Route attributes, action signatures, redirect route names, anti-forgery/authorization attributes, JSON-result behavior

## Repository research (2026-11-11)
- Controller inventory in current source is broad (44 controller classes across root + Areas).
- High-complexity controllers include long multi-action flows (for example `Areas/Groups/Controllers/GroupController.cs`, >1200 lines) and rely heavily on route names and redirect targets.
- Shared base and helper controller surfaces still use MVC5 contracts:
  - `Controllers/EduBaseController.cs` uses `System.Web.Mvc` and `JsonRequestBehavior.AllowGet`.
  - Multiple controllers still use MVC5-only helpers such as `HttpNotFound()`, `UrlHelper(Request.RequestContext)`, `System.Web.HttpCookie`, and legacy route attribute namespaces.
- Routing currently uses ASP.NET Core endpoint mapping (`MapControllers` + default conventional route) in `App_Start/CoreRouteConfig.cs`; preserving attribute route prefixes and named routes in migrated controllers is a contract-critical dependency.
- Active compile slice currently includes API controllers only; MVC/Area migration must expand compile surfaces in controlled slices to prevent broad breakage and preserve guided verification quality.

## Risk notes
- Route-name drift can silently break `RedirectToRoute(...)` behavior.
- JSON helper behavior from custom base controllers can alter client payload casing/shape if not preserved.
- Cookie/session helpers have differing APIs between `System.Web` and ASP.NET Core, requiring targeted contract checks.

## Execution strategy
Due to scope and blast radius, execute this task as decomposed subtasks by controller group and shared contract dependencies, validating each slice before moving on.

## Steps
1. Migrate controller base/usings/attributes to ASP.NET Core equivalents.
2. Preserve route templates and named-route dependencies used by redirects and links.
3. Preserve action result and JSON behavior expectations in controller outputs.
4. Build and fix controller-surface compilation blockers.

## Done when
- MVC/Area controllers compile on ASP.NET Core and route mappings remain intact.
- Contract-sensitive behavior changes are either preserved or explicitly documented as blockers.
