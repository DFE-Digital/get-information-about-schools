using Edubase.Data.Entity;
using Edubase.Data.Repositories.TableStorage;
using Microsoft.WindowsAzure.Storage.Table;
using System.Threading.Tasks;
using MoreLinq;
using System.Collections.Generic;
using System.Linq;

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
    }
}
