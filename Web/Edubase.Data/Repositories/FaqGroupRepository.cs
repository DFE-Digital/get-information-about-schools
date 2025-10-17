using System.ClientModel;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure;
using Azure.Data.Tables;
using Edubase.Data.Entity;
using Edubase.Data.Repositories.TableStorage;
using MoreLinq;

namespace Edubase.Data.Repositories;

public class FaqGroupRepository : TableStorageBase<FaqGroup>
{
    public FaqGroupRepository()
        : base("DataConnectionString")
    {
    }

    public async Task CreateAsync(FaqGroup entity) => await Table.AddEntityAsync(entity);

    public async Task<IEnumerable<FaqGroup>> GetAllAsync(int take)
    {
        var query = Table.QueryAsync<FaqGroup>();

        List<FaqGroup> results = [];
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

    public async Task<FaqGroup> GetAsync(string id)
    {

        try
        {
            var response = await Table.GetEntityAsync<FaqGroup>(partitionKey: string.Empty, rowKey: id);
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

    public async Task UpdateAsync(FaqGroup item) => await Table.UpdateEntityAsync(item, item.ETag, TableUpdateMode.Replace);
}
