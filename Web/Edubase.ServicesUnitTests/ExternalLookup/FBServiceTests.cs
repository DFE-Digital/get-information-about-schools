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
        [InlineData(1, FbType.School, "https://example.com/school/detail?urn=1", "PublicURL_ReturnsCorrectUrl_School")]
        [InlineData(1, FbType.Federation, "https://example.com/federation?fuid=1", "PublicURL_ReturnsCorrectUrl_Federation")]
        [InlineData(1, FbType.Trust, "https://example.com/Trust?companyNo=1", "PublicURL_ReturnsCorrectUrl_Trust")]
        public void PublicURL_ReturnsCorrectUrl(int? lookupId, FbType lookupType, string expectedUrl, string testName)
        {
            _output.WriteLine(testName);
            var subject = new FBService(new HttpClient());
            var publicUrl = subject.PublicURL(lookupId, lookupType);
            Assert.Equal(expectedUrl, publicUrl);
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
