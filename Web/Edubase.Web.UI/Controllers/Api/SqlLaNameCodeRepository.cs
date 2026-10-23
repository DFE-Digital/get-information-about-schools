using Edubase.Common.Cache;
using Edubase.Web.UI.Models;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Edubase.Web.UI.Controllers.Api
{
    public class SqlLaNameCodeRepository : ISqlLaNameCodeRepository
    {
        private readonly ICacheAccessor _cacheAccessor;

        public SqlLaNameCodeRepository(ICacheAccessor cacheAccessor)
        {
            _cacheAccessor = cacheAccessor;
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

        private static async Task<List<SqlLaNameCode>> QueryAllAsync()
        {
            using (var context = new LaNameCodesDbContext(new SqlConnection(BuildConnectionString(ConfigurationManager.ConnectionStrings))))
            {
                return await context.LaNameCodes
                    .AsNoTracking()
                    .OrderBy(x => x.GroupCode)
                    .ThenBy(x => x.LaName)
                    .ToListAsync();
            }
        }

        private static async Task<List<SqlLaNameCode>> QueryByGroupAsync(string groupCode)
        {
            using (var context = new LaNameCodesDbContext(new SqlConnection(BuildConnectionString(ConfigurationManager.ConnectionStrings))))
            {
                return await context.LaNameCodes
                    .AsNoTracking()
                    .Where(x => x.GroupCode == groupCode)
                    .OrderBy(x => x.LaName)
                    .ToListAsync();
            }
        }

        internal static string BuildConnectionString(ConnectionStringSettingsCollection connectionStrings)
        {
            // LocalSetup supplies Windows-authenticated local SQL; hosted runs use Azure authentication.
            if (string.Equals(ConfigurationManager.AppSettings["Environment"], "localdev", System.StringComparison.OrdinalIgnoreCase))
                return connectionStrings["FrontEndDatabase"]?.ConnectionString;

            var serverName = ConfigurationManager.AppSettings["SQLServer"];
            var databaseName = ConfigurationManager.AppSettings["SQLDatabase"];
            return
                $"Server=tcp:{serverName},1433;" +
                $"database={databaseName};" +
                "authentication=Active Directory Default;" +
                "encrypt=True;TrustServerCertificate=False;";
        }
    }
}
