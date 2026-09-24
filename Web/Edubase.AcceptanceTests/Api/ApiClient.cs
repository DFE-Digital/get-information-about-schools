using System.Net;
using System.Net.Http.Headers;
using System.Text;
using Edubase.AcceptanceTests.Authentication;
using Edubase.AcceptanceTests.Users;

namespace Edubase.AcceptanceTests.Api
{
    public class ApiClient
    {
        private readonly string environment;

        public HttpClient HttpClient { get; }
        public User User { get; }

        public ApiClient(string baseAddress, string environment)
        {
            this.environment = environment;

            HttpClient = new HttpClient(new HttpClientHandler
            {
                AllowAutoRedirect = false,
                UseDefaultCredentials = true,
                CookieContainer = new CookieContainer() // fresh container = no cookies
            });

            HttpClient.BaseAddress = new Uri(baseAddress);

            var basicAuthorizationUserName = string.Empty;
            var basicAuthorizationUserPassword = string.Empty;

            HttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(
                    "Basic",
                    Convert.ToBase64String(
                        Encoding.UTF8.GetBytes($"{basicAuthorizationUserName}:{basicAuthorizationUserPassword}")
                    )
                );
            this.environment = environment;
        }

        internal async Task Signin(User user)
        {
            var signinSimulator = new LoginSignInSimulator(HttpClient, environment);

            await signinSimulator.SignIn(user);
        }
    }
}
