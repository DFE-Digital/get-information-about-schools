using System.Net;
using System.Net.Http.Headers;
using System.Text;

namespace Edubase.AcceptanceTests.Authentication
{
    public class AuthenticatedHttpClient
    {
        private readonly AppSettings _appSettings;

        public AuthenticatedHttpClient(AppSettings appSettings)
        {
            _appSettings = appSettings;
        }

        public HttpClient AuthenicatedClient(HttpMessageHandler handler = null)
        {
            var webConfig = _appSettings.WebConfig();

            // If a handler is passed in, it uses it. If not, it defaults to the parameterless constructor
            var client = handler != null
                ? new HttpClient(handler)
                : new HttpClient(new HttpClientHandler
                {
                    AllowAutoRedirect = false,
                    UseDefaultCredentials = true,
                    CookieContainer = new CookieContainer() // fresh container = no cookies
                });

            client.BaseAddress = new Uri(webConfig.WebUri);

            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(
                    "Basic",
                    Convert.ToBase64String(
                        Encoding.UTF8.GetBytes($"{webConfig.Username}:{webConfig.Password}")
                    )
                );

            return client;
        }
    }

}
