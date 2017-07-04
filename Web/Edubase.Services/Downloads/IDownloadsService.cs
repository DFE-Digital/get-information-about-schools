using System.Threading.Tasks;
using Edubase.Services.Downloads.Models;
using System.Security.Principal;
using System;
using Edubase.Services.Domain;

namespace Edubase.Services.Downloads
{
    public interface IDownloadsService
    {
        Task<FileDownload[]> GetListAsync(IPrincipal principal);
        Task<ScheduledExtractsResult> GetScheduledExtractsAsync(int skip, int take, IPrincipal principal);
        Task<string> GenerateScheduledExtractAsync(int id, IPrincipal principal);
        Task<ProgressDto> GetProgressOfScheduledExtractGenerationAsync(Guid id, IPrincipal principal);
    }
}