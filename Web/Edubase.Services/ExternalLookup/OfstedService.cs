using System;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Caching;
using System.Threading.Tasks;
using Edubase.Common;
using Edubase.Services.IntegrationEndPoints;
using Polly;

namespace Edubase.Services.ExternalLookup
{
    public class OfstedService : IOfstedService
    {
        private readonly HttpClient _client;

        private const string OfstedServiceTimeoutKey = "OfstedService_TimeoutSeconds";

        private IAsyncPolicy<HttpResponseMessage> RetryPolicy => PollyUtil.CreateRetryPolicy(RetryIntervals, OfstedServiceTimeoutKey);

        private TimeSpan[] RetryIntervals => PollyUtil.CsvSecondsToTimeSpans(ConfigurationManager.AppSettings["OfstedService_RetryIntervals"]);

        private int CacheHours => ConfigurationManager.AppSettings["OfstedService_CacheHours"].ToInteger() ?? 8;

        private string BaseUrl => ConfigurationManager.AppSettings["OfstedService_BaseAddress"];


        public OfstedService(HttpClient client)
        {
            _client = client;
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
                using (var response = await RetryPolicy.ExecuteAsync(async () =>
                           await _client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead)))
                {
                    var isOk = response.StatusCode == HttpStatusCode.OK;
                    MemoryCache.Default.Set(
                        new CacheItem(key, isOk),
                        new CacheItemPolicy {AbsoluteExpiration = DateTimeOffset.Now.AddHours(CacheHours)}
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
            return BaseUrl + urn;
        }
    }
}
