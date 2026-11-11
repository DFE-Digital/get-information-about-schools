# 04-net10-application-migration: Migrate project TFMs, dependencies and application behaviour

Upgrade all 12 remaining project TFMs to net10.0, retaining the already-net10.0 acceptance project. Break this task into dependency-ordered subtasks using the milestones above; keep test migration with the production work that requires it, including cross-referencing Texuna and Resources tests. Resolve package and API incompatibilities inline. The assessment places most API impact in System.Web, with Common and Web.UI high-impact areas; research each subtask and preserve real behaviour rather than add placeholders. No warning suppression is authorised. For each library, justify temporary multi-targeting against the complete consumer/dependency chain; where in-place work temporarily breaks old consumers, state that explicitly and validate the migrated closure before proceeding.

In Data retain EF6 and validate provider/SQL/spatial operations, mappings, initialization and migrations without an implicit schema redesign or destructive startup changes. Use the completed SQL migration when confirmed; otherwise stop affected work for the agreed overlap decision. Replace the two excluded logging repositories and AzureTableLogger behaviour through ILogger using the user-approved destination and required read/recording semantics. In Services and Texuna preserve outbound Java API authentication, headers, payloads, serialization and error handling. Preserve Web.Resources names and generated/localized content. Apply verified security remediation and resolve actual package usage rather than interpreting every flagged package as a mandatory protocol or architecture replacement.

Migrate Web.UI in place to ASP.NET Core: startup, middleware, authentication/authorization, antiforgery, sessions/cookies, routing, filters, model binding/validation, DI registrations/lifetimes, controllers, Razor views and build/static assets. Preserve JSON behaviour, both download-generation response shapes, polling, file streaming and redirects, report/export contents, WSDL paths and all UI styling/layout/features. Use native APIs for straightforward System.Web changes; test supported temporary adapters only where justified and replace/remove them before completion. Apply the configuration mapping, automate mechanical transformations, and keep hosting rules where required. No YARP or cross-app identity/session infrastructure is implied. Preserve the Java SOAP backend and its contract boundaries.

## Research findings
- Current topology (dependency-first): Common -> Data -> Services -> Services.Texuna/Web.Resources -> Web.UI -> tests.
- Current TFM state:
  - `Edubase.AcceptanceTests` already targets `net10.0`.
  - Remaining projects are still `net48` / `v4.8` and must be migrated under this task.
- Task 03 output context now in effect:
  - Core migration targets are SDK-style, including `Web.UI`.
  - `Edubase.Services.TexunaUnitTests` remains legacy non-SDK (`TargetFrameworkVersion v4.8`) and is in scope for Task 04 migration.
  - Web.UI conversion blocker captured: FluentValidation MVC5-era usage is incompatible with transitive FluentValidation 11.x behavior and is explicitly deferred for resolution in Task 04.
- User decision constraints carried from scenario instructions:
  - Keep EF6 in Data.
  - Application logging and API recording destination is Application Insights; retire legacy admin log viewer.
  - OData disposition is still deferred and requires runtime verification before any removal/replacement decision.
  - Acceptance environment prerequisites remain an external blocker for fully green acceptance validation.

## Decomposition rationale
- The task spans framework retargeting, package/API migration, and full Web.UI host/pipeline/auth/view behavior migration with high blast radius.
- Dependency-ordered execution is mandatory to avoid cascading consumer failures.
- Web.UI migration should be separated into foundation and behavior-preservation phases due size/risk and explicit FluentValidation/auth/session/contract gates.

**Done when**: Every included project targets net10.0; all migrated dependency closures and relevant tests pass; the full application starts; affected contract/file/UI checks pass; all temporary Framework targets/shims and placeholders are removed; runtime dependency conflicts and required warning fixes are resolved. Storage, logging destination, OData and authentication decisions affecting this work are closed with evidence and user approval where required.
