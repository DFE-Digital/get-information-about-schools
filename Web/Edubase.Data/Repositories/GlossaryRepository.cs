using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Azure;
using Azure.Data.Tables;
using Edubase.Data.Entity;
using Edubase.Data.Repositories.TableStorage;
using Microsoft.Extensions.Configuration;

namespace Edubase.Data.Repositories;

public class GlossaryRepository : TableStorageBase<GlossaryItem>
{
    private const string PartitionKey = "Glossary";

    public GlossaryRepository(IConfiguration configuration)
        : base(configuration, "DataConnectionString", "GlossaryItems")
    {
    }

    public async Task CreateAsync(GlossaryItem message)
    {
        message.PartitionKey = PartitionKey;
        message.RowKey ??= Guid.NewGuid().ToString();
        await Table.AddEntityAsync(message);
    }

    public async Task<IEnumerable<GlossaryItem>> GetAllAsync(int take)
    {
        var results = new List<GlossaryItem>();
        await foreach (var item in Table.QueryAsync<GlossaryItem>(x => x.PartitionKey == PartitionKey))
        {
            results.Add(item);
            if (results.Count >= take)
            {
                break;
            }
        }

        return results;
    }

    public async Task<GlossaryItem?> GetAsync(string id)
    {
        try
        {
            var response = await Table.GetEntityAsync<GlossaryItem>(PartitionKey, id);
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

    public async Task UpdateAsync(GlossaryItem item)
    {
        item.PartitionKey = PartitionKey;
        await Table.UpdateEntityAsync(item, item.ETag, TableUpdateMode.Replace);
    }
}
