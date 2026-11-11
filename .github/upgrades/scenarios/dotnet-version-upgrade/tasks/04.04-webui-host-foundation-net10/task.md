# 04.04-webui-host-foundation-net10: Retarget Web.UI and migrate host/pipeline/config/DI foundation

# 04.04-webui-host-foundation-net10

## Objective
Migrate Web.UI host foundations to net10 with ASP.NET Core startup, configuration, middleware, and DI scaffolding while preserving required compatibility boundaries.

## Scope
- Project: `Edubase.Web.UI`
- Startup/Program host bootstrap, configuration migration, middleware pipeline foundation, DI registrations
- Authentication/session/antiforgery baseline wiring (behavior-preserving setup)

## Research findings (2026-11-11)
- `Web/Edubase.Web.UI/Edubase.Web.UI.csproj` is SDK-style but still targets `net48` with extensive `System.Web`/MVC5/WebApi/OWIN-era package and framework references, plus large explicit content/include lists and web-specific project extensions.
- Host startup currently relies on `Global.asax.cs` (`Application_Start`, `Application_Error`, `Application_BeginRequest`) and `System.Web` pipeline semantics, including:
  - Autofac MVC5 + WebApi2 resolver bootstrapping (`IocConfig.Register(HttpConfiguration)`)
  - WebApi formatter/filter setup via `GlobalConfiguration.Configure`
  - MVC route/filter registration via `RouteConfig` and `FilterConfig`
  - FluentValidation MVC5 provider setup and custom binder/value provider registration
  - Startup-time cache/external-lookup/logging service initialization.
- OWIN authentication is configured through assembly startups in `App_Start/StartupSecureAccess.cs` and `App_Start/StartupSASimulator.cs` (cookie + SAML2 + CSP nonce middleware). Current controllers consume `HttpContext.GetOwinContext().Authentication`.
- DI root (`App_Start/IocConfig.cs`) is tightly coupled to `System.Web` (`HttpContextWrapper(HttpContext.Current)`, MVC/WebApi Autofac integrations) and to `ConfigurationManager` app settings/connection strings.
- `Web.config` carries broad app settings/connection strings plus critical runtime sections (`system.web`, `system.webServer`, module/handler registrations, custom errors/httpErrors, binding redirects, entityFramework, httpAuthModule). Multiple sections are Framework/IIS specific and require ASP.NET Core equivalents or explicit deferral/removal.
- Existing pipeline modules/handlers include Glimpse/ApplicationInsights/HttpAuthModule and TransferRequest handlers for `sitemap*.xml` and `service.wsdl`; these are migration hotspots for middleware/endpoint parity.

## Affected files (planned)
- `Web/Edubase.Web.UI/Edubase.Web.UI.csproj`
- `Web/Edubase.Web.UI/Program.cs` (new)
- `Web/Edubase.Web.UI/Global.asax.cs` (migration source; final removal likely in cleanup task)
- `Web/Edubase.Web.UI/App_Start/IocConfig.cs`
- `Web/Edubase.Web.UI/App_Start/RouteConfig.cs`
- `Web/Edubase.Web.UI/App_Start/FilterConfig.cs`
- `Web/Edubase.Web.UI/App_Start/StartupSecureAccess.cs`
- `Web/Edubase.Web.UI/App_Start/StartupSASimulator.cs`
- `Web/Edubase.Web.UI/Web.config`
- `Web/Edubase.Web.UI/appsettings.json` (new)
- `Web/Edubase.Web.UI/appsettings.Development.json` (new, as needed)

## Implementation notes for this task
- This task focuses on host foundation and project retargeting scaffolding, not full controller/view parity migration.
- Preserve existing behavior intent by wiring equivalent ASP.NET Core primitives (configuration, DI, middleware order, auth/session/antiforgery baseline) and defer endpoint-by-endpoint controller/view porting to 04.05.
- Keep no-secret policy: map keys and structure without persisting secret values.

## Steps
1. Retarget Web.UI to net10.0 and establish runnable host baseline.
2. Apply configuration mapping outputs from Task 02 (no secret leakage).
3. Migrate core middleware/DI wiring and required auth/session foundation behavior.
4. Resolve high-risk package/API blockers needed before controller/view migration (including FluentValidation baseline decision implementation path).

## Done when
- Web.UI host foundation builds/runs on net10 baseline.
- Config/DI/middleware foundation is in place without silent behavior changes.
- FluentValidation baseline migration path is implemented for Web.UI continuation
