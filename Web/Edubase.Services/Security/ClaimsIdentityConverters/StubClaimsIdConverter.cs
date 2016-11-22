using Edubase.Services.Security.Permissions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Edubase.Common;
using Newtonsoft.Json;
using Microsoft.AspNet.Identity;

namespace Edubase.Services.Security.ClaimsIdentityConverters
{
    /// <summary>
    /// Responsible for converting the incoming external claim identity from the Stub IDP into the internally-used Claims Id.
    /// </summary>
    public class StubClaimsIdConverter : IClaimsIdConverter
    {
        public ClaimsIdentity Convert(ClaimsIdentity id)
        {
            var nameClaimType = id.NameClaimType;
            var roleClaimType = id.RoleClaimType;

            var nameClaim = id.FindFirst(nameClaimType);
            var roleClaim = id.FindFirst(roleClaimType);
            var idClaim = id.FindFirst(ClaimTypes.NameIdentifier);

            var editEstablishmentClaim = id.FindFirst(EduClaimTypes.EditEstablishment);
            var editGroupClaim = id.FindFirst(EduClaimTypes.EditGroup);
            var createEstablishmentClaim = id.FindFirst(EduClaimTypes.CreateEstablishment);
            var createGroupClaim = id.FindFirst(EduClaimTypes.CreateGroup);

            var claims = new List<Claim>();
            if (nameClaim != null) claims.Add(nameClaim);
            if (roleClaim != null) claims.Add(roleClaim);
            if (idClaim != null) claims.Add(idClaim);
            if (editEstablishmentClaim != null) claims.Add(editEstablishmentClaim);
            if (editGroupClaim != null) claims.Add(editGroupClaim);
            if (createEstablishmentClaim != null) claims.Add(createEstablishmentClaim);
            if (createGroupClaim != null) claims.Add(createGroupClaim);

            var authType = DefaultAuthenticationTypes.ApplicationCookie;

            return new ClaimsIdentity(claims, authType, nameClaimType, roleClaimType);
        }
        
    }
}
