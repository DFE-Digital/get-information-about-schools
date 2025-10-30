using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Azure;
using Azure.Data.Tables;
using Edubase.Data.Entity;
using Edubase.Data.Repositories.TableStorage;
using Microsoft.Extensions.Configuration;

namespace Edubase.Data.Repositories;

public class FaqGroupRepository : TableStorageBase<FaqGroup>
{
    private const string PartitionKey = "FaqGroup";

    public FaqGroupRepository(IConfiguration configuration)
        : base(configuration, "DataConnectionString", "FaqGroups")
    {
    }

    public async Task CreateAsync(FaqGroup entity)
    {
        entity.PartitionKey = PartitionKey;
        entity.RowKey ??= Guid.NewGuid().ToString();
        await Table.AddEntityAsync(entity);
    }

    public async Task<IEnumerable<FaqGroup>> GetAllAsync(int take)
    {
        var results = new List<FaqGroup>();
        await foreach (var item in Table.QueryAsync<FaqGroup>(x => x.PartitionKey == PartitionKey))
        {
            results.Add(item);
            if (results.Count >= take)
            {
                break;
            }
        }

        return results;
    }

    public async Task<FaqGroup?> GetAsync(string id)
    {
        try
        {
            var response = await Table.GetEntityAsync<FaqGroup>(PartitionKey, id);
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

    public async Task UpdateAsync(FaqGroup item)
    {
        item.PartitionKey = PartitionKey;
        await Table.UpdateEntityAsync(item, item.ETag, TableUpdateMode.Replace);
    }
}
