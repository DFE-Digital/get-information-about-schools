# 04.04-webui-host-foundation-net10: Retarget Web.UI and migrate host/pipeline/config/DI foundation

# 04.04-webui-host-foundation-net10

## Objective
Migrate Web.UI host foundations to net10 with ASP.NET Core startup, configuration, middleware, and DI scaffolding while preserving required compatibility boundaries.

## Scope
- Project: `Edubase.Web.UI`
- Startup/Program host bootstrap, configuration migration, middleware pipeline foundation, DI registrations
- Authentication/session/antiforgery baseline wiring (behavior-preserving setup)

## Steps
1. Retarget Web.UI to net10.0 and establish runnable host baseline.
2. Apply configuration mapping outputs from Task 02 (no secret leakage).
3. Migrate core middleware/DI wiring and required auth/session foundation behavior.
4. Resolve high-risk package/API blockers needed before controller/view migration (including FluentValidation baseline decision implementation path).

## Done when
- Web.UI host foundation builds/runs on net10 baseline.
- Config/DI/middleware foundation is in place without silent behavior changes.
- FluentValidation baseline migration path is implemented for Web.UI continuation.
