using System.Security.Claims;

namespace Web.Identity
{
    public interface IUserIdentity
    {
        Claim FindFirstClaim(string type);
        bool IsInRole(string role);
    }
}