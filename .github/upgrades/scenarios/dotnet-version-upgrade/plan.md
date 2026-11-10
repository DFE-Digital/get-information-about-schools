# .NET 10 Upgrade Plan

## Overview

**Target**: Web\EdubaseWeb.sln and all 13 included projects on .NET 10, preserving existing clients, file outputs and UI.
**Scope**: Large Framework-to-modern migration: 12 projects currently target net48; Edubase.AcceptanceTests already targets net10.0. Planning only; implementation requires separate approval. Production release is a single cutover, not incremental feature routing. Commits remain manual.

### Selected Strategy

**Bottom-Up (Dependency-First)** - Upgrade from leaf dependencies to the application, validating dependency closures. The TFM migration is one top-level task, decomposed by dependency order during execution, distinct from SDK-style conversion.
**Rationale**: The assessment shows dependent libraries, a high-impact System.Web application, and test references extending beyond their apparent subject projects. Selective multi-targeting is justified per dependency closure, not imposed universally; intermediate in-place states are not promised to build or run as a complete application.

### Dependency Milestones

```text
Common
  -> Data
	 -> Services
		-> Services.Texuna + Web.Resources
		   -> Web.UI
			  -> remaining web-dependent tests
AcceptanceTests: already net10.0; run against the completed application
```

CommonUnitTests accompanies Common; DataUnitTests accompanies Data; ServicesUnitTests accompanies Services. Services.TexunaUnitTests also references Web.UI, and Web.ResourcesUnitTests references Services.TexunaUnitTests: do not claim those suites can validate independently before the web dependency is available. Preserve existing tests and update their references with the migration that makes this necessary.

Each milestone completes only when its required migrated dependency closure builds and its runnable tests pass. Validate preserved Framework consumers where supported; document intentionally unavailable consumers and validate them once migrated. No circular dependency was reported in the assessment graph; re-check the live graph before implementation because ongoing SQL work may change it.

## Upgrade Options

| Option | Selected | Why |
|--------|----------|-----|
| Upgrade Strategy | Bottom-Up | Dependency-first development isolates Framework-to-modern migration failures. |
| Web Projects | In-place rewrite | Convert the existing application for one production cutover without redesigning the UI. |
| Class Libraries without System.Web | Selective temporary Multi-targeting; otherwise In-place | Preserve old consumers only where the value outweighs compatibility work across the dependency closure. |
| System.Web Libraries | In-place | Replace legacy web dependencies with appropriate abstractions before consumers. |
| Package Management | Per-Project (defer CPM to post-migration) | Avoid adding centralisation churn during project-system and framework changes. |
| Unsupported Packages | Resolve Inline | Real replacements and compatible behaviour are required, not compile-only stubs. |
| Unsupported API Handling | Fix Inline | Resolve complex behaviour within its owning work and validation. |
| System.Web Adapters | Selective temporary use | Native APIs for straightforward changes; supported shims only where they materially reduce risk, with tests and removal. |
| Configuration Migration | Manual Migration with Mapping Document | Map complex settings and hosting behaviour explicitly; automate mechanical conversions where appropriate. |
| Logging Framework | Microsoft.Extensions.Logging ILogger | User requirement; replacement destination remains a decision gate. |
| Dependency Injection | Migrate to Built-in DI Container | Replace MVC5 integration while preserving registration and lifetime semantics. |
| Assembly Binding Redirects | Document and Review Before Removing | Resolve underlying dependency conflicts rather than blindly deleting redirects. |
| Entity Framework | Keep EF6 | Avoid simultaneous ORM migration; validate SQL provider, spatial behaviour and existing migrations. |

## Tasks

### 01-readiness-and-compatibility: Establish toolchain, compatibility baselines and decision gates

Verify the .NET 10 SDK, global.json compatibility, build tooling, current project graph and the existing solution/test baseline across all 13 projects. Identify required local services, credentials supplied through approved mechanisms, test environments and external integration access. Establish baseline fixtures and visual/functional checks before changing affected behaviour: routes, methods, authorization, errors, JSON casing/null/date/enum behaviour, file schemas/ordering/encoding, names, headers, downloads, reports and critical UI journeys. Extend existing suites as needed; the absence of an automated test-coverage recommendation is not evidence that parity is already covered. Record pre-existing build/test failures explicitly instead of claiming a passing baseline.

