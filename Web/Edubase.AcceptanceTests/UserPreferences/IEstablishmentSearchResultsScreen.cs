namespace Edubase.AcceptanceTests.UserPreferences
{
    public interface IEstablishmentSearchResultsScreen
    {
        Task<EstablishmentSearchResults> GetSearchResults(EstablishmentSearchFilter filter);
        Task SaveFilterSet(EstablishmentSearchFilter filterSet);
    }
}
