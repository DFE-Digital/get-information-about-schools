using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using Edubase.Data.Identity;
using Edubase.Services.Security.ClaimsIdentityConverters;
using Edubase.Services.Security.Permissions;

namespace Edubase.Services.Security
{
    public interface ISecurityService
    {
        /// <summary>
        /// Creates a system principal whose permissions are the same as Admin.
        /// This allows internal services to gain full permissions.
        /// </summary>
        /// <returns></returns>
        IPrincipal CreateSystemPrincipal();
        CreateEstablishmentPermissions GetCreateEstablishmentPermission(IPrincipal principal);
        CreateGroupPermissions GetCreateGroupPermission(IPrincipal principal);
        EditEstablishmentPermissions GetEditEstablishmentPermission(IPrincipal principal);
        EditGroupPermissions GetEditGroupPermission(IPrincipal principal);
        Task<ClaimsIdentity> LoginAsync(ClaimsIdentity id, IClaimsIdConverter claimsIdConverter, ApplicationUserManager userManager);
        string GetUserId(IPrincipal principal);
    }
}