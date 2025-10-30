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

public class LocalAuthoritySetRepository : TableStorageBase<LocalAuthoritySet>, ILocalAuthoritySetRepository
{
    private const string PartitionKey = "LocalAuthoritySet";

    public LocalAuthoritySetRepository(IConfiguration configuration)
        : base(configuration, "DataConnectionString", "LocalAuthoritySets")
    {
    }

    public async Task CreateAsync(LocalAuthoritySet message)
    {
        message.PartitionKey = PartitionKey;
        message.RowKey ??= Guid.NewGuid().ToString();
        await Table.AddEntityAsync(message);
    }

    public async Task<IEnumerable<LocalAuthoritySet>> GetAllAsync(int? take = null)
    {
        var results = new List<LocalAuthoritySet>();
        await foreach (var item in Table.QueryAsync<LocalAuthoritySet>(x => x.PartitionKey == PartitionKey))
        {
            results.Add(item);
            if (take.HasValue && results.Count >= take.Value)
            {
                break;
            }
        }

        return take.HasValue ? results.Take(take.Value) : results;
    }

    public async Task<LocalAuthoritySet?> GetAsync(string id)
    {
        try
        {
            var response = await Table.GetEntityAsync<LocalAuthoritySet>(PartitionKey, id);
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

    public async Task UpdateAsync(LocalAuthoritySet item)
    {
        item.PartitionKey = PartitionKey;
        await Table.UpdateEntityAsync(item, item.ETag, TableUpdateMode.Replace);
    }
}
