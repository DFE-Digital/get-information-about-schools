using System.Net;
using AngleSharp.Common;
using FluentAssertions;

namespace Edubase.AcceptanceTests.Authentication
{
    public sealed class LoginSignInSimulator
    {
        private HttpClient _client;
        private readonly UserOptions _userConfig;

        public LoginSignInSimulator(HttpClient client, UserOptions userConfig)
        {
            _client = client;
            _userConfig = userConfig ?? throw new ArgumentNullException(nameof(userConfig));

            if (_userConfig.Users == null || !_userConfig.Users.Any())
                throw new InvalidOperationException("UserOptions.Users is null or empty. Check your configuration file and environment.");
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

        public async Task<IEnumerable<string>> SignInClient(string userKey)
        {
            _client.DefaultRequestHeaders.Remove("Cookie");

            if (!_userConfig.Users.TryGetValue(userKey, out var user))
                throw new ArgumentException($"User '{userKey}' not found in configuration.");

            var nameId = user.NameId;
            var attributeStatementValue = user.AttributeStatementValue;

            var appSettings = new AppSettings();

            // Step 1: Initial GET to login page
            var signInButton = new HttpRequestMessage(HttpMethod.Get, new Uri(appSettings.WebConfig().WebUri + WebRoutes.SignIn));
            var signInButtonResponse = await _client.SendAsync(signInButton);
            signInButtonResponse.StatusCode.Should().Be(HttpStatusCode.SeeOther);

            var signInCookies = signInButtonResponse.Headers.SingleOrDefault(h => h.Key == "Set-Cookie").Value;
            var redirectReturnUrlLocation = signInButtonResponse.Headers.Location;
            redirectReturnUrlLocation.Should().NotBeNull();

            // Step 2: GET to Sign-In Simulator
            var signInSimulatorRequest = new HttpRequestMessage(HttpMethod.Get, redirectReturnUrlLocation);
            var signInSimulatorResponse = await _client.SendAsync(signInSimulatorRequest);
            signInSimulatorResponse.EnsureSuccessStatusCode();

            var signInSimDocument = await signInSimulatorResponse.GetDocumentAsync();
            var assertionModelId = signInSimDocument.QuerySelector("#AssertionModel_InResponseTo").GetAttribute("value");
            var relayState = signInSimDocument.QuerySelector("#AssertionModel_RelayState").GetAttribute("value");

            // Step 3: POST form to simulator
            var signInFormData = new List<KeyValuePair<string, string>>
            {
                new("CustomDescription", GetCustomDescription(appSettings.TestConfig().Environment)),
                new("AssertionModel.InResponseTo", assertionModelId),
                new("AssertionModel.AssertionConsumerServiceUrl", $"{appSettings.WebConfig().WebUri}/Saml2/Acs"),
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

            string signInSimulatorUri = GetAssertionConsumerUrl(appSettings.TestConfig().Environment);
            var signInResponse = await _client.PostAsync(signInSimulatorUri, signInContent);
            signInResponse.StatusCode.Should().Be(HttpStatusCode.OK);

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
                _client.DefaultRequestHeaders.Add("Cookie", cookie);
            }

            var acsRequest = new HttpRequestMessage(HttpMethod.Post, new Uri(appSettings.WebConfig().WebUri + "/Saml2/Acs"))
            {
                Content = acsContent
            };

            var acsResponse = await _client.SendAsync(acsRequest);
            acsResponse.StatusCode.Should().Be(HttpStatusCode.SeeOther);

            var acsCookies = acsResponse.Headers.SingleOrDefault(h => h.Key == "Set-Cookie").Value;
            var aspNetExternalCookie = acsCookies.GetItemByIndex(1);
            var loginCallbackLocation = acsResponse.Headers.Location;
            loginCallbackLocation.Should().NotBeNull();

            // Step 5: Final GET to ExternalLoginCallback
            var externalLoginCallbackRequest = new HttpRequestMessage(HttpMethod.Get, new Uri(appSettings.WebConfig().WebUri + loginCallbackLocation.ToString()));
            var externalLoginCallbackResponse = await _client.SendAsync(externalLoginCallbackRequest);
            externalLoginCallbackResponse.StatusCode.Should().Be(HttpStatusCode.Found);

            var externalLoginCallbackCookie = externalLoginCallbackResponse.Headers.SingleOrDefault(h => h.Key == "Set-Cookie").Value;
            var aspNetApplicationCookie = externalLoginCallbackCookie.First();

            // Step 6: GET to home page to confirm login
            var message = new HttpRequestMessage(HttpMethod.Get, new Uri(appSettings.WebConfig().WebUri + "/"));
            message.Headers.Add("Cookie", aspNetApplicationCookie);

            var loggedInResponse = await _client.SendAsync(message);
            loggedInResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            var loggedInDocument = await loggedInResponse.GetDocumentAsync();
            loggedInDocument.QuerySelector("#logout-link").Should().NotBeNull();

            var requestVerificationToken = loggedInDocument.QuerySelector("input[name='__RequestVerificationToken']").GetAttribute("value");

            _client.DefaultRequestHeaders.Add("Cookie", $"{aspNetExternalCookie}; {aspNetApplicationCookie}; __RequestVerificationToken={requestVerificationToken}");

            return _client.DefaultRequestHeaders.GetValues("Cookie");
        }

        public Task<IEnumerable<string>> SignInClientBackOffice() =>
            SignInClient("backOfficeUserId");

        public Task<IEnumerable<string>> SignInClientAcademySecureSixteenToNineteenUser() =>
            SignInClient("academySecure16To19Id");

        public Task<IEnumerable<string>> SignInClientIebt() =>
            SignInClient("iebtUserNameId");

        public Task<IEnumerable<string>> SignInClientOliveAppAcademy() =>
            SignInClient("oliveAppAcademyId");

        public Task<IEnumerable<string>> SignInClientParkHillPrimarySchool() =>
            SignInClient("parkHillPrimarySchoolId");

        public Task<IEnumerable<string>> SignInClientServiceChildrensEducation() =>
            SignInClient("serviceChildrensEducationId");

        public Task<IEnumerable<string>> SignInClientTrams() =>
            SignInClient("tramsId");

        public Task<IEnumerable<string>> SignInClientWestLondonFreeSchool() =>
            SignInClient("westLondonFreeSchoolId");

        public Task<IEnumerable<string>> SignInClientYcs() =>
            SignInClient("ycsId");

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

    public class AuthenticatedStrings
    {
        public string AspNetExternalCookie { get; set; }
        public string AspNetApplicationCookie { get; set; }
        public string RequestVerificationToken { get; set; }
    }
}
