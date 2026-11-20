using Edubase.AcceptanceTests.Establishments;
using Edubase.AcceptanceTests.GiasFrontEnd;
using Edubase.AcceptanceTests.Users;
using Xunit;
using static System.Net.WebRequestMethods;

namespace Edubase.AcceptanceTests.UserPreferences
{
    [Binding]
    internal class UserPreferencesStepDefinitions
    {
        private readonly IUsers users;
        private readonly IGiasFrontEnd giasFrontEnd;
        private readonly IEstablishmentSearchResultsScreen establishmentSearchResultsScreen;
        private readonly IUserPreferencesInPersistentStorage userPreferencesInPersistentStorage;
        private readonly IUserPreferencesInMigrationSource userPreferencesInMigrationSource;
        private readonly IUserPreferencesInMigrationTarget userPreferencesInMigrationTarget;
        private readonly IAcceptanceTestLogger acceptanceTestLogger;
        private EstablishmentSearchFilter establishmentSearchFilter;
        private User user;
        private string savedSearchToken;
        private EstablishmentSearchResults? searchResults;
        private string selectedSaveFilterSet;

        public UserPreferencesStepDefinitions(
            IUsers users,
            IGiasFrontEnd giasFrontEnd,
            IEstablishmentSearchResultsScreen establishmentSearchResultsScreen,
            IUserPreferencesInPersistentStorage userPreferencesInPersistentStorage,
            IUserPreferencesInMigrationSource userPreferencesInMigrationSource,
            IUserPreferencesInMigrationTarget userPreferencesInMigrationTarget,
            IAcceptanceTestLogger acceptanceTestLogger)
        {
            this.users = users;
            this.giasFrontEnd = giasFrontEnd;
            this.establishmentSearchResultsScreen = establishmentSearchResultsScreen;
            this.userPreferencesInPersistentStorage = userPreferencesInPersistentStorage;
            this.userPreferencesInMigrationSource = userPreferencesInMigrationSource;
            this.userPreferencesInMigrationTarget = userPreferencesInMigrationTarget;
            this.acceptanceTestLogger = acceptanceTestLogger;
        }

        [Given("the required data extracted from table storage")]
        public void GivenTheRequiredDataExtractedFromTableStorage()
        {
            // assume the data was extracted by the DBA
        }

        [When("the migration runs")]
        public void WhenTheMigrationRuns()
        {
            // assume the migration was run by the DBA
        }

        [Then("all the existing UserPreferencesRepository data extracted from the table storage should be migrated to the SQL table.")]
        public async Task ThenAllTheExistingUserPreferencesRepositoryDataExtractedFromTheTableStorageShouldBeMigratedToTheSQLTable_()
        {
            var sourceUserPreferences = (await userPreferencesInMigrationSource.GetAllUserPreferences()).ToList();
            var targetUserPreferences = (await userPreferencesInMigrationTarget.GetAllUserPreferences()).ToList();

            var targetPreferencesByUserId = targetUserPreferences
                .ToLookup(preference => preference.RowKey, StringComparer.Ordinal);

            var missingSourceUserIds = sourceUserPreferences
                .Where(sourcePreference => !targetPreferencesByUserId[sourcePreference.RowKey].Any())
                .Select(sourcePreference => sourcePreference.RowKey)
                .ToList();

            var tokenDifferences = sourceUserPreferences
                .Where(sourcePreference => targetPreferencesByUserId[sourcePreference.RowKey].Any())
                .SelectMany(sourcePreference => targetPreferencesByUserId[sourcePreference.RowKey]
                    .Where(targetPreference => !string.Equals(
                        sourcePreference.SavedSearchToken,
                        targetPreference.SavedSearchToken,
                        StringComparison.Ordinal))
                    .Select(targetPreference => new
                    {
                        sourcePreference.RowKey,
                        SourceToken = sourcePreference.SavedSearchToken,
                        TargetToken = targetPreference.SavedSearchToken
                    }))
                .ToList();

            var matchingCount = sourceUserPreferences.Count - missingSourceUserIds.Count;

            acceptanceTestLogger.LogInformation($"User preference migration: {sourceUserPreferences.Count} source rows.");
            acceptanceTestLogger.LogInformation($"User preference migration: {targetUserPreferences.Count} target rows.");
            acceptanceTestLogger.LogInformation($"User preference migration: {matchingCount} source users have a target row.");
            acceptanceTestLogger.LogInformation($"User preference migration: {missingSourceUserIds.Count} source users are missing from the target.");
            acceptanceTestLogger.LogInformation($"User preference migration: {tokenDifferences.Count} token values differ.");

            foreach (var missingUserId in missingSourceUserIds)
            {
                acceptanceTestLogger.LogInformation($"No target preference row exists for source user '{missingUserId}'.");
            }

            foreach (var difference in tokenDifferences)
            {
                acceptanceTestLogger.LogInformation(
                    $"Token differs for user '{difference.RowKey}': source='{difference.SourceToken ?? "<null>"}', " +
                    $"target='{difference.TargetToken ?? "<null>"}'.");
            }

            Assert.Empty(missingSourceUserIds);
            Assert.True(sourceUserPreferences.Count <= targetUserPreferences.Count);
        }