Reconcile the ongoing SQL migration with the assessed code. The user owns confirmation of SQL migration completion and fallback requirements; the executor supplies an inventory of remaining Table Storage consumers and migration evidence. Pause affected storage changes until this is settled; temporary Table Storage support requires a user decision if schedules overlap. Investigate actual OData usage before proposing removal or replacement. The executor verifies the AutoMapper advisory, suitable versions and licensing implications; ask before significant contractual or licensing changes. Identify authentication, token and session compatibility requirements without assuming reauthentication is acceptable.

For ApiRecorderSessionItemRepository and WebLogItemRepository, investigate both writes and reads, correlation, retention, access, sensitive-data handling and any recording/replay functions before proposing an ILogger destination. The user selects the destination and required semantics; Application Insights remains a candidate. Record each unresolved decision with an owner, evidence required, and the task it blocks. These decisions may be resolved during execution research, but must be closed before their affected implementation; implementation approval is not blanket approval of an unresolved choice.

**Done when**: Toolchain and baseline results are recorded; compatibility checks/fixtures are identified or captured through existing test tooling; every external dependency or decision has an owner and blocking task; no storage, logging, OData or authentication strategy is silently assumed. Missing baseline access or significant unresolved decisions are explicit blockers, not successful fallbacks.

---

### 02-configuration-and-dependencies: Map configuration and review assembly/dependency conflicts

Inventory configuration names and purposes across Common, Data, Services, Services.Texuna, Web.Resources, Web.UI and their tests. Map settings, custom sections, transforms, connection strings, secret sources, feature flags, defaults and precedence to their intended .NET 10 equivalents. Include IIS handlers/modules, the service.wsdl mapping, authentication, logging, caching and EF6 setup; moving a section to JSON alone does not recreate behaviour. Prepare the mapping without secret values and ask the user to resolve unclear business/environment semantics. Automate straightforward conversion later rather than hand-copying every key.

Review binding redirects and package-version divergence to distinguish obsolete Framework assembly resolution from genuine conflicts that must be resolved. Verify existing package usage before applying suggested alternatives, particularly OData, authentication and storage. Define selective multi-targeting and adapter candidates from actual dependency/API evidence, including tests and removal criteria. Do not select a new identity provider, protocol version, logging destination or ORM implicitly.

**Done when**: A portable configuration mapping and dependency-conflict review exist; environment/secret precedence and custom-component replacements are accounted for; ambiguities that affect implementation have user decisions or explicit blockers. No secrets are written to documentation.

---

### 03-sdk-project-conversion: Convert legacy project structure without changing target frameworks

Convert the assessment's nine non-SDK projects to SDK style: Common, Data, Services, Services.Texuna, Web.Resources, Web.UI, CommonUnitTests, DataUnitTests and ServicesUnitTests. Convert packages.config to PackageReference as part of this structural work, retaining current TFMs and avoiding unrelated package/API changes. Keep the four existing SDK-style projects intact except where directly necessary for references. Reconcile this inventory against the current branch before execution.

Preserve explicit content/resource inclusions, generated resources, assembly metadata, build/publish assets, test discovery and existing Razor/static-asset tooling. The in-place Framework web project needs project-type-specific conversion research; do not assume a generic class-library conversion preserves its build and publish semantics. If the conversion tool cannot preserve the web project's current-TFM behaviour, stop for a decision rather than silently combining conversion and ASP.NET Core migration. Convert projects sequentially and validate structural changes against the recorded baseline.

**Done when**: Applicable projects are SDK-style with PackageReference, their original TFMs remain unchanged, resources/content and test discovery are preserved, and builds/tests show no unexplained regression from the baseline. Any conversion limitation is resolved explicitly before completion.

---

### 04-net10-application-migration: Migrate project TFMs, dependencies and application behaviour

