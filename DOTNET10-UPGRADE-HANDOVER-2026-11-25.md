# .NET 10 upgrade developer handover

Prepared: 25 November 2026. Validation evidence below is from 24 November, not a fresh test run.

## Current status

- Branch: `copilot/upgrade-dot-net-10`.
- This session continued an existing upgrade; it did not perform the whole migration from scratch.
- Solution projects inspected under `Web` target `net10.0`. Full solution build and Web UI unit tests pass.
- Local publishing, ZIP packaging, startup and static-asset checks pass.
- **The application is not release-ready.** Razor page rendering fails and database/integration journeys have not been verified.
- Task 04 is historically complete but its recheck exposed remaining migration gaps. Task 05 is in progress; Task 06 needs full validation.
- Existing session changes remain uncommitted. No production deployment, branch sync or Task 05 completion tag was performed.
- Only this standalone file was added for this handover; existing files were not modified.

## Working rules

Continue in Guided mode with manual commits. No production deployment. Obtain specific authorization for persistent pipeline changes and any non-production deployment. Preserve UI, authentication/session semantics, file outputs and Java backend contracts. Retain EF6 rather than introducing an ORM migration. Never copy credentials into source, documents, logs or chat.

The machine is shared. Do not terminate unrelated processes or broadly delete directories.

## Read before continuing

