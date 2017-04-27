using System.Threading.Tasks;
using Edubase.Services.Downloads.Models;
using System.Security.Principal;

namespace Edubase.Services.Downloads
{
    public interface IDownloadsService
    {
        Task<FileDownload[]> GetListAsync(IPrincipal principal);
    }
}