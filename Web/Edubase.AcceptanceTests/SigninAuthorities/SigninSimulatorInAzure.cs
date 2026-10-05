using AngleSharp.Common;
using Edubase.AcceptanceTests.GiasFrontEnd;
using Edubase.AcceptanceTests.Apis;
using Edubase.AcceptanceTests.Users;

namespace Edubase.AcceptanceTests.SigninAuthorities
{
    public sealed class SigninSimulatorInAzure : ISignInAuthority
    {
        private readonly HttpClient httpClient;
        private readonly IApi simulatorApi;
        private readonly string environment;

        public SigninSimulatorInAzure(HttpClient httpClient, IApi simulatorApi, string environment)
        {
            this.httpClient = httpClient;
            this.simulatorApi = simulatorApi;
            this.environment = environment;
        }

        public async Task SignIn(User user)
        {
            var (authorityLocation, cookies) = await StartSignIn();
            var assertionConsumerServiceUrl = new Uri(httpClient.BaseAddress!, "/Saml2/Acs");
            var (samlResponse, relayState) = await GetSamlResponse(user, authorityLocation, assertionConsumerServiceUrl);
            await CompleteSignIn(samlResponse, relayState, cookies);
        }

        private async Task<(string SamlResponse, string RelayState)> GetSamlResponse(User user, Uri authorityLocation, Uri assertionConsumerServiceUrl)
        {
            // Step 2: GET to Sign-In Simulator
            var signInSimDocument = await simulatorApi.GetHtmlAsync(authorityLocation.AbsoluteUri);
            var assertionModelId = signInSimDocument.QuerySelector("#AssertionModel_InResponseTo").GetAttribute("value");
            var relayState = signInSimDocument.QuerySelector("#AssertionModel_RelayState").GetAttribute("value");

            // Step 3: POST form to simulator
            var signInFormData = new List<KeyValuePair<string, string>>
            {
                new("CustomDescription", GetCustomDescription(environment)),
                new("AssertionModel.InResponseTo", assertionModelId),
                new("AssertionModel.AssertionConsumerServiceUrl", assertionConsumerServiceUrl.AbsoluteUri),
                new("AssertionModel.Audience", "http://edubase.gov"),
                new("AssertionModel.ResponseBinding", "HttpPost"),
                new("AssertionModel.RelayState", relayState),
                new("AssertionModel.NameId", user.NameId),
                new("AssertionModel.SessionIndex", "42"),
                new("AssertionModel.AttributeStatements.Index", "0"),
                new("AssertionModel.AttributeStatements[0].Type", "http://www.edubase.gov.uk/SAUserId"),
                new("AssertionModel.AttributeStatements[0].Value", user.AttributeStatementValue),
                new("AssertionModel.AttributeStatements.Index", "1"),
                new("AssertionModel.AttributeStatements[1].Type", "urn:oid:2.5.4.45"),
                new("AssertionModel.AttributeStatements[1].Value", user.AttributeStatementValue)
            };

            string signInSimulatorUri = GetSimulatorUrl(environment);
            var signInDocument = await simulatorApi.PostFormAsync(signInSimulatorUri, signInFormData);
            var samlResponse = signInDocument.QuerySelector("input[name='SAMLResponse']").GetAttribute("value");

            return (samlResponse!, relayState!);
        }

        private async Task<(Uri AuthorityLocation, IEnumerable<string> Cookies)> StartSignIn()
        {
            httpClient.DefaultRequestHeaders.Remove("Cookie");

            // Step 1: Initial GET to login page
            using var signInButton = new HttpRequestMessage(HttpMethod.Get, new Uri(httpClient.BaseAddress!, WebRoutes.SignIn));
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

            using var acsRequest = new HttpRequestMessage(HttpMethod.Post, new Uri(httpClient.BaseAddress!, "/Saml2/Acs"))
            {
                Content = acsContent
            };

            var acsResponse = await httpClient.SendAsync(acsRequest);

            var acsCookies = acsResponse.Headers.SingleOrDefault(h => h.Key == "Set-Cookie").Value;
            var aspNetExternalCookie = acsCookies.GetItemByIndex(1);
            var loginCallbackLocation = acsResponse.Headers.Location;

            // Step 5: Final GET to ExternalLoginCallback
            using var externalLoginCallbackRequest = new HttpRequestMessage(HttpMethod.Get, new Uri(httpClient.BaseAddress!, loginCallbackLocation!));
            var externalLoginCallbackResponse = await httpClient.SendAsync(externalLoginCallbackRequest);

            var externalLoginCallbackCookie = externalLoginCallbackResponse.Headers.SingleOrDefault(h => h.Key == "Set-Cookie").Value;
            var aspNetApplicationCookie = externalLoginCallbackCookie.First();

            // Step 6: GET to home page to confirm login
            using var message = new HttpRequestMessage(HttpMethod.Get, new Uri(httpClient.BaseAddress!, "/"));
            message.Headers.Add("Cookie", aspNetApplicationCookie);

            var loggedInResponse = await httpClient.SendAsync(message);

            var loggedInDocument = await loggedInResponse.GetHtmlDocumentAsync();

            var requestVerificationToken = loggedInDocument.QuerySelector("input[name='__RequestVerificationToken']").GetAttribute("value");

            httpClient.DefaultRequestHeaders.Add("Cookie", $"{aspNetExternalCookie}; {aspNetApplicationCookie}; __RequestVerificationToken={requestVerificationToken}");
        }

        private string GetCustomDescription(string environmentName) =>
        environmentName.ToLowerInvariant() switch
        {
            "sandbox1" => "GIAS - Dev / Exp",
            "sandbox2" => "GIAS - Dev / Exp",
            "dev" => "GIAS - Dev / Exp",
            "test" => "GIAS - Stage / Test",
            _ => throw new ArgumentException($"Unexpected environment: {environmentName}", nameof(environmentName))
        };

        private string GetSimulatorUrl(string environmentName) =>
            environmentName.ToLowerInvariant() switch
            {
                "sandbox1" => "https://dfe-sign-in-simulator.azurewebsites.net/c4cdae40-d07b-469e-b505-350e07ee2e32",
                "sandbox2" => "https://dfe-sign-in-simulator.azurewebsites.net/c4cdae40-d07b-469e-b505-350e07ee2e32",
                "dev" => "https://dfe-sign-in-simulator.azurewebsites.net/c4cdae40-d07b-469e-b505-350e07ee2e32",
                "test" => "https://dfe-sign-in-simulator.azurewebsites.net/e00bdaf5-4cee-47c2-b76c-41b00bb59d02",
                _ => throw new ArgumentException($"Unexpected environment: {environmentName}", nameof(environmentName))
            };
    }
}
