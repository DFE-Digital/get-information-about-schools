using System.Collections.Generic;
using System.Threading.Tasks;
using Edubase.Data.Entity;

namespace Edubase.Data.Repositories;

public interface ILocalAuthoritySetRepository
{
    Task CreateAsync(LocalAuthoritySet message);
    Task DeleteAsync(string id);
    Task<IEnumerable<LocalAuthoritySet>> GetAllAsync(int? take = null);
    Task<LocalAuthoritySet> GetAsync(string id);
    Task UpdateAsync(LocalAuthoritySet item);
}
