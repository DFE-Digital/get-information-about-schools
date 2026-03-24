using Edubase.Services.Core;
using Edubase.Services.Exceptions;
using Edubase.Web.UI;
using Moq;
using Xunit;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Principal;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Edubase.Services.TexunaUnitTests.Core
{
    public class HttpClientWrapperTest
    {
        private readonly IPrincipal _principal = new GenericPrincipal(new GenericIdentity(""), null);

        private const string MimeTypeHtml = "text/html";
        private const string MimeTypeJson = "application/json";

        private class TestObject
        {
            public int Int1 { get; set; }
        }

        [Fact]
        public async Task GetAsync_SuccessfullyReturnsObject()
        {
            var u = new Uri(@"https://test.com/test");
            var mockHandler = new MockHttpMessageHandler();
            var subject = CreateWrapper(mockHandler);
            var r = Ok(JsonConvert.SerializeObject(new TestObject() { Int1 = 1 }));
            mockHandler.Add(u, r);
            var result = await subject.GetAsync<TestObject>(u.ToString(), _principal, false);
            Assert.Equal(1, result.Response.Int1);
        }

        [Fact]
        public async Task GetAsync_404ErrorThrowsException_ReturnsObject()
        {
            var u = new Uri("https://test.com/test");
            var mockHandler = new MockHttpMessageHandler();
            var subject = CreateWrapper(mockHandler);
            await Assert.ThrowsAsync<TexunaApiNotFoundException>(() =>
                subject.GetAsync<TestObject>(u.ToString(), _principal, true));
        }


        [Fact]
        public async Task GetAsync_404ErrorReturnsNull_ReturnsObject()
        {
            var u = new Uri("https://test.com/test");
            var mockHandler = new MockHttpMessageHandler();
            var subject = CreateWrapper(mockHandler);
            Assert.Null((await subject.GetAsync<TestObject>(u.ToString(), _principal, false)).Response);
        }

        [Fact]
        public async Task GetAsync_RejectsNonJSONContent()
        {
            var u = new Uri("https://test.com/test");
            var mockHandler = new MockHttpMessageHandler();
            var subject = CreateWrapper(mockHandler);
            mockHandler.Add(u, Ok("<html><head><title>system exception</title></head></html>", MimeTypeHtml));
            await Assert.ThrowsAsync<TexunaApiSystemException>(() =>
                subject.GetAsync<TestObject>(u.ToString(), _principal));
        }

        [Fact]
        public async Task GetAsync_PropagatesInternalServerError()
        {
            var u = new Uri("https://test.com/test");
            var mockHandler = new MockHttpMessageHandler();
            var subject = CreateWrapper(mockHandler);
            mockHandler.Add(u, new HttpResponseMessage(HttpStatusCode.InternalServerError));
            await Assert.ThrowsAsync<TexunaApiSystemException>(() =>
                subject.GetAsync<TestObject>(u.ToString(), _principal));
        }

        [Fact]
        public async Task GetAsync_RejectsCorruptJSONContent()
        {
            var u = new Uri("http://test.com/test");
            var mockHandler = new MockHttpMessageHandler();
            var subject = CreateWrapper(mockHandler);
            mockHandler.Add(u, Ok("=--%^&*{}{}{}{}{{@bgnffsegnj"));
            await Assert.ThrowsAsync<TexunaApiSystemException>(() =>
                subject.GetAsync<TestObject>(u.ToString(), _principal));
        }

        [Fact]
        public async Task GetAsync_HandlesTimeout()
        {
            var mockHandler = new MockHttpMessageHandler { AlwaysTimeout = true };
            var subject = CreateWrapper(mockHandler);
            var exception = await Assert.ThrowsAsync<TexunaApiSystemException>(() =>
                subject.GetAsync<TestObject>("http://test.com/do-timeout", _principal));
            Assert.Contains("timely", exception.Message);
        }

        [Fact]
        public async Task GetAsync_SuccessfullyReturnsPrimitives()
        {
            var mockHandler = new MockHttpMessageHandler();
            var subject = CreateWrapper(mockHandler);

            var u1 = new Uri("https://test.com/test/int");
            mockHandler.Add(u1, Ok("1"));

            var u2 = new Uri("https://test.com/test/string");
            mockHandler.Add(u2, Ok("bob"));

            var result1 = await subject.GetAsync<int?>(u1.ToString(), _principal, false);
            var result2 = await subject.GetAsync<string>(u2.ToString(), _principal, false);

            Assert.Equal(1, result1.Response);
            Assert.Equal("bob", result2.Response);
        }

        private static HttpResponseMessage Ok(string content, string mimeType = MimeTypeJson)
        {
            var retVal = new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(content) };
            retVal.Content.Headers.ContentType = new MediaTypeHeaderValue(mimeType);
            return retVal;
        }

        private static HttpClientWrapper CreateWrapper(HttpMessageHandler mockHandler) =>
            new HttpClientWrapper(new HttpClient(mockHandler), IocConfig.CreateJsonMediaTypeFormatter(),
                new Mock<IClientStorage>(MockBehavior.Loose).Object);

        [Fact]
        public async Task SendAsync_TaskCanceled_ShouldNotThrow()
        {
            var handler = new Mock<HttpMessageHandler>();

            handler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ThrowsAsync(new TaskCanceledException());

            var client = new HttpClient(handler.Object);
            var wrapper = new HttpClientWrapper(client);

            var request = new HttpRequestMessage(HttpMethod.Get, "http://test.com");

            var ex = await Record.ExceptionAsync(async () => await wrapper.SendAsync(request));
            Assert.Null(ex);
        }

        [Fact]
        public async Task SendAsync_HttpRequestException_ShouldThrow()
        {
            var handler = new Mock<HttpMessageHandler>();

            handler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ThrowsAsync(new HttpRequestException("fail"));

            var client = new HttpClient(handler.Object);
            var wrapper = new HttpClientWrapper(client);
            var request = new HttpRequestMessage(HttpMethod.Get, "http://test.com");

            await Assert.ThrowsAsync<HttpRequestException>(() => wrapper.SendAsync(request));
        }
    }
}
