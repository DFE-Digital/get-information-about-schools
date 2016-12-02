using System.Threading.Tasks;
using Edubase.Services.Groups.Models;

namespace Edubase.Services.Groups
{
    public interface IGroupReadService
    {
        Task<GroupModel> GetByEstablishmentUrnAsync(int urn);
    }
}