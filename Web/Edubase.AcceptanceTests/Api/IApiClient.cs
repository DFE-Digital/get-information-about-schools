using Edubase.AcceptanceTests.Users;

namespace Edubase.AcceptanceTests.Api
{
    public interface IApiClient
    {
        Task Signin(User user);
        Task<T> GetAsync<T>(string url);
    }
}
