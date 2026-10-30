using System.Xml.Linq;
using Edubase.AcceptanceTests.Users;
using Microsoft.EntityFrameworkCore;

namespace Edubase.AcceptanceTests.UserPreferences
{
    public sealed class UserPreferencesInSQLServer : IUserPreferencesInPersistentStorage
    {
        private readonly UserPreferencesDbContext dbContext;

        public UserPreferencesInSQLServer(UserPreferencesDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<string?> GetSavedSearchTokenForUser(User user)
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

            return preference?.SavedSearchToken;
        }

        public static string LoadFrontEndDatabaseConnectionString()
        {
            var directory = new DirectoryInfo(AppContext.BaseDirectory);
            while (directory != null)
            {
                var configPath = Path.Combine(directory.FullName, "devsecrets.gias.config.alwaysignore");
                if (File.Exists(configPath))
                {
                    return ReadFrontEndDatabaseConnectionString(configPath);
                }

                directory = directory.Parent;
            }

            throw new FileNotFoundException(
                "The Web App local config file was not found. Configure devsecrets.gias.config.alwaysignore with the FrontEndDatabase connection string.");
        }

        private static string ReadFrontEndDatabaseConnectionString(string configPath)
        {
            var connectionString = XDocument.Load(configPath)
                .Root?
                .Element("connectionStrings")?
                .Elements("add")
                .SingleOrDefault(element => (string?)element.Attribute("name") == "FrontEndDatabase")?
                .Attribute("connectionString")?
                .Value;

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException(
                    $"The FrontEndDatabase connection string was not found in '{configPath}'.");
            }

            return connectionString;
        }
    }
}