Upgrade all 12 remaining project TFMs to net10.0, retaining the already-net10.0 acceptance project. Break this task into dependency-ordered subtasks using the milestones above; keep test migration with the production work that requires it, including cross-referencing Texuna and Resources tests. Resolve package and API incompatibilities inline. The assessment places most API impact in System.Web, with Common and Web.UI high-impact areas; research each subtask and preserve real behaviour rather than add placeholders. No warning suppression is authorised. For each library, justify temporary multi-targeting against the complete consumer/dependency chain; where in-place work temporarily breaks old consumers, state that explicitly and validate the migrated closure before proceeding.

In Data retain EF6 and validate provider/SQL/spatial operations, mappings, initialization and migrations without an implicit schema redesign or destructive startup changes. Use the completed SQL migration when confirmed; otherwise stop affected work for the agreed overlap decision. Replace the two excluded logging repositories and AzureTableLogger behaviour through ILogger using the user-approved destination and required read/recording semantics. In Services and Texuna preserve outbound Java API authentication, headers, payloads, serialization and error handling. Preserve Web.Resources names and generated/localized content. Apply verified security remediation and resolve actual package usage rather than interpreting every flagged package as a mandatory protocol or architecture replacement.

Migrate Web.UI in place to ASP.NET Core: startup, middleware, authentication/authorization, antiforgery, sessions/cookies, routing, filters, model binding/validation, DI registrations/lifetimes, controllers, Razor views and build/static assets. Preserve JSON behaviour, both download-generation response shapes, polling, file streaming and redirects, report/export contents, WSDL paths and all UI styling/layout/features. Use native APIs for straightforward System.Web changes; test supported temporary adapters only where justified and replace/remove them before completion. Apply the configuration mapping, automate mechanical transformations, and keep hosting rules where required. No YARP or cross-app identity/session infrastructure is implied. Preserve the Java SOAP backend and its contract boundaries.

**Done when**: Every included project targets net10.0; all migrated dependency closures and relevant tests pass; the full application starts; affected contract/file/UI checks pass; all temporary Framework targets/shims and placeholders are removed; runtime dependency conflicts and required warning fixes are resolved. Storage, logging destination, OData and authentication decisions affecting this work are closed with evidence and user approval where required.

---

### 05-release-readiness: Prepare and rehearse the single-cutover release

Update existing CI/build/publish and deployment configuration for the completed .NET 10 application while preserving frontend assets and external service integration. Confirm the hosting/runtime choice, user-approved downtime and session expectations, health checks, observability and operational rollback requirements. Identify schema/data compatibility with the last known-good deployment so rollback is not assumed to be merely reverting code. Do not introduce parallel production request routing or deploy to production under this task.

Rehearse publishing, startup, critical journeys and recovery in an approved non-production environment using representative configuration and integrations. Include WSDL redirects, authentication/roles, backend requests, file generation/polling/downloads, report/export parity and destination logging. Document recovery of the previous release artifact/configuration and treatment of data written after cutover, with deployment-owner acceptance. Use existing deployment documentation locations and avoid secrets.

**Done when**: A reproducible .NET 10 artifact is produced, non-production deployment and operational recovery are evidenced, and the deployment owner/user has accepted the single-cutover, session/downtime and data-aware rollback arrangements. No production deployment has occurred.

---

### 06-solution-validation: Validate full solution, parity and handover

Build the complete solution and run all unit, integration and acceptance suites against the completed application, including Edubase.AcceptanceTests. Verify representative existing client requests and file fixtures, SOAP/WSDL integration boundaries, and browser/visual/accessibility journeys rather than treating compilation as compatibility proof. Compare results with baseline expectations; unexplained differences, missing external access and unexecuted required checks prevent a parity claim. Check published output and dependency/security results, remove migration-only assets, and confirm no temporary adapters, Framework targets, unsupported package fallbacks or placeholder code remain.

Record unresolved limitations honestly and update directly related operational/developer documentation. Register CPM as a deferred recommendation, not an authorised implementation: 'All projects are now SDK-style and on a single TFM - CPM can be added cleanly without VersionOverride friction.' Hand over evidence and release-readiness decisions for user review; executing this plan does not authorise production cutover.

**Done when**: Full solution builds warning-free, required tests and agreed client/file/UI checks pass, verified dependency findings are addressed, publication/rehearsal evidence and documentation are available, and no required compatibility gate is unverified. Any unavailable verification is reported as blocked, not complete. User approval remains required for production release.
