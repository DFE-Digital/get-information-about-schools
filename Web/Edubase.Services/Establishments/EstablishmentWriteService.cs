using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Data.Entity;

namespace Edubase.Services.Establishments
{
    using AutoMapper;
    using Common;
    using Common.Reflection;
    using Data.DbContext;
    using Data.Entity;
    using Data.Repositories.Establishments;
    using Exceptions;
    using Groups;
    using IntegrationEndPoints.ServiceBus;
    using Models;
    using Security;
    using System.Collections.Generic;

    public class EstablishmentWriteService : IEstablishmentWriteService
    {
        private readonly IEstablishmentReadService _readService;
        private readonly IGroupReadService _groupReadService;
        private readonly IApplicationDbContextFactory _dbContextFactory;
        private readonly IMapper _mapper;
        private readonly ICachedEstablishmentReadRepository _cachedEstablishmentReadRepository;
        private readonly ISecurityService _securityService;
        private readonly IServiceBusEndPoint _serviceBusEndPoint;

        public EstablishmentWriteService(IEstablishmentReadService readService, 
            IGroupReadService groupReadService, 
            IApplicationDbContextFactory dbContextFactory,
            IMapper mapper, 
            ICachedEstablishmentReadRepository cachedEstablishmentReadRepository,
            ISecurityService securityService,
            IServiceBusEndPoint serviceBusEndPoint)
        {
            _readService = readService;
            _groupReadService = groupReadService;
            _dbContextFactory = dbContextFactory;
            _mapper = mapper;
            _cachedEstablishmentReadRepository = cachedEstablishmentReadRepository;
            _securityService = securityService;
            _serviceBusEndPoint = serviceBusEndPoint;
        }

        public async Task SaveAsync(EstablishmentModel model, IPrincipal principal)
        {
            var cp = principal.AsClaimsPrincipal();
            var originalModel = (await _readService.GetAsync(model.Urn.Value, _securityService.CreateSystemPrincipal())).GetResult();
            if (model.Urn.HasValue)
            {
                var editPermissions = cp.GetEditEstablishmentPermissions();
                var groupIds = editPermissions.GroupIds.Any() ? await _groupReadService.GetParentGroupIdsAsync(model.Urn.Value) : null as int[];
                if (!editPermissions.CanEdit(originalModel.Urn.Value, originalModel.TypeId, groupIds, originalModel.LocalAuthorityId, originalModel.EstablishmentTypeGroupId))
                {
                    throw new PermissionDeniedException("Principal cannot edit Establishment. Permission denied.");
                }
            }
            else if (cp.GetCreateEstablishmentPermissions() == null) throw new PermissionDeniedException("Principal cannot create an Establishment");

            RestoreRestrictedAddresses(model, principal, originalModel);

            using (var db = _dbContextFactory.Obtain())
            {
                var entity = await db.Establishments.FirstOrDefaultAsync(x => x.Urn == model.Urn);
                _mapper.Map(model, entity);
                await db.SaveChangesAsync();

                await _cachedEstablishmentReadRepository.ClearRelationshipCacheAsync(model.Urn);

                await Task.Delay(2000); // allow enough time for the cache to clear on other servers in the webfarm (could replace this with messaging, or server-affinity).

                await _serviceBusEndPoint.SendEstablishmentUpdateMessageAsync(entity);

            }
        }

        public async Task<List<ChangeDescriptor>> GetModelChangesAsync(EstablishmentModel model)
        {
            var originalModel = (await _readService.GetAsync(model.Urn.Value, _securityService.CreateSystemPrincipal())).GetResult();
            return ReflectionHelper.DetectChanges(model, originalModel);
        }

        private void RestoreRestrictedAddresses(EstablishmentModel model, IPrincipal principal, EstablishmentModel originalModel)
        {
            if (!principal.InRole(EdubaseRoles.Admin, EdubaseRoles.IEBT))
            {
                var restrictedAddresses = originalModel.AdditionalAddresses.Where(x => x.IsRestricted == true);
                if (restrictedAddresses.Any()) model.AdditionalAddresses.AddRange(restrictedAddresses);
            }
        }
    }
}