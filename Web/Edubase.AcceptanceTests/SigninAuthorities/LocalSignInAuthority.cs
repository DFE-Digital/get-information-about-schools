using System.Net;
using Edubase.AcceptanceTests.Users;

namespace Edubase.AcceptanceTests.SigninAuthorities
{
    public sealed class LocalSignInAuthority : ISignInAuthority
    {
        private readonly HttpClient httpClient;
        private readonly string localSignInKey;

        public LocalSignInAuthority(HttpClient httpClient, string localSignInKey)
        {
            ArgumentNullException.ThrowIfNull(httpClient);
            ArgumentException.ThrowIfNullOrWhiteSpace(localSignInKey);
            var websiteAddress = httpClient.BaseAddress;
            if (websiteAddress == null || !websiteAddress.IsLoopback ||
                (websiteAddress.Scheme != Uri.UriSchemeHttps && websiteAddress.Scheme != Uri.UriSchemeHttp))
            {
                throw new ArgumentException("Local sign-in requires an HTTP(S) loopback website.", nameof(httpClient));
            }
            this.httpClient = httpClient;
            this.localSignInKey = localSignInKey;
        }

        public async Task<bool> TrySignInLocally(User user)
        {
            ArgumentNullException.ThrowIfNull(user);
            ArgumentException.ThrowIfNullOrWhiteSpace(user.AttributeStatementValue);
            using var request = new HttpRequestMessage(HttpMethod.Post, "/Account/LocalSignIn")
            {
                Content = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    ["userId"] = user.AttributeStatementValue
                })
            };
            request.Headers.Add("X-Gias-Local-Sign-In-Key", localSignInKey);
            using var response = await httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            if (response.StatusCode != HttpStatusCode.NoContent)
            {
                throw new InvalidOperationException("The website did not return the expected local sign-in response.");
            }
            // The shared, scenario-scoped handler retains the normal application cookie.
            return true;
        }

        public Task<(string SamlResponse, string RelayState)> SignIn(
            User user, Uri authorityLocation, Uri assertionConsumerServiceUrl)
        {
            throw new NotSupportedException("Use TrySignInLocally; this authority does not issue SAML responses.");
        }
    }
}
