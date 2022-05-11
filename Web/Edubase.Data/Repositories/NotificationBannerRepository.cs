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
    public class NotificationBannerRepository : TableStorageBase<NotificationBanner>// INotificationBannerRepository
    {
        public NotificationBannerRepository()
            : base("DataConnectionString")
        {
        }

        public async Task CreateAsync(NotificationBanner entity) => await Table.ExecuteAsync(TableOperation.Insert(entity));

        public async Task CreateAsync(params NotificationBanner[] entities)
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

        public async Task CreateAsync(IEnumerable<NotificationBanner> entities) => await CreateAsync(entities.ToArray());

        public Page<NotificationBanner> GetAll(int take, TableContinuationToken skip = null, bool visibleOnly = true, eNotificationBannerPartition partitionKey = eNotificationBannerPartition.Current)
        {
            var query = Table.CreateQuery<NotificationBanner>().Where(x => x.PartitionKey == partitionKey.ToString()).AsQueryable();
            if (visibleOnly)
            {
                query = query.Where(x => x.Start <= DateTime.Now && x.End >= DateTime.Now);
            }
            query = query.Take(take);
            var results = Table.ExecuteQuerySegmentedAsync(query.AsTableQuery(), skip).Result;
            return new Page<NotificationBanner>(results, results.ContinuationToken);
        }

        public async Task<Page<NotificationBanner>> GetAllAsync(int take, TableContinuationToken skip = null, bool excludeExpired = false, eNotificationBannerPartition partitionKey = eNotificationBannerPartition.Current)
        {
            var query = Table.CreateQuery<NotificationBanner>().Where(x => x.PartitionKey == partitionKey.ToString()).AsQueryable();
            if (excludeExpired)
            {
                query = query.Where(x => x.End >= DateTime.Now);
            }
            query = query.Take(take);
            var results = await Table.ExecuteQuerySegmentedAsync(query.AsTableQuery(), skip);
            return new Page<NotificationBanner>(results, results.ContinuationToken);
        }

        public async Task<NotificationBanner> GetAsync(string id, eNotificationBannerPartition partitionKey = eNotificationBannerPartition.Current)
        {
            var q = Table.CreateQuery<NotificationBanner>().Where(x => x.PartitionKey == partitionKey.ToString() && x.RowKey == id).AsTableQuery();
            var results = await q.ExecuteSegmentedAsync(null);
            return results.FirstOrDefault();
        }

        public async Task DeleteAsync(string id, string auditUser)
        {
            await ArchiveAsync(id, auditUser);

            var item = await GetAsync(id);
            await Table.ExecuteAsync(TableOperation.Delete(item));
        }

        private async Task ArchiveAsync(string id, string auditUser = "")
        {
            var item = await GetAsync(id);
            item.PartitionKey = eNotificationBannerPartition.Archive.ToString();
            if (item.Version > 1)
            {
                item.RowKey = Guid.NewGuid().ToString("N").Substring(0, 8);
            }
            await CreateAsync(item);


            if (!string.IsNullOrEmpty(auditUser))
            {
                // if this is triggered as part of a delete, once we've ported the original entry over to the audit, we want to create a final one which is the delete entry
                item.Version++;
                item.AuditEvent = eNotificationBannerEvent.Delete.ToString();
                item.AuditUser = auditUser;
                item.AuditTimestamp = DateTime.Now;
                item.RowKey = Guid.NewGuid().ToString("N").Substring(0, 8);
                await CreateAsync(item);
            }
        }

        public async Task UpdateAsync(NotificationBanner item)
        {
            // archive the existing one first so we have a snapshot
            await ArchiveAsync(item.RowKey);

            // now update the record
            item.Version++;
            await Table.ExecuteAsync(TableOperation.Replace(item));
        }

    }
}
