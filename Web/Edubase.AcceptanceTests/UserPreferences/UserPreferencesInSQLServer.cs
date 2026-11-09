using Edubase.AcceptanceTests.Users;
using Microsoft.EntityFrameworkCore;

namespace Edubase.AcceptanceTests.UserPreferences
{
    public sealed class UserPreferencesInSQLServer : IUserPreferencesInMigrationTarget
    {
        private readonly UserPreferencesDbContext dbContext;

        public UserPreferencesInSQLServer(UserPreferencesDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<IEnumerable<UserPreference>> GetAllUserPreferences()
        {
            return await dbContext.UserPreferences
                .AsNoTracking()
                .ToListAsync();
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
