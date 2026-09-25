using Edubase.AcceptanceTests.Users;

namespace Edubase.AcceptanceTests.Api
{
    public interface IApiClient
    {
        Task<HttpResponseMessage> GetAsync(string url);
        Task Signin(User user);
    }
}
