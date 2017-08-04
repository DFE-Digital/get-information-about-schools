using System;
using System.Security.Principal;
using System.Threading.Tasks;
using Edubase.Services.Domain;
using Edubase.Services.Texuna.ChangeHistory.Models;
using System.Collections.Generic;

namespace Edubase.Services.Texuna.ChangeHistory
{
    public interface IChangeHistoryService
    {
        Task<ProgressDto> GetDownloadGenerationProgressAsync(Guid taskId, IPrincipal principal);
        Task<List<EstablishmentField>> GetEstablishmentFieldsAsync(IPrincipal principal);
        Task<ApiPagedResult<ChangeHistorySearchItem>> SearchAsync(SearchChangeHistoryBrowsePayload payload, IPrincipal principal);
        Task<ProgressDto> SearchWithDownloadGenerationAsync(SearchChangeHistoryDownloadPayload payload, IPrincipal principal);
        Task<List<UserGroupModel>> GetSuggesterGroupsAsync(IPrincipal principal);
        Task<List<UserGroupModel>> GetApproversGroupsAsync(IPrincipal principal);
    }
}