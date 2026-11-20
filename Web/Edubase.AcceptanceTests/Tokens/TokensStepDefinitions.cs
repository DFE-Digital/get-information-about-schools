using System;
using Edubase.AcceptanceTests.Establishments;
using Edubase.AcceptanceTests.GiasFrontEnd;
using Edubase.AcceptanceTests.UserPreferences;
using Edubase.AcceptanceTests.Users;
using Reqnroll;
using Xunit;

namespace Edubase.AcceptanceTests
{
    [Binding]
    public class TokensStepDefinitions
    {
        private readonly IUsers users;
        private readonly IGiasFrontEnd giasFrontEnd;
        private readonly IEstablishmentSearchResultsScreen establishmentSearchResultsScreen;
        private User user;
        private EstablishmentSearchFilter establishmentSearchFilter;
        private EstablishmentSearchResults searchResults;
        private string savedSearchToken;
        private readonly IUserPreferencesInPersistentStorage userPreferencesInPersistentStorage;

        public TokensStepDefinitions(
            IUsers users,
            IGiasFrontEnd giasFrontEnd,
            IEstablishmentSearchResultsScreen establishmentSearchResultsScreen,
            IUserPreferencesInPersistentStorage userPreferencesInPersistentStorage)
        {
            this.users = users;
            this.giasFrontEnd = giasFrontEnd;
            this.establishmentSearchResultsScreen = establishmentSearchResultsScreen;
            this.userPreferencesInPersistentStorage = userPreferencesInPersistentStorage;
        }

        [Given("that I am a logged-in GIAS user")]
        public async Task GivenThatIAmALogged_InGIASUser()
        {
            user = await users.GetBackOfficeUser();

            await giasFrontEnd.SignIn(user);
        }

        [Given("I have applied one or more filters within GIAS")]
        public async Task GivenIHaveAppliedOneOrMoreFiltersWithinGIAS()
        {
            establishmentSearchFilter = new EstablishmentSearchFilter { };

            searchResults = await establishmentSearchResultsScreen.GetSearchResults(establishmentSearchFilter);
        }

        [When("I save the selected filters as a filter set")]
        public async Task WhenISaveTheSelectedFiltersAsAFilterSet()
        {
            savedSearchToken = await establishmentSearchResultsScreen.SaveFilterSet(establishmentSearchFilter);
        }

        [When("I log out of GIAS")]
        public async Task WhenILogOutOfGIAS()
        {
            await giasFrontEnd.SignOut(user);
        }

        [When("I subsequently log back into GIAS using the same user account")]
        public async Task WhenISubsequentlyLogBackIntoGIASUsingTheSameUserAccount()
        {
            await giasFrontEnd.SignIn(user);
        }

        [Then("the system should remember my previously saved filter set")]
        public async Task ThenTheSystemShouldRememberMyPreviouslySavedFilterSet()
        {
            var persistedSearchToken = await userPreferencesInPersistentStorage.GetSavedSearchTokenForUser(user);

            Assert.Equal(savedSearchToken, persistedSearchToken);
        }

        [Then("my saved filter set should be available for me to select and apply")]
        public async Task ThenMySavedFilterSetShouldBeAvailableForMeToSelectAndApply()
        {
            await Task.Delay(0);
        }

        [Then("the saved filter set should retain the filter criteria I previously selected")]
        public async Task ThenTheSavedFilterSetShouldRetainTheFilterCriteriaIPreviouslySelected()
        {
            await Task.Delay(0);
        }

        [Then("applying the saved filter set should return the same filtering results.")]
        public async Task ThenApplyingTheSavedFilterSetShouldReturnTheSameFilteringResults_()
        {
            await Task.Delay(0);
        }

    }
}
