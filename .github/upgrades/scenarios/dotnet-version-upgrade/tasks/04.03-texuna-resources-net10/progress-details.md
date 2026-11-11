## 04.03 execution details

### Scope delivered
Retargeted Texuna/Resources closure projects and related tests to `net10.0`, removed legacy project couplings that blocked closure migration, and resolved net10 API/compatibility breaks while preserving existing behavior.

### Changes made

#### Project and package migration
- Converted legacy test project to SDK-style:
  - `Web/Edubase.Services.TexunaUnitTests/Edubase.Services.TexunaUnitTests.csproj` (via conversion tool)
- Retargeted to `net10.0`:
  - `Web/Edubase.Services.Texuna/Edubase.Services.Texuna.csproj`
  - `Web/Edubase.Web.Resources/Edubase.Web.Resources.csproj`
  - `Web/Edubase.Services.TexunaUnitTests/Edubase.Services.TexunaUnitTests.csproj`
  - `Web/Edubase.Web.ResourcesUnitTests/Edubase.Web.ResourcesUnitTests.csproj`
- Removed obsolete framework-only assembly references from migrated SDK projects.
- Removed test-project coupling not needed for Resources tests:
  - `Edubase.Web.ResourcesUnitTests` no longer references `Edubase.Services.TexunaUnitTests`.

#### Texuna HttpClientWrapper net10 compatibility
- Updated `Web/Edubase.Services.Texuna/Core/HttpClientWrapper.cs`:
  - Removed direct `System.Web` and `Microsoft.AspNet.Identity` dependencies.
  - Replaced identity extraction with local claim-based helper methods.
  - Replaced `HttpRequestMessage.Properties` usage (obsolete) with `HttpRequestMessage.Options` + typed key.
  - Removed usage of unavailable `GetPrincipal()` extension and read `sa_user_id` from request headers for API interaction logging.
  - Removed the transitional `#if NETFRAMEWORK` context block to keep this implementation explicitly net10-only.
  - Preserved timeout handling and request/response logging behavior.

#### Test decoupling from Web.UI
- Updated `Web/Edubase.Services.TexunaUnitTests/Core/HttpClientWrapperTests.cs` to construct `JsonMediaTypeFormatter` directly instead of calling `Web.UI` IOC setup, removing net48 Web.UI dependency from this closure.

### Validation

#### Builds
- `run_build` succeeded:
  - `Web/Edubase.Services.TexunaUnitTests/Edubase.Services.TexunaUnitTests.csproj`
  - `Web/Edubase.Web.ResourcesUnitTests/Edubase.Web.ResourcesUnitTests.csproj`

#### Tests
- `run_tests` (Projects: `Edubase.Services.TexunaUnitTests`, `Edubase.Web.ResourcesUnitTests`):
  - **118 passed, 0 failed**

### Notes
- NU1903 AutoMapper warnings may still appear in CLI `dotnet build` output due to repository-wide intentional pinning to 10.1.1 (user-approved in earlier task). No new package-version decision was made in this task.
