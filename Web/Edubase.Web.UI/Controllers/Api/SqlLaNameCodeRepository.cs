using Edubase.Common.Cache;
using Edubase.Web.UI.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Edubase.Web.UI.Controllers.Api
{
    public class SqlLaNameCodeRepository : ISqlLaNameCodeRepository
    {
        private readonly IConfiguration _configuration;

        private readonly ICacheAccessor _cacheAccessor;

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
            using (var context = new LaNameCodesDbContext(new SqlConnection(BuildConnectionString())))
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
            using (var context = new LaNameCodesDbContext(new SqlConnection(BuildConnectionString())))
            {
                return await context.LaNameCodes
                    .AsNoTracking()
                    .Where(x => x.GroupCode == groupCode)
                    .OrderBy(x => x.LaName)
                    .ToListAsync();
            }
        }

        private string BuildConnectionString()
        {
            var serverName = _configuration["SQLServer"];
            var databaseName = _configuration["SQLDatabase"];
            return
                $"Server=tcp:{serverName},1433;" +
                $"database={databaseName};" +
                "authentication=Active Directory Default;" +
                "encrypt=True;TrustServerCertificate=False;";
        }
    }
}
