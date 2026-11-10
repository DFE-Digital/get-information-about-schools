# .NET 10 Planning Options - Reference Copy

Recorded: 2026-11-09

**Status: options confirmed by the user on 2026-11-09, including selective multi-targeting and selective temporary adapter refinements.** This remains a standalone backup/reference, not the workflow source of truth. Alternatives are retained for context and are not selected. Confirmation authorises plan generation only; implementation requires separate approval.

## Existing Requirements

- Target .NET 10 across the solution and projects.
- Preserve existing client compatibility, including SOAP/WSDL behaviour, contracts, exports, downloads and reports.
- Preserve UI styling, layout, features and behaviour.
- Prefer a single production cutover, not incremental deployment. Development and testing may still proceed in dependency order.
- Use Microsoft.Extensions.Logging ILogger.
- The feature-flagged Azure Table Storage to SQL migration is expected to complete before .NET 10 implementation. Confirm completion and fallback requirements before omitting Table Storage; ask about temporary support if schedules overlap.
- ApiRecorderSessionItemRepository and WebLogItemRepository are excluded from the SQL migration. Their recording/logging needs a replacement destination; Application Insights is a candidate, not confirmed.
- Ask the user about ambiguous directions or decisions. Planning is authorised, but implementation requires separate approval.
- Commits remain manual.

## Strategy

### Upgrade Strategy

**Confirmed: Bottom-Up.** Migrate and validate dependencies before the web application. This is development order only: production deployment remains one cutover. The solution crosses the .NET Framework to modern .NET boundary, with dependencies extending from Common through Data and Services to the web application.

## Project Structure

### Web Projects

**Confirmed: In-place rewrite.** Convert the existing web project to ASP.NET Core for the single-step migration. Its large System.Web surface makes this higher risk; preserving the UI and behaviour remains mandatory. This means replacing the hosting/framework implementation, not redesigning the product or its UI.

- **Alternative - Side-by-side:** Create a new ASP.NET Core project alongside the existing Framework project. Migrate assets incrementally while the old project stays live, with separate scaffold and migration tasks. This is an alternative development approach, not an approved change to the single-cutover deployment requirement.

### Class Libraries without System.Web

**User refinement: selective temporary multi-targeting, not a blanket requirement.** Use it only where keeping existing consumers buildable provides enough value to justify the extra work. Evaluate each library and its dependency chain; otherwise migrate in-place. Remove temporary Framework targets before release.

> **Note**: net48 and net10.0 are separate builds, not mixed runtimes or incremental deployment. A runnable old application needs a complete working Framework dependency chain, and the new application needs its migrated dependencies and host. Builds alone do not prove runtime compatibility; not every intermediate state must run.

### Libraries Using System.Web

**Confirmed: In-place.** Replace System.Web dependencies with injectable abstractions before migrating consumers. Do not rely on a nominal modern target whose functionality has been removed or stubbed.

### Package Management

**Confirmed: Per-Project (defer CPM to post-migration).** Avoid adding centralised package management during the framework migration. Conversion from packages.config to PackageReference remains part of SDK-style conversion.

- **Alternative - Central Package Management (CPM):** Create Directory.Packages.props and move package versions out of project files for central management.

## Compatibility

### Unsupported Packages

**Confirmed: Resolve Inline.** Find real replacements within each migration task rather than accepting compile-only stubs. Contract-changing substitutions still require user approval; an automated replacement suggestion is not proof of compatibility.

- **Alternative - Defer Resolution:** Make projects compile using minimal type stubs, then create follow-up tasks for real replacements. Compilation alone does not establish working behaviour.
- **Alternative - Compatibility Mode:** Retain .NET Framework references, add Microsoft.NETFramework.ReferenceAssemblies and suppress NU1701. This is not recommended because runtime failures remain possible. Warning suppression requires separate approval.

### Unsupported API Handling

**Confirmed: Fix Inline.** Resolve behaviour as part of each task rather than leave placeholder implementations.

