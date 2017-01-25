using System;
using System.Security.Principal;
using System.Threading.Tasks;
using Edubase.Services.Core;
using Edubase.Services.Groups.Search;
using Edubase.Services.Governors.Search;

namespace Edubase.Services.Governors.Downloads
{
    public interface IGovernorDownloadService : IFileDownloadFactoryService
    {
        Task SearchWithDownloadGenerationAsync(Guid taskId, GovernorSearchPayload payload, IPrincipal principal, FileDownloadFactoryService.eFileFormat format);
    }
}