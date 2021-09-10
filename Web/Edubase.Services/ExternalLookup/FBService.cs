using System;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Runtime.Caching;
using System.Threading.Tasks;
using Edubase.Common;
using Polly;

namespace Edubase.Services.ExternalLookup
{
    public enum FbType
    {
        School,
        Federation,
        Trust
    }

    public class FBService : IFBService
    {
        private static HttpClient _client = new HttpClient
        {
            BaseAddress = new Uri(ConfigurationManager.AppSettings["FinancialBenchmarkingURL"]),
            Timeout = TimeSpan.FromSeconds(10)
        };

        private static readonly Policy RetryPolicy = Policy.TimeoutAsync(1).Wrap(Policy
            .Handle<HttpRequestException>()
            .WaitAndRetryAsync(new[]
            {
                TimeSpan.FromSeconds(1)
            }));

        public FBService(HttpClient client)
        {
            _client = client;
        }

        public string PublicURL(int? lookupId, FbType lookupType)
        {
            return $"{_client.BaseAddress.AbsoluteUri}{UrlPath(lookupId, lookupType)}";
        }

        private string UrlPath(int? lookupId, FbType lookupType)
        {
            var url = $"school/detail?urn={lookupId}";
            switch (lookupType)
            {
                case FbType.Trust:
                    url = $"Trust?companyNo={lookupId}";
                    break;
                case FbType.Federation:
                    url = $"federation?fuid={lookupId}";
                    break;
            }

            return url;
        }

        private HttpRequestMessage HeadRestRequest(int? lookupId, FbType lookupType)
        {
            return new HttpRequestMessage(HttpMethod.Head, UrlPath(lookupId, lookupType));
        }

        public async Task<bool> CheckExists(int? lookupId, FbType lookupType)
        {
            var key = $"sfb-{lookupType.ToString()}-{lookupId}";
            var value = MemoryCache.Default.Get(key);
            if (value != null)
            {
                return (bool) value;
            }
            else
            {
                var cacheTime = ConfigurationManager.AppSettings["FinancialBenchmarkingCacheHours"].ToInteger() ?? 8;
                var request = HeadRestRequest(lookupId, lookupType);

                try
                {
                    using (var response = await RetryPolicy.ExecuteAsync(async () => await _client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead)))
                    {
                        var isOk = response.StatusCode == HttpStatusCode.OK;
                        MemoryCache.Default.Set(new CacheItem(key, isOk), new CacheItemPolicy { AbsoluteExpiration = DateTimeOffset.Now.AddHours(cacheTime) });
                        return isOk;
                    }
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
        }
    }
}
