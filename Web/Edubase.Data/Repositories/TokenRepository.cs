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
    public class TokenRepository : TableStorageBase<Token>, ITokenRepository
    {
        public TokenRepository()
            : base("DataConnectionString")
        {
        }

        public async Task CreateAsync(Token message) => await Table.ExecuteAsync(TableOperation.Insert(message));

        public async Task CreateAsync(params Token[] messages)
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

        public async Task CreateAsync(IEnumerable<Token> messages) => await CreateAsync(messages.ToArray());

        public async Task<Page<Token>> GetAllAsync(int? take = null, TableContinuationToken skip = null)
        {
            var query = Table.CreateQuery<Token>().AsQueryable();
            if (take.HasValue) query = query.Take(take.Value);
            var results = await Table.ExecuteQuerySegmentedAsync(query.AsTableQuery(), skip);
            return new Page<Token>(results, results.ContinuationToken);
        }

        public async Task<Token> GetAsync(string id)
        {
            if (id.Length < 5) throw new ArgumentException("Id is not valid", nameof(id));

            var q = Table.CreateQuery<Token>().Where(x => x.PartitionKey == id.Substring(0, 4) && x.RowKey == id.Substring(4)).AsTableQuery();
            var results = await q.ExecuteSegmentedAsync(null);
            return results.FirstOrDefault();
        }

        public Token Get(string id)
        {
            if (id.Length < 5) throw new ArgumentException("Id is not valid", nameof(id));

            var q = Table.CreateQuery<Token>().Where(x => x.PartitionKey == id.Substring(0, 4) && x.RowKey == id.Substring(4)).AsTableQuery();
            var results = q.ExecuteSegmented(null);
            return results.FirstOrDefault();
        }

        public async Task DeleteAsync(string id)
        {
            var item = await GetAsync(id);
            await Table.ExecuteAsync(TableOperation.Delete(item));
        }

        public async Task UpdateAsync(Token item) => await Table.ExecuteAsync(TableOperation.Replace(item));
        
    }
}
