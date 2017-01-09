using AutoMapper;
using Edubase.Common;
using Edubase.Data;
using Edubase.Data.Entity;
using Edubase.Data.Repositories.Establishments;
using Edubase.Data.Repositories.LocalAuthorities;
using MoreLinq;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;

namespace Edubase.Services.Establishments
{
    using Domain;
    using Services.Enums;
    using DisplayPolicies;
    using Models;
    using Search;
    using Exceptions;
    using Groups.Models;
    using IntegrationEndPoints.AzureSearch;
    using IntegrationEndPoints.AzureSearch.Models;
    using Security;
    using Data.DbContext;
    using Common.Cache;
    using System.IO;
    using Common.IO;
    using Ionic.Zip;

    public class EstablishmentReadService : IEstablishmentReadService
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly ICachedLookupService _cachedLookupService;
        private readonly IAzureSearchEndPoint _azureSearchService;
        private readonly IEstablishmentReadRepository _establishmentRepository;
        private readonly ILAReadRepository _laRepository;
        private ICacheAccessor _cacheAccessor;
        private IBlobService _blobService;

        /// <summary>
        /// Allow these roles to see establishments of all statuses
        /// </summary>
        private readonly string[] _nonStatusRestrictiveRoles = new[] { EdubaseRoles.EFA, EdubaseRoles.AOS, EdubaseRoles.FSG,
            EdubaseRoles.IEBT, EdubaseRoles.School, EdubaseRoles.PRU, EdubaseRoles.Admin };


        private readonly int[] _restrictedStatuses = new[]
        {
            eLookupEstablishmentStatus.Closed,
            eLookupEstablishmentStatus.Open,
            eLookupEstablishmentStatus.OpenButProposedToClose,
            eLookupEstablishmentStatus.ProposedToOpen
        }.Select(x => (int)x).ToArray();
        

