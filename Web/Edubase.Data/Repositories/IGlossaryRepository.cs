using System.Collections.Generic;
using System.Threading.Tasks;
using Edubase.Data.Entity;
using Edubase.Data.Repositories.TableStorage;
using Microsoft.WindowsAzure.Storage.Table;

namespace Edubase.Data.Repositories
{
    public interface IGlossaryRepository
    {
        Task CreateAsync(GlossaryItem messages);
        Task CreateAsync(params GlossaryItem[] messages);
        Task CreateAsync(IEnumerable<GlossaryItem> messages);
        Task<Page<GlossaryItem>> GetAllAsync(int take, TableContinuationToken skip = null);
        Task<GlossaryItem> GetAsync(string id);
        Task DeleteAsync(string id);
        Task UpdateAsync(GlossaryItem item);
    }
}
