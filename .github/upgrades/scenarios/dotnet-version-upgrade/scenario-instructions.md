# .NET 10 Upgrade

## Preferences
- **Flow Mode**: Guided
- **Target Framework**: net10.0
- **Assessment Scope**: Solution, Web\EdubaseWeb.sln, all included projects.
- **Current Stage Limit**: Upgrade options confirmed; generate plan for review. Implementation requires separate explicit approval.

## Source Control
- **Source Branch**: main
- **Working Branch**: copilot/upgrade-dot-net-10
- **Commit Strategy**: Manual
- **Branch Sync**: Disabled
- **Tagging Policy**: Create a tag before implementation starts and another tag at completion of each implementation-plan task.

## User Preferences
### Technical Preferences
- Upgrade this solution and projects to .NET 10.
- Need to keep backward compatibility with existing clients, eg SOAP endpoints, contracts, exports, downloads, reports.
- UI style, layout, features should all be preserved.
- Do not substitute .NET 11 preview for the requested .NET 10 target.
- Use standard Microsoft.Extensions.Logging ILogger for logging.
- ApiRecorderSessionItemRepository and WebLogItemRepository currently write to Azure Table Storage and will not migrate to SQL.
- The existing application's feature-flagged Table Storage to SQL migration is expected to complete before .NET 10 implementation. If timings overlap, assess temporary Table Storage support complexity and ask the user before choosing. Confirm migration completion and fallback requirements before omitting Table Storage; the two logging repositories require separate treatment.
- SQL migration scope is the ten data areas already represented by SQL repositories and migration flags; assume this SQL migration completes before .NET 10 implementation and these areas should no longer use Azure Table Storage.
- Logging and API recording remain outside SQL migration scope; use Application Insights for both application logging and API session recordings, and retire the current admin log viewer.
- OData disposition is deferred until deeper runtime verification confirms whether active contracts exist.
- Acceptance-test environment access (`devsecrets.gias.config.alwaysignore` + reachable target environment) is an explicit external blocker until provided.
- Deployment preference: migrate in one step, not incrementally; prefer a single production cutover rather than a staged feature-by-feature migration.
- Use temporary library multi-targeting only where keeping existing consumers buildable provides enough value to justify the extra work, not automatically for every library. Evaluate each library and its dependency chain; do not assume successful builds establish runtime compatibility.
- Use System.Web Adapters selectively, not automatically: prefer native ASP.NET Core APIs for straightforward changes; consider temporary shims only for supported, tightly coupled legacy request-context usages where they materially reduce migration risk. Verify actual usage and behaviour with explicit tests and remove temporary shims before completion. Adapters do not make MVC5 run on ASP.NET Core or require side-by-side deployment.
- Enforce SQL-only repository wiring now for the migrated data areas and remove associated migration feature flags; handle logging/API-recording storage path separately.

## Key Decisions Log
- 2026-11-06: User confirmed .NET 10 solution-wide assessment, Guided mode, new upgrade-dotnet-10 branch from clean main, manual commits and disabled automatic branch sync.
- 2026-11-09: User prefers single-step deployment. Detailed cutover, downtime and rollback arrangements remain to be confirmed during planning.
- 2026-11-09: User authorised the planning stage only. Retain Guided mode and obtain approval before implementation.
- 2026-11-09: User confirmed the presented options with selective multi-targeting and selective temporary System.Web Adapters refinements. Confirmation authorises plan generation, not implementation.
- 2026-11-10: User renamed the working branch to copilot/upgrade-dot-net-10 (superseding the initial working branch name above) and committed assessment/planning work as 0fdcd8cb0. Verified the working tree was clean before recording this update. Commits remain manual; implementation still requires explicit approval.
- 2026-11-10: User approved implementation start with task 01 readiness/compatibility, requested branch tagging before implementation and after each task completion, reaffirmed Guided mode and manual commits, and prohibited production deployment.
- 2026-11-10: User confirmed SQL migration scope covers the ten feature-flagged data areas and will complete before .NET 10 implementation (no Azure Table Storage use expected for those areas). Logging/API recording are excluded from SQL migration; application logging direction is ILogger/Application Insights, while admin log viewer and API session recording destination remain unresolved.
- 2026-11-10: User decided to use Application Insights for both application logging and API session recordings, and to retire the existing admin log viewer in the .NET 10 migration.
- 2026-11-10: User deferred the OData removal/preservation decision until deeper runtime verification is completed.
- 2026-11-10: User directed acceptance-test environment prerequisites to remain an explicit external blocker until valid secrets and environment access are provided.
- 2026-11-11: User agreed Task 03 should not perform FluentValidation 11.x behavioral MVC5 migration work; keep Task 03 structural and defer FluentValidation behavior migration to Task 04.
- 2026-11-11: User confirmed SQL replacement is expected to be complete in this branch and instructed enforcement of SQL-only wiring now, including removal of associated migration feature flags, while handling logging/API-recording path separately.
- 2026-11-11: User accepted replacing System.Device.Location usage as long as new functionality remains equivalent.
