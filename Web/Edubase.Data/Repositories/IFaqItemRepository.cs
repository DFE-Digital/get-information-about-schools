using System.Collections.Generic;
using System.Threading.Tasks;
using Edubase.Data.Entity;
using Edubase.Data.Repositories.TableStorage;
using Microsoft.WindowsAzure.Storage.Table;
namespace Edubase.Data.Repositories
{
    public interface IFaqItemRepository
    {
        Task CreateAsync(FaqItem entity);
        Task CreateAsync(params FaqItem[] entities);
        Task CreateAsync(IEnumerable<FaqItem> entities);
        Task<Page<FaqItem>> GetAllAsync(int take, TableContinuationToken skip = null);
        Task<FaqItem> GetAsync(string id);
        Task DeleteAsync(string id);
        Task UpdateAsync(FaqItem item);
    }
}
