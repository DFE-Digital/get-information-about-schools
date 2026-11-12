# 04.05.02.04.03-establishments-area-controller-contracts: Migrate Establishments area controllers and their route/redirect contracts

## Objective
Migrate Establishments area controller surfaces to ASP.NET Core with route-name and action-result parity.

## Scope
- Web/Edubase.Web.UI/Areas/Establishments/Controllers/*.cs

## Steps
1. Convert base classes, attributes, and action signatures.
2. Preserve area route prefixes, named routes, and redirection targets.
3. Resolve compile blockers introduced by area-specific dependencies.

## Research notes (2026-11-12)

### Controller inventory in scope
- `Areas/Establishments/Controllers/BulkCreateFreeSchoolsController.cs`
- `Areas/Establishments/Controllers/BulkAssociateEstabs2GroupsController.cs`
- `Areas/Establishments/Controllers/BulkUpdateController.cs`
- `Areas/Establishments/Controllers/AcademyOpeningsController.cs`
- `Areas/Establishments/Controllers/EstablishmentsSearchController.cs`
- `Areas/Establishments/Controllers/EstablishmentController.cs` (largest file; high dependency surface)

### Current migration status observed
- All six controllers still import `System.Web.Mvc` and use MVC5 attributes/results.
- `BulkUpdateController` still uses `JsonRequestBehavior.AllowGet` (Core-incompatible overload).
- `AcademyOpeningsController` still uses `HttpNotFound()` and legacy MVC namespace.
- `EstablishmentController` is a large contract-sensitive surface (~1500 lines) with many named routes and mixed result types (`HttpStatusCodeResult`, `HttpNotFound`, `Response.Headers.Add` etc.).

### Build gating constraints
- `Web/Edubase.Web.UI/Edubase.Web.UI.csproj` has `EnableDefaultCompileItems=false`.
- Establishments controllers are not yet listed in explicit `<Compile Include="...">` entries.
- Any migrated controller must be explicitly wired in the csproj together with required dependent models/helpers to avoid false-positive validation.

### Execution approach for this task
1. Migrate Establishments controller batch with lower blast radius first (`BulkCreateFreeSchools`, `BulkAssociateEstabs2Groups`, `BulkUpdate`, `AcademyOpenings`).
2. Add explicit compile entries for migrated files and immediate model dependencies.
3. Run targeted `Web.UI` build and fix compile regressions.
4. Continue to `EstablishmentsSearchController` and `EstablishmentController` once batch compiles cleanly.

### Completion notes (2026-11-12)
- All six Establishments area controllers are now migrated and compile-validated in the .NET 10 compile slice.
- `EstablishmentController` and its required model/helper/validation dependency graph were brought into the explicit compile list.
- Core compatibility updates were applied to legacy MVC5 helper/validation/model surfaces required by `EstablishmentController`.
- Targeted validation (`dotnet build Web/Edubase.Web.UI/Edubase.Web.UI.csproj -c Release`) passes with zero errors.
- Remaining warnings are baseline items tracked separately (NU1903 AutoMapper advisory; existing `RequiredDateTimeViewModel` CS0108 warnings).
