using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Azure;
using Azure.Data.Tables;
using Edubase.Data.Entity;
using Edubase.Data.Repositories.TableStorage;
using Microsoft.Extensions.Configuration;

namespace Edubase.Data.Repositories;

public class FaqItemRepository : TableStorageBase<FaqItem>
{
    private const string PartitionKey = "FaqItem";

    public FaqItemRepository(IConfiguration configuration)
        : base(configuration, "DataConnectionString", "FaqItems")
    {
    }

    public async Task CreateAsync(FaqItem entity)
    {
        entity.PartitionKey = PartitionKey;
        entity.RowKey ??= Guid.NewGuid().ToString();
        await Table.AddEntityAsync(entity);
    }

    public async Task<IEnumerable<FaqItem>> GetAllAsync(int take)
    {
        var results = new List<FaqItem>();
        await foreach (var item in Table.QueryAsync<FaqItem>(x => x.PartitionKey == PartitionKey))
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
            var response = await Table.GetEntityAsync<FaqItem>(PartitionKey, id);
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
            await Table.DeleteEntityAsync(item.PartitionKey, item.RowKey);
        }
    }

    public async Task UpdateAsync(FaqItem item)
    {
        item.PartitionKey = PartitionKey;
        await Table.UpdateEntityAsync(item, item.ETag, TableUpdateMode.Replace);
    }
}
