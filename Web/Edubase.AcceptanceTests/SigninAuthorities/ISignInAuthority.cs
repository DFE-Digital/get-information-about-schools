using Edubase.AcceptanceTests.Users;

namespace Edubase.AcceptanceTests.SigninAuthorities
{
    public interface ISignInAuthority
    {
        Task<(string SamlResponse, string RelayState)> SignIn(User user, Uri authorityLocation, Uri assertionConsumerServiceUrl);
    }
}
