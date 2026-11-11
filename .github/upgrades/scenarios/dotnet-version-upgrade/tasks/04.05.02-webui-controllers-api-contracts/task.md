# 04.05.02-webui-controllers-api-contracts: Migrate controllers/filters/model-binding and preserve endpoint contracts

## Objective
Migrate MVC/API/Area controllers and supporting filters/model binding to ASP.NET Core while preserving endpoint behavior, response semantics, and contract-sensitive paths.

## Scope
- Controllers under Controllers, Controllers/Api, and Areas/*/Controllers
- Filter and model-binding components used by those controllers
- Contract-sensitive routes and behaviors including JSON semantics, download/polling/redirect paths, sitemap/service.wsdl handling

## Research findings (2026-11-11)
- API/controller migration scope is large and high-risk:
  - `Controllers/Api`: 13 files (9 API controllers still inherit `ApiController` + helper/repo interfaces/classes).
  - `Areas/*/Controllers`: includes 1 additional API controller (`Areas/Groups/Controllers/GroupApiController.cs`) plus extensive MVC controllers with attribute-route and route-name dependencies.
- Web API 2 patterns still present and must be migrated:
  - `ApiController`, `IHttpActionResult`, `HttpResponseMessage`, `Request.CreateResponse(...)`, `ResponseMessage(...)`, `System.Web.Http.Route` attributes.
  - Example hotspots: `ApprovalsApiController`, `AcademyOpeningsApiController`, `GroupApiController`.
- Legacy global startup still wires Web API + filter/model-binding behavior:
  - `Global.asax.cs` registers `MapHttpAttributeRoutes`, `ApiExceptionFilter`, `DefaultModelBinderEx`, and `TokenValueProviderFactory`.
- Model-binding/value-provider migration is required:
  - `Helpers/ModelBinding/DefaultModelBinderEx.cs` uses MVC5 `DefaultModelBinder` override pattern.
  - `Helpers/ValueProviders/TokenValueProviderFactory.cs` uses MVC5 `ValueProviderFactory` and `DependencyResolver.Current`.
- Filter migration is required:
  - `Filters/ApiExceptionFilter.cs` depends on WebApi exception filter types and `Request.Properties["MS_HttpContext"]`.
  - `App_Start/UnauthorizedAccessAttribute.cs` is MVC5 filter type, while new Core equivalent exists (`CoreUnauthorizedAccessExceptionFilter`).

## Dependency and parity constraints
- Preserve route names and route templates used by redirects and client-side/API consumers.
- Preserve API JSON behavior and status-code semantics (especially bad-request/not-found/no-content flows).
- Preserve anti-forgery and authorization behavior for mutation endpoints.
- Do not silently change binding-source behavior; explicitly map `[FromUri]`/complex-parameter semantics where needed.

## Decomposition rationale
This task exceeds safe atomic size for one edit pass due to:
- high file count across independent controller groups,
- mixed MVC + WebApi migration patterns,
- shared filter/model-binding infrastructure that can break many endpoints if changed in one batch.

The task should execute in ordered slices: (1) API surface conversion, (2) filter/model-binding migration, (3) MVC/area contract route parity, (4) targeted parity validation.

## Steps
1. Port controller base classes/usings/action result patterns to ASP.NET Core equivalents.
2. Migrate custom filters/model binders/value providers and route mappings used by migrated controllers.
3. Preserve JSON behavior and API response expectations for existing consumers.
4. Validate key endpoint flows for routing/status/shape parity.

## Done when
- Controller and API surfaces build and route under ASP.NET Core.
- Contract-sensitive endpoint behavior is preserved or explicitly documented as blocker for user decision.
