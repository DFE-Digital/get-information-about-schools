using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using Edubase.Services.Security.ClaimsIdentityConverters;
using Edubase.Services.Domain;

namespace Edubase.Services.Security
{
    public interface ISecurityService
    {
        IPrincipal CreateAnonymousPrincipal();
        Task<CreateGroupPermissionDto> GetCreateGroupPermissionAsync(IPrincipal principal);
        Task<CreateEstablishmentPermissionDto> GetCreateEstablishmentPermissionAsync(IPrincipal principal);
        Task<string[]> GetRolesAsync(IPrincipal principal);

        Task<int?> GetMyEstablishmentUrn(IPrincipal principal);
        Task<int?> GetMyMATUId(IPrincipal principal);

    }
}