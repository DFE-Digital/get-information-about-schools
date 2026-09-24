using Edubase.AcceptanceTests.Api;
using Edubase.AcceptanceTests.Users;
using Xunit;

namespace Edubase.AcceptanceTests.Establishments
{

    [Binding]
    public partial class EstablishmentsStepDefinitions
    {
        private string baseAddress = "https://localhost:44309";
        private string environment = "dev";
        private IUsers users = new HardCodedUsers();
        private int urn;
        private IEstablishments establishments;
        private Establishment establishment;

        [Given("a back office user is signed in")]
        public async Task GivenTheUserIsABackOfficeUser()
        {
            var user = users.GetBackOfficeUser();

            var apiClient = new ApiClient(baseAddress, environment);

            await apiClient.Signin(user);

            establishments = new EstablishmentsViaApi(apiClient);
        }

        [Given("an Establishment with URN {string} exists")]
        public void GivenEstablishmentWithURNExists(int p0)
        {
            urn = p0;
        }

        [When("the user requests the Establishment with URN {string}")]
        public async Task WhenEstablishmentWithURNIsRequested(int p0)
        {
            establishment = await establishments.GetEstablishment(urn);
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
