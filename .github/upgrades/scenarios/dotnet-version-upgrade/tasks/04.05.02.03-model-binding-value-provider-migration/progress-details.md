## Task
04.05.02.03-model-binding-value-provider-migration

## Summary
Migrated model-binding alias and token query value-provider infrastructure from MVC5 `System.Web.Mvc` contracts to ASP.NET Core MVC options/value-provider contracts.

## Changes made
- Migrated `Helpers/ValueProviders/TokenValueProviderFactory.cs` to ASP.NET Core:
  - Replaced MVC5 `ValueProviderFactory` override with `IValueProviderFactory`.
  - Replaced `DependencyResolver.Current` with request-scoped `RequestServices` resolution of `ITokenRepository`.
  - Replaced `HttpUtility.ParseQueryString` with `QueryHelpers.ParseQuery` and injected token values as `QueryStringValueProvider` entries.
- Migrated `Helpers/ModelBinding/DefaultModelBinderEx.cs` from MVC5 binder inheritance to ASP.NET Core helper role:
  - Replaced `DefaultModelBinder` extension implementation with action/parameter reflection helper that builds `[BindAlias]` alias maps for complex action parameter types.
- Added `Helpers/ModelBinding/BindAliasValueProviderFactory.cs`:
  - New ASP.NET Core `IValueProviderFactory` that applies alias-to-canonical key mapping for `[BindAlias]`-decorated properties at request-binding time.
  - Registered as first value-provider factory so alias mapping is available before default query providers.
- Updated `App_Start/CoreServiceRegistration.cs`:
  - Added `BindAliasValueProviderFactory` and `TokenValueProviderFactory` registration in `AddControllersWithViews` MVC options.
- Updated `Helpers/ModelBinding/BindAliasAttribute.cs` documentation to reflect ASP.NET Core value-provider-based alias resolution.
- Updated `Edubase.Web.UI.csproj` active compile slice:
  - Included migrated model-binding/value-provider files in net10 compile list.

## Validation
- Build:
  - `run_build` on `Web/Edubase.Web.UI/Edubase.Web.UI.csproj`
  - Result: **Build successful**.

## Notes
- Legacy MVC5-only `Global.asax` binder/value-provider registration remains non-active for net10 host and is intentionally not used by current ASP.NET Core startup pipeline.