- **Alternative - Defer Complex Changes:** Apply simple replacements directly; use temporary compilable stubs and resolution subtasks for complex changes. All placeholders would need real implementations before completion.

### System.Web Adapters

**User refinement: selective temporary adapter use, not a blanket requirement.** Prefer native ASP.NET Core APIs for straightforward changes. Consider adapters only for supported, tightly coupled legacy request-context usages where they materially reduce migration risk. Establish actual eligible usages before selecting package scope; the assessment System.Web issue count is not a count of issues adapters can resolve.

Adapters can reduce simultaneous changes to callers and helpers using supported APIs such as HttpContext.Current, allowing behavioural testing before replacing remaining compatibility code. They support only part of System.Web and may have behavioural differences. They do not make MVC5 run on ASP.NET Core: controllers, routing, filters, authentication, views and startup still need appropriate migration.

Identify each temporary usage, verify behaviour with explicit tests, and include removal and native-API replacement before completion. Do not introduce cross-application proxying, remote authentication or shared-session infrastructure merely for request-context shims; those are not inherently required for the single-cutover approach.

- **Alternative - Direct Migration to ASP.NET Core APIs:** Replace all System.Web usage immediately with native equivalents, without adapter shims. More upfront work, but no compatibility layer to remove later.

## Modernization

### Configuration Migration

**Confirmed: Manual Migration with Mapping Document.** Map custom configuration sections and settings explicitly to avoid losing environment-specific behaviour. Do not copy secret values into documentation.

- **Alternative - Auto-migrate to .NET Core Configuration:** Automatically convert app.config/web.config application settings to appsettings.json and migrate code to IConfiguration. Hosting-specific configuration still needs appropriate handling.

### Dependency Injection

**Confirmed: Migrate to Built-in DI Container.** Replace MVC5 Autofac integration while preserving named registrations, lifetimes and startup behaviour. Verify container-specific behaviour before replacing it.

- **Alternative - Keep Existing IoC Container:** Retain Autofac with an ASP.NET Core adapter and preserve container-specific features.

### Assembly Binding Redirects

**Confirmed: Document and Review Before Removing.** Investigate existing version conflicts before removing Framework-only redirects.

- **Alternative - Remove Binding Redirects:** Remove redirects because modern .NET resolves assemblies differently. Underlying dependency conflicts still require resolution.

### Entity Framework

**Confirmed: Keep EF6.** Avoid combining an ORM migration with the framework upgrade. Verify existing provider and spatial-data behaviour; package compatibility alone does not prove runtime compatibility.

- **Alternative - Migrate to EF Core:** Migrate the ORM simultaneously, adding query, mapping and schema/migration-tooling risks.

## Outstanding Decisions and Validation Gates

- Upgrade options are confirmed; review plan.md before authorising implementation. The other gates below remain unresolved until their evidence and decisions are obtained.
- Confirm the replacement logging destination, retention, access and any recording/retrieval requirements for the two logging repositories. Application Insights is not yet selected.
- Confirm SQL migration completion and any remaining Table Storage fallback requirements.
- Establish actual OData usage before removal or replacement. Moving to OData v4 is not approved.
- Confirm authentication/session behaviour at cutover, downtime tolerance, hosting and operational rollback arrangements.
- Establish baseline fixtures and visual/functional regression criteria for client contracts, file formats and UI behaviour. Static analysis and successful builds are not proof of parity.
- Preserve the frontend's /service-wsdl and /service.wsdl redirects. Existing documentation identifies SOAP hosting in the separate Java backend; do not implicitly migrate that backend.
- Verify the AutoMapper vulnerability advisory, compatible remediation version and any licensing implications before choosing an update.

## Approval Boundary

The user confirmed the options and refinements for plan generation, not implementation. Alternatives remain reference-only. The generated assessment remains unchanged. No application code changes, package updates, or production deployment are authorised by this document or its confirmation.
