using Edubase.AcceptanceTests.SigninAuthorities;
using Edubase.AcceptanceTests.Users;

namespace Edubase.AcceptanceTests.Api
{
    public class ApiForGiasFrontEnd : IApiClient
    {
        private HttpClient httpClient;
        private readonly ISignInAuthority signinAuthority;

        public ApiForGiasFrontEnd(HttpClient httpClient, ISignInAuthority signinAuthority)
        {
            this.httpClient = httpClient;
            this.signinAuthority = signinAuthority;
        }

        public async Task Signin(User user)
        {
            await signinAuthority.SignIn(user);
        }

        public async Task<HttpResponseMessage> GetAsync(string url)
        {
            var response = await httpClient.GetAsync($"{httpClient.BaseAddress}{url}");

            return response;
        }
    }
}
