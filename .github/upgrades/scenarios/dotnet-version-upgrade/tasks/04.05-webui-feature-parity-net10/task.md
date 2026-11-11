# 04.05-webui-feature-parity-net10: Migrate Web.UI controllers/views/contracts to behavior parity

# 04.05-webui-feature-parity-net10

## Objective
Complete in-place Web.UI feature migration to ASP.NET Core preserving route/API/file/UI behavior and compatibility gates.

## Scope
- Controllers, routing, filters, model binding/validation, Razor views/static assets
- Download/report/export flows, polling, redirects, JSON behavior, WSDL redirect paths
- Authentication/authorization behavior parity checks and supporting tests

## Steps
1. Migrate Web.UI feature surface area in dependency-safe slices.
2. Preserve agreed contract/file/UI behaviors and document parity evidence.
3. Apply selective supported adapters only where justified and remove temporary shims before completion.
4. Run targeted Web.UI and UI test validations.

## Done when
- Web.UI feature surface builds/runs on net10 with parity evidence across required behaviors.
- Temporary migration shims/placeholders are removed or explicitly tracked as blockers.
