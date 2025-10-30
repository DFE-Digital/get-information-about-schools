using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Azure;
using Azure.Data.Tables;
using Edubase.Data.Entity;
using Edubase.Data.Repositories.TableStorage;
using Microsoft.Extensions.Configuration;

namespace Edubase.Data.Repositories;

public class NotificationTemplateRepository : TableStorageBase<NotificationTemplate>
{
    private const string PartitionKey = "NotificationTemplate";

    public NotificationTemplateRepository(IConfiguration configuration)
        : base(configuration, "DataConnectionString", "NotificationTemplates"){
    }

    public async Task CreateAsync(NotificationTemplate entity)
    {
        entity.PartitionKey = PartitionKey;
        entity.RowKey ??= Guid.NewGuid().ToString("N").Substring(0, 8);
        await Table.AddEntityAsync(entity);
    }

    public async Task<IEnumerable<NotificationTemplate>> GetAllAsync(int take)
    {
        var results = new List<NotificationTemplate>();
        await foreach (var item in Table.QueryAsync<NotificationTemplate>(x => x.PartitionKey == PartitionKey))
        {
            results.Add(item);
            if (results.Count >= take)
            {
                break;
            }
        }

        return results;
    }

    public async Task<NotificationTemplate?> GetAsync(string id)
    {
        try
        {
            var response = await Table.GetEntityAsync<NotificationTemplate>(PartitionKey, id);
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

    public async Task UpdateAsync(NotificationTemplate item)
    {
        item.PartitionKey = PartitionKey;
        await Table.UpdateEntityAsync(item, item.ETag, TableUpdateMode.Replace);
    }
}
