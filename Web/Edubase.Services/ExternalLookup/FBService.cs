using System;
using System.Configuration;
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

        public string SchoolURL(int? urn)
        {
            return $"{_client.BaseAddress.AbsoluteUri}school/detail?urn={urn}";
        }

        private HttpRequestMessage HeadSchoolRestRequest(int? urn)
        {
            return new HttpRequestMessage(HttpMethod.Head, $"school/status?urn={urn}");
        }

        public async Task<bool> CheckExists(int? urn)
        {
            var key = $"sfb-{urn}";
            var value = MemoryCache.Default.Get(key);
            if (value != null)
            {
                return (bool) value;
            }
            else
            {
                var cacheTime = ConfigurationManager.AppSettings["FinancialBenchmarkingCacheHours"].ToInteger() ?? 8;
                var request = HeadSchoolRestRequest(urn);

                try
                {
                    using (var response = await RetryPolicy.ExecuteAsync(async () => await _client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead)))
                    {
                        var isSuccess = response.IsSuccessStatusCode;
                        MemoryCache.Default.Set(new CacheItem(key, isSuccess), new CacheItemPolicy { AbsoluteExpiration = DateTimeOffset.Now.AddHours(cacheTime) });
                        return isSuccess;
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
