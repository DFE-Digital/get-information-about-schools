using Edubase.AcceptanceTests.Users;

namespace Edubase.AcceptanceTests.UserPreferences
{
    public interface IUserPreferencesInMigrationTarget : IUserPreferencesInPersistentStorage
    {
        Task<string> GetSavedSearchTokenForUser(User user);
    }
}
