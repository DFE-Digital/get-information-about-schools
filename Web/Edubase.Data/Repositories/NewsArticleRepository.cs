using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure;
using Azure.Data.Tables;
using Edubase.Data.Entity;
using Edubase.Data.Repositories.TableStorage;
using Microsoft.Extensions.Configuration;

namespace Edubase.Data.Repositories;

public class NewsArticleRepository : TableStorageBase<NewsArticle>
{
    public NewsArticleRepository(IConfiguration configuration)
        : base(configuration, "DataConnectionString", "NewsArticles")
    {
    }

    public async Task CreateAsync(NewsArticle entity)
    {
        entity.PartitionKey ??= eNewsArticlePartition.Current.ToString();
        entity.RowKey ??= Guid.NewGuid().ToString("N").Substring(0, 8);
        await Table.AddEntityAsync(entity);
    }

    public async Task<IEnumerable<NewsArticle>> GetAllAsync(
        int take,
        bool visible = true,
        int? year = null,
        eNewsArticlePartition partitionKey = eNewsArticlePartition.Current)
    {
        var results = new List<NewsArticle>();

        // Query only by PartitionKey to avoid complex expression evaluation
        var query = Table.QueryAsync<NewsArticle>(a =>
            a.PartitionKey == partitionKey.ToString());

        await foreach (var article in query)
        {
            if ((!visible || (article.ArticleDate != null && article.ArticleDate <= DateTime.UtcNow)) &&
                (!year.HasValue ||
                    (article.ArticleDate != null &&
                     article.ArticleDate >= new DateTime(year.Value, 1, 1) &&
                     article.ArticleDate < new DateTime(year.Value, 12, 31, 23, 59, 59))))
            {
                results.Add(article);
                if (results.Count >= take)
                    break;
            }
        }

        return results;
    }

    public async Task<NewsArticle?> GetAsync(string id, eNewsArticlePartition partitionKey = eNewsArticlePartition.Current)
    {
        try
        {
            var response = await Table.GetEntityAsync<NewsArticle>(partitionKey.ToString(), id);
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
        if (item is null)
        {
            return;
        }

        item.PartitionKey = eNewsArticlePartition.Archive.ToString();
        item.RowKey = item.Version > 1
            ? Guid.NewGuid().ToString("N").Substring(0, 8)
            : item.RowKey;

        await CreateAsync(item);

        if (!string.IsNullOrEmpty(auditUser))
        {
            item.Version++;
            item.AuditEvent = eNewsArticleEvent.Delete.ToString();
            item.AuditUser = auditUser;
            item.AuditTimestamp = DateTime.UtcNow;
            item.RowKey = Guid.NewGuid().ToString("N").Substring(0, 8);
            await CreateAsync(item);
        }
    }

    public async Task DeleteAsync(string id, string auditUser)
    {
        await ArchiveAsync(id, auditUser);

        var item = await GetAsync(id);
        if (item is not null)
        {
            await Table.DeleteEntityAsync(item.PartitionKey, item.RowKey);
        }
    }

    public async Task UpdateAsync(NewsArticle item)
    {
        await ArchiveAsync(item.RowKey);

        item.Version++;
        await Table.UpdateEntityAsync(item, item.ETag, TableUpdateMode.Replace);
    }
}
