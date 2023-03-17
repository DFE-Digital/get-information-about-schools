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
    public class FSCPDService : IFSCPDService
    {
        private static HttpClient _client;
        private string _matAddress = "multi-academy-trust";
        private string _schoolAddress = "school";

        private static readonly Policy RetryPolicy = Policy.TimeoutAsync(1).Wrap(Policy
            .Handle<HttpRequestException>()
            .WaitAndRetryAsync(new[]
            {
                TimeSpan.FromSeconds(1)
            }));

        public FSCPDService(HttpClient client)
        {
            _client = client;
        }

        private string GetCollection(bool mat)
        {
            var collection = mat ? _matAddress : _schoolAddress;
            return collection;
        }

        public string PublicURL(int? urn, string name, bool mat = false)
        {
            var collection = GetCollection(mat);
            var safeName = UriHelper.SchoolNameUrl(name);
            return $"{_client.BaseAddress.AbsoluteUri}{collection}/{urn}/{safeName}";
        }

        private HttpRequestMessage HeadRestRequest(int? urn, string name, string collection)
        {
            var safeName = UriHelper.SchoolNameUrl(name);
            return new HttpRequestMessage(HttpMethod.Head, $"{collection}/{urn}/{safeName}");
        }

        public async Task<bool> CheckExists(int? urn, string name, bool mat = false)
        {
            var collection = GetCollection(mat);
            var key = $"fscpd-{collection}-{urn}";
            var value = MemoryCache.Default.Get(key);
            if (value != null)
            {
                return (bool) value;
            }
            else
            {
                var cacheTime = ConfigurationManager.AppSettings["FscpdCacheHours"].ToInteger() ?? 8;
                var request = HeadRestRequest(urn, name, collection);

                try
                {
                    using (var response = await RetryPolicy.ExecuteAsync(async () => await _client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead)))
                    {
                        var isOk = response.StatusCode == HttpStatusCode.OK;
                        MemoryCache.Default.Set(new CacheItem(key, isOk), new CacheItemPolicy { AbsoluteExpiration = DateTimeOffset.Now.AddHours(cacheTime) });
                        return isOk;
                    }
                }
                catch
                {
                    return false;
                }
            }
        }
    }
}
