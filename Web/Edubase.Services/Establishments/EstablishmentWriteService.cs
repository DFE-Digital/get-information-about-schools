using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Data.Entity;

namespace Edubase.Services.Establishments
{
    using AutoMapper;
    using Data.DbContext;
    using Data.Entity;
    using Data.Repositories.Establishments;
    using Exceptions;
    using Groups;
    using Models;
    using Security;

    public class EstablishmentWriteService : IEstablishmentWriteService
    {
        private IEstablishmentReadService _readService;
        private IGroupReadService _groupReadService;
        private IApplicationDbContextFactory _dbContextFactory;
        private IMapper _mapper;
        private ICachedEstablishmentReadRepository _cachedEstablishmentReadRepository;

        public EstablishmentWriteService(IEstablishmentReadService readService, 
            IGroupReadService groupReadService, 
            IApplicationDbContextFactory dbContextFactory,
            IMapper mapper, 
            ICachedEstablishmentReadRepository cachedEstablishmentReadRepository)
        {
            _readService = readService;
            _groupReadService = groupReadService;
            _dbContextFactory = dbContextFactory;
            _mapper = mapper;
            _cachedEstablishmentReadRepository = cachedEstablishmentReadRepository;
        }

        public async Task SaveAsync(EstablishmentModel model, IPrincipal principal)
        {
            var cp = principal.AsClaimsPrincipal();
            if (model.Urn.HasValue)
            {
                var original = (await _readService.GetAsync(model.Urn.Value, principal)).GetResult();
                var editPermissions = cp.GetEditEstablishmentPermissions();
                var groupIds = editPermissions.GroupIds.Any() ? await _groupReadService.GetParentGroupIdsAsync(model.Urn.Value) : null as int[];
                if(!editPermissions.CanEdit(original.Urn.Value, original.TypeId, groupIds, original.LocalAuthorityId, original.EstablishmentTypeGroupId))
                {
                    throw new PermissionDeniedException("Principal cannot edit Establishment. Permission denied.");
                }
            }
            else if (cp.GetCreateEstablishmentPermissions() == null) throw new PermissionDeniedException("Principal cannot create an Establishment");

            using (var db = _dbContextFactory.Obtain())
            {
                var entity = await db.Establishments.FirstOrDefaultAsync(x => x.Urn == model.Urn);
                _mapper.Map(model, entity);
                await db.SaveChangesAsync();

                await _cachedEstablishmentReadRepository.ClearRelationshipCacheAsync(model.Urn);
            }
        }
    }
}