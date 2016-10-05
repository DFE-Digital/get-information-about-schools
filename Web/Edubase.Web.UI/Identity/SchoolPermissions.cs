using System;
using System.Collections.Generic;
using System.Linq;

namespace Edubase.Web.UI.Identity
{
    public class SchoolPermissions : ISchoolPermissions
    {
        private readonly IUserIdentity _userIdentity;

        public SchoolPermissions(IUserIdentity userIdentity)
        {
            _userIdentity = userIdentity;
        }

        public IEnumerable<int> GetAccessibleSchoolIds()
        {
            var claim = _userIdentity.FindFirstClaim(IdentityConstants.AccessibleSchoolIdsClaimTypeName);
            return claim?.Value?.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(Int32.Parse) ?? new int[] { };
        }

        public void EnsureHasAccessToSchool(int id)
        {
            if (!GetAccessibleSchoolIds().Contains(id) && !_userIdentity.IsInRole(IdentityConstants.AccessAllSchoolsRoleName))
            {
                throw new UnauthorizedAccessException($"User does not have permission to access school with ID {id}");
            }
        }
    }
}
