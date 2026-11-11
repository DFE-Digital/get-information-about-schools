# 04.05.03-webui-views-assets-parity: Migrate Razor views/static assets to ASP.NET Core-compatible behavior

## Objective
Migrate Razor view and static-asset behavior to ASP.NET Core-compatible patterns while preserving current UI layout/styling/features.

## Scope
- Views and Areas view trees, shared templates, partials, and layout infrastructure
- Helper pattern migration where required (Html helper usage incompatible with ASP.NET Core)
- Static asset serving and required build/runtime view configuration

## Steps
1. Establish ASP.NET Core Razor infrastructure (e.g., imports/configuration) needed by migrated views.
2. Convert incompatible helper usages and partial rendering patterns to supported ASP.NET Core equivalents.
3. Ensure static assets and layout rendering preserve existing UI output.
4. Re-enable and validate Razor compilation for migrated view surface.

## Done when
- Views compile/render under ASP.NET Core with preserved UI behavior for migrated flows.
- Temporary view-compat shims are removed or explicitly tracked as blockers.
