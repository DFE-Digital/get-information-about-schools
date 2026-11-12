# Progress details — 04.05.02.04.03 Establishments area controller contracts

## Summary
Completed migration of the remaining Establishments-area controller contract surface to .NET 10 compile parity for the compile-slice gate.

This task now includes:
- `EstablishmentController` in the explicit compile slice.
- Core-compatible updates for Establishments model dependencies required by `EstablishmentController`.
- Core-compatible updates for validation and helper extensions used by Establishments controller/model flows.

## Key changes

### Controllers
- Migrated and compile-validated:
  - `Areas/Establishments/Controllers/BulkCreateFreeSchoolsController.cs`
  - `Areas/Establishments/Controllers/BulkAssociateEstabs2GroupsController.cs`
  - `Areas/Establishments/Controllers/BulkUpdateController.cs`
  - `Areas/Establishments/Controllers/AcademyOpeningsController.cs`
  - `Areas/Establishments/Controllers/EstablishmentsSearchController.cs`
  - `Areas/Establishments/Controllers/EstablishmentController.cs`
- Fixed a route-signature regression in `EstablishmentController.EditLocation` by restoring the optional `locationField` parameter.

### Compile-slice wiring (`EnableDefaultCompileItems=false`)
- Expanded explicit includes in `Web/Edubase.Web.UI/Edubase.Web.UI.csproj` for:
  - Establishments controller graph
  - Required Establishments models/validators
  - Required shared models used by Establishments flows
  - Helper/validation extension files used by the migrated controller surface

### Core compatibility updates
- Replaced legacy MVC5 `SelectListItem` namespace usage in newly included model files with ASP.NET Core namespace (`Microsoft.AspNetCore.Mvc.Rendering`).
- Updated `EstablishmentDetailViewModel` route-dictionary usage to `Microsoft.AspNetCore.Routing.RouteValueDictionary`.
- Simplified `EdubaseAbstractValidator<T>` to remove MVC5 FluentValidation interceptor coupling not available in current stack.
- Ported helper/validation extension code to ASP.NET Core model-state APIs (`ModelStateDictionary`, `ValueProviderResult`, `ControllerContext.ModelState`).
- Removed obsolete framework-only using dependencies from `TabWarningsModel`.
- Replaced obsolete FluentValidation cascade API usage in:
  - `CreateEstablishmentViewModelValidator`
  - `SearchForEstablishmentViewModelValidator`

## Validation
- Command:
  - `dotnet build Web/Edubase.Web.UI/Edubase.Web.UI.csproj -c Release`
- Result:
  - **Succeeded** with zero errors.
  - Existing baseline warnings remain (NU1903 AutoMapper advisory, existing `RequiredDateTimeViewModel` CS0108 warnings).
