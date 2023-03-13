using System.Collections.Generic;
using System.Security.Principal;
using System.Threading.Tasks;
using Edubase.Services.Governors.Search;
using Edubase.Services.Governors.Models;
using Edubase.Services.Governors.DisplayPolicies;
using Edubase.Services.Enums;
using Edubase.Services.Domain;

namespace Edubase.Services.Governors
{
    public interface IGovernorsReadService
    {
        Task<ApiPagedResult<SearchGovernorModel>> SearchAsync(GovernorSearchPayload payload, IPrincipal principal);
        Task<GovernorsDetailsDto> GetGovernorListAsync(int? urn = null, int? groupUId = null, IPrincipal principal = null);
        Task<GovernorPermissions> GetGovernorPermissions(int? urn = default(int?), int? groupUId = default(int?), IPrincipal principal = null);
        Task<GovernorDisplayPolicy> GetDisplayPolicyAsync(eLookupGovernorRole role, int? urn = default(int?), int? groupUId = default(int?), IPrincipal principal = null);
        Task<GovernorEditPolicy> GetEditPolicyAsync(eLookupGovernorRole role, bool isGroup, IPrincipal principal);
        Task<GovernorModel> GetGovernorAsync(int gid, IPrincipal principal);
        Task<IEnumerable<GovernorModel>> GetSharedGovernorsAsync(int establishmentUrn, IPrincipal principal);
        Task<string> GetGovernorBulkUpdateTemplateUri(IPrincipal principal);
    }
}
