## Task
04.05.02.04.01-shared-controller-base-and-helpers

## Summary
Migrated the shared MVC controller foundation (`EduBaseController` + custom camel-case JSON result) from MVC5 `System.Web.Mvc` contracts to ASP.NET Core while preserving existing helper semantics used by downstream controllers.

## Changes made
- Migrated `Web/Edubase.Web.UI/Controllers/EduBaseController.cs`:
  - Replaced `System.Web.Mvc.Controller` usage with ASP.NET Core `Microsoft.AspNetCore.Mvc.Controller`.
  - Preserved base helper behavior by keeping custom `Json(...)` helpers returning `JsonCamelCaseResult`.
  - Added overload preserving `JsonRequestBehavior` compatibility for downstream controllers.
  - Preserved API error to `ModelState` mapping semantics using existing message-key resolution logic.
- Migrated `Web/Edubase.Web.UI/Helpers/JsonCamelCaseResult.cs`:
  - Replaced MVC5 `ActionResult` + `ExecuteResult(ControllerContext)` with ASP.NET Core `ActionResult` + `ExecuteResultAsync(ActionContext)`.
  - Preserved camel-case JSON serialization via Newtonsoft `CamelCasePropertyNamesContractResolver`.
  - Preserved GET-allow/deny behavior with compatibility `JsonRequestBehavior` enum.
- Updated active compile slice in `Web/Edubase.Web.UI/Edubase.Web.UI.csproj`:
  - Included `Controllers/EduBaseController.cs` and `Helpers/JsonCamelCaseResult.cs` in the net10 compilation set for this migration slice.

## Validation
- Build:
  - `dotnet build C:\copilot\get-information-about-schools\Web\Edubase.Web.UI\Edubase.Web.UI.csproj -v minimal`
  - Result: **Success**.
  - Known warnings: existing user-approved AutoMapper advisory warnings (`NU1903`).

## Notes / Deferred to next subtask
- Root and area controllers that still call MVC5 overloads like `Json(..., JsonRequestBehavior.AllowGet)` are addressed in subsequent controller migration slices under `04.05.02.04.*`.
