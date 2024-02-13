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
            ConfigurationManager.AppSettings["FinancialBenchmarkingApiURL"] = @"http://example/";
            ConfigurationManager.AppSettings["FinancialBenchmarkingURL"] = @"http://example/";
            ConfigurationManager.AppSettings["FscpdClient_Timeout"] = "2";
            ConfigurationManager.AppSettings["FBService_RetryIntervals"] = "1";
        }

        [Theory]
        [InlineData(1, FbType.School, "http://example/school/detail?urn=1", "PublicURL_ReturnsCorrectUrl_School")]
        [InlineData(1, FbType.Federation, "http://example/federation?fuid=1", "PublicURL_ReturnsCorrectUrl_Federation")]
        [InlineData(1, FbType.Trust, "http://example/Trust?companyNo=1", "PublicURL_ReturnsCorrectUrl_Trust")]
        public void PublicURL_ReturnsCorrectUrl(int? lookupId, FbType lookupType, string expectedUrl, string testName)
        {
            _output.WriteLine(testName);
            var subject = new FBService(new HttpClient());
            var publicUrl = subject.PublicURL(lookupId, lookupType);
            Assert.Equal(expectedUrl, publicUrl);
        }

        [Theory]
        [InlineData(1, FbType.School, "http://example/api/schoolstatus/1", "ApiUrl_ReturnsCorrectUrl_School")]
        [InlineData(1, FbType.Federation, "http://example/api/federationstatus/1", "ApiUrl_ReturnsCorrectUrl_Federation")]
        [InlineData(1, FbType.Trust, "http://example/api/truststatus/1", "ApiUrl_ReturnsCorrectUrl_Trust")]
        public void ApiUrl_ReturnsCorrectUrl(int? lookupId, FbType lookupType, string expectedUrl, string testName)
        {
            _output.WriteLine(testName);
            var subject = new FBService(new HttpClient());
            var apiUrl = subject.ApiUrl(lookupId, lookupType);
            Assert.Equal(expectedUrl, apiUrl);
        }
    }
}
