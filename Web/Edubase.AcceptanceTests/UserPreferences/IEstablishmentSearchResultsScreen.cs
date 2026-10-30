namespace Edubase.AcceptanceTests.UserPreferences
{
    public interface IEstablishmentSearchResultsScreen
    {
        Task<EstablishmentSearchResults> GetSearchResults(EstablishmentSearchFilter filter);
        Task<string> SaveFilterSet(EstablishmentSearchFilter filterSet);
    }
}
