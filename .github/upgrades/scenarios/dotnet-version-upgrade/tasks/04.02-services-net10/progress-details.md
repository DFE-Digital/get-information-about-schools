# 04.02-services-net10 Progress Details

## Summary
Completed migration of the Services closure to `net10.0` and resolved compile/test issues plus package/compatibility/security warnings in the closure dependency path (Services, ServicesUnitTests, and directly impacted Common/Data package metadata).

## Changes made

### Target framework and test host alignment
- Retargeted:
  - `Web/Edubase.Services/Edubase.Services.csproj` -> `net10.0`
  - `Web/Edubase.ServicesUnitTests/Edubase.ServicesUnitTests.csproj` -> `net10.0`
- Added explicit test SDK package for stable .NET 10 test discovery/execution:
  - `Microsoft.NET.Test.Sdk` in `Edubase.ServicesUnitTests.csproj`

### Services code compatibility fixes
- `Web/Edubase.Services/BlobService.cs`
  - Replaced removed synchronous Azure Storage APIs with async equivalents and synchronous `GetAwaiter().GetResult()` bridging to preserve existing method signatures.
- `Web/Edubase.Services/IntegrationEndPoints/PollyUtil.cs`
  - Added null/whitespace guard in `CsvSecondsToTimeSpans` to avoid null dereference under .NET 10 config behavior.
- `Web/Edubase.Services/Security/ClaimsIdentityConverters/StubClaimsIdConverter.cs`
- `Web/Edubase.Services/Security/ClaimsIdentityConverters/SecureAccessClaimsIdConverter.cs`
  - Removed dependency on `Microsoft.AspNet.Identity.DefaultAuthenticationTypes` by using local authentication-type constant value (`"ApplicationCookie"`).

### Warning/security remediation within expanded 04.02 scope
- Updated Services package metadata:
  - Removed net10-inbox and legacy compatibility package references no longer required.
  - Updated `Newtonsoft.Json.Bson` to `1.0.3`.
- Decoupled Services log DTO from `AzureTableLogger` package:
  - `Web/Edubase.Services/Domain/LogMessagesDto.cs` now uses Services-owned `WebLogMessage` model.
  - Added `Web/Edubase.Services/Domain/WebLogMessage.cs`.
  - Updated `Web/Edubase.Web.UI/Models/Admin/LogMessagesViewModel.cs` to consume the Services-owned model type.
- Upgraded/remediated dependency versions in `Edubase.Common` and aligned transitive versions consumed by Services/Data closure.
- Removed legacy framework assembly references from SDK-style projects in this closure path that caused MSBuild conflict warnings.
- Removed obsolete formatter-serialization exception constructors that produced SYSLIB warnings in net10 builds:
  - `EdubaseException`, `EduSecurityException`, `EntityNotFoundException`, `PermissionDeniedException`, `RuleViolationEmptyParamException`, `TexunaApiNotFoundException`, `TexunaApiSystemException`.
- Updated enum helper casting in `LanguageExtensions` to remove CA2021 warnings.
- Replaced obsolete `FormatterServices.GetUninitializedObject` usage in tests with `RuntimeHelpers.GetUninitializedObject`.

## Validation evidence
- Build (closure):
  - `dotnet build Web/Edubase.ServicesUnitTests/Edubase.ServicesUnitTests.csproj -v minimal`
  - Result: **Build succeeded, 0 warnings, 0 errors**.
- Test run:
  - Services unit tests project via test runner
  - Result: **346 passed, 0 failed**.
- Final build verification tool run:
  - `run_build` on `Web/Edubase.ServicesUnitTests/Edubase.ServicesUnitTests.csproj`
  - Result: **Build successful**.

## Notes
- Scope was explicitly expanded during Guided execution by user decision to resolve remaining warning surface in 04.02 rather than carrying it to later subtasks.
