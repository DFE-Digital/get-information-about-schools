using System.Collections.Generic;
using System.Threading.Tasks;
using Edubase.Data.Entity;
using Edubase.Data.Repositories.TableStorage;
using Microsoft.WindowsAzure.Storage.Table;

namespace Edubase.Data.Repositories
{
    public interface ILocalAuthoritySetRepository
    {
        Task CreateAsync(IEnumerable<LocalAuthoritySet> messages);
        Task CreateAsync(LocalAuthoritySet message);
        Task CreateAsync(params LocalAuthoritySet[] messages);
        Task DeleteAsync(string id);
        Task<Page<LocalAuthoritySet>> GetAllAsync(int? take = null, TableContinuationToken skip = null);
        Task<LocalAuthoritySet> GetAsync(string id);
        Task UpdateAsync(LocalAuthoritySet item);
    }
}