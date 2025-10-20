using Edubase.Data.Entity;
using Edubase.Data.Repositories.TableStorage;
using Microsoft.WindowsAzure.Storage.Table;
using System.Threading.Tasks;
using MoreLinq;
using System.Collections.Generic;
using System.Linq;
using System;
using Azure;
using Azure.Data.Tables;

namespace Edubase.Data.Repositories;

public class LocalAuthoritySetRepository : TableStorageBase<LocalAuthoritySet>, ILocalAuthoritySetRepository
{
    public LocalAuthoritySetRepository()
        : base("DataConnectionString")
    {
    }

    public async Task CreateAsync(LocalAuthoritySet message) => await Table.AddEntityAsync(message);

    public async Task<IEnumerable<LocalAuthoritySet>> GetAllAsync(int? take = null)
    {
        var query = Table.QueryAsync<LocalAuthoritySet>();

        List<LocalAuthoritySet> results = [];

        await foreach (var item in query) // Haven't specified how large each page is, so many return more than Take
        {
            results.Add(item);

            if (take.HasValue && results.Count >= take)
            {
                return results.Take(take.Value);
            }
        }

        return results;
    }

    public async Task<LocalAuthoritySet> GetAsync(string id)
    {
        try
        {
            var response = await Table.GetEntityAsync<LocalAuthoritySet>(partitionKey: string.Empty, rowKey: id);
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

    public async Task UpdateAsync(LocalAuthoritySet item) => await Table.UpdateEntityAsync(item, item.ETag, TableUpdateMode.Replace);

}
