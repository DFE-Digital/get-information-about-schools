using Edubase.AcceptanceTests.Api;
using Edubase.AcceptanceTests.SigninAuthorities;
using Edubase.AcceptanceTests.Users;
using Xunit;

namespace Edubase.AcceptanceTests.Establishments
{

    [Binding]
    public partial class EstablishmentsStepDefinitions
    {
        private readonly IUsers users;
        private readonly IApiClient apiClient;
        private readonly ISignInAuthority signinAuthority;
        private IEstablishments establishments;
        private Establishment establishment;

        public EstablishmentsStepDefinitions(
            IUsers users,
            IApiClient apiClient,
            ISignInAuthority signinAuthority,
            IEstablishments establishments)
        {
            this.users = users;
            this.apiClient = apiClient;
            this.signinAuthority = signinAuthority;
            this.establishments = establishments;
        }

        [Given("a back office user is signed in")]
        public async Task GivenTheUserIsABackOfficeUser()
        {
            var user = users.GetBackOfficeUser();

            await signinAuthority.SignIn(user);
        }

        [Given("an Establishment with URN {string} exists")]
        public void GivenEstablishmentWithURNExists(int p0)
        {
        }

        [When("the user requests the Establishment with URN {string}")]
        public async Task WhenEstablishmentWithURNIsRequested(int p0)
        {
            establishment = await establishments.GetEstablishment(p0);
        }

        [Then("the Establishment URN is {string}")]
        public void ThenTheEstablishmentURNIs(int p0)
        {
            Assert.Equal(p0, establishment.Urn);
        }

        [Then("the Establishment Name is {string}")]
        public void ThenTheEstablishmentNameIsNotEmpty(string p0)
        {
            Assert.Equal(p0, establishment.Name);
        }

        [Then("the Establishment Type is {string}")]
        public void ThenTheEstablishmentTypeIsNotEmpty(string p0)
        {
            Assert.Equal(p0, establishment.TypeName);
        }
    }
}
