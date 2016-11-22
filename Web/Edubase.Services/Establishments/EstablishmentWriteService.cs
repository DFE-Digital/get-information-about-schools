using Edubase.Data.Entity;
using Edubase.Services.Exceptions;
using Edubase.Services.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using MoreLinq;
using Edubase.Common;
using Edubase.Services.Security.Permissions;
using Edubase.Services.Domain;

namespace Edubase.Services.Establishments
{
    public class EstablishmentWriteService
    {
        private ApplicationDbContext _dbContext;

        public EstablishmentWriteService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }


        public async Task UpdateAsync(EstablishmentDto dto, ClaimsPrincipal principal)
        {
            var predicate = principal.GetEditEstablishmentPermissions();
            Guard.IsFalse(predicate.HasNoEditingPermission, () => new PermissionDeniedException("User does not have editing permissions"));
            Guard.IsTrue(predicate.IsUrnAllowed(dto.Urn), () => new PermissionDeniedException("User does not have editing permission for this urn"));
            var establishment = _dbContext.Establishments.SingleOrThrow(x => x.Urn == dto.Urn);

            Guard.IsTrue<PermissionDeniedException>(predicate.CanEdit(establishment.Urn, establishment.TypeId, 
                null, establishment.LocalAuthorityId, establishment.EstablishmentTypeGroupId));
            
            
            await UpdateInternalAsync(dto, establishment, principal);
        }

        private async Task UpdateInternalAsync(EstablishmentDto dto, Establishment entity, ClaimsPrincipal principal)
        {


            
        }
        
    }
}
