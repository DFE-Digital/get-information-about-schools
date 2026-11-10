# .NET 10 Upgrade

## Preferences
- **Flow Mode**: Guided
- **Target Framework**: net10.0
- **Assessment Scope**: Solution, Web\EdubaseWeb.sln, all included projects.
- **Current Stage Limit**: Upgrade options confirmed; generate plan for review. Implementation requires separate explicit approval.

## Source Control
- **Source Branch**: main
- **Working Branch**: upgrade-dotnet-10
- **Commit Strategy**: Manual
- **Branch Sync**: Disabled

## User Preferences
### Technical Preferences
- Upgrade this solution and projects to .NET 10.
- Need to keep backward compatibility with existing clients, eg SOAP endpoints, contracts, exports, downloads, reports.
- UI style, layout, features should all be preserved.
- Do not substitute .NET 11 preview for the requested .NET 10 target.
- Use standard Microsoft.Extensions.Logging ILogger for logging.
- ApiRecorderSessionItemRepository and WebLogItemRepository currently write to Azure Table Storage and will not migrate to SQL. Their recording/logging requires a replacement destination, to be determined; Application Insights is a candidate, not confirmed.
- The existing application's feature-flagged Table Storage to SQL migration is expected to complete before .NET 10 implementation. If timings overlap, assess temporary Table Storage support complexity and ask the user before choosing. Confirm migration completion and fallback requirements before omitting Table Storage; the two logging repositories require separate treatment.
- Deployment preference: migrate in one step, not incrementally; prefer a single production cutover rather than a staged feature-by-feature migration.
- Use temporary library multi-targeting only where keeping existing consumers buildable provides enough value to justify the extra work, not automatically for every library. Evaluate each library and its dependency chain; do not assume successful builds establish runtime compatibility.
- Use System.Web Adapters selectively, not automatically: prefer native ASP.NET Core APIs for straightforward changes; consider temporary shims only for supported, tightly coupled legacy request-context usages where they materially reduce migration risk. Verify actual usage and behaviour with explicit tests and remove temporary shims before completion. Adapters do not make MVC5 run on ASP.NET Core or require side-by-side deployment.

### Execution Style
- If anything is ambiguous or unclear about what direction/strategy/option to take then ask the user.
- Assessment reviewed; planning authorised. Confirm migration options with the user and do not change application code during planning.
- Planning is authorised; implementation remains paused until separately authorised. Recording feedback does not authorise implementation.

## Key Decisions Log
- 2026-11-06: User confirmed .NET 10 solution-wide assessment, Guided mode, new upgrade-dotnet-10 branch from clean main, manual commits and disabled automatic branch sync.
- 2026-11-09: User prefers single-step deployment. Detailed cutover, downtime and rollback arrangements remain to be confirmed during planning.
- 2026-11-09: User authorised the planning stage only. Retain Guided mode and obtain approval before implementation.
- 2026-11-09: User confirmed the presented options with selective multi-targeting and selective temporary System.Web Adapters refinements. Confirmation authorises plan generation, not implementation.
- 2026-11-10: User renamed the working branch to copilot/upgrade-dot-net-10 (superseding the initial working branch name above) and committed assessment/planning work as 0fdcd8cb0. Verified the working tree was clean before recording this update. Commits remain manual; implementation still requires explicit approval.

## Compatibility Review Gates
- Static assessment does not prove runtime or visual parity; no application code changes or compatibility test execution during assessment.
- documentation\service\container.md identifies Java SOAP hosting outside this .NET solution. Do not implicitly migrate or replace it.
- Preserve HomeController's /service-wsdl and /service.wsdl redirect paths and behavior.
- Preserve DownloadsController and DownloadsApiService generation, polling, redirects, streamed files and MAT closure CSV reports. Validate URLs, methods, authorization, errors, filenames, content types, content-disposition, schemas, encoding and ordering against baseline fixtures.
- Preserve Newtonsoft.Json wire behavior, including both ApiResultDto and ProgressDto generation responses and current AJAX response shapes.
- The report's suggested OData v4 replacement is not approved; confirm consumer/protocol compatibility before changing contracts.
- Require visual and functional regression coverage of Razor views, CSS, JavaScript/Vue assets, forms, validation, navigation and accessibility.
- AutoMapper 10.1.1 is flagged as vulnerable in Common, Web.UI and Web.UIUnitTests; include remediation in planning and verify advisory details and replacement-version compatibility.
- Use the confirmed Bottom-Up development order and in-place web migration with one production cutover; resolve outstanding implementation and operational details before affected work.

## Upgrade Options

### Strategy
- Upgrade Strategy: Bottom-Up

### Project Structure
- Project Approach: Web Projects - In-place rewrite; Class Libraries - selective Multi-targeting only where justified, otherwise In-place; System.Web Libraries - In-place
- Package Management: Per-Project (defer CPM to post-migration)

### Compatibility
- Unsupported Packages: Resolve Inline
- Unsupported API Handling: Fix Inline
- System.Web Adapters: Use System.Web Adapters (selective temporary use only; native APIs preferred for straightforward changes)
  Skill: migrating-mvc-system-web-adapters

### Modernization
- Configuration Migration: Manual Migration with Mapping Document
- Logging Framework: Migrate to Microsoft.Extensions.Logging (user requirement; destination undecided)
- Dependency Injection: Migrate to Built-in DI Container
- Assembly Binding Redirects: Document and Review Before Removing
- Entity Framework: Keep EF6

## Strategy
**Selected**: Bottom-Up (Dependency-First), in-place web migration, single production cutover.
**Rationale**: Framework-to-modern migration across dependent libraries and a large System.Web application; dependency ordering isolates failures without requiring incremental deployment.

### Execution Constraints
- Separate SDK-style conversion on existing TFMs from TFM/API migration; perform project conversions sequentially.
- Break the TFM migration task into dependency-ordered subtasks. Validate each migrated dependency closure and its tests before consumers; update test TFMs and references with the work that requires them.
- Preserve and validate old consumers where selective multi-targeting justifies it. Where in-place changes intentionally make unmigrated consumers unbuildable, record the affected closure and validate it when migrated; do not claim every intermediate state builds or runs. Ask if this creates an unagreed significant trade-off.
- Resolve packages and APIs inline without success-shaped placeholders or warning suppression; test contracts and runtime behaviour, not just compilation.
- Use adapters only where supported usages reduce risk; test them and remove temporary shims and Framework targets before release. Do not introduce side-by-side production routing or cross-app infrastructure implicitly.
- Keep manual commits as explicitly agreed. Require full build, tests, client/file/UI parity checks and release rehearsal before proposing the single production cutover; no deployment is authorised by this plan.
