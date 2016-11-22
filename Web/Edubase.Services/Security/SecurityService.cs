using Edubase.Common;
using Edubase.Services.Domain;
using Edubase.Services.Security.Permissions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Edubase.Services.Security
{
    public class SecurityService
    {
        public EditEstablishmentPermissions GetEditEstablishmentPermission(IPrincipal principal)
            => principal.AsClaimsPrincipal().GetEditEstablishmentPermissions();
        
        public CreateEstablishmentPermissions GetCreateEstablishmentPermission(IPrincipal principal) 
            => principal.AsClaimsPrincipal().GetCreateEstablishmentPermissions();

        public CreateGroupPermissions GetCreateGroupPermission(IPrincipal principal)
            => principal.AsClaimsPrincipal().GetCreateGroupPermissions();

        public EditGroupPermissions GetEditGroupPermission(IPrincipal principal)
            => principal.AsClaimsPrincipal().GetEditGroupPermissions();



    }
}
