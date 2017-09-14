using Edubase.Data.Entity;
using Edubase.Data.Repositories.TableStorage;
using Microsoft.WindowsAzure.Storage.Table;
using System.Threading.Tasks;
using MoreLinq;
using System.Collections.Generic;
using System.Linq;
using System;
using Microsoft.WindowsAzure.Storage.Table.Queryable;

namespace Edubase.Data.Repositories
{
    public class LocalAuthoritySetRepository : TableStorageBase<LocalAuthoritySet>, ILocalAuthoritySetRepository
    {
        public LocalAuthoritySetRepository()
            : base("DataConnectionString")
        {
        }

        public async Task CreateAsync(LocalAuthoritySet message) => await Table.ExecuteAsync(TableOperation.Insert(message));

        public async Task CreateAsync(params LocalAuthoritySet[] messages)
        {
            /*
             * A batch operation is a collection of table operations which are executed by the Storage Service REST API as a 
             * single atomic operation, by invoking an Entity Group Transaction. A batch operation may contain up to 100 individual table operations, 
             * with the requirement that each operation entity must have same partition key. A batch with a retrieve operation cannot contain 
             * any other operations. Note that the total payload of a batch operation is limited to 4MB.
             */
            var partitionKeys = messages.Select(x => x.PartitionKey).Distinct();
            foreach (var key in partitionKeys)
            {
                var partitionMessages = messages.Where(x => x.PartitionKey == key);
                foreach (var batch in partitionMessages.Batch(100))
                {
                    var batchOperation = new TableBatchOperation();
                    batch.ForEach(message => batchOperation.Insert(message));
                    await Table.ExecuteBatchAsync(batchOperation);
                }
            }
        }

        public async Task CreateAsync(IEnumerable<LocalAuthoritySet> messages) => await CreateAsync(messages.ToArray());

        public async Task<Page<LocalAuthoritySet>> GetAllAsync(int? take = null, TableContinuationToken skip = null)
        {
            var query = Table.CreateQuery<LocalAuthoritySet>().AsQueryable();
            if (take.HasValue) query = query.Take(take.Value);
            var results = await Table.ExecuteQuerySegmentedAsync(query.AsTableQuery(), skip);
            return new Page<LocalAuthoritySet>(results, results.ContinuationToken);
        }

        public async Task<LocalAuthoritySet> GetAsync(string id)
        {
            var q = Table.CreateQuery<LocalAuthoritySet>().Where(x => x.PartitionKey == string.Empty && x.RowKey == id).AsTableQuery();
            var results = await q.ExecuteSegmentedAsync(null);
            return results.FirstOrDefault();
        }

        public async Task DeleteAsync(string id)
        {
            var item = await GetAsync(id);
            await Table.ExecuteAsync(TableOperation.Delete(item));
        }

        public async Task UpdateAsync(LocalAuthoritySet item) => await Table.ExecuteAsync(TableOperation.Replace(item));
        
    }
}
