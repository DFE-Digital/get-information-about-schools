# 02.02-binding-and-package-conflicts: Review binding redirects and package divergence

## Objective
Produce a dependency-conflict review that distinguishes obsolete .NET Framework assembly-resolution patterns from true compatibility issues that must be addressed during migration.

## Scope
- Binding redirects in `Web.config` and all project `app.config` files with runtime assembly bindings
- Legacy package sources (`packages.config`, direct assembly references/HintPaths)
- Assessment-reported redirect conflicts and package incompatibilities

## Research findings
- Binding redirect density is high across legacy config files:
  - `Web.UI/Web.config`: 49 redirects
  - `Services.TexunaUnitTests/app.config`: 36 redirects
  - `Data/App.config`: 15, `Services/App.config`: 16, `Services.Texuna/app.config`: 16, `Web.Resources/app.config`: 16, `Common/app.config`: 10
- Assessment includes many mandatory redirect issues (`MSB3836` conflict category) and downgrade/coverage warnings.
- Package ecosystem is still legacy/packages.config-centric in six projects; direct version divergence exists for shared low-level packages (`System.*`, `Microsoft.Bcl.AsyncInterfaces`, `EntityFramework`, `FluentValidation`, `Microsoft.AspNet.Identity.Core`, `etc.`).
- High-risk packages for migration constraints are confirmed in current config/package files: OData (`Microsoft.AspNet.WebApi.OData`, `Microsoft.Data.OData`), auth (`Sustainsys.Saml2*`, `Microsoft.AspNet.Identity*`, `Microsoft.Owin*`), storage/logging (`WindowsAzure.Storage`, `AzureTableLogger`, App Insights classic web SDK stack).

## Deliverable
- Dependency conflict baseline authored at:
  - `.github/upgrades/scenarios/dotnet-version-upgrade/tasks/02.02-binding-and-package-conflicts/dependency-conflict-review.md`

## Done when
- Binding redirect review exists with conflict categorization and migration risk notes.
- Package divergence review exists with project-level visibility and prioritized risk categories.
- No package or project-file behavior changes were introduced as a side effect.
