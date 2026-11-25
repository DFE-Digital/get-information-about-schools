using Edubase.Common.Cache;
using Edubase.Web.UI.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Edubase.Web.UI.Controllers.Api
{
    public class SqlLaNameCodeRepository : ISqlLaNameCodeRepository
    {
        private readonly ICacheAccessor _cacheAccessor;
        private readonly IConfiguration _configuration;

        public SqlLaNameCodeRepository(ICacheAccessor cacheAccessor, IConfiguration configuration)
        {
            _cacheAccessor = cacheAccessor;
            _configuration = configuration;
        }

        public async Task<IEnumerable<SqlLaNameCode>> GetAllAsync()
        {
            return await _cacheAccessor.AutoAsync(
                async () => await QueryAllAsync(),
                string.Empty,
                GetType().Name);
        }

        public async Task<IEnumerable<SqlLaNameCode>> GetByGroupAsync(string groupCode)
        {
            return await _cacheAccessor.AutoAsync(
                async () => await QueryByGroupAsync(groupCode),
                groupCode,
                GetType().Name);
        }

        private async Task<List<SqlLaNameCode>> QueryAllAsync()
        {
            using (var context = new LaNameCodesDbContext(new Microsoft.Data.SqlClient.SqlConnection(BuildConnectionString(_configuration))))
            {
                return await context.LaNameCodes
                    .AsNoTracking()
                    .OrderBy(x => x.GroupCode)
                    .ThenBy(x => x.LaName)
                    .ToListAsync();
            }
        }

        private async Task<List<SqlLaNameCode>> QueryByGroupAsync(string groupCode)
        {
            using (var context = new LaNameCodesDbContext(new Microsoft.Data.SqlClient.SqlConnection(BuildConnectionString(_configuration))))
            {
                return await context.LaNameCodes
                    .AsNoTracking()
                    .Where(x => x.GroupCode == groupCode)
                    .OrderBy(x => x.LaName)
                    .ToListAsync();
            }
        }

        internal static string BuildConnectionString(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("FrontEndDatabase");
            if (!string.IsNullOrWhiteSpace(connectionString))
            {
                return connectionString;
            }

            if (string.Equals(configuration["Environment"], "localdev", StringComparison.OrdinalIgnoreCase))
            {
                connectionString = configuration["FrontEndDatabase"];
                if (string.IsNullOrWhiteSpace(connectionString))
                {
                    throw new InvalidOperationException("Configure ConnectionStrings:FrontEndDatabase for local SQL access.");
                }

                return connectionString;
            }

            var serverName = configuration["SQLServer"];
            var databaseName = configuration["SQLDatabase"];
            if (string.IsNullOrWhiteSpace(serverName) || string.IsNullOrWhiteSpace(databaseName))
            {
                throw new InvalidOperationException("Configure ConnectionStrings:FrontEndDatabase or both SQLServer and SQLDatabase for SQL access.");
            }

            return
                $"Server=tcp:{serverName},1433;" +
                $"database={databaseName};" +
                "authentication=Active Directory Default;" +
                "encrypt=True;TrustServerCertificate=False;";
        }
    }
}
