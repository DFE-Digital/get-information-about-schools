using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Azure;
using Azure.Data.Tables;
using Edubase.Data.Entity;

namespace Edubase.Data.Repositories;

public class FaqItemRepository
{
    private const string PartitionKey = "FaqItem";
    private const string TableNameKey = "FaqItems";

    private readonly TableClient _faqGroupTableClient;

    public FaqItemRepository(TableServiceClient tableServiceClient)
    {
        _faqGroupTableClient = tableServiceClient.GetTableClient(TableNameKey);
    }

    public async Task CreateAsync(FaqItem entity)
    {
        entity.PartitionKey = PartitionKey;
        entity.RowKey ??= Guid.NewGuid().ToString();
        await _faqGroupTableClient.AddEntityAsync(entity);
    }

    public async Task<IEnumerable<FaqItem>> GetAllAsync(int take)
    {
        var results = new List<FaqItem>();
        await foreach (var item
            in _faqGroupTableClient.QueryAsync<FaqItem>(x => x.PartitionKey == PartitionKey))
        {
            results.Add(item);
            if (results.Count >= take)
            {
                break;
            }
        }

        return results;
    }

    public async Task<FaqItem?> GetAsync(string id)
    {
        try
        {
            var response = await _faqGroupTableClient.GetEntityAsync<FaqItem>(PartitionKey, id);
            return response.Value;
        }
        catch (RequestFailedException ex) when (ex.Status == 404)
        {
            return null;
        }
    }

    public async Task DeleteAsync(string id)
    {
        var item = await GetAsync(id);
        if (item is not null)
        {
            await _faqGroupTableClient.DeleteEntityAsync(item.PartitionKey, item.RowKey);
        }
    }

    public async Task UpdateAsync(FaqItem item)
    {
        item.PartitionKey = PartitionKey;
        await _faqGroupTableClient.UpdateEntityAsync(item, item.ETag, TableUpdateMode.Replace);
    }
}
