# Progress details — 04.05.02.05 (Endpoint contract validation)

## Validation scope executed
- Focused validation run for migrated controller surfaces from 04.05.02.01-.04.
- Contract checks targeted at route/redirect contracts, anti-forgery post endpoints, and API status-code response patterns.

## Evidence captured

### 1) Focused Web.UI build
- Command: `run_build` on `Web/Edubase.Web.UI/Edubase.Web.UI.csproj`
- Result: **Build successful**.

### 2) Targeted API/MVC route + contract checks (static verification)
- Route/redirect contract presence checks:
  - `rg "RedirectToRoute\(|Name\s*=\s*nameof\(" ...` across migrated Establishments/Groups/Governors controllers.
  - Observed active `RedirectToRoute(...)` usages in migrated controllers and corresponding named route declarations.
- Anti-forgery/postback checks:
  - `rg "\[ValidateAntiForgeryToken\]|\[HttpPost" ...` across migrated API/MVC controllers.
  - Confirmed anti-forgery protection remains present on contract-sensitive POST actions in migrated Establishments/Groups/Governors surfaces.
- API status code surface checks:
  - `rg "Json\(|JsonCamelCaseResult|return\s+Ok\(|return\s+NotFound\(|return\s+BadRequest\(" ...` across migrated API controllers.
  - Confirmed expected explicit status code patterns remain in migrated API endpoints (e.g., `Ok`, `BadRequest`, `NotFound`).

### 3) Named-route resolution sanity check
- Command: PowerShell scan over `Web/Edubase.Web.UI/**/*.cs`:
  - Extract literal `RedirectToRoute("...")` names.
  - Extract named route declarations `Name = "..."`.
  - Diff for missing names.
- Result:
  - `Redirect route refs: 22`
  - `Unique named routes declared: 207`
  - `No missing named-route declarations detected for literal RedirectToRoute("...") usages.`

## Runtime/automated-test blockers captured
- Attempted targeted test execution:
  - `dotnet test Web/Edubase.Web.UIUnitTests/Edubase.Web.UIUnitTests.csproj -c Release`
- Result: **Blocked** with restore/build incompatibility:
  - `NU1201`: `Edubase.Web.UIUnitTests` targets `net48` and references `Edubase.Web.UI` targeting `net10.0`.
- Owner/next step:
  - Address test-project target/reference alignment under migration validation closure (`04.06` / `06`), then rerun endpoint-focused automated tests.

## Outcome
- Targeted endpoint contract validation evidence is now recorded and reproducible for migrated controller surfaces.
- Remaining unresolved runtime parity verification is explicitly blocked by the current Web.UIUnitTests framework mismatch and tracked with next-step ownership.
