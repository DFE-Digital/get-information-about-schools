using System.Security.Principal;
using System.Threading.Tasks;
using Edubase.Services.Domain;
using System;
using Edubase.Services.Groups.Search;

namespace Edubase.Services.Groups.Downloads
{
    public interface IGroupDownloadService
    {
        Task<DownloadDto> DownloadGroupHistory(int groupUid, DownloadType downloadType, DateTime? dateFrom, DateTime? dateTo, string suggestedBy, IPrincipal principal);
        Task<DownloadDto> DownloadGroupData(int groupUId, DownloadType downloadType, IPrincipal principal);
        Task<ProgressDto> GetDownloadGenerationProgressAsync(Guid taskId, IPrincipal principal);
        Task<Guid> SearchWithDownloadGenerationAsync(SearchDownloadDto<GroupSearchPayload> payload, IPrincipal principal);
        Task<DownloadDto> GetGovernanceChangeHistoryDownloadAsync(int uid, DownloadType downloadType, IPrincipal user);
    }
}
