using System.Configuration;

namespace Edubase.Web.UI.Controllers.Api
{
    public abstract class SqlRepositoryBase
    {
        protected static string BuildConnectionString()
        {
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
