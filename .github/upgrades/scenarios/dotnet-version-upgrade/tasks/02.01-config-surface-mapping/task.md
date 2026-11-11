# 02.01-config-surface-mapping: Inventory and map configuration surfaces

## Objective
Create a portable, non-secret configuration mapping for the upgrade by inventorying all active configuration sources and documenting their intended .NET 10 equivalents and precedence.

## Scope
- `Edubase.Web.UI/Web.config`, `Web.Debug.config`, `Web.Release.config`
- `app.config` files in Common, Data, Services, Services.Texuna, Web.Resources, Services.TexunaUnitTests
- Related configuration access points in code (ConfigurationManager usage hotspots)

## Research findings
- Config files discovered:
  - Web-layer: `Web.config`, `Web.Debug.config`, `Web.Release.config`, and view-level Web.config files
  - Library/test-level: `app.config` in Common/Data/Services/Services.Texuna/Web.Resources/Services.TexunaUnitTests
- Legacy package/config coupling discovered:
  - `packages.config` in six legacy projects (`Common`, `Data`, `Services`, `Services.Texuna`, `Services.TexunaUnitTests`, `Web.UI`)
  - Extensive binding redirects in `Web.UI/Web.config` and multiple app.config files
- `ConfigurationManager` usage remains widespread (116 matches across 41 files), with highest concentration in `Web.UI/App_Start/IocConfig.cs`.
- Web config includes migration-critical surfaces: connection strings, large app settings set (including feature flags), auth/session/OWIN/SAML settings, IIS handlers/modules, custom sections (`glimpse`, `httpAuthModule`, `entityFramework`), EF6/provider entries, and `service.wsdl` handler mapping.

## Deliverable
- Portable non-secret mapping authored at:
  - `.github/upgrades/scenarios/dotnet-version-upgrade/tasks/02.01-config-surface-mapping/configuration-mapping.md`

## Done when
- A portable mapping exists for key configuration surfaces and precedence.
- IIS/module/handler/rewrite and WSDL mapping implications are documented for migration.
- Secret-handling guidance is explicit and no secrets are documented.
