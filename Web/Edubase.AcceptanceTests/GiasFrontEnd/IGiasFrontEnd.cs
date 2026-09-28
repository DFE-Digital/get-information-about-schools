using Edubase.AcceptanceTests.Users;

namespace Edubase.AcceptanceTests.GiasFrontEnd
{
    public interface IGiasFrontEnd : IApi
    {
        Task SignIn(User user);
    }
}
