using Edubase.Data.Entity;
using Edubase.Data.Repositories.TableStorage;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage.Table.Queryable;
using System.Linq;
using System.Threading.Tasks;

namespace Edubase.Data.Repositories
{
    public class ApiRecorderSessionItemRepository : TableStorageBase<ApiRecorderSessionItem>
    {
        public ApiRecorderSessionItemRepository()
            : base("DataConnectionString")
        {
        }

        public async Task CreateAsync(ApiRecorderSessionItem message) => await Table.ExecuteAsync(TableOperation.Insert(message));

        public async Task<Page<ApiRecorderSessionItem>> GetAllAsync(int take, TableContinuationToken skip = null)
        {
            var query = Table.CreateQuery<ApiRecorderSessionItem>().AsQueryable();
            query = query.Take(take);
            var results = await Table.ExecuteQuerySegmentedAsync(query.AsTableQuery(), skip);
            return new Page<ApiRecorderSessionItem>(results, results.ContinuationToken);
        }
    }
}
