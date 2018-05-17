using System.Collections.Generic;
using System.Threading.Tasks;
using Edubase.Data.Entity;
using Edubase.Data.Repositories.TableStorage;
using Microsoft.WindowsAzure.Storage.Table;

namespace Edubase.Data.Repositories
{
    public interface IUserPreferenceRepository
    {
        Task UpsertAsync(UserPreference item);
        Task CreateAsync(IEnumerable<UserPreference> messages);
        Task CreateAsync(params UserPreference[] models);
        Task CreateAsync(UserPreference model);
        Task DeleteAsync(string userId);
        UserPreference Get(string userId);
        Task<Page<UserPreference>> GetAllAsync(int? take = null, TableContinuationToken skip = null);
        Task<UserPreference> GetAsync(string userId);
    }
}