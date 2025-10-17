using Edubase.Data.Entity;
using Edubase.Data.Repositories.TableStorage;
using System.Threading.Tasks;
using MoreLinq;
using System.Collections.Generic;
using System.Linq;
using System;
using Azure.Data.Tables;
using Azure;

namespace Edubase.Data.Repositories;

public class FaqItemRepository : TableStorageBase<FaqItem>
{
    public FaqItemRepository()
        : base("DataConnectionString")
    {
    }

    public async Task CreateAsync(FaqItem entity) => await Table.AddEntityAsync(entity);

    public async Task<IEnumerable<FaqItem>> GetAllAsync(int take)
    {
        var query = Table.QueryAsync<FaqItem>();

        List<FaqItem> results = [];

        await foreach (var item in query)
        {
            results.Add(item);

            if (results.Count >= take)
            {
                return results.Take(take);
            }
        }

        return results;
    }

    public async Task<FaqItem> GetAsync(string id)
    {
        try
        {
            var response = await Table.GetEntityAsync<FaqItem>(partitionKey: string.Empty, rowKey: id);
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

        if (item != null)
        {
            await Table.DeleteEntityAsync(item.PartitionKey, item.RowKey);
        }
    }

    public async Task UpdateAsync(FaqItem item) => await Table.UpdateEntityAsync(item, item.ETag, TableUpdateMode.Replace);
    
}
