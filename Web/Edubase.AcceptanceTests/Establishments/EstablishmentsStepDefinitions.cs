using Edubase.AcceptanceTests.GiasFrontEnd;
using Edubase.AcceptanceTests.Users;
using Xunit;

namespace Edubase.AcceptanceTests.Establishments
{

    [Binding]
    public partial class EstablishmentsStepDefinitions
    {
        private readonly IUsers users;
        private readonly IGiasFrontEnd giasFrontEnd;
        private readonly IEstablishments establishments;
        private Establishment establishment;
        private string errorMessage;
        private int urn;

        public EstablishmentsStepDefinitions(
            IUsers users,
            IGiasFrontEnd giasFrontEnd,
            IEstablishments establishments)
        {
            this.users = users;
            this.giasFrontEnd = giasFrontEnd;
            this.establishments = establishments;
        }

        [Given("a user is not signed in")]
        public void GivenAUserIsNotSignedIn()
        {
        }

        [Given("a back office user is signed in")]
        public async Task GivenTheUserIsABackOfficeUser()
        {
            var user = users.GetBackOfficeUser();

            await giasFrontEnd.SignIn(user);
        }

        [Given("an Establishment with URN {string} exists")]
        public void GivenEstablishmentWithURNExists(int p0)
        {
            urn = p0;
        }

        [When("the user requests the Establishment with URN {string}")]
        public async Task WhenEstablishmentWithURNIsRequested(int p0)
        {
            try
            {
                establishment = await establishments.GetEstablishment(p0);
            }
            catch (Exception e)
            {
                errorMessage = $"An error occurred. Failed to retrieve establishment with URN {p0}. {e.Message}";
            }
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

        [Then("an error occurs")]
        public void ThenAnErrorOccurs()
        {
            Assert.Contains("An error occurred.", errorMessage);
        }

        [Then("the Establishment is not found")]
        public void ThenTheEstablishmentIsNotFound()
        {
            Assert.Contains("Not Found", errorMessage);
        }
    }
}
