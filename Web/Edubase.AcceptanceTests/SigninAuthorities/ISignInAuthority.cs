using Edubase.AcceptanceTests.Users;

namespace Edubase.AcceptanceTests.SigninAuthorities
{
    public interface ISignInAuthority
    {
        // Returns false when the authority requires the existing SAML flow.
        Task<bool> TrySignInLocally(User user) => Task.FromResult(false);

        Task<(string SamlResponse, string RelayState)> SignIn(User user, Uri authorityLocation, Uri assertionConsumerServiceUrl);
    }
}
