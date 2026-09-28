using Edubase.AcceptanceTests.GiasFrontEnd;
using Newtonsoft.Json;

namespace Edubase.AcceptanceTests.Establishments
{
    internal class EstablishmentsFromGiasFrontEnd : IEstablishments
    {
        private readonly IGiasFrontEnd api;

        public EstablishmentsFromGiasFrontEnd(IGiasFrontEnd api)
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
