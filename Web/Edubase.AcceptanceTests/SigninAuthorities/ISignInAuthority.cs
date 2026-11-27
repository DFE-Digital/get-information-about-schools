using Edubase.AcceptanceTests.Users;

namespace Edubase.AcceptanceTests.SigninAuthorities
{
    public interface ISignInAuthority
    {
        Task SignIn(User user);
        Task SignOut(User user);
    }
}
