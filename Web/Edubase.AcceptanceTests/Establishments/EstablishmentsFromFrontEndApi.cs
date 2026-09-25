using Edubase.AcceptanceTests.Api;
using Newtonsoft.Json;

namespace Edubase.AcceptanceTests.Establishments
{
    internal class EstablishmentsFromFrontEndApi : IEstablishments
    {
        private readonly IApiClient api;

        public EstablishmentsFromFrontEndApi(IApiClient api)
        {
            this.api = api;
        }

        public async Task<Establishment> GetEstablishment(int urn)
        {
            var response = await api.GetAsync($"/api/establishment/{urn}");

            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();

            var establishmentResponse = JsonConvert.DeserializeObject<GetEstablishmentResponse>(json);

            return new Establishment
            {
                Name = establishmentResponse.returnValue.name,
                TypeName = establishmentResponse.returnValue.typeName,
                Urn = int.Parse(establishmentResponse.returnValue.urn)
            };
        }
    }
}
