using System.Threading.Tasks;

namespace Edubase.Services.Groups
{
    public interface IGroupsWriteService
    {
        Task UpdateEstablishmentCount(int groupUId, int? count = default(int?));
    }
}