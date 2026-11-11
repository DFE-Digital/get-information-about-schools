# 01-readiness-and-compatibility: Establish toolchain, compatibility baselines and decision gates

Verify the .NET 10 SDK, global.json compatibility, build tooling, current project graph and the existing solution/test baseline across all 13 projects. Identify required local services, credentials supplied through approved mechanisms, test environments and external integration access. Establish baseline fixtures and visual/functional checks before changing affected behaviour: routes, methods, authorization, errors, JSON casing/null/date/enum behaviour, file schemas/ordering/encoding, names, headers, downloads, reports and critical UI journeys. Extend existing suites as needed; the absence of an automated test-coverage recommendation is not evidence that parity is already covered. Record pre-existing build/test failures explicitly instead of claiming a passing baseline.

Reconcile the ongoing SQL migration with the assessed code. The user owns confirmation of SQL migration completion and fallback requirements; the executor supplies an inventory of remaining Table Storage consumers and migration evidence. Pause affected storage changes until this is settled; temporary Table Storage support requires a user decision if schedules overlap. Investigate actual OData usage before proposing removal or replacement. The executor verifies the AutoMapper advisory, suitable versions and licensing implications; ask before significant contractual or licensing changes. Identify authentication, token and session compatibility requirements without assuming reauthentication is acceptable.

For ApiRecorderSessionItemRepository and WebLogItemRepository, investigate both writes and reads, correlation, retention, access, sensitive-data handling and any recording/replay functions before proposing an ILogger destination. The user selects the destination and required semantics; Application Insights remains a candidate. Record each unresolved decision with an owner, evidence required, and the task it blocks. These decisions may be resolved during execution research, but must be closed before their affected implementation; implementation approval is not blanket approval of an unresolved choice.

## Scope inventory
- Projects in solution: 13 (12 net48 + 1 net10.0 acceptance test project).
- Test projects discovered: 7 (`Edubase.CommonUnitTests`, `Edubase.DataUnitTests`, `Edubase.ServicesUnitTests`, `Edubase.Web.ResourcesUnitTests`, `Edubase.Web.UIUnitTests`, `Edubase.Services.TexunaUnitTests`, `Edubase.AcceptanceTests`).
- Topological dependency order confirmed (leaf-first): Common -> Data -> Services -> Web.Resources -> Services.Texuna -> Web.UI -> test projects.
- Distinct concerns for this task: toolchain readiness, baseline build/test evidence, external dependency inventory, unresolved decision log.

## Baseline findings (readiness)
### Toolchain and build
- `validate_dotnet_sdk_installation(net10.0)`: compatible SDK found.
- `validate_dotnet_sdk_in_globaljson(net10.0)`: no `global.json` found.
- `run_build(projectPath: null)`: solution build baseline is successful.
- `dotnet list package --vulnerable` on solution reports:
  - `AutoMapper 10.1.1` high severity advisory (`GHSA-rvv3-g6hj-g44x`) in `Edubase.Web.UIUnitTests` (assessment also flags `Common` and `Web.UI`).
  - `System.Security.Cryptography.Xml 4.5.0` moderate advisory (`GHSA-vh55-786g-wjwj`) in `Edubase.AcceptanceTests`.
  - Command also confirms CLI limitation for legacy Web Application project evaluation (`Microsoft.WebApplication.targets` import missing under dotnet SDK path), reinforcing that legacy web project validation should use VS/MSBuild-oriented tooling during migration.

### Test baseline
- Full discovered test baseline run: 1359 tests, 1356 passed, 3 failed.
- All 3 failures are acceptance scenarios:
  - "Get an Establishment by URN that does not exist"
  - "Get an Establishment by URN without signing in"
  - "Get an Establishment by URN for a back office user"
- Shared failure cause: `System.ArgumentNullException (uriString)` creating `new Uri(baseAddress)` in `Edubase.AcceptanceTests/DependencyInjection/DependencyInjection.cs:59`, with `baseAddress` sourced from settings (`GiasFrontEndBaseAddress`).
- Baseline interpretation: failures are environment/configuration-dependent and pre-existing for this run; they are recorded, not silently treated as application regressions.

