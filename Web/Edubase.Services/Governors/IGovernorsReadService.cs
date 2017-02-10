using System.Collections.Generic;
using System.Security.Principal;
using System.Threading.Tasks;
using Edubase.Data.Entity;
using Edubase.Services.Governors.Search;
using Edubase.Services.IntegrationEndPoints.AzureSearch.Models;
using Edubase.Services.Governors.Models;

namespace Edubase.Services.Governors
{
    public interface IGovernorsReadService
    {
        Task<IEnumerable<Governor>> GetCurrentByGroupUID(int groupUID);
        Task<IEnumerable<Governor>> GetCurrentByUrn(int urn);
        Task<IEnumerable<Governor>> GetHistoricalByGroupUID(int groupUID);
        Task<IEnumerable<Governor>> GetHistoricalByUrn(int urn);

        Task<AzureSearchResult<SearchGovernorDocument>> SearchAsync(GovernorSearchPayload payload);
        Task<GovernorsListDto> GetGovernorListAsync(int? urn = null, int? groupUId = null, IPrincipal principal = null);
    }
}