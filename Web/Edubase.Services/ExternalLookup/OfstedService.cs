using System;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Caching;
using System.Threading.Tasks;
using Edubase.Common;
using Edubase.Services.IntegrationEndPoints;
using Microsoft.Extensions.Configuration;
using Polly;

namespace Edubase.Services.ExternalLookup
{
    public class OfstedService : IOfstedService
    {
        private readonly HttpClient _client;
        private readonly IAsyncPolicy<HttpResponseMessage> _retryPolicy;
        private readonly TimeSpan[] _retryIntervals;
        private readonly int _cacheHours;
        private readonly string _baseUrl;

        public OfstedService(HttpClient client, IConfiguration configuration)
        {
            _client = client;

            // Read configuration values
            var retryCsv = configuration["AppSettings:OfstedService_RetryIntervals"] ?? "1,2,5";
            var timeoutValue = configuration["AppSettings:OfstedService_TimeoutSeconds"] ?? "10";
            var cacheRaw = configuration["AppSettings:OfstedService_CacheHours"];
            var baseUrlRaw = configuration["AppSettings:OfstedService_BaseAddress"];

            _retryIntervals = PollyUtil.CsvSecondsToTimeSpans(retryCsv);
            _retryPolicy = PollyUtil.CreateRetryPolicy(configuration, _retryIntervals, timeoutValue);
            _cacheHours = int.TryParse(cacheRaw, out var hours) ? hours : 8;
            _baseUrl = baseUrlRaw ?? throw new ArgumentNullException("AppSettings:OfstedService_BaseAddress");
        }

        private HttpRequestMessage HeadRestRequest(int? urn)
        {
            return new HttpRequestMessage(HttpMethod.Head, PublicURL(urn));
        }


        public async Task<bool> CheckExists(int? urn)
        {
            var key = $"ofsted-report-{urn}";
            var cachedPageStatus = MemoryCache.Default.Get(key);
            if (cachedPageStatus != null)
            {
                return (bool) cachedPageStatus;
            }

            var request = HeadRestRequest(urn);
            try
            {
                using (var response = await _retryPolicy.ExecuteAsync(async () =>
                           await _client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead)))
                {
                    var isOk = response.StatusCode == HttpStatusCode.OK;
                    MemoryCache.Default.Set(
                        new CacheItem(key, isOk),
                        new CacheItemPolicy {AbsoluteExpiration = DateTimeOffset.Now.AddHours(_cacheHours) }
                    );
                    return isOk;
                }
            }
            catch
            {
                return false;
            }
        }

        public string PublicURL(int? urn)
        {
            return _baseUrl + urn;
        }
    }
}
