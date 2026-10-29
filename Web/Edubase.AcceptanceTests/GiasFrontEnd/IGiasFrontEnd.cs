using Edubase.AcceptanceTests.Apis;
using Edubase.AcceptanceTests.UserPreferences;
using Edubase.AcceptanceTests.Users;

namespace Edubase.AcceptanceTests.GiasFrontEnd
{
    public interface IGiasFrontEnd : IApi
    {
        Task SaveEstablishmentSearchFilter(string tokens);
        Task SignIn(User user);
        Task<string> TokenizeEstablishmentSearchFilter(EstablishmentSearchFilter establishmentSearchFilter);
    }
}
