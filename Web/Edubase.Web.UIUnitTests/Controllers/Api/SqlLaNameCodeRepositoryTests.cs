using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace Edubase.Web.UI.Controllers.Api.UnitTests
{

    public class SqlLaNameCodeRepositoryTests 
    {
        [Fact]
        public void BuildConnectionStringShouldUseHostedSettings()
        {
            var configuration = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string>
            {
                ["Environment"] = "dev",
                ["FrontEndDatabase"] = "IgnoredLocalConnection",
                ["SQLServer"] = "test.database.windows.net",
                ["SQLDatabase"] = "gias-test"
            }).Build();

            var connectionString = new Microsoft.Data.SqlClient.SqlConnectionStringBuilder(
                SqlLaNameCodeRepository.BuildConnectionString(configuration));

            Assert.Equal("tcp:test.database.windows.net,1433", connectionString.DataSource);
            Assert.Equal("gias-test", connectionString.InitialCatalog);
            Assert.Equal(Microsoft.Data.SqlClient.SqlAuthenticationMethod.ActiveDirectoryDefault, connectionString.Authentication);
            Assert.False(connectionString.TrustServerCertificate);
        }

        [Fact]
        public void BuildConnectionStringShouldUseLocalDevSetting()
        {
            var configuration = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string>
            {
                ["Environment"] = "localdev",
                ["FrontEndDatabase"] = "Server=(localdb)\\MSSQLLocalDB;Database=gias;Integrated Security=true"
            }).Build();

            Assert.Equal(configuration["FrontEndDatabase"], SqlLaNameCodeRepository.BuildConnectionString(configuration));
        }

        [Theory]
        [InlineData("localdev")]
        [InlineData("dev")]
        public void BuildConnectionStringShouldPreferConfiguredConnectionString(string environment)
        {
            var configuration = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string>
            {
                ["Environment"] = environment,
                ["FrontEndDatabase"] = "IgnoredLocalConnection",
                ["ConnectionStrings:FrontEndDatabase"] = "Server=localhost;Database=gias;Integrated Security=true"
            }).Build();

            Assert.Equal(configuration.GetConnectionString("FrontEndDatabase"), SqlLaNameCodeRepository.BuildConnectionString(configuration));
        }

        [Theory]
        [InlineData("localdev", null, null)]
        [InlineData("dev", null, null)]
        [InlineData("dev", "test.database.windows.net", null)]
        [InlineData("dev", null, "gias-test")]
        public void BuildConnectionStringShouldRejectMissingConfiguration(string environment, string server, string database)
        {
            var configuration = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string>
            {
                ["Environment"] = environment,
                ["SQLServer"] = server,
                ["SQLDatabase"] = database
            }).Build();

            var exception = Assert.Throws<InvalidOperationException>(() => SqlLaNameCodeRepository.BuildConnectionString(configuration));

            Assert.Contains("ConnectionStrings:FrontEndDatabase", exception.Message);
        }
    }
}
