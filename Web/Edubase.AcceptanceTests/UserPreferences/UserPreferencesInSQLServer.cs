using Edubase.AcceptanceTests.Repositories;
using Edubase.AcceptanceTests.Users;
using Microsoft.EntityFrameworkCore;

namespace Edubase.AcceptanceTests.UserPreferences
{
    public sealed class UserPreferencesInSqlServer : IUserPreferencesInMigrationTarget
    {
        private readonly AcceptanceTestsDbContext dbContext;

        public UserPreferencesInSqlServer(AcceptanceTestsDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<IEnumerable<UserPreference>> GetAllUserPreferences()
        {
            var userPreferences = await dbContext.UserPreferences
                .AsNoTracking()
                .ToListAsync();

            return userPreferences.Select(preference => new UserPreference
            {
                RowKey = preference.RowKey,
                SavedSearchToken = preference.SavedSearchToken
            });
        }

        public async Task<string> GetSavedSearchTokenForUser(User user)
        {
            ArgumentNullException.ThrowIfNull(user);

            var userId = user.AttributeStatementValue;

            if (string.IsNullOrWhiteSpace(userId))
            {
                throw new ArgumentException("The user does not have a persisted user ID.", nameof(user));
            }

            var preference = await dbContext.UserPreferences
                .AsNoTracking()
                .SingleOrDefaultAsync(preference => preference.RowKey == userId);

            return preference.SavedSearchToken;
        }
    }
}
