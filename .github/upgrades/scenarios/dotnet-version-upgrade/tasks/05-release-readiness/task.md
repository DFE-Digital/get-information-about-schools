# 05-release-readiness: Prepare and rehearse the single-cutover release

Update existing CI/build/publish and deployment configuration for the completed .NET 10 application while preserving frontend assets and external service integration. Confirm the hosting/runtime choice, user-approved downtime and session expectations, health checks, observability and operational rollback requirements. Identify schema/data compatibility with the last known-good deployment so rollback is not assumed to be merely reverting code. Do not introduce parallel production request routing or deploy to production under this task.

Rehearse publishing, startup, critical journeys and recovery in an approved non-production environment using representative configuration and integrations. Include WSDL redirects, authentication/roles, backend requests, file generation/polling/downloads, report/export parity and destination logging. Document recovery of the previous release artifact/configuration and treatment of data written after cutover, with deployment-owner acceptance. Use existing deployment documentation locations and avoid secrets.

## Current handoff status (2026-11-12)
- Migration execution is at post-application-migration stage.
- Latest validation evidence before this task:
  - `run_build` => successful workspace build.
  - `dotnet test Web/Edubase.Web.UIUnitTests/Edubase.Web.UIUnitTests.csproj -c Release --no-build` => 887/887 passed.
- Task 04 closure evidence is recorded at:
  - `tasks/04.06-cross-closure-validation-and-gates/progress-details.md`.
- Deployment constraint remains active:
  - No production deployment without explicit user approval.

## Carry-forward checks for this task
- Confirm and document release rehearsal in non-production only.
- Confirm cutover/rollback runbook is explicit about data compatibility and post-cutover writes.
- Verify remaining non-test csproj compile excludes are intentionally retained or removed before final closure:
  - `Web/Edubase.Web.UI/Edubase.Web.UI.csproj`: `Models\\BaseViewModel.cs`
  - `Web/Edubase.Services/Edubase.Services.csproj`: `Security\\Permissions\\Permission.cs`

## Local release preparation (2026-11-24)

**Status: in progress; release gates remain blocked. No deployment, commit or completion tag created.**

- Reproduced a Release publish failure: the legacy `web.Release.config` expected Framework IIS rewrite sections absent from the SDK-generated ASP.NET Core configuration.
- Replaced the Release transform with an ASP.NET Core-compatible transform and excluded legacy root/view IIS configuration from publish. Verified the generated `web.config` uses `AspNetCoreModuleV2` and launches `Edubase.Web.UI.dll`, without Framework config sections/modules.
- Configured the existing `public` directory as the web root, retained the `/public` URL prefix used by views/helpers, and included all public assets in build/publish output.
- Migrated the shared SQL connection builder and its callers to `IConfiguration`. `ConnectionStrings:FrontEndDatabase` takes precedence; the local `FrontEndDatabase` setting and hosted `SQLServer`/`SQLDatabase` settings remain supported. Missing settings now fail explicitly rather than constructing an invalid SQL endpoint. Removed exception-swallowing around EF provider registration.

### Validation evidence

- `npm run buildCi` in `Web/Edubase.Web.UI`: passed, 36 webpack warnings.
- `dotnet test .\Web\Edubase.Web.UIUnitTests\Edubase.Web.UIUnitTests.csproj -c Release -p:OutputPath=bin\release-readiness\ --filter "FullyQualifiedName~SqlLaNameCodeRepositoryTests|FullyQualifiedName~GuidanceControllerTests"`: 19 passed.
- Full Web UI unit suite against the same output: 893 passed, 0 failed.
- Full solution build with `-c Release -p:OutputPath=bin\release-readiness\`: passed, 86 warnings, 0 errors.
- `dotnet publish .\Web\Edubase.Web.UI\Edubase.Web.UI.csproj -c Release -p:OutputPath=bin\release-readiness\ -o .\Web\Edubase.Web.UI\bin\release-artifact-validated`: passed with existing warnings.
- Published artifact contains runtime files, JSON configuration, generated IIS hosting configuration, both main/GOV.UK CSS, fonts and 33 JavaScript bundles.
- Started the published DLL locally from its publish directory at `http://127.0.0.1:5084`:
  - `/health`: HTTP 200.
  - `/public/assets/stylesheets/main.css`: HTTP 200, 287929 bytes.
  - `/public/govuk-frontend/fonts/light-94a07e06a1-v2.woff2`: HTTP 200, 33382 bytes.
  - `/about`: HTTP 500, view `About` not found. Razor compilation remains disabled in the application project; view-rendering parity is not established.
  - `/`: HTTP 500 with an explicit missing SQL configuration error. No configured SQL/integration rehearsal was performed.
  - Stopped the local validation host in the check's cleanup.
- Packaged publish-directory contents as `Web/Edubase.Web.UI/bin/release-candidate.zip` (18706458 bytes). ZIP root layout verified; published CSS/JavaScript hashes match the frontend build.
- Extracted the ZIP locally and started its DLL at `http://127.0.0.1:5085`: health, main CSS and the hashed main JavaScript bundle all returned HTTP 200. Stopped the extracted-artifact host. This was a local packaging smoke test, not the required non-production integration/recovery rehearsal.
- ZIP SHA-256: `99716AB14D937DD430733CA8385443DC4B53BDBAE61B270541068BB418CB9703`.
- `git diff --check`: passed.

### Remaining gates and ownership

- **Application migration work:** restore compiled/runtime Razor views and verify real rendered pages. Review other Task 04 carry-forwards; compile/test success alone is not feature parity.
- **CI configuration approval (user):** edits to the Azure build template were denied because persistent pipeline changes require specific authorization. Pipeline files remain unchanged. Azure still uses VSBuild/Web Deploy packaging; GitHub's build workflow still selects .NET 6 and Framework test paths. Required changes are SDK build/test/publish, correct unit-test discovery and ZIP deployment of the new artifact. No pipeline was triggered.
- **Environment access (user/deployment owner):** identify an approved non-production target and provide SQL, Java backend, sign-in and telemetry settings via approved secret providers, not chat or committed files. Several legacy `ConfigurationManager` consumers remain outside the SQL builder; their configuration parity is still a gate.
- **Operational acceptance (deployment owner):** verify .NET 10 Windows hosting/Hosting Bundle availability, identity/session continuity or agreed reauthentication, downtime and observability. Rehearse restoring the previous artifact plus its configuration, and agree how to handle data written after cutover.
- **Compatibility validation:** authentication/roles, SOAP/WSDL, downloads/polling, reports and visual/accessibility journeys remain required. Dependency/security and webpack warnings remain unresolved.
- A request for CI authorization and a rehearsal target received no available user response. Continue local preparation only; do not infer approval or mark Task 05 complete.

Operational preparation and rollback gates are documented in [deployment architecture](../../../../../../documentation/operations/deployment-architecture.md#net-10-release-preparation-and-recovery-gates).

**Done when**: A reproducible .NET 10 artifact is produced, non-production deployment and operational recovery are evidenced, and the deployment owner/user has accepted the single-cutover, session/downtime and data-aware rollback arrangements. No production deployment has occurred.
