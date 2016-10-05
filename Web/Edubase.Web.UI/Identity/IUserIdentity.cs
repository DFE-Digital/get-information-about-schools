using System.Security.Claims;

namespace Edubase.Web.UI.Identity
{
    public interface IUserIdentity
    {
        Claim FindFirstClaim(string type);
        bool IsInRole(string role);
    }
}