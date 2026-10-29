using Edubase.AcceptanceTests.GiasFrontEnd;
using Edubase.AcceptanceTests.Users;

namespace Edubase.AcceptanceTests.UserPreferences
{
    [Binding]
    internal class UserPreferencesStepDefinitions
    {
        private readonly IUsers users;
        private readonly IGiasFrontEnd giasFrontEnd;
        private readonly IEstablishmentSearchResultsScreen establishmentSearchResultsScreen;
        private readonly IUserPreferencesInPersitentStorage userPreferencesInPersistentStorage;
        private EstablishmentSearchFilter establishmentSearchFilter;
        private User user;

        public UserPreferencesStepDefinitions(
            IUsers users,
            IGiasFrontEnd giasFrontEnd,
            IEstablishmentSearchResultsScreen establishmentSearchResultsScreen,
            IUserPreferencesInPersitentStorage userPreferencesInPersistentStorage)
        {
            this.users = users;
            this.giasFrontEnd = giasFrontEnd;
            this.establishmentSearchResultsScreen = establishmentSearchResultsScreen;
            this.userPreferencesInPersistentStorage = userPreferencesInPersistentStorage;
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

            var results = await  establishmentSearchResultsScreen.GetSearchResults(establishmentSearchFilter);
        }

        [Then("I can save the filter set to create a saved filter")]
        public async Task ThenICanSaveTheFilterSetToCreateASavedFilter()
        {
            await establishmentSearchResultsScreen.SaveFilterSet(establishmentSearchFilter);

            var actualEstablishmentSearchFilter = await userPreferencesInPersistentStorage.GetEstablishmentSearchFilterForUser(user);
        }

        [Given("I have a saved filter")]
        public void GivenIHaveASavedFilter()
        {
            throw new PendingStepException();
        }

        [When("I select My saved filter set on the establishment search results screen")]
        public void WhenISelectMySavedFilterSetOnTheEstablishmentSearchResultsScreen()
        {
            throw new PendingStepException();
        }

        [Then("the saved filters are applied to the list of establishments")]
        public void ThenTheSavedFiltersAreAppliedToTheListOfEstablishments()
        {
            throw new PendingStepException();
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
