using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using Edubase.Services.Security.ClaimsIdentityConverters;

namespace Edubase.Services.Security
{
    public interface ISecurityService
    {
        IPrincipal CreateAnonymousPrincipal();
    }
}