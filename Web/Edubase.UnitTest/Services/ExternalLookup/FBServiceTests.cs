using System;
using System.Configuration;
using System.Net.Http;
using Edubase.Services.ExternalLookup;
using NUnit.Framework;

namespace Edubase.UnitTest.Services.ExternalLookup
{
    [TestFixture]
    public class FBServiceTests
    {
        public FBServiceTests()
        {
        }

        [Test]
        [TestCase("1", "School", "http://test/school/detail?urn=1", TestName = "PublicURL_ReturnsCorrectUrl_School")]
        [TestCase("1", "Federation", "http://test/federation?fuid=1", TestName = "PublicURL_ReturnsCorrectUrl_Federation")]
        [TestCase("1", "Trust", "http://test/Trust?companyNo=1", TestName = "PublicURL_ReturnsCorrectUrl_Trust")]
        public void PublicURL_ReturnsCorrectUrl(int? lookupId, FbType lookupType, string expectedUrl)
        {
            var subject = new FBService(new HttpClient(), "http://test/");
            var publicUrl = subject.PublicURL(lookupId, lookupType);
            Assert.That(publicUrl, Is.EqualTo(expectedUrl));
        }

        [Test]
        [TestCase("1", "School", "http://test/api/schoolstatus/1", TestName = "ApiUrl_ReturnsCorrectUrl_School")]
        [TestCase("1", "Federation", "http://test/api/federationstatus/1", TestName = "ApiUrl_ReturnsCorrectUrl_Federation")]
        [TestCase("1", "Trust", "http://test/api/truststatus/1", TestName = "ApiUrl_ReturnsCorrectUrl_Trust")]
        public void ApiUrl_ReturnsCorrectUrl(int? lookupId, FbType lookupType, string expectedUrl)
        {
            var subject = new FBService(new HttpClient(), "http://test/");
            var apiUrl = subject.ApiUrl(lookupId, lookupType);
            Assert.That(apiUrl, Is.EqualTo(expectedUrl));
        }
    }
}
