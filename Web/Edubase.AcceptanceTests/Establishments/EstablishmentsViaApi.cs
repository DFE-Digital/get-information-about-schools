using Edubase.AcceptanceTests.Api;
using Newtonsoft.Json;

namespace Edubase.AcceptanceTests.Establishments
{
    internal class EstablishmentsViaApi : IEstablishments
    {
        private readonly ApiClient api;

        public EstablishmentsViaApi(ApiClient api)
        {
            this.api = api;
        }

        public async Task<Establishment> GetEstablishment(int urn)
        {
            var response = await api.HttpClient.GetAsync($"{api.HttpClient.BaseAddress}/api/establishment/{urn}");

            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();

            var establishmentResponse = JsonConvert.DeserializeObject<EstablishmentResponse>(json);

            return new Establishment
            {
                Name = establishmentResponse.returnValue.name,
                TypeName = establishmentResponse.returnValue.typeName,
                Urn = int.Parse(establishmentResponse.returnValue.urn)
            };
        }
    }
}
