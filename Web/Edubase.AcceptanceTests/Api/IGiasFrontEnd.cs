using Edubase.AcceptanceTests.Users;

namespace Edubase.AcceptanceTests.Api
{
    public interface IGiasFrontEnd
    {
        Task SignIn(User user);
        Task<T> GetAsync<T>(string url);
    }
}
