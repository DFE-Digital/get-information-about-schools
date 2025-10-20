using System.Collections.Generic;
using System.Threading.Tasks;
using Edubase.Data.Entity;
using Edubase.Data.Repositories.TableStorage;
using Microsoft.WindowsAzure.Storage.Table;

namespace Edubase.Data.Repositories;

public interface IUserPreferenceRepository
{
    Task UpsertAsync(UserPreference item);
    UserPreference Get(string userId);
    Task<UserPreference> GetAsync(string userId);
}
