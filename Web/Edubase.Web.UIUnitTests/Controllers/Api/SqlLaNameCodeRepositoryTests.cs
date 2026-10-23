using System;
using System.Configuration;
using System.Reflection;
using Xunit;

namespace Edubase.Web.UI.Controllers.Api.UnitTests
{

    public class SqlLaNameCodeRepositoryTests 
    {
        [Fact]
        public void BuildConnectionStringShouldIgnoreAnyLocalDevSetting()
        {
            ConfigurationManager.AppSettings["Environment"] = "dev";

            AddConnectionStringToConfigurationManager("FrontEndDatabase", "FrontEndTestConnectionString");

            var expected = "Server=tcp:,1433;database=;authentication=Active Directory Default;encrypt=True;TrustServerCertificate=False;";

            Assert.Equal(expected, SqlLaNameCodeRepository.BuildConnectionString());

            RemoveConnectionStringFromConfigurationManager("FrontEndDatabase");
        }

        [Fact]
        public void BuildConnectionStringShouldUseLocalDevSetting()
        {
            ConfigurationManager.AppSettings["Environment"] = "localdev";

            AddConnectionStringToConfigurationManager("FrontEndDatabase", "FrontEndTestConnectionString");

            var expected = "FrontEndTestConnectionString";

            Assert.Equal(expected, SqlLaNameCodeRepository.BuildConnectionString());

            RemoveConnectionStringFromConfigurationManager("FrontEndDatabase");
        }

        private void AddConnectionStringToConfigurationManager(string key, string value)
        {
            var connections = GetConfigurationManagerConnectionStrings();

            connections.Remove(key);
            connections.Add(new ConnectionStringSettings(key, value));
        }

        private void RemoveConnectionStringFromConfigurationManager(string key)
        {
            var connections = GetConfigurationManagerConnectionStrings();

            connections.Remove(key);
        }

        private ConnectionStringSettingsCollection GetConfigurationManagerConnectionStrings()
        {
            var connections = ConfigurationManager.ConnectionStrings;

            var readOnlyField = typeof(ConfigurationElementCollection)
                .GetField("bReadOnly", BindingFlags.Instance | BindingFlags.NonPublic);

            if (readOnlyField == null)
                throw new InvalidOperationException("Configuration read-only field not found.");

            readOnlyField.SetValue(connections, false);

            return connections;
        }
    }
}
