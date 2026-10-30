using Edubase.Web.UI.Models;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Threading.Tasks;

namespace Edubase.Web.UI.Controllers.Api
{
    public class SqlGlossaryItemRepository : ISqlGlossaryItemRepository
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

        public async Task<SqlGlossaryItem> GetAsync(string partitionKey, string rowKey)
        {
            using (var context = new GlossaryItemsDbContext(new SqlConnection(BuildConnectionString())))
            {
                return await context.GlossaryItems.FindAsync(partitionKey, rowKey);
            }
        }

        public SqlGlossaryItem Get(string partitionKey, string rowKey) => GetAsync(partitionKey, rowKey).GetAwaiter().GetResult();

        public async Task UpsertAsync(SqlGlossaryItem item)
        {
            using (var context = new GlossaryItemsDbContext(new SqlConnection(BuildConnectionString())))
            {
                if (string.IsNullOrWhiteSpace(item.PartitionKey))
                {
                    item.PartitionKey = string.Empty;
                }

                var existing = await context.GlossaryItems.FindAsync(item.PartitionKey, item.RowKey);
                if (existing == null)
                {
                    context.GlossaryItems.Add(item);
                }
                else
                {
                    context.Entry(existing).CurrentValues.SetValues(item);
                }
                await context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<SqlGlossaryItem>> GetAllAsync()
        {
            using (var context = new GlossaryItemsDbContext(new SqlConnection(BuildConnectionString())))
            {
                return await context.GlossaryItems.ToListAsync();
            }
        }
    }
}
