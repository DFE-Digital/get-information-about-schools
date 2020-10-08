using System.Threading.Tasks;
using Edubase.Services.Downloads.Models;
using System.Security.Principal;
using System;
using Edubase.Services.Domain;
using Edubase.Services.Core;

namespace Edubase.Services.Downloads
{
    public interface IDownloadsService
    {
        Task<FileDownload[]> GetListAsync(DateTime filterDate, IPrincipal principal);
        Task<PaginatedResult<ScheduledExtract>> GetScheduledExtractsAsync(int skip, int take, IPrincipal principal);
        Task<string> GenerateScheduledExtractAsync(int id, IPrincipal principal);
        Task<ProgressDto> GetProgressOfScheduledExtractGenerationAsync(Guid id, IPrincipal principal);
        Task<string> GenerateExtractAsync(string id, IPrincipal principal);
        Task<ProgressDto> GetProgressOfGeneratedExtractAsync(Guid id, IPrincipal principal);
        Task<bool> IsDownloadAvailable(string path, string id, IPrincipal principal);
    }
}
