using AngleSharp.Common;
using Edubase.AcceptanceTests.SigninAuthorities;
using Edubase.AcceptanceTests.Users;

namespace Edubase.AcceptanceTests.GiasFrontEnd
{
    public class GiasFrontEndViaHttp : HttpApi, IGiasFrontEnd
    {
        private readonly ISignInAuthority signInAuthority;

        public GiasFrontEndViaHttp(HttpClient httpClient, ISignInAuthority signInAuthority)
            : base(httpClient)
        {
            this.signInAuthority = signInAuthority;
        }

        public async Task SignIn(User user)
        {
            var (authorityLocation, cookies) = await StartSignIn();
            var assertionConsumerServiceUrl = new Uri(httpClient.BaseAddress!, "/Saml2/Acs");
            var (samlResponse, relayState) = await signInAuthority.SignIn(user, authorityLocation, assertionConsumerServiceUrl);
            await CompleteSignIn(samlResponse, relayState, cookies);
        }

        private async Task<(Uri AuthorityLocation, IEnumerable<string> Cookies)> StartSignIn()
        {
            httpClient.DefaultRequestHeaders.Remove("Cookie");

            // Step 1: Initial GET to login page
            var signInButton = new HttpRequestMessage(HttpMethod.Get, new Uri(httpClient.BaseAddress!, WebRoutes.SignIn));
            var signInButtonResponse = await httpClient.SendAsync(signInButton);

            var signInCookies = signInButtonResponse.Headers.SingleOrDefault(h => h.Key == "Set-Cookie").Value;
            var redirectReturnUrlLocation = signInButtonResponse.Headers.Location;

            return (redirectReturnUrlLocation!, signInCookies);
        }

        private async Task CompleteSignIn(string samlResponse, string relayState, IEnumerable<string> signInCookies)
        {
            // Step 4: POST SAML response to ACS
            var acsFormData = new List<KeyValuePair<string, string>>
            {
                new("RelayState", relayState),
                new("SAMLResponse", samlResponse)
            };

            var acsContent = new FormUrlEncodedContent(acsFormData);
            foreach (var cookie in signInCookies)
            {
                httpClient.DefaultRequestHeaders.Add("Cookie", cookie);
            }

            var acsRequest = new HttpRequestMessage(HttpMethod.Post, new Uri(httpClient.BaseAddress!, "/Saml2/Acs"))
            {
                Content = acsContent
            };

            var acsResponse = await httpClient.SendAsync(acsRequest);

            var acsCookies = acsResponse.Headers.SingleOrDefault(h => h.Key == "Set-Cookie").Value;
            var aspNetExternalCookie = acsCookies.GetItemByIndex(1);
            var loginCallbackLocation = acsResponse.Headers.Location;

            // Step 5: Final GET to ExternalLoginCallback
            var externalLoginCallbackRequest = new HttpRequestMessage(HttpMethod.Get, new Uri(httpClient.BaseAddress!, loginCallbackLocation!));
            var externalLoginCallbackResponse = await httpClient.SendAsync(externalLoginCallbackRequest);

            var externalLoginCallbackCookie = externalLoginCallbackResponse.Headers.SingleOrDefault(h => h.Key == "Set-Cookie").Value;
            var aspNetApplicationCookie = externalLoginCallbackCookie.First();

            // Step 6: GET to home page to confirm login
            var message = new HttpRequestMessage(HttpMethod.Get, new Uri(httpClient.BaseAddress!, "/"));
            message.Headers.Add("Cookie", aspNetApplicationCookie);

            var loggedInResponse = await httpClient.SendAsync(message);

            var loggedInDocument = await loggedInResponse.GetHtmlDocumentAsync();

            var requestVerificationToken = loggedInDocument.QuerySelector("input[name='__RequestVerificationToken']").GetAttribute("value");

            httpClient.DefaultRequestHeaders.Add("Cookie", $"{aspNetExternalCookie}; {aspNetApplicationCookie}; __RequestVerificationToken={requestVerificationToken}");
        }
    }
}
