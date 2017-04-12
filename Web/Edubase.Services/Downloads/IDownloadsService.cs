using System.Threading.Tasks;
using Edubase.Services.Downloads.Models;

namespace Edubase.Services.Downloads
{
    public interface IDownloadsService
    {
        Task<FileDownload[]> GetListAsync();
    }
}