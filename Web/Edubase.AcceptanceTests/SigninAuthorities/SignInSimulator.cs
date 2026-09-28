using AngleSharp.Common;
using Edubase.AcceptanceTests.Api;
using Edubase.AcceptanceTests.Users;

namespace Edubase.AcceptanceTests.SigninAuthorities
{
    public sealed class SignInSimulator : ISignInAuthority
    {
        private readonly HttpClient giasFrontEndClient;
        private readonly HttpClient simulatorClient;
        private readonly string environment;

        public SignInSimulator(HttpClient giasFrontEndClient, HttpClient simulatorClient, string environment)
        {
            this.giasFrontEndClient = giasFrontEndClient;
            this.simulatorClient = simulatorClient;
            this.environment = environment;
        }
        // GIAS owns the login session; the Azure simulator generates the SAML assertion.
        public async Task SignIn(User user)
        {
            giasFrontEndClient.DefaultRequestHeaders.Remove("Cookie");

            var (simulatorLocation, signInCookies) = await StartGiasSignIn();
            var (samlResponse, relayState) = await GetSimulatorSamlResponse(simulatorLocation, user);
            await CompleteGiasSignIn(samlResponse, relayState, signInCookies);
        }

        private async Task<(Uri SimulatorLocation, IEnumerable<string> Cookies)> StartGiasSignIn()
        {
            // Step 1: Initial GET to login page
            var signInButton = new HttpRequestMessage(HttpMethod.Get, new Uri(giasFrontEndClient.BaseAddress!, WebRoutes.SignIn));
            var signInButtonResponse = await giasFrontEndClient.SendAsync(signInButton);

            var signInCookies = signInButtonResponse.Headers.SingleOrDefault(h => h.Key == "Set-Cookie").Value;
            var redirectReturnUrlLocation = signInButtonResponse.Headers.Location;

            return (redirectReturnUrlLocation!, signInCookies);
        }

        private async Task<(string SamlResponse, string RelayState)> GetSimulatorSamlResponse(Uri simulatorLocation, User user)
        {

            // Step 2: GET to Sign-In Simulator
            var signInSimulatorRequest = new HttpRequestMessage(HttpMethod.Get, simulatorLocation);
            var signInSimulatorResponse = await simulatorClient.SendAsync(signInSimulatorRequest);
            signInSimulatorResponse.EnsureSuccessStatusCode();

            var signInSimDocument = await signInSimulatorResponse.GetHtmlDocumentAsync();
            var assertionModelId = signInSimDocument.QuerySelector("#AssertionModel_InResponseTo").GetAttribute("value");
            var relayState = signInSimDocument.QuerySelector("#AssertionModel_RelayState").GetAttribute("value");

            // Step 3: POST form to simulator
            var signInFormData = new List<KeyValuePair<string, string>>
            {
                new("CustomDescription", GetCustomDescription(environment)),
                new("AssertionModel.InResponseTo", assertionModelId),
                new("AssertionModel.AssertionConsumerServiceUrl", new Uri(giasFrontEndClient.BaseAddress!, "/Saml2/Acs").AbsoluteUri),
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

            var signInContent = new FormUrlEncodedContent(signInFormData);

            string signInSimulatorUri = GetSimulatorUrl(environment);
            var signInResponse = await simulatorClient.PostAsync(signInSimulatorUri, signInContent);
            signInResponse.EnsureSuccessStatusCode();

            var signInDocument = await signInResponse.GetHtmlDocumentAsync();
            var samlResponse = signInDocument.QuerySelector("input[name='SAMLResponse']").GetAttribute("value");

            return (samlResponse!, relayState!);
        }

        private async Task CompleteGiasSignIn(string samlResponse, string relayState, IEnumerable<string> signInCookies)
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
                giasFrontEndClient.DefaultRequestHeaders.Add("Cookie", cookie);
            }

            var acsRequest = new HttpRequestMessage(HttpMethod.Post, new Uri(giasFrontEndClient.BaseAddress!, "/Saml2/Acs"))
            {
                Content = acsContent
            };

            var acsResponse = await giasFrontEndClient.SendAsync(acsRequest);

            var acsCookies = acsResponse.Headers.SingleOrDefault(h => h.Key == "Set-Cookie").Value;
            var aspNetExternalCookie = acsCookies.GetItemByIndex(1);
            var loginCallbackLocation = acsResponse.Headers.Location;

            // Step 5: Final GET to ExternalLoginCallback
            var externalLoginCallbackRequest = new HttpRequestMessage(HttpMethod.Get, new Uri(giasFrontEndClient.BaseAddress!, loginCallbackLocation!));
            var externalLoginCallbackResponse = await giasFrontEndClient.SendAsync(externalLoginCallbackRequest);

            var externalLoginCallbackCookie = externalLoginCallbackResponse.Headers.SingleOrDefault(h => h.Key == "Set-Cookie").Value;
            var aspNetApplicationCookie = externalLoginCallbackCookie.First();

            // Step 6: GET to home page to confirm login
            var message = new HttpRequestMessage(HttpMethod.Get, new Uri(giasFrontEndClient.BaseAddress!, "/"));
            message.Headers.Add("Cookie", aspNetApplicationCookie);

            var loggedInResponse = await giasFrontEndClient.SendAsync(message);

            var loggedInDocument = await loggedInResponse.GetHtmlDocumentAsync();

            var requestVerificationToken = loggedInDocument.QuerySelector("input[name='__RequestVerificationToken']").GetAttribute("value");

            giasFrontEndClient.DefaultRequestHeaders.Add("Cookie", $"{aspNetExternalCookie}; {aspNetApplicationCookie}; __RequestVerificationToken={requestVerificationToken}");
        }

        private string GetCustomDescription(string environment) =>
        environment.ToLowerInvariant() switch
        {
            "sandbox1" => "GIAS - Dev / Exp",
            "sandbox2" => "GIAS - Dev / Exp",
            "dev" => "GIAS - Dev / Exp",
            "test" => "GIAS - Stage / Test",
            _ => throw new ArgumentException($"Unexpected environment: {environment}", nameof(environment))
        };

        private string GetSimulatorUrl(string environment) =>
            environment.ToLowerInvariant() switch
            {
                "sandbox1" => "https://dfe-sign-in-simulator.azurewebsites.net/c4cdae40-d07b-469e-b505-350e07ee2e32",
                "sandbox2" => "https://dfe-sign-in-simulator.azurewebsites.net/c4cdae40-d07b-469e-b505-350e07ee2e32",
                "dev" => "https://dfe-sign-in-simulator.azurewebsites.net/c4cdae40-d07b-469e-b505-350e07ee2e32",
                "test" => "https://dfe-sign-in-simulator.azurewebsites.net/e00bdaf5-4cee-47c2-b76c-41b00bb59d02",
                _ => throw new ArgumentException($"Unexpected environment: {environment}", nameof(environment))
            };
    }
}
