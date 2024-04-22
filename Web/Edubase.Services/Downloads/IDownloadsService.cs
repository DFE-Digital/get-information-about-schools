using System.Threading.Tasks;
using Edubase.Services.Downloads.Models;
using System.Security.Principal;
using System;
using System.Collections.Generic;
using Edubase.Services.Domain;
using Edubase.Services.Core;
using System.Net.Http;

namespace Edubase.Services.Downloads
{
    public interface IDownloadsService
    {
        Task<FileDownload[]> GetListAsync(DateTime filterDate, IPrincipal principal);
        Task<PaginatedResult<ScheduledExtract>> GetScheduledExtractsAsync(int skip, int take, IPrincipal principal);
        Task<string> GenerateScheduledExtractAsync(int id, IPrincipal principal);
        Task<ProgressDto> GetProgressOfScheduledExtractGenerationAsync(Guid id, IPrincipal principal);
        Task<string> CollateDownloadsAsync(List<FileDownloadRequest> collection, IPrincipal principal);
        Task<string> GenerateExtractAsync(string id, IPrincipal principal);
        Task<ProgressDto> GetProgressOfGeneratedExtractAsync(Guid id, IPrincipal principal);
        Task<bool> IsDownloadAvailable(string path, string id, IPrincipal principal);

        Task<HttpResponseMessage> DownloadFile(FileDownload file, IPrincipal user);

        Task<HttpResponseMessage> DownloadMATClosureReport(IPrincipal user);
    }
}
