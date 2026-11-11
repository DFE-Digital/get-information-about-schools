# 04.05-webui-feature-parity-net10: Migrate Web.UI controllers/views/contracts to behavior parity

# 04.05-webui-feature-parity-net10

## Objective
Complete in-place Web.UI feature migration to ASP.NET Core preserving route/API/file/UI behavior and compatibility gates.

## Scope
- Controllers, routing, filters, model binding/validation, Razor views/static assets
- Download/report/export flows, polling, redirects, JSON behavior, WSDL redirect paths
- Authentication/authorization behavior parity checks and supporting tests

## Research findings (2026-11-11)
- Current `Web.UI` net10 host is intentionally a shell from 04.04:
  - `Program.cs` provides health endpoint, baseline middleware/auth/session/antiforgery, and default route mapping.
  - `Edubase.Web.UI.csproj` currently compiles only `Program.cs` and disables Razor build/source generation (`RazorCompileOnBuild=false`, `RazorCompileOnPublish=false`, `UseRazorSourceGenerator=false`).
- Feature surface size is substantial and requires phased migration:
  - Controllers: 32 under `Controllers` (including 13 under `Controllers/Api`).
  - Area controllers: 16 under `Areas/*/Controllers`.
  - Views: 126 under `Views` and 80 under `Areas/*/Views`.
- Legacy System.Web/MVC5/WebApi2 coupling remains pervasive in source (300+ pattern hits in `.cs` files for migration-significant APIs/usages), concentrated in:
  - `App_Start/IocConfig.cs` (Autofac MVC5+WebApi resolver wiring, `HttpContext.Current` registration, `ConfigurationManager` usage)
  - `Global.asax.cs` (startup/bootstrap pipeline, WebApi formatter config, error handling, header handling)
  - API controllers and custom filters/model-binding helpers
- Routing and API registration currently come from legacy boot paths:
  - `RouteConfig.RegisterRoutes` only ignores `.axd` and enables attribute routing.
  - `Global.asax` calls `MapHttpAttributeRoutes`, Autofac WebApi resolver, JSON formatter camelCase resolver, and API exception filter.
- Contract-sensitive hosting behaviors to preserve from `Web.config`:
  - IIS handler mappings for `sitemap*.xml` and `service.wsdl`.
  - Module behaviors including `HttpAuthModule`, telemetry modules, and header/error handling semantics.
  - Existing custom headers and static content mappings.
- View migration hotspots found in Razor files:
  - Extensive HtmlHelper/partial/action helper usage (`Html.Action`, `Html.RenderAction`, `Html.Partial`, `Html.RenderPartial`) and `@Scripts.Render` / `@Styles.Render` patterns.
  - These require staged conversion to ASP.NET Core-compatible patterns while preserving current UI and behavior.

## Dependencies and migration constraints
- Must preserve user-mandated UI/contract behavior parity and existing styling/layout/features.
- Must keep SQL-only wiring decision from earlier tasks; logging/API-recording path remains separately governed by scenario decisions.
- Must maintain non-preview target (`net10.0`) and current coding standards/style conventions.
- Must preserve `service.wsdl` and download/report/export behavior compatibility.

## Planned execution slices (proposed subtasks)
1. Re-enable `Web.UI` compile surface and migrate host bootstrapping (Global.asax/App_Start to Program + DI/routing/filter pipeline) with targeted controller compilation readiness.
2. Migrate API/controller/filter/model-binding/routing behaviors for contract-sensitive endpoints, including JSON behavior and WSDL/sitemap path handling.
3. Migrate Razor/view/static-asset compatibility (`_ViewImports`, helper replacements, partial/component patterns) preserving UI output.
4. Run focused Web.UI parity validation (build/runtime checks and targeted tests) and record remaining blockers, if any.

## Steps
1. Migrate Web.UI feature surface area in dependency-safe slices.
2. Preserve agreed contract/file/UI behaviors and document parity evidence.
3. Apply selective supported adapters only where justified and remove temporary shims before completion.
4. Run targeted Web.UI and UI test validations.

## Done when
- Web.UI feature surface builds/runs on net10 with parity evidence across required behaviors.
- Temporary migration shims/placeholders are removed or explicitly tracked as blockers.
