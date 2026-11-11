# 04.01-common-data-net10 progress details

## Summary
- Retargeted the Common/Data closure to `net10.0` and validated build/test baseline for this closure.
- Enforced SQL-only repository wiring in Web.UI for migrated data domains and removed associated migration feature flags.
- Kept logging/API-recording table-storage path separate as requested.

## Files modified
- `.github/upgrades/scenarios/dotnet-version-upgrade/scenario-instructions.md`
- `.github/upgrades/scenarios/dotnet-version-upgrade/tasks/04.01-common-data-net10/task.md`
- `Web/Edubase.Common/Edubase.Common.csproj`
- `Web/Edubase.Common/StringUtil.cs`
- `Web/Edubase.Common/UriHelper.cs`
- `Web/Edubase.Common/IO/FileHelper.cs`
- `Web/Edubase.Common/Spatial/DistanceCalculator.cs`
- `Web/Edubase.Data/Edubase.Data.csproj`
- `Web/Edubase.Data/Repositories/ApiRecorderSessionItemRepository.cs`
- `Web/Edubase.Data/Repositories/TableStorage/TableStorageBase.cs`
- `Web/Edubase.Data/Repositories/DataQualityStatusRepository.cs`
- `Web/Edubase.Data/Repositories/FaqGroupRepository.cs`
- `Web/Edubase.Data/Repositories/FaqItemRepository.cs`
- `Web/Edubase.Data/Repositories/GlossaryRepository.cs`
- `Web/Edubase.Data/Repositories/LocalAuthoritySetRepository.cs`
- `Web/Edubase.Data/Repositories/NewsArticleRepository.cs`
- `Web/Edubase.Data/Repositories/NotificationBannerRepository.cs`
- `Web/Edubase.Data/Repositories/NotificationTemplateRepository.cs`
- `Web/Edubase.Data/Repositories/TokenRepository.cs`
- `Web/Edubase.Data/Repositories/UserPreferenceRepository.cs`
- `Web/Edubase.CommonUnitTests/Edubase.CommonUnitTests.csproj`
- `Web/Edubase.DataUnitTests/Edubase.DataUnitTests.csproj`
- `Web/Edubase.Web.UI/App_Start/IocConfig.cs`
- `Web/Edubase.Web.UI/Web.config`

## Validation evidence
- Build succeeded:
  - `Web/Edubase.Common/Edubase.Common.csproj`
  - `Web/Edubase.Data/Edubase.Data.csproj`
  - `Web/Edubase.CommonUnitTests/Edubase.CommonUnitTests.csproj`
  - `Web/Edubase.DataUnitTests/Edubase.DataUnitTests.csproj`
- Tests succeeded via `dotnet test`:
  - `Edubase.CommonUnitTests`: Passed 7/7
  - `Edubase.DataUnitTests`: Passed 1/1

## Notes
- Data closure removed compile-time participation of table-storage repository implementations for migrated domains by excluding their source files in `Edubase.Data.csproj`, matching the SQL-only wiring decision.
- As follow-up cleanup, those excluded legacy table-storage repository implementation files were physically removed from the repository and the temporary `<Compile Remove=...>` entries were removed from `Edubase.Data.csproj`.
- `ApiRecorderSessionItemRepository` and `WebLogItemRepository` table-storage path remains separate and preserved for later dedicated handling.
- `run_tests` (IDE test runner) did not provide reliable execution for this retargeted closure; direct `dotnet test` succeeded and is recorded as task evidence.