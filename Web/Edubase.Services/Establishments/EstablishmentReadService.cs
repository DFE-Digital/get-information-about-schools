#if (!TEXAPI)
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
    using Lookup;
    using Doc = Search.SearchEstablishmentDocument;
    using Common.Spatial;
    using Services.Core.Search;
    using Common.Reflection;

    public class EstablishmentReadService : IEstablishmentReadService
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly ICachedLookupService _cachedLookupService;
        private readonly IAzureSearchEndPoint _azureSearchService;
        private readonly IEstablishmentReadRepository _establishmentRepository;
        private readonly ILAReadRepository _laRepository;
        private readonly ICacheAccessor _cacheAccessor;
        private readonly IBlobService _blobService;
        private readonly ISecurityService _securityService;

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
            IBlobService blobService,
            ISecurityService securityService)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _cachedLookupService = cachedLookupService;
            _establishmentRepository = establishmentRepository;
            _azureSearchService = azureSearchService;
            _laRepository = laRepository;
            _cacheAccessor = cacheAccessor;
            _blobService = blobService;
            _securityService = securityService;
        }
        
        public async Task<ServiceResultDto<EstablishmentModel>> GetAsync(int urn, IPrincipal principal)
        {
            var dataModel = await _establishmentRepository.GetAsync(urn);

            if (dataModel != null)
            {
                if (await HasAccessAsync(principal, dataModel.StatusId))
                {
                    var domainModel = _mapper.Map<Establishment, EstablishmentModel>(dataModel);
                    domainModel.AdditionalAddressesCount = domainModel.AdditionalAddresses.Count;

                    if (!principal.InRole(EdubaseRoles.Admin, EdubaseRoles.IEBT))
                    {
                        var toRemove = domainModel.AdditionalAddresses.Where(x => x.IsRestricted == true).ToArray();
                        for (int i = 0; i < toRemove.Length; i++)
                        {
                            domainModel.AdditionalAddresses.Remove(toRemove[i]);
                        }
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

        private async Task<bool> HasAccessAsync(IPrincipal principal, int? statusId)
        {
            var isRestricted = IsRoleRestrictedOnStatus(principal);
            if (isRestricted && !statusId.HasValue) throw new Exception("StatusId is null but the principal has restricted access; impossible to acertain permissions");
            return !IsRoleRestrictedOnStatus(principal)
              || (await GetPermittedStatusIdsAsync(principal)).Any(x => x == statusId.Value);
        }

        public async Task<EstablishmentDisplayPolicy> GetDisplayPolicyAsync(IPrincipal user, EstablishmentModelBase establishment) 
            => new DisplayPolicyFactory().Create(user, establishment);


        public async Task<IEnumerable<LinkedEstablishmentModel>> GetLinkedEstablishmentsAsync(int urn)
        {
            return (await _dbContext.EstablishmentLinks
                    .Include(x => x.LinkedEstablishment)
                    .Include(x => x.LinkType)
                    .Where(x => x.EstablishmentUrn == urn && x.IsDeleted == false).ToArrayAsync())
                    .Select(x => new LinkedEstablishmentModel(x)).ToArray();
        }

        public async Task<IEnumerable<EstablishmentChangeDto>> GetChangeHistoryAsync(int urn, int take, IPrincipal user)
        {
            return await _dbContext.EstablishmentChangeHistories
                .Join(_dbContext.Users, x => x.OriginatorUserId, x => x.Id, (ch, u) => new { Change = ch, Originator = u })
                .Where(x => x.Change.Urn == urn)
                .OrderByDescending(x => x.Change.CreatedUtc)
                .Skip(0).Take(take).Select(x => new EstablishmentChangeDto
                {
                    EffectiveDateUtc = x.Change.EffectiveDateUtc,
                    Id = x.Change.Id,
                    Name = x.Change.Name,
                    NewValue = x.Change.NewValue,
                    OldValue = x.Change.OldValue,
                    OriginatorUserId = x.Change.OriginatorUserId,
                    RequestedDateUtc = x.Change.RequestedDateUtc,
                    Urn = x.Change.Urn,
                    OriginatorUserName = x.Originator.UserName
                }).ToListAsync();
        }

        public async Task<List<ChangeDescriptorDto>> GetModelChangesAsync(EstablishmentModel original, EstablishmentModel model)
        {
            var changes = ReflectionHelper.DetectChanges(model, original);
            var retVal = new List<ChangeDescriptorDto>();

            foreach (var change in changes)
            {
                if (_cachedLookupService.IsLookupField(change.Name))
                {
                    change.OldValue = await _cachedLookupService.GetNameAsync(change.Name, change.OldValue.ToInteger());
                    change.NewValue = await _cachedLookupService.GetNameAsync(change.Name, change.NewValue.ToInteger());
                }

                if (change.Name.EndsWith("Id", StringComparison.Ordinal)) change.Name = change.Name.Substring(0, change.Name.Length - 2);
                change.Name = change.Name.Replace("_", "");
                change.Name = change.Name.ToProperCase(true);

                retVal.Add(new ChangeDescriptorDto
                {
                    Name = change.DisplayName ?? change.Name,
                    NewValue = change.NewValue.Clean(),
                    OldValue = change.OldValue.Clean()
                });
            }

            return retVal;
        }

        public async Task<List<ChangeDescriptorDto>> GetModelChangesAsync(EstablishmentModel model)
        {
            var originalModel = (await GetAsync(model.Urn.Value, _securityService.CreateSystemPrincipal())).GetResult();
            return await GetModelChangesAsync(originalModel, model);
        }

        public async Task<IEnumerable<EstablishmentSuggestionItem>> SuggestAsync(string text, IPrincipal principal, int take = 10)
        {
            var oDataFilters = new ODataFilterList(ODataFilterList.AND, AzureSearchEndPoint.ODATA_FILTER_DELETED);
            if (IsRoleRestrictedOnStatus(principal))
            {
                oDataFilters.Add(ODataUtil.Or(nameof(Doc.StatusId), _restrictedStatuses));
            }
            return await _azureSearchService.SuggestAsync<EstablishmentSuggestionItem>(EstablishmentsSearchIndex.INDEX_NAME, EstablishmentsSearchIndex.SUGGESTER_NAME, text, oDataFilters.ToString(), take);
        }

        public async Task<int[]> GetPermittedStatusIdsAsync(IPrincipal principal)
        {
            if (IsRoleRestrictedOnStatus(principal)) return _restrictedStatuses;
            else return null;
        }

        /// <summary>
        /// Searches establishments based on the supplied payload/filters.
        /// </summary>
        /// <param name="payload"></param>
        /// <param name="principal"></param>
        /// <returns></returns>
        /// <exception cref="SearchQueryTooLargeException">
        ///     There's a chance that when you pass in a large query with 100s of filters
        ///     you'll get a SearchQueryTooLargeException.  There is no work-around; the size of the query needs to be reduced; this is due to a limitation in Azure Search.
        /// </exception>
        public async Task<ApiSearchResult<Doc>> SearchAsync(EstablishmentSearchPayload payload, IPrincipal principal)
        {
            if (IsRoleRestrictedOnStatus(principal))
            {
                if (payload.Filters.StatusIds.Any())
                {
                    if (!payload.Filters.StatusIds.All(x => _restrictedStatuses.Any(s => s == x)))
                        throw new EduSecurityException("One or more of the status ids requested are outside the permissions of the current principal");
                }
                else payload.Filters.StatusIds = _restrictedStatuses.ToArray();
            }

            var predicates = payload.Filters.ToODataPredicateList(AzureSearchEndPoint.ODATA_FILTER_DELETED);

            string orderByODataExpression = null;
            if (payload.GeoSearchLocation != null)
            {
                var distance = new Distance(payload.RadiusInMiles ?? 3);
                var geoPredicate = new ODataGeographyExpression(payload.GeoSearchLocation, nameof(Doc.Location));
                predicates.Add(geoPredicate.ToFilterODataExpression(distance.Kilometres));
                if(payload.SortBy == eSortBy.Distance) orderByODataExpression = geoPredicate.ToODataExpression();
            }

            if (payload.SortBy.OneOfThese(eSortBy.NameAlphabeticalAZ, eSortBy.NameAlphabeticalZA))
                orderByODataExpression = string.Concat(nameof(Doc.NameDistilled), " ", (payload.SortBy == eSortBy.NameAlphabeticalAZ ? "asc" : "desc"));

            var oDataFilterExpression = string.Join(" and ", predicates);

            return await _azureSearchService.SearchAsync<Doc>(EstablishmentsSearchIndex.INDEX_NAME,
                payload.Text,
                oDataFilterExpression,
                payload.Skip,
                payload.Take,
                new List<string> { nameof(Doc.NameDistilled) },
                new List<string> { orderByODataExpression });
        }

        private bool IsRoleRestrictedOnStatus(IPrincipal principal)
            => !_nonStatusRestrictiveRoles.Any(x => principal.IsInRole(x));
        
        public async Task<ServiceResultDto<bool>> CanAccess(int urn, IPrincipal principal)
        {
            var statusId = await _establishmentRepository.GetStatusAsync(urn);
            if (statusId.HasValue)
            {
                if (await HasAccessAsync(principal, statusId)) return new ServiceResultDto<bool>(true);
                else return new ServiceResultDto<bool>(eServiceResultStatus.PermissionDenied);
            }
            else return new ServiceResultDto<bool>(eServiceResultStatus.NotFound);
        }

    }
}
#endif