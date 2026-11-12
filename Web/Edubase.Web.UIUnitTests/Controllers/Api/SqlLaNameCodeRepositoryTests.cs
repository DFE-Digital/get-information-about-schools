using System;
using System.Configuration;
using Xunit;
using ConfigManager = System.Configuration.ConfigurationManager;

namespace Edubase.Web.UI.Controllers.Api.UnitTests
{

    public class SqlLaNameCodeRepositoryTests 
    {
        [Fact]
        public void BuildConnectionStringShouldIgnoreAnyLocalDevSetting()
        {
            ConfigManager.AppSettings["Environment"] = "dev";
            ConfigManager.AppSettings["FrontEndDatabase"] = "FrontEndTestConnectionString";

            var expected = "Server=tcp:,1433;database=;authentication=Active Directory Default;encrypt=True;TrustServerCertificate=False;";

            Assert.Equal(expected, SqlLaNameCodeRepository.BuildConnectionString());

            ConfigManager.AppSettings["FrontEndDatabase"] = string.Empty;
        }

        [Fact]
        public void BuildConnectionStringShouldUseLocalDevSetting()
        {
            ConfigManager.AppSettings["Environment"] = "localdev";
            ConfigManager.AppSettings["FrontEndDatabase"] = "FrontEndTestConnectionString";

            var expected = "FrontEndTestConnectionString";

            Assert.Equal(expected, SqlLaNameCodeRepository.BuildConnectionString());

            ConfigManager.AppSettings["FrontEndDatabase"] = string.Empty;
        }
    }
}
