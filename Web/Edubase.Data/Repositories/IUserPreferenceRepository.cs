using System.Threading.Tasks;
using Edubase.Data.Entity;

namespace Edubase.Data.Repositories;

public interface IUserPreferenceRepository
{
    Task UpsertAsync(UserPreference item);
    UserPreference Get(string userId);
    Task<UserPreference> GetAsync(string userId);
}
