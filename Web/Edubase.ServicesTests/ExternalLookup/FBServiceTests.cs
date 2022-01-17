using Xunit;
using Edubase.Services.ExternalLookup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Abstractions;
using System.Net.Http;

namespace Edubase.Services.ExternalLookup.Tests
{
    public class FBServiceTests
    {
        private readonly ITestOutputHelper _output;

        public FBServiceTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Theory]
        [InlineData(1, FbType.School, "http://test/school/detail?urn=1", "PublicURL_ReturnsCorrectUrl_School")]
        [InlineData(1, FbType.Federation, "http://test/federation?fuid=1", "PublicURL_ReturnsCorrectUrl_Federation")]
        [InlineData(1, FbType.Trust, "http://test/Trust?companyNo=1", "PublicURL_ReturnsCorrectUrl_Trust")]
        public void PublicURL_ReturnsCorrectUrl(int? lookupId, FbType lookupType, string expectedUrl, string testName)
        {
            _output.WriteLine(testName);
            var subject = new FBService(new HttpClient(), "http://test/");
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
            var subject = new FBService(new HttpClient(), "http://test/");
            var apiUrl = subject.ApiUrl(lookupId, lookupType);
            Assert.Equal(expectedUrl, apiUrl);
        }
    }
}
