## Task
04.05.02.01-webapi-controller-surface

## Summary
Migrated the primary Web API controller slice from Web API 2 (`ApiController`/`IHttpActionResult`) to ASP.NET Core controller patterns and activated a narrow compile slice so these controllers are now built on net10.

## Changes made
- API controller migrations to ASP.NET Core (`ControllerBase` + `IActionResult` patterns):
  - `Web/Edubase.Web.UI/Controllers/Api/AcademyOpeningsApiController.cs`
  - `Web/Edubase.Web.UI/Controllers/Api/AmalgamateMergeApiController.cs`
  - `Web/Edubase.Web.UI/Controllers/Api/ApprovalsApiController.cs`
  - `Web/Edubase.Web.UI/Controllers/Api/BulkCreateAcademiesApiController.cs`
  - `Web/Edubase.Web.UI/Controllers/Api/EstablishmentApiController.cs`
  - `Web/Edubase.Web.UI/Controllers/Api/SearchApiController.cs`
  - `Web/Edubase.Web.UI/Controllers/Api/UtilApiController.cs`
- Source migration also applied to area API controller (integration compile deferred to MVC/Area slice):
  - `Web/Edubase.Web.UI/Areas/Groups/Controllers/GroupApiController.cs`
- Role authorization helper migration for API attributes:
  - `Web/Edubase.Web.UI/Helpers/AuthorizeRolesAttribute.cs` moved from `System.Web.*` authorize attributes to ASP.NET Core authorization attributes.
- Compile-slice activation for this subtask:
  - `Web/Edubase.Web.UI/Edubase.Web.UI.csproj` now includes API controller files required for this slice plus helper dependencies (`AuthorizeRolesAttribute`, `AuthorizedRoles`, `AcademyUtility`).

## Validation
- Build:
  - `dotnet build C:\copilot\get-information-about-schools\Web\Edubase.Web.UI\Edubase.Web.UI.csproj -v minimal`
  - Result: **Success**.
  - Warnings: known user-accepted AutoMapper advisory warnings (`NU1903`) remain.

## Explicitly deferred/blockers in this slice
- `Controllers/Api/SqlDataController.cs` migration was updated in source but excluded from this compile slice because required package surfaces are currently net48-conditioned in the project file; it is tracked for follow-on work in the remaining 04.05.02 subtasks.
- `Areas/Groups/Controllers/GroupApiController.cs` was migrated in source but not yet part of the active compile slice; it is to be brought into compile coverage in the MVC/Area controller parity subtask.
