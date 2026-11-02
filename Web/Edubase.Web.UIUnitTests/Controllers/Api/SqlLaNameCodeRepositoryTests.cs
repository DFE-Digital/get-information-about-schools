using System.Collections.Generic;
using Edubase.Common.Cache;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

namespace Edubase.Web.UI.Controllers.Api.UnitTests
{

    public class SqlLaNameCodeRepositoryTests 
    {
        [Fact]
        public void BuildConnectionStringShouldIgnoreAnyLocalDevSetting()
        {
            var configuration = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string>
            {
                ["AppSettings:Environment"] = "dev",
                ["AppSettings:SQLServer"] = "test-server",
                ["AppSettings:SQLDatabase"] = "test-database",
                ["ConnectionStrings:FrontEndDatabase"] = "FrontEndTestConnectionString"
            }).Build();

            var repository = new SqlLaNameCodeRepository(Mock.Of<ICacheAccessor>(), configuration);          

            var expected = "Server=tcp:test-server,1433;database=test-database;authentication=Active Directory Default;encrypt=True;TrustServerCertificate=False;";

            Assert.Equal(expected, repository.BuildConnectionString());            
        }

        [Fact]
        public void BuildConnectionStringShouldUseLocalDevSetting()
        {
            var configuration = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string>
            {
                ["AppSettings:Environment"] = "localdev",                
                ["ConnectionStrings:FrontEndDatabase"] = "FrontEndTestConnectionString"
            }).Build();

            var repository = new SqlLaNameCodeRepository(Mock.Of<ICacheAccessor>(), configuration);

            var expected = "FrontEndTestConnectionString";

            Assert.Equal(expected, repository.BuildConnectionString());            
        }       
    }
}
