using Edubase.AcceptanceTests.GiasFrontEnd;

namespace Edubase.AcceptanceTests.Establishments
{
    internal class EstablishmentsFromGiasFrontEnd : IEstablishments
    {
        private readonly IGiasFrontEnd gias;

        public EstablishmentsFromGiasFrontEnd(IGiasFrontEnd gias)
        {
            this.gias = gias;
        }

        public async Task<Establishment> GetEstablishment(int urn)
        {
            var establishmentResponse = await gias.GetAsync<GetEstablishmentResponse>($"/api/establishment/{urn}");

            return new Establishment
            {
                Name = establishmentResponse.returnValue.name,
                TypeName = establishmentResponse.returnValue.typeName,
                Urn = int.Parse(establishmentResponse.returnValue.urn)
            };
        }
    }
}
