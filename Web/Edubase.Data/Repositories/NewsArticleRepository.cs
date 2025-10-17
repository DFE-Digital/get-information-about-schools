using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure;
using Azure.Data.Tables;
using Edubase.Data.Entity;
using Edubase.Data.Repositories.TableStorage;

namespace Edubase.Data.Repositories;

public class NewsArticleRepository : TableStorageBase<NewsArticle>
{
    public NewsArticleRepository()
        : base("DataConnectionString")
    {
    }

    public async Task CreateAsync(NewsArticle entity) => await Table.AddEntityAsync(entity);

    public async Task<IEnumerable<NewsArticle>> GetAllAsync(
        int take,
        bool visible = true,
        int? year = null,
        eNewsArticlePartition partitionKey = eNewsArticlePartition.Current)
    {
        AsyncPageable<NewsArticle> query
            = Table.QueryAsync<NewsArticle>(
                (article)
                    => article.PartitionKey == partitionKey.ToString() &&
                        (!visible || article.ArticleDate <= DateTime.UtcNow) &&
                        (!year.HasValue ||
                            (article.ArticleDate >= new DateTime(year.Value, 1, 1) &&
                            (article.ArticleDate < new DateTime(year.Value, 12, 31, 23, 59, 59)))));

        List<NewsArticle> results = [];

        await foreach (var article in query)
        {
            results.Add(article);
            if (results.Count >= take)
            {
                return results.Take(take);
            }
        }

        return results;
    }

    public async Task<NewsArticle> GetAsync(string id, eNewsArticlePartition partitionKey = eNewsArticlePartition.Current)
    {
        try
        {
            var response = await Table.GetEntityAsync<NewsArticle>(partitionKey: partitionKey.ToString(), rowKey: id);
            return response.Value;
        }
        catch (RequestFailedException ex) when (ex.Status == 404)
        {
            return null;
        }
    }

    private async Task ArchiveAsync(string id, string auditUser = "")
    {
        var item = await GetAsync(id);
        item.PartitionKey = eNewsArticlePartition.Archive.ToString();

        if (item.Version > 1)
        {
            item.RowKey = Guid.NewGuid().ToString("N").Substring(0, 8);
        }

        await CreateAsync(item);


        if (!string.IsNullOrEmpty(auditUser))
        {
            // if this is triggered as part of a delete, once we've ported the original entry over to the audit, we want to create a final one which is the delete entry
            item.Version++;
            item.AuditEvent = eNewsArticleEvent.Delete.ToString();
            item.AuditUser = auditUser;
            item.AuditTimestamp = DateTime.Now;
            item.RowKey = Guid.NewGuid().ToString("N").Substring(0, 8);
            await CreateAsync(item);
        }
    }

    public async Task DeleteAsync(string id, string auditUser)
    {
        await ArchiveAsync(id, auditUser);

        var item = await GetAsync(id);

        if (item != null)
        {
            await Table.DeleteEntityAsync(item.PartitionKey, item.RowKey);
        }
    }

    public async Task UpdateAsync(NewsArticle item)
    {
        // archive the existing one first so we have a snapshot
        await ArchiveAsync(item.RowKey);

        // now update the record
        item.Version++;
        await Table.UpdateEntityAsync(item, item.ETag, TableUpdateMode.Replace);
    }
}
