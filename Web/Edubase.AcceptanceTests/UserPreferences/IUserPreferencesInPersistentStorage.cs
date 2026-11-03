using Edubase.AcceptanceTests.Users;

namespace Edubase.AcceptanceTests.UserPreferences
{
    public interface IUserPreferencesInPersistentStorage
    {
        Task<string> GetSavedSearchTokenForUser(User user);
    }
}
