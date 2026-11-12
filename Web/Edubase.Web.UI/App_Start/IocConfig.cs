using System;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using Edubase.Services.Texuna.Core;
using Edubase.Services.Texuna.Serialization;
using Newtonsoft.Json;

namespace Edubase.Web.UI
{
    public static class IocConfig
    {
        private const string Basic = "Basic";

        public static JsonMediaTypeFormatter CreateJsonMediaTypeFormatter()
        {
            return new JsonMediaTypeFormatter
            {
                SerializerSettings = new JsonSerializerSettings
                {
                    Formatting = Formatting.Indented,
                    ContractResolver = new TexunaCamelCasePropertyNamesContractResolver(),
                    DateTimeZoneHandling = DateTimeZoneHandling.Unspecified
                }
            };
        }

        public static HttpClient CreateLookupClient(string lookupApiAddress, string lookupApiUsername, string lookupApiPassword)
        {
            if (string.IsNullOrWhiteSpace(lookupApiAddress))
            {
                throw new ArgumentNullException(nameof(lookupApiAddress));
            }

            var client = new HttpClient(new HttpClientHandler { UseCookies = false })
            {
                BaseAddress = new Uri(lookupApiAddress)
            };

            if (!string.IsNullOrEmpty(lookupApiUsername) && !string.IsNullOrEmpty(lookupApiPassword))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                    Basic,
                    new BasicAuthCredentials(lookupApiUsername, lookupApiPassword).ToString());
            }

            return client;
        }
    }
}
