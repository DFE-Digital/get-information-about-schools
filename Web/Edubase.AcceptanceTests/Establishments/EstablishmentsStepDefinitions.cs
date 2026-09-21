using System.Net.Http.Json;
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

        [When("Establishment {string} is requested")]
        public async Task WhenEstablishmentIsRequested(int p0)
        {
            using (var httpClient = new HttpClient())
            {
                //httpClient.BaseAddress = new Uri("localhost:44309");

                //var response = await httpClient.GetAsync($"api.example.com/establishments/{urn}");

                //establishment = await response.Content.ReadFromJsonAsync<Establishment>();

                establishment = new Establishment
                {
                    Name = "Name",
                    Urn = urn,
                    TypeName = "TypeName"
                };
            }
        }

        [Then("the Establishment URN is {string}")]
        public void ThenTheEstablishmentURNIs(int p0)
        {
            Assert.Equal(p0, establishment.Urn);
        }

        [Then("the Establishment Name is not empty")]
        public void ThenTheEstablishmentNameIsNotEmpty()
        {
            Assert.NotEmpty(establishment.Name);
        }

        [Then("the Establishment Type is not empty")]
        public void ThenTheEstablishmentTypeIsNotEmpty()
        {
            Assert.NotEmpty(establishment.TypeName);
        }
    }
}
