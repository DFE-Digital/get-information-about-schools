using Edubase.AcceptanceTests.GiasFrontEnd;

namespace Edubase.AcceptanceTests.UserPreferences
{
    public class EstablishmentSearchResultsScreenFromGiasFrontEnd : IEstablishmentSearchResultsScreen
    {
        private readonly IGiasFrontEnd giasFrontEnd;

        public EstablishmentSearchResultsScreenFromGiasFrontEnd(IGiasFrontEnd giasFrontEnd)
        {
            this.giasFrontEnd = giasFrontEnd;
        }
        
        public async Task<EstablishmentSearchResults> GetSearchResults(EstablishmentSearchFilter filter)
        {
            return await Task.FromResult(new EstablishmentSearchResults());
        }

        public async Task<string> SaveFilterSet(EstablishmentSearchFilter establishmentSearchFilter)
        {
            var token = await giasFrontEnd.TokenizeEstablishmentSearchFilter(establishmentSearchFilter);

            await giasFrontEnd.SaveEstablishmentSearchFilter(token);

            return token;
        }   
    }
}
