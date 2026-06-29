using System.Configuration;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;
using System.Web.Http;

namespace Edubase.Web.UI.Controllers.Api
{
    public class SqlDataController : ApiController
    {
        [Route("api/sql"), HttpGet]
        public async Task<IHttpActionResult> Sql()
        {
            var serverName = ConfigurationManager.AppSettings["SQLServer"];
            var databaseName = ConfigurationManager.AppSettings["SQLDatabase"];

            var connectionString =
                $"Server=tcp:{serverName}.database.windows.net,1433;" +
                $"database={databaseName};" +
                "authentication=Active Directory Default;" + // leverages Managed Identity on App Service
                "encrypt=True;TrustServerCertificate=False;";

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
    }
}
