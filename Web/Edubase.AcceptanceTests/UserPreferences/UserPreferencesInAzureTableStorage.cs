using Azure.Data.Tables;
using Edubase.AcceptanceTests.DependencyInjection;
using Edubase.AcceptanceTests.Users;

namespace Edubase.AcceptanceTests.UserPreferences
{
    internal class UserPreferencesInAzureTableStorage : IUserPreferencesInPersistentStorage
    {
        private readonly TableClient tableClient;

        public UserPreferencesInAzureTableStorage(Settings settings)
        {
            ArgumentNullException.ThrowIfNull(settings);

            tableClient = new TableClient(settings.DataConnectionString, "UserPreferences");
        }

        public async Task<string> GetSavedSearchTokenForUser(User user)
        {
            ArgumentNullException.ThrowIfNull(user);

            var userId = user.AttributeStatementValue;

            if (string.IsNullOrWhiteSpace(userId))
            {
                throw new ArgumentException("The user does not have a persisted user ID.", nameof(user));
            }

            var preference = await tableClient.GetEntityAsync<TableEntity>(string.Empty, userId);

            return preference.Value.GetString("SavedSearchToken");
        }
    }
}
