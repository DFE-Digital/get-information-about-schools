using System;
using System.Collections.Generic;
using Edubase.Web.UI.Models;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Edubase.Data.Entity;
using Edubase.Data.Repositories;
using Edubase.Data.Repositories.TableStorage;
using Microsoft.WindowsAzure.Storage.Table;

namespace Edubase.Web.UI.Controllers.Api
{
    public class SqlTokenRepository : ITokenRepository
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

        public async Task CreateAsync(Token token)
        {
            using (var context = new TokensDbContext(new System.Data.SqlClient.SqlConnection(BuildConnectionString())))
            {
                context.Tokens.Add(ToSqlToken(token));
                await context.SaveChangesAsync();
            }
        }

        public async Task<Token> GetAsync(string id)
        {
            SplitId(id, out var partitionKey, out var rowKey);

            using (var context = new TokensDbContext(new System.Data.SqlClient.SqlConnection(BuildConnectionString())))
            {
                var row = await context.Tokens.FindAsync(partitionKey, rowKey);
                return row == null ? null : FromSqlToken(row);
            }
        }

        public Token Get(string id) => GetAsync(id).GetAwaiter().GetResult();

        public async Task UpdateAsync(Token token)
        {
            using (var context = new TokensDbContext(new System.Data.SqlClient.SqlConnection(BuildConnectionString())))
            {
                var row = ToSqlToken(token);
                context.Tokens.Attach(row);
                context.Entry(row).State = EntityState.Modified;
                await context.SaveChangesAsync();
            }
        }

        public async Task DeleteAsync(string id)
        {
            SplitId(id, out var partitionKey, out var rowKey);

            using (var context = new TokensDbContext(new System.Data.SqlClient.SqlConnection(BuildConnectionString())))
            {
                var row = await context.Tokens.FindAsync(partitionKey, rowKey);
                if (row != null)
                {
                    context.Tokens.Remove(row);
                    await context.SaveChangesAsync();
                }
            }
        }

        public static void SplitId(string id, out string partitionKey, out string rowKey)
        {
            if (string.IsNullOrEmpty(id) || id.Length < 5)
            {
                throw new ArgumentException("Id is not valid", nameof(id));
            }

            partitionKey = id.Substring(0, 4);
            rowKey = id.Substring(4);
        }

        public static SqlToken ToSqlToken(Token token) => new SqlToken
        {
            PartitionKey = token.PartitionKey ?? string.Empty, RowKey = token.RowKey, Data = token.Data
        };

        public static Token FromSqlToken(SqlToken row) => new Token
        {
            PartitionKey = row.PartitionKey,
            RowKey = row.RowKey,
            Data = row.Data
        };
    }
}
