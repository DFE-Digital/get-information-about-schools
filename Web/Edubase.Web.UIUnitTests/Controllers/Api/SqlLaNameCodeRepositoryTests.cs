using System.Configuration;
using Xunit;

namespace Edubase.Web.UI.Controllers.Api.UnitTests
{

    public class SqlLaNameCodeRepositoryTests 
    {
        [Fact]
        public void BuildConnectionStringShouldIgnoreAnyLocalDevSetting()
        {
            ConfigurationManager.AppSettings["Environment"] = "dev";

            var connectionStrings = new ConnectionStringSettingsCollection();

            connectionStrings.Add(new ConnectionStringSettings("FrontEndDatabase", "FrontEndDatabaseConnectionString"));

            var expected = "Server=tcp:,1433;database=;authentication=Active Directory Default;encrypt=True;TrustServerCertificate=False;";

            Assert.Equal(expected, SqlLaNameCodeRepository.BuildConnectionString(ConfigurationManager.ConnectionStrings));
        }

        [Fact]
        public void BuildConnectionStringShouldUseLocalDevSetting()
        {
            ConfigurationManager.AppSettings["Environment"] = "localdev";

            var connectionStrings = new ConnectionStringSettingsCollection();

            connectionStrings.Add(new ConnectionStringSettings("FrontEndDatabase", "FrontEndDatabaseConnectionString"));

            Assert.Equal("FrontEndDatabaseConnectionString", SqlLaNameCodeRepository.BuildConnectionString(connectionStrings));
        }
    }
}
