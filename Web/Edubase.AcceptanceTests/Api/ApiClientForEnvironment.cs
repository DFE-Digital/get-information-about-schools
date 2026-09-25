using Edubase.AcceptanceTests.Users;

namespace Edubase.AcceptanceTests.Api
{
    public class ApiClientForEnvironment : IApiClient
    {
        private readonly string environment;
        private HttpClient httpClient { get; }

        public ApiClientForEnvironment(HttpClient httpClient, string environment)
        {
            this.httpClient = httpClient;
            this.environment = environment;
        }

        public async Task Signin(User user)
        {
            var signinSimulator = new SignInSimulator(httpClient, environment);

            await signinSimulator.SignIn(user);
        }

        public async Task<HttpResponseMessage> GetAsync(string url)
        {
            var response = await httpClient.GetAsync($"{httpClient.BaseAddress}{url}");

            return response;
        }
    }
}