- [Scenario instructions and decisions](.github/upgrades/scenarios/dotnet-version-upgrade/scenario-instructions.md)
- [Task tracker](.github/upgrades/scenarios/dotnet-version-upgrade/tasks.md)
- [Upgrade plan and gates](.github/upgrades/scenarios/dotnet-version-upgrade/plan.md)
- [Task 04 recheck evidence](.github/upgrades/scenarios/dotnet-version-upgrade/tasks/04.06-cross-closure-validation-and-gates/progress-details.md)
- [Task 05 evidence and remaining gates](.github/upgrades/scenarios/dotnet-version-upgrade/tasks/05-release-readiness/task.md)
- [Configuration mapping](.github/upgrades/scenarios/dotnet-version-upgrade/tasks/02.01-config-surface-mapping/configuration-mapping.md)
- [Operational preparation and recovery gates](documentation/operations/deployment-architecture.md#net-10-release-preparation-and-recovery-gates)

Older records can be stale: Task 06's historical net48 test-reference failure predates the current successful build. Task 04 completion does not establish end-to-end parity.

## Changes made in this continuation

Review the uncommitted diff before committing or continuing. The following existing files were changed before this handover request:

| File | Change |
| --- | --- |
| [HomeController.cs](Web/Edubase.Web.UI/Controllers/HomeController.cs) | Removed unused constructor dependencies that blocked activation. News/blob dependencies moved to action-level `[FromServices]` injection. Homepage still queries SQL-backed news before rendering. |
| [CoreServiceRegistration.cs](Web/Edubase.Web.UI/App_Start/CoreServiceRegistration.cs) | Registered scoped SQL connection, `FrontEndDbContext` and SQL news repository. Configured EF6 `MicrosoftSqlDbConfiguration` and `Microsoft.Data.SqlClient` factory. Final registration code does not silently catch failures. Connection creation uses `IConfiguration`. |
| [SqlLaNameCodeRepository.cs](Web/Edubase.Web.UI/Controllers/Api/SqlLaNameCodeRepository.cs) | Uses `Microsoft.Data.SqlClient` and injected `IConfiguration`. Shared builder prefers `ConnectionStrings:FrontEndDatabase`; supports local `FrontEndDatabase` in `localdev` or hosted `SQLServer`/`SQLDatabase`. Missing settings now throw explicit errors. |
| [Web UI project](Web/Edubase.Web.UI/Edubase.Web.UI.csproj) | Added EF6 Microsoft SQL provider for net10, included all `public` assets in output/publish, and excluded legacy root/view IIS configuration from publish so the SDK generates ASP.NET Core hosting configuration. |
| [Program.cs](Web/Edubase.Web.UI/Program.cs) | Configured `public` as web root with `/public` URL prefix, preserving existing asset URLs. |
| [Web.Release.config](Web/Edubase.Web.UI/Web.Release.config) | Replaced Framework-era transform with an ASP.NET Core-compatible empty transform. Old transform failed because it expected missing IIS rewrite sections. HTTPS/HSTS remain middleware responsibilities. |
| [SQL configuration tests](Web/Edubase.Web.UIUnitTests/Controllers/Api/SqlLaNameCodeRepositoryTests.cs) | Added in-memory configuration tests for hosted/local settings, precedence and missing settings. This edit applied before a multi-file patch was denied at the CI template; test changes were retained. |
| [AcceptanceTests project](Web/Edubase.AcceptanceTests/Edubase.AcceptanceTests.csproj) | Excluded `obj`/`artifacts` from compile globs to prevent duplicate generated target-framework attributes. Project build passes; acceptance tests were not executed. |
| [Common project](Web/Edubase.Common/Edubase.Common.csproj) | Excluded generated C# under `obj`/`artifacts` after alternate-output experiments caused duplicate-source errors. Review necessity after targeted artifact cleanup. |
| [Task 04 progress](.github/upgrades/scenarios/dotnet-version-upgrade/tasks/04.06-cross-closure-validation-and-gates/progress-details.md) | Recorded build/startup, process locks and provider investigations. |
| [Task 05 record](.github/upgrades/scenarios/dotnet-version-upgrade/tasks/05-release-readiness/task.md) | Recorded publishing fixes, validations and outstanding release gates. |
| [Task tracker](.github/upgrades/scenarios/dotnet-version-upgrade/tasks.md) | Added an explicit release-gate warning. |
| [Deployment architecture](documentation/operations/deployment-architecture.md) | Added local artifact preparation, non-production prerequisites and data-aware rollback/recovery gates. |

No CI/pipeline files were changed.

## Validation results

| Check | Observed result |
| --- | --- |
| Full solution Release build with alternate per-project output | Passed: 0 errors, 86 warnings. |
| Focused SQL configuration and Guidance tests | 19 passed, 0 failed. |
| Full Web UI unit suite | 893 passed, 0 failed. Earlier 887-test evidence predates the new SQL cases. |
| AcceptanceTests project build | Passed; not an acceptance-suite run. |
| `npm run buildCi` | Passed with 36 webpack warnings. |
| Release publish | Passed with existing dependency/compiler warnings. |
| Artifact inspection | Runtime files, JSON settings, generated ASP.NET Core IIS configuration, CSS/fonts and 33 JS bundles present. |
| Published and extracted ZIP startup | Host started; local validation hosts stopped afterwards. |
| `/health` | HTTP 200; liveness only, no SQL/backend/auth checks. |
| Main CSS | HTTP 200, 287929 bytes. |
| GOV.UK font | HTTP 200, 33382 bytes. |
| Main hashed JS bundle from extracted ZIP | HTTP 200, 191831 bytes. |
| `/about` | HTTP 500: view `About` not found. |
| `/` | HTTP 500: explicit missing SQL configuration. Earlier errors were missing DI, provider resolution and invalid SQL connection. |
| `git diff --check` | Passed. |

Local artifact: `Web\Edubase.Web.UI\bin\release-candidate.zip`, 18706458 bytes.
SHA-256: `99716AB14D937DD430733CA8385443DC4B53BDBAE61B270541068BB418CB9703`.
This ignored/local ZIP is not an approved release. Regenerate after changes; it may not exist in a fresh clone.

## Remaining work, in recommended order

### 1. Restore Razor rendering (does not require SQL)

The Web UI project disables Razor build/publish compilation and source generation. Copying views alone does not make rendering work. Restore a supported ASP.NET Core view compilation/runtime path and fix actual errors, without placeholders or further exclusions hiding failures.

Verify `/about` and its layout/helpers/assets, then other pages and areas. Verify homepage rendering after SQL is available.

### 2. Investigate existing development configuration before requesting new credentials

A git-ignored `devsecrets.gias.config.alwaysignore` file exists locally at the repository root. It contains a populated `FrontEndDatabase` entry, but inspection did not establish conventional SQL server/database/authentication fields or usable connectivity. **Do not assume it contains working credentials.** No values were displayed or copied.

The ASP.NET Core host does not automatically load that legacy XML file. Investigate its intended format/loading mechanism and securely map approved settings into `IConfiguration`.

The modern SQL path accepts environment setting `ConnectionStrings__FrontEndDatabase`. Local or approved non-production SQL is sufficient; production access is unnecessary. Verify news queries and EF6 initialization/provider compatibility without destructive schema/startup changes.

Other legacy `ConfigurationManager` consumers remain for integrations, authentication, caching and view models. The SQL builder fix does not migrate all runtime settings.

### 3. Obtain authorization and update CI/deployment configuration

- [Azure build template](pipelines/template-build-and-test.yml): replace Framework-era VSBuild/Web Deploy packaging with SDK restore/build/test/publish and correct unit-test discovery.
- [Azure pipeline](azure-pipelines.yml): review legacy agent assumptions.
- [Deployment template](pipelines/template-deployment-stage.yml): consume a ZIP of publish-directory contents instead of a legacy Web Deploy package; preserve environment protections.
- [GitHub build workflow](.github/workflows/build-and-test.yml): replace .NET 6 selection and Framework test DLL paths.

The Azure build-template write was denied for lack of specific authorization. Subsequent Azure pipeline/deployment-template hunks were not attempted. The user was unavailable when approval/rehearsal details were requested. Obtain specific approval before retrying. Editing approval alone does not authorize pushing, pipeline execution or deployment.

### 4. Finish migration and contract checks

- Review Web UI exclusion `Models\BaseViewModel.cs`, Services exclusion `Security\Permissions\Permission.cs`, remaining net48-conditional project blocks, adapters and legacy packages.
- Verify authentication schemes, roles, sessions/cookies, antiforgery, filters, routing, serialization, logging/API recording and errors.
- Check `/service.wsdl`: the host maps NotFound while `HomeController` also declares a redirect route. Contract behavior is unverified.
- Verify Java backend/SOAP, file generation/polling/downloads, exports/reports and agreed browser/visual/accessibility journeys.
- Resolve required dependency/security/compiler warnings per the plan and prior user decisions. AutoMapper 10.1.1 was retained by prior decision; do not silently introduce licensing changes.
- Run all required unit/integration/acceptance suites. The latest full test result covers Web UI unit tests only.

### 5. Complete Task 05 rehearsal, then Task 06

Identify an approved non-production target and supply integration/sign-in/telemetry settings through approved secret mechanisms. Confirm Windows .NET 10 hosting/Hosting Bundle, downtime/session expectations, observability and abort criteria.

Retain the last known-good artifact with matching configuration/runtime. Have the database owner establish backward schema compatibility and treatment of post-cutover writes; code-only rollback may be unsafe. Rehearse deployment, representative journeys and recovery with deployment-owner acceptance.

Do not mark Task 05/06 complete while required gates are unavailable. No production cutover is authorized.

## Resume commands

Run from the repository root with Windows PowerShell paths. Check exit codes before continuing.

```powershell
git --no-pager branch --show-current
git --no-pager status --short
dotnet build .\Web\EdubaseWeb.sln -c Release -p:OutputPath=bin\release-readiness\
dotnet test .\Web\Edubase.Web.UIUnitTests\Edubase.Web.UIUnitTests.csproj -c Release -p:OutputPath=bin\release-readiness\ --no-build

# Run from Web\Edubase.Web.UI:
npm run buildCi

# Back at repository root; use an empty destination:
dotnet publish .\Web\Edubase.Web.UI\Edubase.Web.UI.csproj -c Release -p:OutputPath=bin\release-readiness\ -o .\Web\Edubase.Web.UI\bin\release-candidate-next
```

Restore frontend dependencies with `npm ci` if missing. Start the published DLL from its publish directory on a free loopback port. Stop only the validation host you started.

## Environment cautions

Default-output builds were blocked by running hosts holding DLLs. Historical PIDs are not safe termination targets. Alternate per-project `OutputPath` worked; globally shared `BaseIntermediateOutputPath` experiments caused asset collisions/duplicate sources and should not be repeated.

Some process termination and temporary/artifact cleanup attempts were denied. Leftovers may remain; inspect exact paths and seek necessary authorization. Final ZIP extraction scratch output was removed; published output/ZIP were retained. A later authorized health request passed, but this is not blanket network approval.

**Immediate next objective:** restore a real Razor-rendered page, then resolve existing development configuration and verify the SQL-backed homepage. Continue independent local work while waiting for specific CI/environment prerequisites.
