using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Azure;
using Azure.Data.Tables;
using Edubase.Data.Entity;

namespace Edubase.Data.Repositories;

public class GlossaryRepository
{
    private const string PartitionKey = "Glossary";
    private const string TableNameKey = "GlossaryItems";

    private readonly TableClient _glossaryTableClient;

    public GlossaryRepository(TableServiceClient tableServiceClient)
    {
        _glossaryTableClient = tableServiceClient.GetTableClient(TableNameKey);
    }

    public async Task CreateAsync(GlossaryItem message)
    {
        message.PartitionKey = PartitionKey;
        message.RowKey ??= Guid.NewGuid().ToString();

       await _glossaryTableClient.AddEntityAsync(message);
    }

    public async Task<Page<GlossaryItem>> GetAllAsync(int take, string continuationToken = null)
    {
        // Build the query (filter optional)
        AsyncPageable<GlossaryItem> query =
            _glossaryTableClient.QueryAsync<GlossaryItem>();

        // Convert to pageable pages
        IAsyncEnumerable<Azure.Page<GlossaryItem>> pages =
            query.AsPages(continuationToken, take);

        // Read the first page only (equivalent to ExecuteQuerySegmentedAsync)
        await foreach (var page in pages)
        {
            return new Page<GlossaryItem>(
                page.Values,
                page.ContinuationToken
            );
        }

        // No pages returned at all
        return new Page<GlossaryItem>([], null);
    }

    public async Task<IEnumerable<string>> GetPartitionKeysAsync()
    {
        HashSet<string> keys = [];

        await foreach (GlossaryItem item in
            _glossaryTableClient.QueryAsync<GlossaryItem>())
        {
            keys.Add(item.PartitionKey);
        }

        return keys;
    }

    public class Page<T>
    {
        public IEnumerable<T> Items { get; }
        public string ContinuationToken { get; }

        public Page(IEnumerable<T> items, string continuationToken)
        {
            Items = items;
            ContinuationToken = continuationToken;
        }
    }

    public async Task<GlossaryItem?> GetAsync(string id)
    {
        try
        {
            Response<GlossaryItem> response =
                await _glossaryTableClient
                    .GetEntityAsync<GlossaryItem>(PartitionKey, id);

            return response.Value;
        }
        catch (RequestFailedException ex) when (ex.Status == 404)
        {
            return null;
        }
    }

    public async Task DeleteAsync(string id)
    {
        GlossaryItem item = await GetAsync(id);

        if (item is not null)
        {
            await _glossaryTableClient
                .DeleteEntityAsync(item.PartitionKey, item.RowKey);
        }
    }

    public async Task UpdateAsync(GlossaryItem item)
    {
        item.PartitionKey = PartitionKey;

        await _glossaryTableClient
            .UpdateEntityAsync(item, item.ETag, TableUpdateMode.Replace);
    }
}
