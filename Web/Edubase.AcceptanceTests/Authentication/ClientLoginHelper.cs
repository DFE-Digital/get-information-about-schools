namespace Edubase.AcceptanceTests.Authentication
{
    public sealed class ClientLoginHelper
    {
        private readonly AuthenticatedHttpClient _authenticatedHttpClient;
        private readonly AppSettings _appSettings;

        public ClientLoginHelper(AuthenticatedHttpClient authenticatedHttpClient, AppSettings appSettings)
        {
            _authenticatedHttpClient = authenticatedHttpClient;
            _appSettings = appSettings;
        }
        public async Task<HttpClient> GetClientForUserAsync(string userKey)
        {
            var newClient = _authenticatedHttpClient.AuthenicatedClient();
            var simulator = new LoginSignInSimulator(newClient, _appSettings.UserConfig());
            await simulator.SignInClient(userKey);
            return newClient;
        }
    }
}
