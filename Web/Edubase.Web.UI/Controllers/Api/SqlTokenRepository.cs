using Edubase.Web.UI.Models;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Threading.Tasks;

namespace Edubase.Web.UI.Controllers.Api
{
    public class SqlTokenRepository : ISqlTokenRepository
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

        public async Task<SqlToken> GetAsync(string partitionKey, string rowKey)
        {
            using (var context = new TokensDbContext(new SqlConnection(BuildConnectionString())))
            {
                return await context.Tokens.FindAsync(partitionKey, rowKey);
            }
        }

        public SqlToken Get(string partitionKey, string rowKey) => GetAsync(partitionKey, rowKey).GetAwaiter().GetResult();

        public async Task UpsertAsync(SqlToken item)
        {
            using (var context = new TokensDbContext(new SqlConnection(BuildConnectionString())))
            {
                if (string.IsNullOrWhiteSpace(item.PartitionKey))
                {
                    item.PartitionKey = string.Empty;
                }

                var existing = await context.Tokens.FindAsync(item.PartitionKey, item.RowKey);
                if (existing == null)
                {
                    context.Tokens.Add(item);
                }
                else
                {
                    context.Entry(existing).CurrentValues.SetValues(item);
                }
                await context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<SqlToken>> GetAllAsync()
        {
            using (var context = new TokensDbContext(new SqlConnection(BuildConnectionString())))
            {
                return await context.Tokens.ToListAsync();
            }
        }
    }
}