        public EstablishmentReadService(
            IApplicationDbContext dbContext, 
            IMapper mapper, 
            ICachedLookupService cachedLookupService, 
            IAzureSearchEndPoint azureSearchService,
            ICachedEstablishmentReadRepository establishmentRepository,
            ICachedLAReadRepository laRepository,
            ICacheAccessor cacheAccessor,
            IBlobService blobService)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _cachedLookupService = cachedLookupService;
            _establishmentRepository = establishmentRepository;
            _azureSearchService = azureSearchService;
            _laRepository = laRepository;
            _cacheAccessor = cacheAccessor;
            _blobService = blobService;
        }
        
        public async Task<ServiceResultDto<EstablishmentModel>> GetAsync(int urn, IPrincipal principal)
        {
            var dataModel = await _establishmentRepository.GetAsync(urn);

            if (dataModel != null)
            {
                if (HasAccess(principal, dataModel.StatusId))
                {
                    var domainModel = _mapper.Map<Establishment, EstablishmentModel>(dataModel);

                    if (!principal.InRole(EdubaseRoles.Admin, EdubaseRoles.IEBT))
                    {
                        var toRemove = domainModel.AdditionalAddresses.Where(x => x.IsRestricted == true);
                        toRemove.ForEach(x => domainModel.AdditionalAddresses.Remove(x));
                    }

                    if (domainModel.TypeId == (int)eLookupEstablishmentType.ChildrensCentre
                        && domainModel.LocalAuthorityId.HasValue) // supply LA contact details
                    {
                        var la = await _laRepository.GetAsync(domainModel.LocalAuthorityId.Value);
                        domainModel.CCLAContactDetail = new ChildrensCentreLocalAuthorityDto(la);
                    }
                    return new ServiceResultDto<EstablishmentModel>(domainModel);
                }
                else return new ServiceResultDto<EstablishmentModel>(eServiceResultStatus.PermissionDenied);
            }
            else return new ServiceResultDto<EstablishmentModel>(eServiceResultStatus.NotFound);
        }

        private bool HasAccess(IPrincipal principal, int? statusId)
        {
            var isRestricted = IsRoleRestrictedOnStatus(principal);
            if (isRestricted && !statusId.HasValue) throw new Exception("StatusId is null but the principal has restricted access; impossible to acertain permissions");
            return !IsRoleRestrictedOnStatus(principal)
              || GetPermittedStatusIds(principal).Any(x => x == statusId.Value);
        }

        public EstablishmentDisplayPolicy GetDisplayPolicy(IPrincipal user, EstablishmentModel establishment, GroupModel group) 
            => new DisplayPolicyFactory().Create(user, establishment, group);

        public async Task<IEnumerable<LinkedEstablishmentModel>> GetLinkedEstablishments(int urn)
        {
            return (await _dbContext.EstablishmentLinks
                    .Include(x => x.LinkedEstablishment)
                    .Include(x => x.LinkType)
                    .Where(x => x.EstablishmentUrn == urn && x.IsDeleted == false).ToArrayAsync())
                    .Select(x => new LinkedEstablishmentModel(x)).ToArray();
        }

        public async Task<IEnumerable<EstablishmentChangeDto>> GetChangeHistoryAsync(int urn, int take, IPrincipal user)
        {
            if (!user.Identity.IsAuthenticated) throw new PermissionDeniedException("Principal not authorised to view change history");

            var changes = await _dbContext.EstablishmentChangeHistories
                    .Include(x => x.OriginatorUser)
                    .Include(x => x.ApproverUser)
                    .Where(x => x.Urn == urn && x.IsDeleted == false)
                    .OrderByDescending(x => x.EffectiveDateUtc)
                    .Take(take)
                    .Select(x => new EstablishmentChangeDto
                    {
                        PropertyName = x.Name,
                        ApproverUserId = x.ApproverUserId,
                        EffectiveDateUtc = x.EffectiveDateUtc,
                        Id = x.Id,
                        NewValue = x.NewValue,
                        OldValue = x.OldValue,
                        OriginatorUserId = x.OriginatorUserId,
                        RequestedDateUtc = x.RequestedDateUtc,
                        Urn = x.Urn,
                        ApproverUserName = x.ApproverUser.UserName,
                        OriginatorUserName = x.OriginatorUser.UserName
                    }).ToArrayAsync();

            GetLookupNames(changes);

            return changes;
        }

        public async Task<IEnumerable<ChangeDescriptorDto>> GetPendingChangesAsync(int urn, IPrincipal principal)
        {
            if (!principal.Identity.IsAuthenticated) throw new PermissionDeniedException("Principal not authorised to view pending changes");
            var pendingChanges = await _dbContext.EstablishmentApprovalQueue.Where(x => x.Urn == urn
                && x.IsApproved == false && x.IsDeleted == false && x.IsRejected == false)
                .Select(x => new ChangeDescriptorDto(x)).ToListAsync();

            GetLookupNames(pendingChanges);

            return pendingChanges;
        }

        private void GetLookupNames(IEnumerable<ChangeDescriptorDto> pendingChanges)
        {
            pendingChanges.ForEach(async x =>
            {
                if (_cachedLookupService.IsLookupField(x.PropertyName))
                {
                    if (x.OldValue.IsInteger())
                        x.OldValue = await _cachedLookupService.GetNameAsync(x.PropertyName, x.OldValue.ToInteger().Value);

                    if (x.NewValue.IsInteger())
                        x.NewValue = await _cachedLookupService.GetNameAsync(x.PropertyName, x.NewValue.ToInteger().Value);
                }
            });
        }
        
        public async Task<IEnumerable<EstablishmentSuggestionItem>> SuggestAsync(string text, IPrincipal principal, int take = 10)
        {
            var oDataFilters = new ODataFilterList(ODataFilterList.AND, AzureSearchEndPoint.ODATA_FILTER_DELETED);
            if (IsRoleRestrictedOnStatus(principal))
            {
                oDataFilters.Add(ODataUtil.Or(nameof(SearchEstablishmentDocument.StatusId), _restrictedStatuses));
            }
            return await _azureSearchService.SuggestAsync<EstablishmentSuggestionItem>(EstablishmentsSearchIndex.INDEX_NAME, EstablishmentsSearchIndex.SUGGESTER_NAME, text,oDataFilters.ToString() , take);
        }

        public int[] GetPermittedStatusIds(IPrincipal principal)
        {
            if (IsRoleRestrictedOnStatus(principal)) return _restrictedStatuses;
            else return null;
        }


        public async Task<AzureSearchResult<SearchEstablishmentDocument>> SearchAsync(EstablishmentSearchPayload payload, IPrincipal principal)
        {
            if (IsRoleRestrictedOnStatus(principal))
            {
                if (payload.Filters.StatusIds.Any())
                {
                    if (!payload.Filters.StatusIds.All(x => _restrictedStatuses.Any(s => s == x)))
                        throw new Exception("One or more of the status ids requested are outside the permissions of the current principal");
                }
                else payload.Filters.StatusIds = _restrictedStatuses.ToArray();
            }

            var predicates = payload.Filters.ToODataPredicateList(AzureSearchEndPoint.ODATA_FILTER_DELETED);
            
            if (payload.GeoSearchLocation != null)
            {
                var geoPredicate = new ODataGeographyExpression(payload.GeoSearchLocation);
                predicates.Add(geoPredicate.ToFilterODataExpression(nameof(SearchEstablishmentDocument.Location), payload.GeoSearchMaxRadiusInKilometres.Value));
                var expression = geoPredicate.ToODataExpression(nameof(SearchEstablishmentDocument.Location));
                if (payload.GeoSearchOrderByDistance && !payload.OrderBy.Contains(expression)) payload.OrderBy.Insert(0, expression);
            }

            var oDataFilterExpression = string.Join(" and ", predicates);

            return await _azureSearchService.SearchAsync<SearchEstablishmentDocument>(EstablishmentsSearchIndex.INDEX_NAME, 
                payload.Text, 
                oDataFilterExpression, 
                payload.Skip, 
                payload.Take, 
                new[] { nameof(SearchEstablishmentDocument.Name) }.ToList(), 
                payload.OrderBy); 
        }

        private bool IsRoleRestrictedOnStatus(IPrincipal principal)
            => !_nonStatusRestrictiveRoles.Any(x => principal.IsInRole(x));
        
        public async Task<ServiceResultDto<bool>> CanAccess(int urn, IPrincipal principal)
        {
            var statusId = await _establishmentRepository.GetStatusAsync(urn);
            if (statusId.HasValue)
            {
                if (HasAccess(principal, statusId)) return new ServiceResultDto<bool>(true);
                else return new ServiceResultDto<bool>(eServiceResultStatus.PermissionDenied);
            }
            else return new ServiceResultDto<bool>(eServiceResultStatus.NotFound);
        }


        


    }
}
