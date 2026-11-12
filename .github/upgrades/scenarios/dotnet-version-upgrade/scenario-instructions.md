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

## Key Decisions Log
- 2026-11-06: User confirmed .NET 10 solution-wide assessment, Guided mode, new upgrade-dotnet-10 branch from clean main, manual commits and disabled automatic branch sync.
- 2026-11-09: User prefers single-step deployment. Detailed cutover, downtime and rollback arrangements remain to be confirmed during planning.
- 2026-11-09: User authorised the planning stage only. Retain Guided mode and obtain approval before implementation.
- 2026-11-09: User confirmed the presented options with selective multi-targeting and selective temporary System.Web Adapters refinements. Confirmation authorises plan generation, not implementation.
- 2026-11-10: User renamed the working branch to copilot/upgrade-dot-net-10 (superseding the initial working branch name above) and committed assessment/planning work as 0fdcd8cb0. Verified the working tree was clean before recording this update. Commits remain manual; implementation still requires explicit approval.
- 2026-11-10: User approved implementation start with task 01 readiness/compatibility, requested branch tagging before implementation and after each task completion, reaffirmed Guided mode and manual commits, and prohibited production deployment.
- 2026-11-10: User confirmed SQL migration scope covers the ten feature-flagged data areas and will complete before .NET 10 implementation (no Azure Table Storage use expected for those areas). Logging/API recording are excluded from SQL migration; application logging direction is ILogger/Application Insights, while admin log viewer and API session recording destination remain unresolved.
- 2026-11-10: User decided to use Application Insights for both application logging and API session recordings, and to retire the current admin log viewer in the .NET 10 migration.
- 2026-11-10: User deferred the OData removal/preservation decision until deeper runtime verification is completed.
- 2026-11-10: User directed acceptance-test environment prerequisites to remain an explicit external blocker until valid secrets and environment access are provided.
- 2026-11-11: User agreed Task 03 should not perform FluentValidation 11.x behavioral MVC5 migration work; keep Task 03 structural and defer FluentValidation behavior migration to Task 04.
- 2026-11-11: User confirmed SQL replacement is expected to be complete in this branch and instructed enforcement of SQL-only wiring now, including removal of associated migration feature flags, while handling logging/API-recording path separately.
- 2026-11-11: User accepted replacing System.Device.Location usage as long as new functionality remains equivalent.
- 2026-11-11: User chose to expand Task 04.02 scope to resolve remaining Services-closure net10 warnings immediately (including package/security warning remediation).
- 2026-11-11: User evaluated non-licensed AutoMapper path and chose to keep AutoMapper at 10.1.1 for now (instead of moving to 14.x), given current net48 compatibility constraints.
- 2026-11-11: User requested adherence to the existing application's coding standards and style conventions, and asked for this preference to be explicitly captured in scenario instructions.
- 2026-11-11: User approved a host-foundation cut for 04.04: produce a runnable ASP.NET Core shell on net10 and temporarily exclude legacy MVC5/System.Web feature code from compile, then restore full feature parity in 04.05.
- 2026-11-11: User requested consistent line endings in edited/generated files to avoid Visual Studio normalization prompts; enforce repository-consistent endings for all future edits.
- 2026-11-11: User requested a pause; stop implementation activity until the user asks to continue.
- 2026-11-12: User asked the agent to take over implementation directly (instead of MAI-Code handoff/review loops) and to use medium-size migration slices for better efficiency.
- 2026-11-12: User chose to defer `AccountController` migration until explicit ASP.NET Core auth-scheme wiring is confirmed.
- 2026-11-12: User chose to close root-controller subtask with `AccountController` deferred and `AdminController` blocked, then proceed to Establishments area controllers.
- 2026-11-12: User requested a documentation-first handoff so another developer/Copilot session can resume with accurate state and tasks artifacts.

## Task 04/05 handover snapshot (2026-11-12)
- Migration status: application code migration is effectively complete for the current compile surface.
- Validation status:
  - `run_build` reports successful workspace build.
  - `Web/Edubase.Web.UIUnitTests` full suite passes (`887/887`) after compatibility/runtime fixes.
- Workflow status:
  - `04.06-cross-closure-validation-and-gates` has completion evidence in `tasks/04.06-cross-closure-validation-and-gates/progress-details.md`.
  - Next execution target is `05-release-readiness`.
  - `06-solution-validation` remains historically failed in tracker and should be re-opened/re-run after task 05 finalization.
- Remaining migration hygiene to track in release/readiness:
  - Non-test csproj compile excludes still present and should be reviewed before final closure:
    - `Web/Edubase.Web.UI/Edubase.Web.UI.csproj`: `Models\\BaseViewModel.cs`
    - `Web/Edubase.Services/Edubase.Services.csproj`: `Security\\Permissions\\Permission.cs`
- Operational constraint remains unchanged: no production deployment from this workflow without explicit user approval.
