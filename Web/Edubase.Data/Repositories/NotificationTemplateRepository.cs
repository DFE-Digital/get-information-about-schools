using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure;
using Azure.Data.Tables;
using Edubase.Data.Entity;
using Edubase.Data.Repositories.TableStorage;
using MoreLinq;

namespace Edubase.Data.Repositories;

public class NotificationTemplateRepository : TableStorageBase<NotificationTemplate>
{
    public NotificationTemplateRepository()
        : base("DataConnectionString")
    {
    }

    public async Task CreateAsync(NotificationTemplate entity) => await Table.AddEntityAsync(entity);


    public async Task<IEnumerable<NotificationTemplate>> GetAllAsync(int take)
    {
        var query = Table.QueryAsync<NotificationTemplate>();

        List<NotificationTemplate> results = [];
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


    public async Task<NotificationTemplate?> GetAsync(string id)
    {
        var query = Table.QueryAsync<NotificationTemplate>(
            (template) =>
                template.PartitionKey == string.Empty &&
                template.RowKey == id
        );

        await foreach (var template in query)
        {
            return template;
        }

        return null;
    }


    public async Task DeleteAsync(string id)
    {
        var item = await GetAsync(id);

        if(item is not null)
        {
            await Table.DeleteEntityAsync(item);
        }
    }

    public async Task UpdateAsync(NotificationTemplate item) => await Table.UpdateEntityAsync(item, item.ETag, TableUpdateMode.Replace);

}
