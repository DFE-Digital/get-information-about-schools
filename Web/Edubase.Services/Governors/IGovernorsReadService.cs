using System.Collections.Generic;
using System.Security.Principal;
using System.Threading.Tasks;
using Edubase.Data.Entity;
using Edubase.Services.Governors.Search;
using Edubase.Services.IntegrationEndPoints.AzureSearch.Models;
using Edubase.Services.Governors.Models;
using Edubase.Services.Governors.DisplayPolicies;
using Edubase.Services.Enums;

namespace Edubase.Services.Governors
{
    public interface IGovernorsReadService
    {
        Task<AzureSearchResult<SearchGovernorDocument>> SearchAsync(GovernorSearchPayload payload);
        Task<GovernorsDetailsDto> GetGovernorListAsync(int? urn = null, int? groupUId = null, IPrincipal principal = null);
        GovernorDisplayPolicy GetEditorDisplayPolicy(eLookupGovernorRole role);
        Task<GovernorModel> GetGovernorAsync(int gid);
    }
}