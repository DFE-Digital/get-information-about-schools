using Edubase.AcceptanceTests.Users;

namespace Edubase.AcceptanceTests.Api
{
    public interface IApiClient
    {
        Task<T> GetAsync<T>(string url);
        Task Signin(User user);
    }
}
