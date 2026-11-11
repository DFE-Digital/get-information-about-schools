## Task
04.04-webui-host-foundation-net10

## Summary
Completed the Web.UI host-foundation migration step for `net10.0` using the agreed host-shell strategy. The project now builds and starts on ASP.NET Core with baseline host/middleware/auth/session/antiforgery wiring and configuration scaffolding, while full controller/view parity remains deferred to Task 04.05.

## Changes made
- Retargeted `Web/Edubase.Web.UI/Edubase.Web.UI.csproj` to `net10.0`.
- Applied phase-scoped project settings to support host-foundation compilation before controller/view migration:
  - Legacy net48-specific references/targets conditioned away from net10 host-shell path.
  - Razor compile/source-generation disabled for this phase to avoid false failures against unmigrated MVC5 surfaces.
  - Added `appsettings.json` and `appsettings.Development.json` as content.
- Added `Web/Edubase.Web.UI/Program.cs` implementing ASP.NET Core host baseline:
  - Startup host bootstrap and routing.
  - Cookie auth/session/antiforgery baseline service registration.
  - Security headers + CSP nonce middleware baseline.
  - Health endpoint for runtime verification.
- Added configuration scaffolding files:
  - `Web/Edubase.Web.UI/appsettings.json`
  - `Web/Edubase.Web.UI/appsettings.Development.json`

## Validation
- Build validation:
  - `run_build(projectPath="C:\copilot\get-information-about-schools\Web\Edubase.Web.UI\Edubase.Web.UI.csproj")`
  - Result: Success.
  - Known warnings: existing `NU1903` AutoMapper advisory warnings (previously acknowledged/pinned by user decision).
- Runtime validation:
  - Started app with `dotnet run --project Web/Edubase.Web.UI/Edubase.Web.UI.csproj --urls http://127.0.0.1:5057`
  - Verified process reached listening state on `http://127.0.0.1:5057`
  - Probed `GET /health` and received `200` with body `"ok"`.

## Notes / follow-up for next task
- No production deployment performed.
- Task 04.05 will port controller/view/runtime behavior and re-enable full Razor compilation once migrated surfaces are in place.
