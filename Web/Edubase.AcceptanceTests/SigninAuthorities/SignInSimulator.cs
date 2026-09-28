using Edubase.AcceptanceTests.Apis;
using Edubase.AcceptanceTests.Users;

namespace Edubase.AcceptanceTests.SigninAuthorities
{
    public sealed class SignInSimulator : ISignInAuthority
    {
        private readonly HttpClient simulatorClient;
        private readonly string environment;

        public SignInSimulator(HttpClient simulatorClient, string environment)
        {
            this.simulatorClient = simulatorClient;
            this.environment = environment;
        }
        // GIAS owns the login session; the Azure simulator generates the SAML assertion.
        public async Task<(string SamlResponse, string RelayState)> SignIn(User user, Uri authorityLocation, Uri assertionConsumerServiceUrl)
        {
            // Step 2: GET to Sign-In Simulator
            var signInSimulatorRequest = new HttpRequestMessage(HttpMethod.Get, authorityLocation);
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

            var signInContent = new FormUrlEncodedContent(signInFormData);

            string signInSimulatorUri = GetSimulatorUrl(environment);
            var signInResponse = await simulatorClient.PostAsync(signInSimulatorUri, signInContent);
            signInResponse.EnsureSuccessStatusCode();

            var signInDocument = await signInResponse.GetHtmlDocumentAsync();
            var samlResponse = signInDocument.QuerySelector("input[name='SAMLResponse']").GetAttribute("value");

            return (samlResponse!, relayState!);
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
