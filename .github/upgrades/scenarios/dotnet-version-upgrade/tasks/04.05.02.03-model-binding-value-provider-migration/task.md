# 04.05.02.03-model-binding-value-provider-migration: Migrate custom model binder and token value-provider infrastructure

## Objective
Port custom MVC5 model-binding and value-provider components used by controller actions to ASP.NET Core model binding infrastructure.

## Scope
- `Helpers/ModelBinding/DefaultModelBinderEx.cs`
- `Helpers/ValueProviders/TokenValueProviderFactory.cs`
- Program/App_Start registration for binder/value-provider behavior

## Repository research (2026-11-11)
- Legacy registration points are still in `Global.asax.cs` (not active in net10 host):
  - `ModelBinders.Binders.DefaultBinder = new DefaultModelBinderEx();`
  - `ValueProviderFactories.Factories.Add(new TokenValueProviderFactory());`
- `DefaultModelBinderEx` currently depends on MVC5 `DefaultModelBinder` and `System.Web.Mvc` contracts.
- Alias binding is driven by `BindAliasAttribute`, which is used in many search/view-models (establishments, groups, governors, change-history, tools). This indicates alias support is still relevant for query-string-driven pages.
- `TokenValueProviderFactory` currently uses:
  - `ValueProviderFactory` / `ControllerContext` (MVC5)
  - `DependencyResolver.Current.GetService<ITokenRepository>()`
  - `HttpUtility.ParseQueryString` to inflate `tok` payload into a value provider.
- Current net10 foundation registration is in `App_Start/CoreServiceRegistration.cs` via `AddControllersWithViews(...)`; this is where ASP.NET Core binder/value-provider registration must be wired.
- Active compile slice in `Edubase.Web.UI.csproj` currently excludes model-binding helpers; this task will include migrated helper files in the compile set.

## Planned migration approach
1. Replace MVC5 binder extension strategy with ASP.NET Core value-provider based alias support:
   - Add a custom ASP.NET Core `IValueProviderFactory` that maps `[BindAlias]` query aliases to canonical property keys during binding.
   - Keep `BindAliasAttribute` as the alias declaration surface to preserve model annotations.
2. Migrate `TokenValueProviderFactory` to ASP.NET Core `IValueProviderFactory`:
   - Resolve `ITokenRepository` from request services.
   - Parse token query payload using ASP.NET Core-compatible utilities.
   - Append token-derived values into MVC value-provider pipeline.
3. Register both factories via `MvcOptions.ValueProviderFactories` in `CoreServiceRegistration`.
4. Expand compile includes and run targeted `dotnet build` for `Web/Edubase.Web.UI`.

## Steps
1. Convert custom binder/value-provider types and signatures to ASP.NET Core contracts.
2. Replace MVC5 static registration paths with ASP.NET Core MvcOptions registration.
3. Preserve token-query and alias-binding behavior semantics for dependent actions.
4. Build and resolve binding-related compile issues.

## Done when
- Custom binder/value-provider components compile and are registered in ASP.NET Core MVC options.
- Binding behavior parity intent is preserved or explicitly documented where deferred.
