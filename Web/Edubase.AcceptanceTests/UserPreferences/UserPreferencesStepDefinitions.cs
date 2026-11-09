using Edubase.AcceptanceTests.Establishments;
using Edubase.AcceptanceTests.GiasFrontEnd;
using Edubase.AcceptanceTests.Users;
using Xunit;

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
        private EstablishmentSearchFilter establishmentSearchFilter;
        private User user;
        private string savedSearchToken;
        private EstablishmentSearchResults? searchResults;
        private string selectedSaveFilter;

        public UserPreferencesStepDefinitions(
            IUsers users,
            IGiasFrontEnd giasFrontEnd,
            IEstablishmentSearchResultsScreen establishmentSearchResultsScreen,
            IUserPreferencesInPersistentStorage userPreferencesInPersistentStorage,
            IUserPreferencesInMigrationSource userPreferencesInMigrationSource,
            IUserPreferencesInMigrationTarget userPreferencesInMigrationTarget)
        {
            this.users = users;
            this.giasFrontEnd = giasFrontEnd;
            this.establishmentSearchResultsScreen = establishmentSearchResultsScreen;
            this.userPreferencesInPersistentStorage = userPreferencesInPersistentStorage;
            this.userPreferencesInMigrationSource = userPreferencesInMigrationSource;
            this.userPreferencesInMigrationTarget = userPreferencesInMigrationTarget;
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
            var expectedUserPreferences = await userPreferencesInMigrationSource.GetAllUserPreferences();
            var acutalUserPreferences = await userPreferencesInMigrationTarget.GetAllUserPreferences();

            Assert.Equal(expectedUserPreferences.Count(), acutalUserPreferences.Count());
        }


        [Given("I am a logged-in GIAS user")]
        public async Task GivenIAmALoggedInGIASUser()
        {
            user = users.GetBackOfficeUser();

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
            selectedSaveFilter = savedSearchToken;
        }

        [Then("the saved filters are applied to the list of establishments")]
        public async Task ThenTheSavedFiltersAreAppliedToTheListOfEstablishments()
        {
            var actualSearchResults = await establishmentSearchResultsScreen.GetSearchResults(selectedSaveFilter);

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
            throw new PendingStepException();
        }

        [Then("I can delete my saved filter")]
        public void ThenICanDeleteMySavedFilter()
        {
            throw new PendingStepException();
        }

    }
}
