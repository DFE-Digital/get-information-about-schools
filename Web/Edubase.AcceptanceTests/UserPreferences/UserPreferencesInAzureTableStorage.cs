using Azure.Data.Tables;
using Edubase.AcceptanceTests.DependencyInjection;
using Edubase.AcceptanceTests.Users;

namespace Edubase.AcceptanceTests.UserPreferences
{
    internal class UserPreferencesInAzureTableStorage : IUserPreferencesInMigrationSource
    {
        private readonly TableClient tableClient;

        public UserPreferencesInAzureTableStorage(string connectionString)
        {
            tableClient = new TableClient(connectionString, "UserPreferences");
        }

        public async Task<IEnumerable<UserPreference>> GetAllUserPreferences()
        {
            var pages = tableClient.QueryAsync<TableEntity>().AsPages();

            var userPreferences = pages.SelectMany(page => page.Values)
                .Select(entity => new UserPreference
                {
                    RowKey = entity.RowKey,
                    SavedSearchToken = entity.GetString("SavedSearchToken")
                });

            return await userPreferences.ToListAsync();
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
