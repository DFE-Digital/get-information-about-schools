using Microsoft.Extensions.Configuration;
using System.Net.Http;
using Xunit;
using Xunit.Abstractions;

namespace Edubase.Services.ExternalLookup.UnitTests
{
    public class FBServiceTests
    {
        private readonly IConfiguration _configuration = new ConfigurationBuilder().AddInMemoryCollection().Build();

        private readonly ITestOutputHelper _output;

        public FBServiceTests(ITestOutputHelper output)
        {
            _output = output;
            _configuration["AppSettings:FinancialBenchmarkingApiURL"] = "https://example.com/";
            _configuration["AppSettings:FinancialBenchmarkingURL"] = "https://example.com/";
            _configuration["FscpdClient_Timeout"] = "2";
            _configuration["AppSettings:FBService_RetryIntervals"] = "1";
        }

        [Theory]
        [InlineData(1, FbType.School, "https://example.com/api/schoolstatus/1")]
        [InlineData(1, FbType.Federation, "https://example.com/api/federationstatus/1")]
        [InlineData(1, FbType.Trust, "https://example.com/api/truststatus/1")]
        public void ApiUrl_ReturnsCorrectUrl(int? lookupId, FbType lookupType, string expectedUrl)
        {
            _configuration["AppSettings:FinancialBenchmarkingApiURL"] = "https://example.com/";
            var client = new HttpClient();
            var subject = new FBService(client, _configuration);
            var apiUrl = subject.ApiUrl(lookupId, lookupType);
            Assert.Equal(expectedUrl, apiUrl);
        }

        [Theory]
        [InlineData(1, FbType.School, "https://example.com/school/detail?urn=1", "PublicURL_ReturnsCorrectUrl_School")]
        [InlineData(1, FbType.Federation, "https://example.com/federation?fuid=1", "PublicURL_ReturnsCorrectUrl_Federation")]
        [InlineData(1, FbType.Trust, "https://example.com/Trust?companyNo=1", "PublicURL_ReturnsCorrectUrl_Trust")]
        public void PublicURL_ReturnsCorrectUrl(int? lookupId, FbType lookupType, string expectedUrl, string testName)
        {
            _output.WriteLine(testName);
            var subject = new FBService(new HttpClient(), _configuration);
            var publicUrl = subject.PublicURL(lookupId, lookupType);
            Assert.Equal(expectedUrl, publicUrl);
        }
    }
}
