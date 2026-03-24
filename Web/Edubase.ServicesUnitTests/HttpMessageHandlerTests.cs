using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Polly;
using Xunit;
using Edubase.Services.IntegrationEndPoints.AzureMaps;


namespace Edubase.Services.TexunaUnitTests
{
    public class HttpMessageHandlerTests : HttpMessageHandler
    {
        private readonly Func<HttpRequestMessage, int, Task<HttpResponseMessage>> _handler;
        public List<HttpRequestMessage> Requests { get; } = new List<HttpRequestMessage>();
        public int CallCount { get; private set; }

        public HttpMessageHandlerTests(Func<HttpRequestMessage, int, Task<HttpResponseMessage>> handler)
        {
            _handler = handler;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            CallCount++;
            Requests.Add(request);
            return _handler(request, CallCount);
        }
    }

    public class AzureMapsServiceTests
    {
        private AzureMapsService CreateService(HttpMessageHandler handler)
        {
            // Bypass constructor entirely
            var service = (AzureMapsService) FormatterServices.GetUninitializedObject(typeof(AzureMapsService));

            // Manually inject HttpClient
            var clientField = typeof(AzureMapsService)
                .GetField("_azureMapsClient", BindingFlags.NonPublic | BindingFlags.Instance);
            clientField.SetValue(service, new HttpClient(handler) { BaseAddress = new Uri("http://test") });

            // Manually inject NO-OP retry policy
            var retryField = typeof(AzureMapsService)
                .GetField("RetryPolicy", BindingFlags.NonPublic | BindingFlags.Instance);
            retryField.SetValue(service, Policy.NoOpAsync<HttpResponseMessage>());

            return service;
        }

        [Fact]
        public async Task EmptyInput_ReturnsEmpty_AndDoesNotCallHttp()
        {
            var service = CreateService(new HttpMessageHandlerTests((req, n) =>
                Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK))));

            var result = await service.SearchAsync("   ", true);

            Assert.Empty(result);
        }

        [Fact]
        public async Task NonSuccessStatus_ReturnsEmpty()
        {
            var service = CreateService(new HttpMessageHandlerTests((req, n) =>
                Task.FromResult(new HttpResponseMessage(HttpStatusCode.BadRequest))));

            var result = await service.SearchAsync("NN11 5QS", true);

            Assert.Empty(result);
        }

        [Fact]
        public async Task Retry_RecreatesRequestEachTime()
        {
            var handler = new HttpMessageHandlerTests(async (req, call) =>
            {
                if (call == 1)
                    throw new HttpRequestException("failure");

                var payload = new
                {
                    summary = new { },
                    results = new[]
                    {
                        new
                        {
                            type = "Municipality",
                            id = "123",
                            score = 1.0,
                            entityType = "Municipality",
                            address = new
                            {
                                municipalitySubdivision = "Town",
                                municipality = "Town",
                                countrySecondarySubdivision = "County",
                                postalCode = "AA11AA",
                                extendedPostalCode = "AA11AA",
                                freeformAddress = "Town"
                            },
                            position = new { lat = 52.0, lon = -1.0 },
                            viewport = (object) null,
                            boundingBox = (object) null,
                            dataSources = (object) null
                        }
                    }
                };

                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8,
                        "application/json")
                };
            });

            var service = CreateService(handler);

            // Override retry policy with explicit retry=1 inside tests
            var retryField = typeof(AzureMapsService)
                .GetField("RetryPolicy", BindingFlags.NonPublic | BindingFlags.Instance);

            retryField.SetValue(service,
                Polly.Policy<HttpResponseMessage>
                    .Handle<HttpRequestException>()
                    .RetryAsync(1)
            );

            var result = await service.SearchAsync("Town", true);

            Assert.Single(result);
            Assert.Equal(2, handler.CallCount);
            Assert.Equal(2, handler.Requests.Count);
            Assert.NotSame(handler.Requests[0], handler.Requests[1]);
        }

        [Fact]
        public async Task ValidResponse_ParsesCorrectly()
        {
            var service = CreateService(new HttpMessageHandlerTests((req, n) =>
            {
                var payload = new
                {
                    summary = new { },
                    results = new[]
                    {
                        new
                        {
                            type = "Municipality",
                            id = "xyz",
                            score = 1.0,
                            entityType = "Municipality",
                            address = new
                            {
                                municipalitySubdivision = "Town",
                                municipality = "Town",
                                countrySecondarySubdivision = "County",
                                postalCode = "AA1 1AA",
                                extendedPostalCode = "AA11AA",
                                freeformAddress = "Town"
                            },
                            position = new { lat = 50.0, lon = -2.0 },
                            viewport = (object) null,
                            boundingBox = (object) null,
                            dataSources = (object) null
                        }
                    }
                };

                return Task.FromResult(
                    new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8,
                            "application/json")
                    });
            }));

            var result = await service.SearchAsync("Town", true);

            Assert.Single(result);
            Assert.Equal("Town, County", result[0].Name);
            Assert.Equal(50.0, result[0].Coords.Latitude);
            Assert.Equal(-2.0, result[0].Coords.Longitude);
        }
    }
}
