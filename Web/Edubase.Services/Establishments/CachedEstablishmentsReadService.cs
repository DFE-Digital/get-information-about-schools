using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Edubase.Services.Domain;
using Edubase.Services.Establishments.DisplayPolicies;
using Edubase.Services.Establishments.Models;
using Edubase.Services.Establishments.Search;
using Edubase.Services.Groups.Models;
using Edubase.Services.IntegrationEndPoints.AzureSearch.Models;
using Edubase.Services.Cache;
using Microsoft.AspNet.Identity;
namespace Edubase.Services.Establishments
{
    public class CachedEstablishmentsReadService : CachedServiceBase, ICachedEstablishmentsReadService
    {
        private IEstablishmentReadService _establishmentReadService;

        public CachedEstablishmentsReadService(ICacheAccessor cache, IEstablishmentReadService establishmentReadService) : base(cache)
        {
            _establishmentReadService = establishmentReadService;
        }

        public async Task<bool> ExistsAsync(int urn, IPrincipal principal)
        {
            return await AutoAsync(async () => await _establishmentReadService.ExistsAsync(urn, principal), 
                Keyify(urn, principal.Identity.GetUserId()));
        }

        public async Task<EstablishmentModel> GetAsync(int urn, IPrincipal principal)
        {
            return await AutoAsync(async () => await _establishmentReadService.GetAsync(urn, principal),
                Keyify(urn, principal.Identity.GetUserId()));
        }

        public async Task<IEnumerable<EstablishmentChangeDto>> GetChangeHistoryAsync(int urn, int take, IPrincipal principal)
        {
            return await AutoAsync(async () => await _establishmentReadService.GetChangeHistoryAsync(urn, take, principal),
                Keyify(urn, take, principal.Identity.GetUserId()));
        }

        public EstablishmentDisplayPolicy GetDisplayPolicy(IPrincipal user, EstablishmentModel establishment, GroupModel group)
            => _establishmentReadService.GetDisplayPolicy(user, establishment, group);

        public async Task<IEnumerable<LinkedEstablishmentModel>> GetLinkedEstablishments(int urn)
            => await _establishmentReadService.GetLinkedEstablishments(urn);

        public async Task<IEnumerable<ChangeDescriptorDto>> GetPendingChangesAsync(int urn, IPrincipal principal)
            => await _establishmentReadService.GetPendingChangesAsync(urn, principal);

        public int[] GetPermittedStatusIds(IPrincipal principal) => _establishmentReadService.GetPermittedStatusIds(principal);

        public async Task<AzureSearchResult<SearchEstablishmentDocument>> SearchAsync(EstablishmentSearchPayload payload, IPrincipal principal)
         => await _establishmentReadService.SearchAsync(payload, principal);

        public async Task<IEnumerable<EstablishmentSuggestionItem>> SuggestAsync(string text, IPrincipal principal, int take = 10)
            => await _establishmentReadService.SuggestAsync(text, principal, take);

        
    }
}
