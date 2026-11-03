namespace Edubase.AcceptanceTests.Establishments
{
    public interface IEstablishmentSearchResultsScreen
    {
        Task<EstablishmentSearchResults> GetSearchResults(string token);
        Task<EstablishmentSearchResults> GetSearchResults(EstablishmentSearchFilter filter);
        Task<string> SaveFilterSet(EstablishmentSearchFilter filterSet);
    }
}