### External environment and credential dependencies
- Acceptance tests require repository-root `devsecrets.gias.config.alwaysignore` (loaded by `Settings.Load()`), including `GiasFrontEndBaseAddress`, `Environment`, `UseLocalSignIn`, plus service credentials/keys (e.g., Companies House, OS Places, Texuna, Lookup API, simulator URI/GUID).
- Acceptance README confirms tests require a separately running GIAS web app and compatible sign-in simulator configuration; runner does not seed/start the app.

## Compatibility and migration evidence inventory
### Table Storage and logging hotspots
- `ApiRecorderSessionItemRepository` and `WebLogItemRepository` both derive from `TableStorageBase<T>` (`Microsoft.WindowsAzure.Storage.Table`).
- `WebLogItemRepository` read/query usage is active in `Web.UI/Controllers/AdminController.cs` (including filter operations over returned messages).
- `ApiRecorderSessionItemRepository` is injected into `HttpClientWrapper` via `IocConfig` and used behind `EnableApiLogging` behavior.
- Feature flags indicate staged SQL migration for multiple tables, including `Feature_ApiRecorderSessionItemsMigration` in `Web.UI/Web.config`.

### OData status (readiness check)
- OData packages/references exist in `Edubase.Web.UI` (`Microsoft.AspNet.WebApi.OData`, `Microsoft.Data.OData`, `System.Web.Http.OData`).
- No direct OData controller/query attribute usage found in current `Web.UI` code search (`ODataConventionModelBuilder`, `EnableQuery`, `ODataQueryOptions` etc.).
- Current evidence suggests package-level presence may exceed active code usage; final decision deferred pending deeper runtime/route verification in migration tasks.

### Authentication/session compatibility baseline
- Current web app startup uses cookie authentication + Sustainsys SAML2 (`StartupSecureAccess.cs`, `StartupSASimulator.cs`), with anti-forgery claim identity configuration and session expiry from app settings.
- This is recorded as a parity-sensitive boundary for later ASP.NET Core migration tasks; no reauthentication assumptions are made.

## Resolved decisions
1. **SQL migration completion and fallback scope**
   - Decision: SQL migration covers the ten feature-flagged data areas and should be assumed complete before .NET 10 implementation.
   - Outcome: no Azure Table Storage support is expected for those ten migrated areas during .NET 10 implementation.
   - Affected tasks: `04-net10-application-migration` (storage migration assumptions now fixed for those areas).

2. **Logging destination and semantics for excluded Table Storage repositories**
   - Decision: use Application Insights for both application logging and API session recordings; retire the current admin log viewer.
   - Outcome: `ApiRecorderSessionItemRepository`/`WebLogItemRepository` replacement work can target App Insights and does not need to preserve the current admin viewer surface.
   - Affected tasks: `04-net10-application-migration` (logging/recording replacement implementation).

## Unresolved decisions and owners
1. **Acceptance-test baseline environment contract**
   - Owner: User/environment owner.
   - Decision status: treat as an explicit external blocker until valid `devsecrets.gias.config.alwaysignore` values and reachable environment access are provided.
   - Evidence required: valid secrets/config plus reachable target environment for acceptance scenarios.
   - Blocks: full green acceptance baseline for this task and final parity claims in later tasks.

2. **OData handling decision**
   - Owner: User (contract compatibility decision), executor (usage/runtime evidence).
   - Decision status: deferred by user until deeper runtime verification.
   - Evidence required: confirmation whether OData endpoints are active consumers/contracts or removable compatibility baggage.
   - Blocks: OData package/API migration approach in tasks `02` and `04`.

**Done when**: Toolchain and baseline results are recorded; compatibility checks/fixtures are identified or captured through existing test tooling; every external dependency or decision has an owner and blocking task; no storage, logging, OData or authentication strategy is silently assumed. Missing baseline access or significant unresolved decisions are explicit blockers, not successful fallbacks.
