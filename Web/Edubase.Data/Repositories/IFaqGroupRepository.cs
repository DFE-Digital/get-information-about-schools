using System.Collections.Generic;
using System.Threading.Tasks;
using Edubase.Data.Entity;
using Edubase.Data.Repositories.TableStorage;
using Microsoft.WindowsAzure.Storage.Table;

namespace Edubase.Data.Repositories
{
    public interface IFaqGroupRepository
    {
        Task CreateAsync(IEnumerable<FaqGroup> entities);
        Task CreateAsync(FaqGroup entity);
        Task CreateAsync(params FaqGroup[] entities);
        Task<Page<FaqGroup>> GetAllAsync(int take, TableContinuationToken skip = null);
        Task<FaqGroup> GetAsync(string id);
        Task DeleteAsync(string id);
        Task UpdateAsync(FaqGroup item);
    }
}
