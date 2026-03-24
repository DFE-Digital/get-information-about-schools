using System.Configuration;
using System.Net.Http;
using Xunit;
using Xunit.Abstractions;

namespace Edubase.Services.ExternalLookup.UnitTests
{
    public class FBServiceTests
    {
        private readonly ITestOutputHelper _output;

        public FBServiceTests(ITestOutputHelper output)
        {
            _output = output;
            ConfigurationManager.AppSettings["FinancialBenchmarkingApiURL"] = @"https://example.com/";
            ConfigurationManager.AppSettings["FinancialBenchmarkingURL"] = @"https://example.com/";
            ConfigurationManager.AppSettings["FscpdClient_Timeout"] = "2";
            ConfigurationManager.AppSettings["FBService_RetryIntervals"] = "1";
        }

        [Theory]
        [InlineData(1, FbType.School, "https://example.com/api/schoolstatus/1")]
        [InlineData(1, FbType.Federation, "https://example.com/api/federationstatus/1")]
        [InlineData(1, FbType.Trust, "https://example.com/api/truststatus/1")]
        public void ApiUrl_ReturnsCorrectUrl(int? lookupId, FbType lookupType, string expectedUrl)
        {
            ConfigurationManager.AppSettings["FinancialBenchmarkingApiURL"] = "https://example.com/";
            var client = new HttpClient();
            var subject = new FBService(client);
            var apiUrl = subject.ApiUrl(lookupId, lookupType);
            Assert.Equal(expectedUrl, apiUrl);
        }
    }
}
