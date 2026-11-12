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

**Done when**: A reproducible .NET 10 artifact is produced, non-production deployment and operational recovery are evidenced, and the deployment owner/user has accepted the single-cutover, session/downtime and data-aware rollback arrangements. No production deployment has occurred.
