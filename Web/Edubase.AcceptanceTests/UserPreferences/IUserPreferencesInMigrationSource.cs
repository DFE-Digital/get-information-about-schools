using Edubase.AcceptanceTests.Users;

namespace Edubase.AcceptanceTests.UserPreferences
{
    public interface IUserPreferencesInMigrationSource : IUserPreferencesInPersistentStorage
    {
        Task<string> GetSavedSearchTokenForUser(User user);
    }
}
