using Edubase.AcceptanceTests.Users;

namespace Edubase.AcceptanceTests.UserPreferences
{
    public interface IUserPreferencesInPersitentStorage
    {
        Task<EstablishmentSearchFilter> GetEstablishmentSearchFilterForUser(User user);
    }
}
