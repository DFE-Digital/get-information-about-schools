using System.Threading.Tasks;
using Edubase.Data.Entity;

namespace Edubase.Data.Repositories.LocalAuthorities
{
    public interface ILAReadRepository
    {
        Task<LocalAuthority> GetAsync(int id);
    }

    public interface ICachedLAReadRepository : ILAReadRepository { }
}