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
    public class CSCPService : ICSCPService
    {
        private static HttpClient _client;

        private static readonly Policy RetryPolicy = Policy.TimeoutAsync(1).Wrap(Policy
            .Handle<HttpRequestException>()
            .WaitAndRetryAsync(new[]
            {
                TimeSpan.FromSeconds(1)
            }));

        public CSCPService(HttpClient client)
        {
            _client = client;
        }

        public string SchoolURL(int? urn, string name)
        {
            var safeName = UriHelper.SchoolNameUrl(name);
            return $"{_client.BaseAddress.AbsoluteUri}school/{urn}/{safeName}";
        }

        private HttpRequestMessage HeadSchoolRestRequest(int? urn, string name)
        {
            var safeName = UriHelper.SchoolNameUrl(name);
            return new HttpRequestMessage(HttpMethod.Head, $"school/{urn}/{safeName}");
        }

        public async Task<bool> CheckExists(int? urn, string name)
        {
            var key = $"cscp-{urn}";
            var value = MemoryCache.Default.Get(key);
            if (value != null)
            {
                return (bool) value;
            }
            else
            {
                var cacheTime = ConfigurationManager.AppSettings["CscpCacheHours"].ToInteger() ?? 8;
                var request = HeadSchoolRestRequest(urn, name);

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
