using Edubase.Web.UI.Models;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Edubase.Web.UI.Controllers.Api
{
    public class SqlUserPreferenceRepository : ISqlUserPreferenceRepository
    {
        private static string BuildConnectionString()
        {
            var serverName = ConfigurationManager.AppSettings["SQLServer"];
            var databaseName = ConfigurationManager.AppSettings["SQLDatabase"];
            return
                $"Server=tcp:{serverName},1433;" +
                $"database={databaseName};" +
                "authentication=Active Directory Default;" +
                "encrypt=True;TrustServerCertificate=False;";
        }

        public async Task<SqlUserPreference> GetAsync(string paritionKey,  string rowKey)
        {
            using (var context = new UserPreferencesDbContext(new SqlConnection(BuildConnectionString())))
            {
                return await context.UserPreferences.FindAsync(paritionKey, rowKey);
            }
        }

        public SqlUserPreference Get(string paritionKey,  string rowKey) => GetAsync(paritionKey, rowKey).GetAwaiter().GetResult();

        public async Task UpsertAsync(SqlUserPreference item)
        {
            using (var context = new UserPreferencesDbContext(new SqlConnection(BuildConnectionString())))
            {
                if (string.IsNullOrWhiteSpace(item.PartitionKey))
                {
                    item.PartitionKey = string.Empty;
                }

                var existing = await context.UserPreferences.FindAsync(item.PartitionKey, item.RowKey);
                if (existing == null)
                {
                    context.UserPreferences.Add(item);
                }
                else
                {
                    context.Entry(existing).CurrentValues.SetValues(item);
                }
                await context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<SqlUserPreference>> GetAllAsync()
        {
            using (var context = new UserPreferencesDbContext(new SqlConnection(BuildConnectionString())))
            {
                return await context.UserPreferences.ToListAsync();
            }
        }
    }
}
