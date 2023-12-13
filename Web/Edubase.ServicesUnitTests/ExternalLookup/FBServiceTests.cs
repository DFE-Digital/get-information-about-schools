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
            ConfigurationManager.AppSettings["FinancialBenchmarkingApiURL"] = @"http://test/";
            ConfigurationManager.AppSettings["FinancialBenchmarkingURL"] = @"http://test/";
        }

        [Theory]
        [InlineData(1, FbType.School, "http://test/school/detail?urn=1", "PublicURL_ReturnsCorrectUrl_School")]
        [InlineData(1, FbType.Federation, "http://test/federation?fuid=1", "PublicURL_ReturnsCorrectUrl_Federation")]
        [InlineData(1, FbType.Trust, "http://test/Trust?companyNo=1", "PublicURL_ReturnsCorrectUrl_Trust")]
        public void PublicURL_ReturnsCorrectUrl(int? lookupId, FbType lookupType, string expectedUrl, string testName)
        {
            _output.WriteLine(testName);
            var subject = new FBService();
            var publicUrl = subject.PublicURL(lookupId, lookupType);
            Assert.Equal(expectedUrl, publicUrl);
        }

        [Theory]
        [InlineData(1, FbType.School, "http://test/api/schoolstatus/1", "ApiUrl_ReturnsCorrectUrl_School")]
        [InlineData(1, FbType.Federation, "http://test/api/federationstatus/1", "ApiUrl_ReturnsCorrectUrl_Federation")]
        [InlineData(1, FbType.Trust, "http://test/api/truststatus/1", "ApiUrl_ReturnsCorrectUrl_Trust")]
        public void ApiUrl_ReturnsCorrectUrl(int? lookupId, FbType lookupType, string expectedUrl, string testName)
        {
            _output.WriteLine(testName);
            var subject = new FBService();
            var apiUrl = subject.ApiUrl(lookupId, lookupType);
            Assert.Equal(expectedUrl, apiUrl);
        }
    }
}
