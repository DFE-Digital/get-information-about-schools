using System.Collections.Generic;
using System.Security.Principal;
using System.Threading.Tasks;
using Edubase.Services.Governors.Search;
using Edubase.Services.IntegrationEndPoints.AzureSearch.Models;
using Edubase.Services.Governors.Models;
using Edubase.Services.Governors.DisplayPolicies;
using Edubase.Services.Enums;

namespace Edubase.Services.Governors
{
    public interface IGovernorsReadService
    {
        Task<ApiSearchResult<SearchGovernorDocument>> SearchAsync(GovernorSearchPayload payload, IPrincipal principal);
        Task<GovernorsDetailsDto> GetGovernorListAsync(int? urn = null, int? groupUId = null, IPrincipal principal = null);
        GovernorDisplayPolicy GetEditorDisplayPolicy(eLookupGovernorRole role, bool isGroup, IPrincipal principal); //TODO: TEXCHANGE - added isGroup
        Task<GovernorModel> GetGovernorAsync(int gid, IPrincipal principal);
        Task<IEnumerable<GovernorModel>> GetSharedGovernorsAsync(int establishmentUrn, IPrincipal principal);
        Task<GovernorModel> GetSharedGovernorAsync(int governorId, int establishmentUrn, IPrincipal principal);
    }
}