using Edubase.Web.UI.Models;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Threading.Tasks;

namespace Edubase.Web.UI.Controllers.Api
{
    public class SqlNotificationTemplateRepository : ISqlNotificationTemplateRepository
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

        public async Task<SqlNotificationTemplate> GetAsync(string partitionKey,  string rowKey)
        {
            using (var context = new NotificationTemplatesDbContext(new SqlConnection(BuildConnectionString())))
            {
                return await context.NotificationTemplates.FindAsync(partitionKey, rowKey);
            }
        }

        public SqlNotificationTemplate Get(string partitionKey,  string rowKey) => GetAsync(partitionKey, rowKey).GetAwaiter().GetResult();

        public async Task UpsertAsync(SqlNotificationTemplate item)
        {
            using (var context = new NotificationTemplatesDbContext(new SqlConnection(BuildConnectionString())))
            {
                if (string.IsNullOrWhiteSpace(item.PartitionKey))
                {
                    item.PartitionKey = string.Empty;
                }

                var existing = await context.NotificationTemplates.FindAsync(item.PartitionKey, item.RowKey);
                if (existing == null)
                {
                    context.NotificationTemplates.Add(item);
                }
                else
                {
                    context.Entry(existing).CurrentValues.SetValues(item);
                }
                await context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<SqlNotificationTemplate>> GetAllAsync()
        {
            using (var context = new NotificationTemplatesDbContext(new SqlConnection(BuildConnectionString())))
            {
                return await context.NotificationTemplates.ToListAsync();
            }
        }
    }
}
