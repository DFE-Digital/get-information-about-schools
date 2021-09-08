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

        public string PublicURL(int? urn, string companiesHouse)
        {
            return companiesHouse.IsNullOrEmpty() ?
                $"{_client.BaseAddress.AbsoluteUri}school/detail?urn={urn}" :
                $"{_client.BaseAddress.AbsoluteUri}Trust?companyNo={companiesHouse.ToInteger()}";
        }

        private HttpRequestMessage HeadRestRequest(int? urn, string companiesHouse)
        {
            return companiesHouse.IsNullOrEmpty() ?
                new HttpRequestMessage(HttpMethod.Head, $"school/status?urn={urn}") :
                new HttpRequestMessage(HttpMethod.Head, $"Trust?companyNo={companiesHouse.ToInteger()}");
        }

        public async Task<bool> CheckExists(int? urn, string companiesHouse)
        {
            var collection = companiesHouse.IsNullOrEmpty() ? "school" : "mat";
            var key = $"sfb-{collection}-{urn}";
            var value = MemoryCache.Default.Get(key);
            if (value != null)
            {
                return (bool) value;
            }
            else
            {
                var cacheTime = ConfigurationManager.AppSettings["FinancialBenchmarkingCacheHours"].ToInteger() ?? 8;
                var request = HeadRestRequest(urn, companiesHouse);

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
