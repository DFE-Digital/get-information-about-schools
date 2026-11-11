# Dependency Conflict Review (Task 02.02)

## 1) Binding redirect inventory and categorization

### Redirect density

| File | Redirect count |
|---|---:|
| `Web/Edubase.Web.UI/Web.config` | 49 |
| `Web/Edubase.Services.TexunaUnitTests/app.config` | 36 |
| `Web/Edubase.Services/App.config` | 16 |
| `Web/Edubase.Services.Texuna/app.config` | 16 |
| `Web/Edubase.Web.Resources/app.config` | 16 |
| `Web/Edubase.Data/App.config` | 15 |
| `Web/Edubase.Common/app.config` | 10 |

### Conflict classes (from assessment + current files)

| Class | Meaning | Baseline status |
|---|---|---|
| Manual redirect conflicts with auto-generated version (`MSB3836`) | Existing manual redirects conflict with assembly version selected by package graph | Widespread; includes `Newtonsoft.Json`, `System.ValueTuple`, `System.Memory`, `System.Buffers`, `System.Threading.Tasks.Extensions`, `System.Numerics.Vectors` and others |
| Redirect forces version downgrade | Redirect `newVersion` is older than package-provided assembly | Present in multiple projects/config files |
| Redirect oldVersion range does not cover referenced assembly | Version coverage gap likely causing runtime/compile mismatch risk | Present (e.g., `Microsoft.WindowsAzure.Storage`, `Microsoft.Data.Services.Client`, `Microsoft.IdentityModel.Protocols` in assessment) |

### Obsolete-vs-required interpretation

- **Likely obsolete after full .NET 10 migration**
  - Most runtime binding redirects under `runtime/assemblyBinding` (classic .NET Framework mechanism not used by ASP.NET Core runtime model).
- **Required until migration completion**
  - Current Framework-hosted projects still rely on these redirects to build/run without assembly load conflicts.
- **Actionable now (risk markers, not edits in this subtask)**
  - Redirects with `MSB3836` conflicts and downgrade gaps should be treated as migration-risk items requiring explicit resolution during task 04 migration execution.

## 2) Package divergence baseline (packages.config projects)

### Cross-project version divergence detected

| Package | Versions found | Projects |
|---|---|---|
| `EntityFramework` | `6.5.1`, `6.5.2` | Data vs Web.UI |
| `FluentValidation` | `11.9.2`, `8.6.2` | Data vs Web.UI |
| `Microsoft.AspNet.Identity.Core` | `2.2.3`, `2.2.4` | Services.Texuna vs Data/Web.UI |
| `Microsoft.Bcl.AsyncInterfaces` | `6.0.0`, `8.0.0` | Common/Services vs Web.UI |
| `System.Runtime.CompilerServices.Unsafe` | `4.5.3`, `6.0.0`, `6.1.2` | Data/Services/Services.Texuna vs Common/Services.TexunaUnitTests/Web.UI |
| `System.Buffers` | `4.5.1`, `4.6.1` | Common/Services/Services.Texuna/Tests vs Web.UI |
| `System.Memory` | `4.5.5`, `4.6.3` | Common/Services/Services.Texuna/Tests vs Web.UI |
| `System.Numerics.Vectors` | `4.5.0`, `4.6.1` | Common/Services/Services.Texuna/Tests vs Web.UI |
| `System.Threading.Tasks.Extensions` | `4.5.4`, `4.6.0` | Common/Data/Services/Services.Texuna/Tests vs Web.UI |
| `System.ComponentModel.Annotations` | `4.5.0`, `5.0.0` | Services vs Web.UI |

## 3) High-risk dependency clusters for migration planning

| Cluster | Evidence | Migration implication |
|---|---|---|
| OData legacy stack | `Microsoft.AspNet.WebApi.OData`, `Microsoft.Data.OData`, `System.Web.Http.OData` references in Web.UI | OData strategy remains deferred; do not remove/replace blindly until runtime contract verification |
| Auth stack | `Sustainsys.Saml2*`, `Microsoft.AspNet.Identity*`, `Microsoft.Owin*` | Preserve identity and session semantics; migration must be compatibility-first |
| Storage/logging stack | `WindowsAzure.Storage`, `AzureTableLogger`, legacy Table-based repositories | SQL migration assumptions and App Insights direction are fixed, but implementation in task 04 must map behavior explicitly |
| Classic App Insights web modules | `ApplicationInsights.config` + Web.config module registrations | Needs migration to ASP.NET Core telemetry model while preserving required observability |

## 4) Recommended treatment timing

### Task 03 (SDK-style conversion)
- Preserve existing package/runtime behavior as much as possible.
- Do not attempt broad version normalization in conversion-only task.
- Carry redirect/package conflict register forward as known risk inventory.

### Task 04 (.NET 10 migration)
- Resolve conflicts inline while migrating each dependency closure.
- Remove/replace obsolete binding redirects as runtime model changes are applied.
- Reconcile divergent package versions intentionally, with compatibility validation (build + behavioral checks).

## 5) Constraints and non-goals in this subtask

- No package version updates were made.
- No project or runtime configuration behavior was changed.
- This review establishes risk visibility and execution order guidance only.
