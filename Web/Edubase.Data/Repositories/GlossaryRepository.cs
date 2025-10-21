using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure;
using Azure.Data.Tables;
using Edubase.Data.Entity;
using Edubase.Data.Repositories.TableStorage;

namespace Edubase.Data.Repositories;

public class GlossaryRepository : TableStorageBase<GlossaryItem>
{
    public GlossaryRepository()
        : base("DataConnectionString")
    {
    }

    public async Task CreateAsync(GlossaryItem message) => await Table.AddEntityAsync(message);

    public async Task<IEnumerable<GlossaryItem>> GetAllAsync(int take)
    {
        var query = Table.QueryAsync<GlossaryItem>();

        List<GlossaryItem> results = [];

        await foreach (var item in query)
        {
            results.Add(item);
            if (results.Count >= take)
            {
                break;
            }
        }

        return results.Take(take);
    }

    public async Task<GlossaryItem?> GetAsync(string id)
    {
        try
        {
            var response = await Table.GetEntityAsync<GlossaryItem>(partitionKey: string.Empty, rowKey: id);
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

    public async Task UpdateAsync(GlossaryItem item) => await Table.UpdateEntityAsync(item, item.ETag, TableUpdateMode.Replace);

}