        [Given("I am a logged-in GIAS user")]
        public async Task GivenIAmALoggedInGIASUser()
        {
            user = await users.GetBackOfficeUser();

            await giasFrontEnd.SignIn(user);
        }

        [When("I filter records on the establishment search results screen")]
        public async Task WhenIFilterRecordsOnTheEstablishmentSearchResultsScreen()
        {
            establishmentSearchFilter = new EstablishmentSearchFilter { };

            searchResults = await establishmentSearchResultsScreen.GetSearchResults(establishmentSearchFilter);
        }

        [Then("I can save the filter set to create a saved filter")]
        public async Task ThenICanSaveTheFilterSetToCreateASavedFilter()
        {
            savedSearchToken = await establishmentSearchResultsScreen.SaveFilterSet(establishmentSearchFilter);

            var persistedSearchToken = await userPreferencesInPersistentStorage.GetSavedSearchTokenForUser(user);

            Assert.Equal(savedSearchToken, persistedSearchToken);
        }

        [Given("I have a saved filter")]
        public async Task GivenIHaveASavedFilter()
        {
            await GivenIAmALoggedInGIASUser();
            await WhenIFilterRecordsOnTheEstablishmentSearchResultsScreen();
            await ThenICanSaveTheFilterSetToCreateASavedFilter();
        }

        [When("I select My saved filter set on the establishment search results screen")]
        public async Task WhenISelectMySavedFilterSetOnTheEstablishmentSearchResultsScreen()
        {
            selectedSaveFilterSet = savedSearchToken;
        }

        [Then("the saved filters are applied to the list of establishments")]
        public async Task ThenTheSavedFiltersAreAppliedToTheListOfEstablishments()
        {
            var actualSearchResults = await establishmentSearchResultsScreen.GetSearchResults(selectedSaveFilterSet);

            var expectedUrns = searchResults!.Establishments
                .Select(establishment => establishment.Urn)
                .OrderBy(urn => urn);

            var actualUrns = actualSearchResults!.Establishments
                .Select(establishment => establishment.Urn)
                .OrderBy(urn => urn);

            Assert.Equal(expectedUrns, actualUrns);
        }

        [When("I select the option My saved filter set")]
        public void WhenISelectTheOptionMySavedFilterSet()
        {
            selectedSaveFilterSet = savedSearchToken;
        }

        [Then("I can delete my saved filter")]
        public async Task ThenICanDeleteMySavedFilter()
        {
            await establishmentSearchResultsScreen.DeleteSavedFilter(selectedSaveFilterSet);

            var persistedSearchToken = await userPreferencesInPersistentStorage.GetSavedSearchTokenForUser(user);

            Assert.Null(persistedSearchToken);
        }

    }
}
