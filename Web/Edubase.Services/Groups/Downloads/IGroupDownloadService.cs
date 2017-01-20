using System;
using System.Security.Principal;
using System.Threading.Tasks;
using Edubase.Services.Core;
using Edubase.Services.Groups.Search;

namespace Edubase.Services.Groups.Downloads
{
    public interface IGroupDownloadService : IFileDownloadFactoryService
    {
        Task SearchWithDownloadGenerationAsync(Guid taskId, GroupSearchPayload payload, IPrincipal principal, FileDownloadFactoryService.eFileFormat format);
    }
}