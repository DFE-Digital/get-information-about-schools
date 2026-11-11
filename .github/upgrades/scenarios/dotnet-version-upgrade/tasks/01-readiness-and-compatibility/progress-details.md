# 01-readiness-and-compatibility progress details

## Summary
- Established baseline readiness for the .NET 10 upgrade without changing application/runtime code.
- Recorded toolchain, project/test inventory, build/test baseline outcomes, external dependency expectations, and unresolved decision gates.

## Files modified
- `.github/upgrades/scenarios/dotnet-version-upgrade/scenario-instructions.md`
- `.github/upgrades/scenarios/dotnet-version-upgrade/tasks/01-readiness-and-compatibility/task.md`

## Validation evidence
- `validate_dotnet_sdk_installation(net10.0)`: compatible SDK found.
- `validate_dotnet_sdk_in_globaljson(net10.0)`: no global.json present.
- `run_build(projectPath: null)`: solution baseline build successful.
- `run_tests` across all discovered test projects: 1359 total, 1356 passed, 3 failed.
  - Failed scenarios are acceptance tests with `ArgumentNullException (uriString)` from missing/invalid `GiasFrontEndBaseAddress` in acceptance-test settings for this environment.
- `dotnet list package --vulnerable` baseline captured:
  - `AutoMapper 10.1.1` high advisory (`GHSA-rvv3-g6hj-g44x`)
  - `System.Security.Cryptography.Xml 4.5.0` moderate advisory (`GHSA-vh55-786g-wjwj`)

## Research outcomes captured
- Confirmed 13-project scope and dependency order for bottom-up execution.
- Confirmed 7 test projects and baseline test execution result.
- Mapped active Table Storage/logging touchpoints (`ApiRecorderSessionItemRepository`, `WebLogItemRepository`, `TableStorageBase<T>`, `EnableApiLogging`, migration feature flags).
- Verified OData package presence and recorded current lack of direct OData API usage in searched code paths.
- Captured authentication/session parity boundaries (cookie auth + Sustainsys SAML2 + anti-forgery/session-expiry setup).

## Unresolved decisions explicitly tracked
- SQL migration completion/fallback scope confirmation.
- Final logging destination and required semantics for excluded Table Storage repositories.
- Acceptance test environment contract for fully green baseline.
- OData contract/usage disposition decision.

Each unresolved item was recorded in task.md with owner, evidence required, and blocked downstream task(s).
