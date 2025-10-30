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
    public class FSCPDService : IFSCPDService
    {
        private readonly HttpClient _client;
        private readonly IAsyncPolicy<HttpResponseMessage> _retryPolicy;
        private readonly IConfiguration _configuration;

        private const string MatAddress = "multi-academy-trust";
        private const string SchoolAddress = "school";
        

        public FSCPDService(HttpClient client, IConfiguration configuration)
        {
            _client = client;
            _configuration = configuration;

            var retryCsv = configuration["AppSettings:FscpdClient_RetryIntervals"] ?? "1,2,5";
            var timeoutValue = configuration["AppSettings:FscpdClient_Timeout"] ?? "10";

            var retryIntervals = PollyUtil.CsvSecondsToTimeSpans(retryCsv);
            _retryPolicy = PollyUtil.CreateRetryPolicy(configuration, retryIntervals, timeoutValue);
        }

        private string GetCollection(bool mat)
        {
            var collection = mat ? MatAddress : SchoolAddress;
            return collection;
        }

        public string PublicURL(int? urn, string name, bool mat = false)
        {
            var collection = GetCollection(mat);
            var originalName = name?.Trim() ?? string.Empty;
            var safeName = UriHelper.SchoolNameUrl(originalName);

            if (safeName.EndsWith("1") && !originalName.EndsWith("1") && !originalName.EndsWith(" 1"))
            {
                safeName = safeName.Substring(0, safeName.Length - 1);
            }
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

            var productValue = new ProductInfoHeaderValue("GIAS", "1.0.0");
            var commentValue = new ProductInfoHeaderValue("(Chrome; Edge; Mozilla; +https://www.get-information-schools.service.gov.uk)");

            if (value != null)
            {
                return (bool) value;
            }
            else
            {
                var cacheTime = _configuration["AppSettings:FscpdCacheHours"].ToInteger() ?? 8;
                var request = HeadRestRequest(urn, name, collection);

                request.Headers.UserAgent.Add(productValue);
                request.Headers.UserAgent.Add(commentValue);

                try
                {
                    using (var response = await _retryPolicy.ExecuteAsync(async () =>
                        await _client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead)))
                    {
                        var isOk = response.StatusCode == HttpStatusCode.OK;
                        MemoryCache.Default.Set(new CacheItem(key, isOk),
                            new CacheItemPolicy { AbsoluteExpiration = DateTimeOffset.Now.AddHours(cacheTime) });
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
