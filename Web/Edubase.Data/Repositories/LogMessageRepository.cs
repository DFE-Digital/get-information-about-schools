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
    public class LogMessageRepository : TableStorageBase<LogMessage>
    {
        public LogMessageRepository()
            : base("DataConnectionString")
        {
        }

        public async Task CreateAsync(LogMessage message) => await Table.ExecuteAsync(TableOperation.Insert(message));

        public async Task CreateAsync(params LogMessage[] messages)
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

        public async Task CreateAsync(IEnumerable<LogMessage> messages) => await CreateAsync(messages.ToArray());

        public async Task<Tuple<IEnumerable<LogMessage>, TableContinuationToken>> GetAllAsync(int take, TableContinuationToken skip = null, DateTime? date = null)
        {
            var query = Table.CreateQuery<LogMessage>().AsQueryable();
            if (date.HasValue) query = query.Where(x => x.PartitionKey == LogMessage.CreatePartitionKey(date.Value));
            query = query.Take(take);
            var results = await Table.ExecuteQuerySegmentedAsync(query.AsTableQuery(), skip);
            return new Tuple<IEnumerable<LogMessage>, TableContinuationToken>(results, results.ContinuationToken);
        }

        public async Task<LogMessage> GetAsync(string id)
        {
            var partitionKey = id.Substring(id.Length - 8);
            var rowKey = id.Substring(0, id.Length - 8);
            return await GetAsync(partitionKey, rowKey);
        }

        public async Task<LogMessage> GetAsync(string partitionKey, string rowKey)
        {
            var q = Table.CreateQuery<LogMessage>().Where(x => x.PartitionKey == partitionKey && x.RowKey == rowKey).AsTableQuery();
            var results = await q.ExecuteSegmentedAsync(null);
            return results.FirstOrDefault();
        }

    }
}
