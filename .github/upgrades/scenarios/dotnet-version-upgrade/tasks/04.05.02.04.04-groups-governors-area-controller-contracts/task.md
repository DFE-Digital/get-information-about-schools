# 04.05.02.04.04-groups-governors-area-controller-contracts: Migrate Groups/Governors area controllers and contract-sensitive flows

## Objective
Migrate Groups and Governors area controllers to ASP.NET Core while preserving contract-sensitive workflow behaviors and route dependencies.

## Scope
- Web/Edubase.Web.UI/Areas/Groups/Controllers/*.cs
- Web/Edubase.Web.UI/Areas/Governors/Controllers/*.cs

## Steps
1. Migrate controller and attribute contracts to ASP.NET Core APIs.
2. Preserve route-area conventions and RedirectToRoute behavior.
3. Maintain anti-forgery/auth attributes and validation flow semantics.
4. Build and resolve compile issues for this slice.

## Research notes (2026-11-12)

### Controller inventory in scope
Groups:
- `Areas/Groups/Controllers/GroupController.cs`
- `Areas/Groups/Controllers/GroupSearchController.cs`
- `Areas/Groups/Controllers/GroupApiController.cs`

Governors:
- `Areas/Governors/Controllers/GovernanceController.cs`
- `Areas/Governors/Controllers/GovernanceDelegationController.cs`
- `Areas/Governors/Controllers/GovernorController.cs`
- `Areas/Governors/Controllers/GovernorSearchController.cs`
- `Areas/Governors/Controllers/GovernorsBulkUpdateController.cs`
- `Areas/Governors/Controllers/SharedGovernorController.cs`
- `Areas/Governors/Controllers/GroupCorporateContactController.cs`

### Current migration risk points observed
- All MVC area controllers still import `System.Web.Mvc` and still use MVC5 area attributes (`RouteArea`, `RoutePrefix`) that must be converted to Core route templates.
- Header writes currently use `HttpContext.Response.Headers.Add(...)` in search controllers; this already caused analyzer friction in prior slices and should be switched to indexer assignment.
- `GroupController` still uses `HttpNotFound()` (must map to `NotFound()`).
- Governors controllers depend on extension/helper flows now migrated in prior slice (`ApplyToModelState`, `ToSelectList`, `GetMessage`) and should reuse those patterns without reintroducing MVC5 APIs.

### Compile-slice constraints
- `Web/Edubase.Web.UI/Edubase.Web.UI.csproj` uses `EnableDefaultCompileItems=false`.
- No Groups/Governors controller compile includes are currently present, so this task must explicitly add controller includes and their direct model/helper dependency closure.

### Execution strategy for credit-efficient batch
1. Migrate all Groups/Governors MVC area controllers in one batch (contracts/routes/results/headers).
2. Add explicit compile includes for these controllers.
3. Build `Web.UI` and iteratively include/fix dependency closure until clean compile.
4. Keep changes constrained to this area slice plus tightly-coupled shared helpers needed to compile under Core.

### Completion notes (2026-11-12)
- Groups/Governors area controllers were migrated to ASP.NET Core-compatible route/HTTP/header/query patterns.
- Tightly-coupled Group/Governor model and validator dependencies were updated where needed for compile compatibility.
- `Edubase.Web.UI.csproj` compile-slice includes were expanded for this area and required helper/model dependencies.
- `Web.UI` Release build succeeds after this slice.
