using Edubase.AcceptanceTests.Apis;
using Edubase.AcceptanceTests.Establishments;
using Edubase.AcceptanceTests.Users;

namespace Edubase.AcceptanceTests.GiasFrontEnd
{
    public interface IGiasFrontEnd : IApi
    {
        Task<EstablishmentSearchResults> GetEstablishmentSearchResults(string token);
        Task<EstablishmentSearchResults> GetEstablishmentSearchResults(EstablishmentSearchFilter filter);
        Task SaveEstablishmentSearchFilter(string token);
        Task SignIn(User user);
        Task<string> TokenizeEstablishmentSearchFilter(EstablishmentSearchFilter establishmentSearchFilter);
    }
}
