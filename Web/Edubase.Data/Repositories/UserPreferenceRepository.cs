using Edubase.Data.Entity;
using Edubase.Data.Repositories.TableStorage;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage.Table.Queryable;
using MoreLinq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Edubase.Data.Repositories
{
    public class UserPreferenceRepository : TableStorageBase<UserPreference>, IUserPreferenceRepository
    {
        public UserPreferenceRepository() : base("DataConnectionString") { }

        public async Task CreateAsync(UserPreference model) => await Table.ExecuteAsync(TableOperation.Insert(model));

        public async Task CreateAsync(params UserPreference[] models)
        {
            /*
             * A batch operation is a collection of table operations which are executed by the Storage Service REST API as a 
             * single atomic operation, by invoking an Entity Group Transaction. A batch operation may contain up to 100 individual table operations, 
             * with the requirement that each operation entity must have same partition key. A batch with a retrieve operation cannot contain 
             * any other operations. Note that the total payload of a batch operation is limited to 4MB.
             */
            var partitionKeys = models.Select(x => x.PartitionKey).Distinct();
            foreach (var key in partitionKeys)
            {
                var partitionMessages = models.Where(x => x.PartitionKey == key);
                foreach (var batch in partitionMessages.Batch(100))
                {
                    var batchOperation = new TableBatchOperation();
                    batch.ForEach(message => batchOperation.Insert(message));
                    await Table.ExecuteBatchAsync(batchOperation);
                }
            }
        }

        public async Task CreateAsync(IEnumerable<UserPreference> messages) => await CreateAsync(messages.ToArray());

        public async Task<Page<UserPreference>> GetAllAsync(int? take = null, TableContinuationToken skip = null)
        {
            var query = Table.CreateQuery<UserPreference>().AsQueryable();
            if (take.HasValue) query = query.Take(take.Value);
            var results = await Table.ExecuteQuerySegmentedAsync(query.AsTableQuery(), skip);
            return new Page<UserPreference>(results, results.ContinuationToken);
        }

        public async Task<UserPreference> GetAsync(string userId)
        {
            var q = Table.CreateQuery<UserPreference>().Where(x => x.PartitionKey == string.Empty && x.RowKey == userId).AsTableQuery();
            var results = await q.ExecuteSegmentedAsync(null);
            return results.FirstOrDefault();
        }

        public UserPreference Get(string userId)
        {
            var q = Table.CreateQuery<UserPreference>().Where(x => x.PartitionKey == string.Empty && x.RowKey == userId).AsTableQuery();
            var results = q.ExecuteSegmented(null);
            return results.FirstOrDefault();
        }

        public async Task DeleteAsync(string userId) => await Table.ExecuteAsync(TableOperation.Delete(await GetAsync(userId)));

        public async Task UpsertAsync(UserPreference item) => await Table.ExecuteAsync(TableOperation.InsertOrReplace(item));
        
    }
}
