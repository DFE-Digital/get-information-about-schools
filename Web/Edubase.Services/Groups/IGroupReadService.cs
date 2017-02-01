using Edubase.Data.Entity;
using Edubase.Services.Groups.Models;
using Edubase.Services.Groups.Search;
using Edubase.Services.IntegrationEndPoints.AzureSearch.Models;
using System.Collections.Generic;
using System.Security.Principal;
using System.Threading.Tasks;

namespace Edubase.Services.Groups
{
    public interface IGroupReadService
    {
        /// <summary>
        /// Retrieves an array of Group Uids for the Establishment urn
        /// </summary>
        /// <param name="urn"></param>
        /// <returns></returns>
        Task<int[]> GetParentGroupIdsAsync(int establishmentUrn);

        Task<GroupModel> GetByEstablishmentUrnAsync(int urn);

        Task<IEnumerable<GroupSuggestionItem>> SuggestAsync(string text, IPrincipal principal, int take = 10);

        Task<AzureSearchResult<SearchGroupDocument>> SearchAsync(GroupSearchPayload payload, IPrincipal principal);

        Task<AzureSearchResult<SearchGroupDocument>> SearchByIdsAsync(string groupId, int? groupUId, string companiesHouseNumber, IPrincipal principal);

        Task<IEnumerable<GroupModel>> GetAllByEstablishmentUrnAsync(int urn);
        Task<GroupModel> GetAsync(int uid);

        /// <summary>
        /// Retrieves the list of Establishment Groups associated with a Group
        /// </summary>
        /// <param name="groupUid"></param>
        /// <returns></returns>
        Task<List<EstablishmentGroup>> GetEstablishmentGroupsAsync(int groupUid);
    }
}