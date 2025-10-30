using System;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Runtime.Caching;
using System.Threading;
using System.Threading.Tasks;
using Edubase.Common;
using Edubase.Services.IntegrationEndPoints;
using Microsoft.Extensions.Configuration;
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
        private readonly HttpClient _client;
        private readonly string urlBaseAddress;
        private readonly string apiBaseAddress;
        private readonly IAsyncPolicy<HttpResponseMessage> RetryPolicy;
        private readonly IConfiguration _configuration;

        public FBService(HttpClient httpClient, IConfiguration configuration)
        {
            _client = httpClient;
            _configuration = configuration;

            // Read configuration values
            apiBaseAddress = configuration["AppSettings:FinancialBenchmarkingApiURL"]
                ?? throw new ArgumentNullException("FinancialBenchmarkingApiURL is missing");

            urlBaseAddress = configuration["AppSettings:FinancialBenchmarkingURL"]
                ?? throw new ArgumentNullException("FinancialBenchmarkingURL is missing");

            var retryCsv = configuration["AppSettings:FBService_RetryIntervals"] ?? "1,2,5";
            var timeoutValue = configuration["FBService_Timeout"] ?? "10";

            var retryIntervals = PollyUtil.CsvSecondsToTimeSpans(retryCsv);
            RetryPolicy = PollyUtil.CreateRetryPolicy(configuration, retryIntervals, timeoutValue);
        }

        public string PublicURL(int? lookupId, FbType lookupType)
        {
            return $"{urlBaseAddress}{PublicUrlPath(lookupId, lookupType)}";
        }

        private string PublicUrlPath(int? lookupId, FbType lookupType)
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

        public string ApiUrl(int? lookupId, FbType lookupType)
        {
            return $"{apiBaseAddress}{ApiUrlPath(lookupId, lookupType)}";
        }

        private string ApiUrlPath(int? lookupId, FbType lookupType)
        {
            var url = $"api/schoolstatus/{lookupId}";
            switch (lookupType)
            {
                case FbType.Trust:
                    url = $"api/truststatus/{lookupId}";
                    break;
                case FbType.Federation:
                    url = $"api/federationstatus/{lookupId}";
                    break;
            }

            return url;
        }

        private HttpRequestMessage HeadRestRequest(int? lookupId, FbType lookupType)
        {
            return new HttpRequestMessage(HttpMethod.Get, ApiUrl(lookupId, lookupType));
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
                var cacheTime = _configuration["AppSettings:FinancialBenchmarkingCacheHours"].ToInteger() ?? 8;
                var request = HeadRestRequest(lookupId, lookupType);

                try
                {
                    using (var response = await RetryPolicy.ExecuteAsync(async (context, cancellationToken) =>
                           {
                               return await _client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead,
                                   cancellationToken);
                           }, [], CancellationToken.None))
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
