# 04.05.01-webui-bootstrap-di-routing: Reinstate Web.UI compile surface and migrate startup/DI/routing foundations

## Objective
Re-enable Web.UI feature compilation on net10 and migrate legacy startup/bootstrap wiring (Global.asax/App_Start) to ASP.NET Core Program/DI/routing/filter registration foundations.

## Scope
- Web.UI project compile/item settings currently limiting build to Program.cs
- Startup and registration surfaces: Global.asax, IocConfig, RouteConfig, FilterConfig
- ASP.NET Core equivalents for controller discovery, route mapping, filter registration, JSON baseline behavior

## Research findings (2026-11-11)
- Current net10 host shell is active (`Program.cs`) but project compilation is intentionally constrained:
  - `EnableDefaultCompileItems=false`
  - explicit `<Compile Include="Program.cs" />`
  - Razor build/source generation disabled.
- Legacy startup bootstrap remains in `Global.asax.cs` and includes:
  - WebApi attribute-route registration and JSON formatter setup
  - Autofac MVC5/WebApi resolver setup via `IocConfig.Register(HttpConfiguration)`
  - global filter registration via `FilterConfig`
  - model binder/value-provider registration and misc startup initializers.
- `App_Start/IocConfig.cs` is Autofac-centric and tied to Framework-specific APIs (`System.Web`, `Autofac.Integration.Mvc`, `Autofac.Integration.WebApi`, `HttpContext.Current`, `ConfigurationManager`), so it cannot be reused unchanged in ASP.NET Core startup.
- `RouteConfig.cs` currently only enables MVC attribute routing + `.axd` ignore route, so ASP.NET Core endpoint routing can absorb this directly.
- `FilterConfig.cs` uses MVC5 `GlobalFilterCollection`; this requires ASP.NET Core MVC options/filter registration equivalent.

## Affected files (expected)
- `Web/Edubase.Web.UI/Edubase.Web.UI.csproj`
- `Web/Edubase.Web.UI/Program.cs`
- `Web/Edubase.Web.UI/Global.asax.cs` (bootstrap migration source)
- `Web/Edubase.Web.UI/Global.asax` (legacy entry artifact)
- `Web/Edubase.Web.UI/App_Start/IocConfig.cs`
- `Web/Edubase.Web.UI/App_Start/RouteConfig.cs`
- `Web/Edubase.Web.UI/App_Start/FilterConfig.cs`

## Implementation boundary for this subtask
- Migrate startup/DI/routing foundations and project compilation settings needed to move past Program-only build.
- Keep full controller/API contract migration in subtask `04.05.02` and view/asset migration in `04.05.03`.
- Preserve behavior intent (no silent fallback): where exact parity cannot yet be established in this subtask, wire explicit TODO/blocker paths to be closed by later subtasks.

## Steps
1. Remove temporary compile restrictions needed only for 04.04 host-shell.
2. Port startup wiring from Global.asax/App_Start into Program/extension methods.
3. Migrate DI bootstrap from MVC5/WebApi Autofac resolver wiring to ASP.NET Core service registration paths.
4. Build Web.UI and resolve compilation blockers created by startup/DI/routing migration.

## Done when
- Web.UI compiles with migrated startup/DI/routing foundations active on net10.
- Legacy Global.asax bootstrap dependency is removed from active startup path.
