using Edubase.AcceptanceTests.Apis;
using Edubase.AcceptanceTests.SigninAuthorities;
using Edubase.AcceptanceTests.Users;

namespace Edubase.AcceptanceTests.GiasFrontEnd
{
    public class GiasFrontEndViaHttp : HttpApi, IGiasFrontEnd
    {
        private readonly ISignInAuthority signInAuthority;

        public GiasFrontEndViaHttp(HttpClient httpClient, ISignInAuthority signInAuthority)
            : base(httpClient)
        {
            this.signInAuthority = signInAuthority;
        }

        public Task SignIn(User user) => signInAuthority.SignIn(user);
    }
}
