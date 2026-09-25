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
            var establishmentResponse = await api.GetAsync<GetEstablishmentResponse>($"/api/establishment/{urn}");

            return new Establishment
            {
                Name = establishmentResponse.returnValue.name,
                TypeName = establishmentResponse.returnValue.typeName,
                Urn = int.Parse(establishmentResponse.returnValue.urn)
            };
        }
    }
}
