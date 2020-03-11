using System;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Caching;
using System.Text;
using Edubase.Common;
using Polly;
using Polly.Wrap;

namespace Edubase.Services.ExternalLookup
{
    public class FBService : IFBService
    {
        private readonly HttpClient _client;
        private static PolicyWrap _retryWithTimeout;
        private static readonly Policy _retryPolicy = Policy.Timeout(1).Wrap(Policy
            .Handle<HttpRequestException>()
            .WaitAndRetry(new[]
            {
                TimeSpan.FromSeconds(1)
            }));

        public FBService()
        {
            var sfbUrl = ConfigurationManager.AppSettings["FinancialBenchmarkingURL"];
            var sfbUsername = ConfigurationManager.AppSettings["FinancialBenchmarkingUsername"];
            var sfbPassword = ConfigurationManager.AppSettings["FinancialBenchmarkingPassword"];

            _client = new HttpClient()
            {
                BaseAddress = new Uri(sfbUrl)
            };
            if (!string.IsNullOrEmpty(sfbUsername) && !string.IsNullOrEmpty(sfbPassword))
            {
                var encodedCredentials = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{sfbUsername}:{sfbPassword}"));
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", encodedCredentials);
            }
            _retryWithTimeout = Policy.TimeoutAsync(1).Wrap(Policy.Handle<HttpRequestException>().WaitAndRetryAsync(new[] { TimeSpan.FromSeconds(1) }));
        }

        public string SchoolURL(int? urn)
        {
            return $"{_client.BaseAddress.AbsoluteUri}school/detail?urn={urn}";
        }

        private HttpRequestMessage HeadSchoolRestRequest(int? urn)
        {
            return new HttpRequestMessage(HttpMethod.Head, $"/school/status?urn={urn}");
        }

        public bool CheckExists(int? urn)
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
                    var result = ExecuteRequest(request);
                    var found = (HttpStatusCode) result == HttpStatusCode.OK;
                    MemoryCache.Default.Set(new CacheItem(key, found), new CacheItemPolicy { AbsoluteExpiration = DateTimeOffset.Now.AddHours(cacheTime) });
                    return found;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
        }

        private dynamic ExecuteRequest(HttpRequestMessage request)
        {
            return _retryPolicy.Execute(() =>
            {
                var response = _client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead).Result;

                return response.StatusCode;
            });
        }
    }
}
