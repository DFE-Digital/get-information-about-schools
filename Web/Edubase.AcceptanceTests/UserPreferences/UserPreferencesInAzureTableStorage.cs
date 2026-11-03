using Edubase.AcceptanceTests.Users;

namespace Edubase.AcceptanceTests.UserPreferences
{
    internal class UserPreferencesInAzureTableStorage : IUserPreferencesInPersistentStorage
    {
        public Task<string> GetSavedSearchTokenForUser(User user)
        {
            throw new NotImplementedException();
        }
    }
}
