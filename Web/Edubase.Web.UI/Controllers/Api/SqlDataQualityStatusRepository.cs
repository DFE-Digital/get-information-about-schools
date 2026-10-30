using Edubase.Web.UI.Models;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Threading.Tasks;

namespace Edubase.Web.UI.Controllers.Api
{
    public class SqlDataQualityStatusRepository : ISqlDataQualityStatusRepository
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

        public async Task<SqlDataQualityStatus> GetAsync(string partitionKey, string rowKey)
        {
            using (var context = new DataQualityStatusDbContext(new SqlConnection(BuildConnectionString())))
            {
                return await context.DataQualityStatus.FindAsync(partitionKey, rowKey);
            }
        }

        public SqlDataQualityStatus Get(string partitionKey, string rowKey) => GetAsync(partitionKey, rowKey).GetAwaiter().GetResult();

        public async Task UpsertAsync(SqlDataQualityStatus item)
        {
            using (var context = new DataQualityStatusDbContext(new SqlConnection(BuildConnectionString())))
            {
                if (string.IsNullOrWhiteSpace(item.PartitionKey))
                {
                    item.PartitionKey = string.Empty;
                }

                var existing = await context.DataQualityStatus.FindAsync(item.PartitionKey, item.RowKey);
                if (existing == null)
                {
                    context.DataQualityStatus.Add(item);
                }
                else
                {
                    context.Entry(existing).CurrentValues.SetValues(item);
                }
                await context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<SqlDataQualityStatus>> GetAllAsync()
        {
            using (var context = new DataQualityStatusDbContext(new SqlConnection(BuildConnectionString())))
            {
                return await context.DataQualityStatus.ToListAsync();
            }
        }
    }
}
