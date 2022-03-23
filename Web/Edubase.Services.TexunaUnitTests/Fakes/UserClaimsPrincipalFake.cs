using System.Security.Claims;

namespace Edubase.Services.TexunaUnitTests.FakeData
{
    public class UserClaimsPrincipalFake
    {
        public ClaimsPrincipal GetUserClaimsPrincipal()
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Email, "fakeuser@email.com"),
            }, "mock"));

            return user;
        }
    }
}
