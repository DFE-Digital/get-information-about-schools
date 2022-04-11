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
    public class NotificationTemplateRepository : TableStorageBase<NotificationTemplate>, INotificationTemplateRepository
    {
        public NotificationTemplateRepository()
            : base("DataConnectionString")
        {
        }

        public async Task CreateAsync(NotificationTemplate entity) => await Table.ExecuteAsync(TableOperation.Insert(entity));

        public async Task CreateAsync(params NotificationTemplate[] entities)
        {
            /*
             * A batch operation is a collection of table operations which are executed by the Storage Service REST API as a 
             * single atomic operation, by invoking an Entity Group Transaction. A batch operation may contain up to 100 individual table operations, 
             * with the requirement that each operation entity must have same partition key. A batch with a retrieve operation cannot contain 
             * any other operations. Note that the total payload of a batch operation is limited to 4MB.
             */
            var partitionKeys = entities.Select(x => x.PartitionKey).Distinct();
            foreach (var key in partitionKeys)
            {
                var partitionentitys = entities.Where(x => x.PartitionKey == key);
                foreach (var batch in partitionentitys.Batch(100))
                {
                    var batchOperation = new TableBatchOperation();
                    batch.ForEach(entity => batchOperation.Insert(entity));
                    await Table.ExecuteBatchAsync(batchOperation);
                }
            }
        }

        public async Task CreateAsync(IEnumerable<NotificationTemplate> entities) => await CreateAsync(entities.ToArray());

        public async Task<Page<NotificationTemplate>> GetAllAsync(int take, TableContinuationToken skip = null)
        {
            var query = Table.CreateQuery<NotificationTemplate>().AsQueryable();
            query = query.Take(take);
            var results = await Table.ExecuteQuerySegmentedAsync(query.AsTableQuery(), skip);
            return new Page<NotificationTemplate>(results, results.ContinuationToken);
        }

        public async Task<NotificationTemplate> GetAsync(string id)
        {
            var q = Table.CreateQuery<NotificationTemplate>().Where(x => x.PartitionKey == string.Empty && x.RowKey == id).AsTableQuery();
            var results = await q.ExecuteSegmentedAsync(null);
            return results.FirstOrDefault();
        }

        public async Task DeleteAsync(string id)
        {
            var item = await GetAsync(id);
            await Table.ExecuteAsync(TableOperation.Delete(item));
        }

        public async Task UpdateAsync(NotificationTemplate item) => await Table.ExecuteAsync(TableOperation.Replace(item));

    }
}
