# Configuration Mapping (Task 02.01)

## 1) Source inventory (non-secret)

| Source file | Type | Purpose highlights |
|---|---|---|
| `Web/Edubase.Web.UI/Web.config` | Primary runtime config | `connectionStrings`, extensive `appSettings`, `system.web`, `runtime` binding redirects, `system.webServer` handlers/modules/rewrite/security, custom sections (`glimpse`, `httpAuthModule`, `entityFramework`) |
| `Web/Edubase.Web.UI/Web.Debug.config` | Transform | Debug/HTTPS/rewrite and lookup API override transform entries |
| `Web/Edubase.Web.UI/Web.Release.config` | Transform template | Release transform template and compile debug attribute removal |
| `Web/Edubase.Data/App.config` | Library config | EF6 config section/providers + binding redirects |
| `Web/Edubase.Services/App.config` | Library config | EF6 config + ServiceBus settings + binding redirects |
| `Web/Edubase.Services.Texuna/app.config` | Library config | Binding redirects |
| `Web/Edubase.Common/app.config` | Library config | Binding redirects |
| `Web/Edubase.Web.Resources/app.config` | Library config | Binding redirects |
| `Web/Edubase.Services.TexunaUnitTests/app.config` | Test config | Binding redirects (test runtime) |

## 2) Current configuration precedence and target precedence

### Current (.NET Framework)
1. Base XML config (`Web.config` / `App.config`)
2. Build transforms (`Web.Debug.config`, `Web.Release.config`, etc.)
3. Environment/host-level overrides (local/azure external config and secrets)
4. Runtime `ConfigurationManager` reads from final merged config

### Target (.NET 10 / ASP.NET Core)
1. `appsettings.json` (base)
2. `appsettings.{Environment}.json`
3. Secret providers (User Secrets in dev, managed secret store/env vars in hosted envs)
4. Environment variables / command-line overrides
5. `IConfiguration` / `IOptions<T>` consumers

## 3) Key mapping table

| Current surface | Examples (non-secret) | Target shape | Notes |
|---|---|---|---|
| `connectionStrings` | `DataConnectionString`, `Redis`, `FrontEndDatabase` | `ConnectionStrings:{Name}` | Keep values out of committed files; resolve via secret providers/env-specific config |
| Feature flags in `appSettings` | `Feature_*Migration`, `Feature_Ofsted_*` | `FeatureFlags` or dedicated options section | Preserve current defaults/behavior; keep ownership with product/domain owners |
| Service endpoints/timeouts/retry settings | `*BaseAddress`, `*Timeout*`, `*RetryIntervals` | Typed options per integration (`Lookup`, `Texuna`, `Fscpd`, `Ofsted`, `OSPlaces`, `AzureMaps`) | Convert to strongly typed options with validation |
| Authentication/session settings | `owin:appStartup`, `ApplicationIdpEntityId`, `MetadataLocation`, `SessionExpireTimeSpan`, `SASimulator*` | Auth options + cookie/session options + environment-specific auth settings | Preserve SAML/cookie/anti-forgery semantics during migration |
| Security headers/content policy | `Content-Security-Policy`, header settings under `system.webServer/httpProtocol` | ASP.NET Core middleware/header policies | Preserve security posture and ordering |
| `system.web` and `system.webServer` pipeline config | handlers/modules/rewrite/customErrors/httpRuntime/requestFiltering/static content | ASP.NET Core middleware + endpoint routing + static file/rewrite/exception handling setup | Requires ordering parity analysis in later migration tasks |
| Service WSDL handler mapping | `ServiceWSDLHandler` for `service.wsdl` | Endpoint/route mapping preserving `service.wsdl` behavior | Must preserve existing external contract and redirect behavior |
| EF6 and provider config | `entityFramework` section, `DbProviderFactories` entries | EF6 + provider registration approach under .NET 10 migration constraints | EF6 retention confirmed; provider behavior must be preserved |
| Legacy custom sections | `httpAuthModule`, `glimpse`, `system.serviceModel` extension declarations | Middleware/options and targeted replacement/removal decisions | Do not assume one-for-one replacements; validate actual runtime usage |

## 4) Configuration access hotspots

- `ConfigurationManager` access remains widespread (116 matches, 41 files).
- Highest concentration: `Web/Edubase.Web.UI/App_Start/IocConfig.cs` (36 matches).
- Additional hotspots: `Edubase.Services` external integration services, `Edubase.Data` repositories, view models/views using direct app setting reads.

## 5) Secret-handling policy for migration artifacts

- Do **not** copy secret values into scenario/task markdown.
- Record only key names and semantic purpose.
- Use placeholders in mapping docs and rely on environment/user secrets at execution time.
- Treat acceptance-test `devsecrets.gias.config.alwaysignore` as an external prerequisite (already tracked as blocker).

## 6) Open/linked decision constraints affecting this mapping

- SQL migration assumption is fixed for ten flagged data areas (no Table Storage expected there by .NET 10 implementation).
- Logging/API recording target is fixed: Application Insights, admin log viewer retired.
- OData disposition remains deferred pending deeper runtime verification.
- Acceptance-test environment access remains an explicit external blocker.
