using System.Net.Http.Json;
using Edubase.AcceptanceTests.Authentication;
using Newtonsoft.Json;
using Reqnroll.CommonModels;
using Xunit;

namespace Edubase.AcceptanceTests.Establishments
{
    public class Establishment
    {
        public string Name { get; set; }
        public int? Urn { get; set; }
        public string TypeName { get; set; }
    }

    [Binding]
    public class EstablishmentsStepDefinitions
    {
        private int urn;
        private Establishment establishment;

        [Given("Establishment with URN {string} exists")]
        public void GivenEstablishmentWithURNExists(int p0)
        {
            urn = p0;
        }

        public class EstablishmentResponse
        {
            public string status;

            public class ReturnValue
            {
                public string name;
                public string typeName;
                public string urn;
            }

            public ReturnValue returnValue;
        }


        [When("Establishment with URN {string} is requested")]
        public async Task WhenEstablishmentWithURNIsRequested(int p0)
        {
            var appSettings = new AppSettings();

            //var client = new AuthenticatedHttpClient(appSettings).AuthenicatedClient();
            var client = new HttpClient();

            var signinSimulator = new LoginSignInSimulator(client, appSettings.UserConfig());

            var response = await client.GetAsync($"https://localhost:44309/api/establishment/{urn}");

            var json = await response.Content.ReadAsStringAsync();

            var establishmentResponse = JsonConvert.DeserializeObject<EstablishmentResponse>(json);

            establishment = new Establishment
            {
                Name = establishmentResponse.returnValue.name,
                TypeName = establishmentResponse.returnValue.typeName,
                Urn = int.Parse(establishmentResponse.returnValue.urn)
            };

            response.EnsureSuccessStatusCode();
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
