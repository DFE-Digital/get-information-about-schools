using Edubase.Services;
using Edubase.Services.Exceptions;
using Edubase.UnitTest.Mocks;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Principal;
using System.Threading.Tasks;

namespace Edubase.UnitTest.Services.Texuna.Core
{
    [TestFixture]
    public class HttpClientWrapperTest
    {
        public IPrincipal p = new GenericPrincipal(new GenericIdentity(""), null);

        private const string mimeTypeHtml = "text/html";
        private const string mimeTypeJson = "application/json";

        class TestObject
        {
            public int Int1 { get; set; }
        }

        [Test]
        public async Task GetAsync_SuccessfullyReturnsObject()
        {
            var u = new Uri("http://test.com/test");
            var mockHandler = new MockHttpMessageHandler();
            var subject = new HttpClientWrapper(new HttpClient(mockHandler));
            var r = Ok(JsonConvert.SerializeObject(new TestObject() { Int1 = 1 }));
            mockHandler.Add(u, r);
            var result = await subject.GetAsync<TestObject>(u.ToString(), p, false);
            Assert.That(result.Response.Int1, Is.EqualTo(1));
        }

        [Test]
        public void GetAsync_404ErrorThrowsException_ReturnsObject()
        {
            var u = new Uri("http://test.com/test");
            var mockHandler = new MockHttpMessageHandler();
            var subject = new HttpClientWrapper(new HttpClient(mockHandler));
            Assert.That(() => subject.GetAsync<TestObject>(u.ToString(), p, true), Throws.TypeOf<TexunaApiNotFoundException>());
        }


        [Test]
        public async Task GetAsync_404ErrorReturnsNull_ReturnsObject()
        {
            var u = new Uri("http://test.com/test");
            var mockHandler = new MockHttpMessageHandler();
            var subject = new HttpClientWrapper(new HttpClient(mockHandler));
            Assert.That((await subject.GetAsync<TestObject>(u.ToString(), p, false)).Response, Is.Null);
        }

        [Test]
        public void GetAsync_RejectsNonJSONContent()
        {
            var u = new Uri("http://test.com/test");
            var mockHandler = new MockHttpMessageHandler();
            var subject = new HttpClientWrapper(new HttpClient(mockHandler));
            mockHandler.Add(u, Ok("<html><head><title>system exception</title></head></html>", mimeTypeHtml));
            Assert.That(() => subject.GetAsync<TestObject>(u.ToString(), p), Throws.TypeOf<TexunaApiSystemException>());
        }

        [Test]
        public void GetAsync_PropagatesInternalServerError()
        {
            var u = new Uri("http://test.com/test");
            var mockHandler = new MockHttpMessageHandler();
            var subject = new HttpClientWrapper(new HttpClient(mockHandler));
            mockHandler.Add(u, new HttpResponseMessage(HttpStatusCode.InternalServerError));
            Assert.That(() => subject.GetAsync<TestObject>(u.ToString(), p), Throws.TypeOf<TexunaApiSystemException>());
        }

        [Test]
        public void GetAsync_RejectsCorruptJSONContent()
        {
            var u = new Uri("http://test.com/test");
            var mockHandler = new MockHttpMessageHandler();
            var subject = new HttpClientWrapper(new HttpClient(mockHandler));
            mockHandler.Add(u, Ok("=--%^&*{}{}{}{}{{@bgnffsegnj"));
            Assert.That(() => subject.GetAsync<TestObject>(u.ToString(), p), Throws.TypeOf<TexunaApiSystemException>());
        }

        [Test]
        public void GetAsync_HandlesTimeout()
        {
            var mockHandler = new MockHttpMessageHandler { AlwaysTimeout = true };
            var subject = new HttpClientWrapper(new HttpClient(mockHandler));
            Assert.That(() => subject.GetAsync<TestObject>("http://test.com/do-timeout", p), Throws.TypeOf<TexunaApiSystemException>().And.With.Message.Contains("timely"));
        }

        [Test]
        public async Task GetAsync_SuccessfullyReturnsPrimitives()
        {
            var mockHandler = new MockHttpMessageHandler();
            var subject = new HttpClientWrapper(new HttpClient(mockHandler));

            var u1 = new Uri("http://test.com/test/int");
            mockHandler.Add(u1, Ok("1"));

            var u2 = new Uri("http://test.com/test/string");
            mockHandler.Add(u2, Ok("bob"));

            var result1 = await subject.GetAsync<int?>(u1.ToString(), p, false);
            var result2 = await subject.GetAsync<string>(u2.ToString(), p, false);

            Assert.That(result1.Response, Is.EqualTo(1));
            Assert.That(result2.Response, Is.EqualTo("bob"));
        }

        private HttpResponseMessage Ok(string content, string mimeType = mimeTypeJson)
        {
            var retVal = new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(content) };
            retVal.Content.Headers.ContentType = new MediaTypeHeaderValue(mimeType);
            return retVal;
        }
        



    }
}
