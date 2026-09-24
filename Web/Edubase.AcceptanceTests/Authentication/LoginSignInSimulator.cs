using System.Net;
using AngleSharp.Common;
using Edubase.AcceptanceTests.Api;
using Edubase.AcceptanceTests.Users;

namespace Edubase.AcceptanceTests.Authentication
{
    public sealed class LoginSignInSimulator
    {
        private HttpClient httpClient;
        private readonly string environment;

        public LoginSignInSimulator(HttpClient httpClient, string environment)
        {
            this.httpClient = httpClient;
            this.environment = environment;
        }
        /// <summary>
        /// Steps to sign in using the SignInSimulator:
        /// GET https://gias-stage-sis.azurewebsites.net/Account/Login?returnUrl=%2F
        /// Returns 303 See Other
        /// Redirects to https://dfe-sign-in-simulator.azurewebsites.net/e00bdaf5-4cee-47c2-b76c-41b00bb59d02/?
        /// with a SAMLRequest and RelayState as query parameters
        /// Header.Location returned includes SAMLRequest and RelayState
        /// The.AspNet.ApplicationCookie is the session cookie seen when taken back to the home page.
        /// It gets returned in the final ExternalLoginCallback response.
        /// And is used in the final request to the home page.
        /// </summary>
        /// <returns></returns>

        public async Task SignIn(User user)
        {
            httpClient.DefaultRequestHeaders.Remove("Cookie");

            var nameId = user.NameId;
            var attributeStatementValue = user.AttributeStatementValue;

            //var appSettings = new AppSettings();

            // Step 1: Initial GET to login page
            var signInButton = new HttpRequestMessage(HttpMethod.Get, new Uri(httpClient.BaseAddress + WebRoutes.SignIn));
            var signInButtonResponse = await httpClient.SendAsync(signInButton);

            var signInCookies = signInButtonResponse.Headers.SingleOrDefault(h => h.Key == "Set-Cookie").Value;
            var redirectReturnUrlLocation = signInButtonResponse.Headers.Location;

            // Step 2: GET to Sign-In Simulator
            var signInSimulatorRequest = new HttpRequestMessage(HttpMethod.Get, redirectReturnUrlLocation);
            var signInSimulatorResponse = await httpClient.SendAsync(signInSimulatorRequest);
            signInSimulatorResponse.EnsureSuccessStatusCode();

            var signInSimDocument = await signInSimulatorResponse.GetDocumentAsync();
            var assertionModelId = signInSimDocument.QuerySelector("#AssertionModel_InResponseTo").GetAttribute("value");
            var relayState = signInSimDocument.QuerySelector("#AssertionModel_RelayState").GetAttribute("value");

            // Step 3: POST form to simulator
            var signInFormData = new List<KeyValuePair<string, string>>
            {
                new("CustomDescription", GetCustomDescription(environment)),
                new("AssertionModel.InResponseTo", assertionModelId),
                new("AssertionModel.AssertionConsumerServiceUrl", $"{httpClient.BaseAddress}/Saml2/Acs"),
                new("AssertionModel.Audience", "http://edubase.gov"),
                new("AssertionModel.ResponseBinding", "HttpPost"),
                new("AssertionModel.RelayState", relayState),
                new("AssertionModel.NameId", nameId),
                new("AssertionModel.SessionIndex", "42"),
                new("AssertionModel.AttributeStatements.Index", "0"),
                new("AssertionModel.AttributeStatements[0].Type", "http://www.edubase.gov.uk/SAUserId"),
                new("AssertionModel.AttributeStatements[0].Value", attributeStatementValue),
                new("AssertionModel.AttributeStatements.Index", "1"),
                new("AssertionModel.AttributeStatements[1].Type", "urn:oid:2.5.4.45"),
                new("AssertionModel.AttributeStatements[1].Value", attributeStatementValue)
            };

            var signInContent = new FormUrlEncodedContent(signInFormData);

            string signInSimulatorUri = GetAssertionConsumerUrl(environment);
            var signInResponse = await httpClient.PostAsync(signInSimulatorUri, signInContent);

            var signInDocument = await signInResponse.GetDocumentAsync();
            var samlResponse = signInDocument.QuerySelector("input[name='SAMLResponse']").GetAttribute("value");

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

            var acsRequest = new HttpRequestMessage(HttpMethod.Post, new Uri(httpClient.BaseAddress + "/Saml2/Acs"))
            {
                Content = acsContent
            };

            var acsResponse = await httpClient.SendAsync(acsRequest);

            var acsCookies = acsResponse.Headers.SingleOrDefault(h => h.Key == "Set-Cookie").Value;
            var aspNetExternalCookie = acsCookies.GetItemByIndex(1);
            var loginCallbackLocation = acsResponse.Headers.Location;

            // Step 5: Final GET to ExternalLoginCallback
            var externalLoginCallbackRequest = new HttpRequestMessage(HttpMethod.Get, new Uri(httpClient.BaseAddress + loginCallbackLocation.ToString()));
            var externalLoginCallbackResponse = await httpClient.SendAsync(externalLoginCallbackRequest);

            var externalLoginCallbackCookie = externalLoginCallbackResponse.Headers.SingleOrDefault(h => h.Key == "Set-Cookie").Value;
            var aspNetApplicationCookie = externalLoginCallbackCookie.First();

            // Step 6: GET to home page to confirm login
            var message = new HttpRequestMessage(HttpMethod.Get, new Uri(httpClient.BaseAddress + "/"));
            message.Headers.Add("Cookie", aspNetApplicationCookie);

            var loggedInResponse = await httpClient.SendAsync(message);

            var loggedInDocument = await loggedInResponse.GetDocumentAsync();

            var requestVerificationToken = loggedInDocument.QuerySelector("input[name='__RequestVerificationToken']").GetAttribute("value");

            httpClient.DefaultRequestHeaders.Add("Cookie", $"{aspNetExternalCookie}; {aspNetApplicationCookie}; __RequestVerificationToken={requestVerificationToken}");
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

        private string GetAssertionConsumerUrl(string? environment) =>
            environment.ToLower() switch
            {
                "sandbox1" => WebRoutes.SignInSimulatorDev,
                "sandbox2" => WebRoutes.SignInSimulatorDev,
                "dev" => WebRoutes.SignInSimulatorDev,
                "test" => WebRoutes.SignInSimulatorTest,
                _ => throw new ArgumentException($"Unexpected environment: {environment}", nameof(environment))
            };
    }
}
