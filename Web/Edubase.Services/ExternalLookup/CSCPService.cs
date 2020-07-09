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
    public class CSCPService : ICSCPService
    {
        private readonly HttpClient _client;
        private static PolicyWrap _retryWithTimeout;
        private static readonly Policy _retryPolicy = Policy.Timeout(1).Wrap(Policy
            .Handle<HttpRequestException>()
            .WaitAndRetry(new[]
            {
                TimeSpan.FromSeconds(1)
            }));

        public CSCPService()
        {
            var cscpUrl = ConfigurationManager.AppSettings["CscpURL"];
            var cscpUsername = ConfigurationManager.AppSettings["CscpUsername"];
            var cscpPassword = ConfigurationManager.AppSettings["CscpPassword"];

            _client = new HttpClient()
            {
                BaseAddress = new Uri(cscpUrl)
            };
            if (!string.IsNullOrEmpty(cscpUsername) && !string.IsNullOrEmpty(cscpPassword))
            {
                var encodedCredentials = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{cscpUsername}:{cscpPassword}"));
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", encodedCredentials);
            }
            _retryWithTimeout = Policy.TimeoutAsync(1).Wrap(Policy.Handle<HttpRequestException>().WaitAndRetryAsync(new[] { TimeSpan.FromSeconds(1) }));
        }

        public string SchoolURL(int? urn, string name)
        {
            var safeName = UriHelper.SchoolNameUrl(name);
            return $"{_client.BaseAddress.AbsoluteUri}school/{urn}/{safeName}";
        }


        private HttpRequestMessage HeadSchoolRestRequest(int? urn, string name)
        {
            var safeName = UriHelper.SchoolNameUrl(name);
            return new HttpRequestMessage(HttpMethod.Head, $"/school/{urn}/{safeName}");
        }

        public bool CheckExists(int? urn, string name)
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
