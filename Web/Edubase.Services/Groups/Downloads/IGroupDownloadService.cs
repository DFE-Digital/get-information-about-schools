using System.Security.Principal;
using System.Threading.Tasks;
using Edubase.Services.Domain;

namespace Edubase.Services.Groups.Downloads
{
    public interface IGroupDownloadService
    {
        Task<DownloadDto> DownloadGroupHistory(int groupUid, DownloadType downloadType, IPrincipal principal);
    }
}