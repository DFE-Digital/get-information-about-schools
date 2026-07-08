using System.Configuration;
using Edubase.Common.Config;
using Edubase.Data.Repositories;
using Microsoft.Data.SqlClient;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using AzureTableLogger;
using AzureTableLogger.LogMessages;

namespace Edubase.Web.UI.Controllers.Api
{
    [Authorize]
    public class SqlDataController : ApiController
    {
        private readonly IAzLogger _logger;
        private readonly IUserPreferenceRepository _tableStorageUserPreferenceRepository;
        private readonly ISqlUserPreferenceRepository _sqlUserPreferenceRepository;

        public SqlDataController(
            IAzLogger logger,
            IUserPreferenceRepository tableStorageUserPreferenceRepository,
            ISqlUserPreferenceRepository sqlUserPreferenceRepository)
        {
            _logger = logger;
            _tableStorageUserPreferenceRepository = tableStorageUserPreferenceRepository;
            _sqlUserPreferenceRepository = sqlUserPreferenceRepository;
        }


        [Route("api/sql"), HttpGet]
        public async Task<IHttpActionResult> Sql()
        {
            var serverName = ConfigurationManager.AppSettings["SQLServer"];
            var databaseName = ConfigurationManager.AppSettings["SQLDatabase"];
            var connectionString =
                $"Server=tcp:{serverName},1433;" +
                $"database={databaseName};" +
                "authentication=Active Directory Default;" +
                "encrypt=True;TrustServerCertificate=False;";

            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new SqlCommand("SELECT GETUTCDATE();", connection))
                    {
                        var result = await command.ExecuteScalarAsync();
                        return Ok(new { serverDate = result, status = "Connected" });
                    }
                }
            }
            catch (SqlException ex)
            {
                _logger.Log(new WebLogMessage
                {
                    Level = LogMessage.LogLevel.ERROR,
                    Environment = ConfigurationManager.AppSettings["Environment"],
                    Message = $"[api/sql] connection failed. SQLServer='{serverName}', " +
                              $"SQLDatabase='{databaseName}', {ex.GetType().Name}: {ex.Message}",
                    Exception = ex.ToString()
                });
                return StatusCode(HttpStatusCode.ServiceUnavailable);
            }
        }

        [Route("api/migrate-user-preferences"), HttpPost]
        public async Task<IHttpActionResult> MigrateUserPreferencesAsync()
        {
            if (!Feature.IsEnabled("UserPreferencesMigration"))
            {
                return NotFound();
            }

            var allPrefs = await _tableStorageUserPreferenceRepository.GetAllAsync();
            var migrated = 0;

            foreach (var pref in allPrefs.Items)
            {
                await _sqlUserPreferenceRepository.UpsertAsync(new Models.SqlUserPreference
                {
                    UserId = pref.UserId,
                    SavedSearchToken = pref.SavedSearchToken
                });
                migrated++;
            }

            return Ok(new { migrated });
        }
    }
}
