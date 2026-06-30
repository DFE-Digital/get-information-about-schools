using System.Configuration;
using Microsoft.Data.SqlClient;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using Trace = System.Diagnostics.Trace;

namespace Edubase.Web.UI.Controllers.Api
{
    public class SqlDataController : ApiController
    {
        [Route("api/sql"), HttpGet]
        public async Task<IHttpActionResult> Sql()
        {
            var serverName = ConfigurationManager.AppSettings["SQLServer"];
            var databaseName = ConfigurationManager.AppSettings["SQLDatabase"];

            Trace.TraceInformation($"[api/sql] SQL Server='{serverName}' (len={serverName?.Length}), " +
                                   $"SQL Database='{databaseName}' (len={databaseName?.Length})");

            var connectionString =
                $"Server=tcp:{serverName},1433;" +
                $"database={databaseName};" +
                "authentication=Active Directory Default;" + // leverages Managed Identity on App Service
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
                Trace.TraceError($"[api/sql] connection failed: {ex.GetType().Name}: {ex.Message}");
                return Content(HttpStatusCode.InternalServerError,
                    new
                    {
                        sqlServer = serverName,
                        sqlDatabase = databaseName,
                        error = ex.GetType().Name,
                        message = ex.Message
                    });
            }
        }
    }
}
