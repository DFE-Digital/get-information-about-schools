using AutoMapper;
using Edubase.Data.Entity;
using Edubase.Services.Domain;
using Edubase.Services.Establishments.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Edubase.Services.Security;
using Edubase.Services.Enums;
using Edubase.Services.Establishments.DisplayPolicies;
using Edubase.Services.Groups.Models;
using Edubase.Services.Exceptions;
using MoreLinq;
using Edubase.Common;
using Edubase.Services.Establishments.Search;
using Edubase.Services.IntegrationEndPoints.AzureSearch;
using Edubase.Services.IntegrationEndPoints.AzureSearch.Models;

namespace Edubase.Services.Establishments
{
    public class EstablishmentReadService : IEstablishmentReadService
    {
        private IApplicationDbContext _dbContext;
        private IMapper _mapper;
        private ICachedLookupService _cachedLookupService;
        private IAzureSearchEndPoint _azureSearchService;

        public EstablishmentReadService(IApplicationDbContext dbContext, IMapper mapper, ICachedLookupService cachedLookupService, IAzureSearchEndPoint azureSearchService)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _cachedLookupService = cachedLookupService;
            _azureSearchService = azureSearchService;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="urn"></param>
        /// <returns></returns>
        public async Task<EstablishmentModel> GetAsync(int urn, IPrincipal principal)
        {
            var establishment = await GetQuery(principal).FirstOrDefaultAsync(x => x.Urn == urn && x.IsDeleted == false);
            var model = _mapper.Map<EstablishmentModel>(establishment);
            return model;
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

        private IQueryable<Establishment> GetQuery(IPrincipal principal)
        {
            var query = _dbContext.Establishments.Where(x => x.IsDeleted == false);
            if(!new[] { EdubaseRoles.EFA, EdubaseRoles.AOS, EdubaseRoles.FSG, EdubaseRoles.IEBT, EdubaseRoles.School, EdubaseRoles.PRU, EdubaseRoles.Admin }
            .Any(x => principal.IsInRole(x)))
            {
                var statusIds = new[]
                {
                    eLookupEstablishmentStatus.Closed,
                    eLookupEstablishmentStatus.Open,
                    eLookupEstablishmentStatus.OpenButProposedToClose,
                    eLookupEstablishmentStatus.ProposedToOpen
                }.Select(x => new int?((int)x));
                query = query.Where(x => statusIds.Contains(x.StatusId));
            }
            return query;
        }

        public async Task<IEnumerable<EstablishmentSuggestionItem>> SuggestAsync(string text, int take = 10) 
            => await _azureSearchService.SuggestAsync<EstablishmentSuggestionItem>(EstablishmentsSearchIndex.INDEX_NAME, EstablishmentsSearchIndex.SUGGESTER_NAME, text, take);
        

        public async Task<AzureSearchResult<SearchEstablishmentDocument>> SearchAsync(EstablishmentSearchPayload payload)
        {
            return await _azureSearchService.SearchAsync<SearchEstablishmentDocument>(EstablishmentsSearchIndex.INDEX_NAME, 
                payload.Text, payload.Filters.ToString().Clean(),payload.Skip, payload.Take, EstablishmentSearchPayload.FullTextSearchFields, payload.OrderBy); 
        }

        public async Task<bool> ExistsAsync(int urn, IPrincipal principal) => await GetQuery(principal).AnyAsync(x => x.Urn == urn && x.IsDeleted == false);

    }
}
