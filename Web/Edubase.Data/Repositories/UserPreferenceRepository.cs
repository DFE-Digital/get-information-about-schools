using System;
using System.Threading.Tasks;
using Azure;
using Edubase.Data.Entity;
using Edubase.Data.Repositories.TableStorage;
using Microsoft.Extensions.Configuration;

namespace Edubase.Data.Repositories;

public class UserPreferenceRepository : TableStorageBase<UserPreference>, IUserPreferenceRepository
{
    private const string PartitionKey = "UserPreference";

    public UserPreferenceRepository(IConfiguration configuration)
        : base(configuration, "DataConnectionString", "UserPreferences")
    {
    }

    public async Task<UserPreference?> GetAsync(string userId)
    {
        try
        {
            var response = await Table.GetEntityAsync<UserPreference>(PartitionKey, userId);
            return response.Value;
        }
        catch (RequestFailedException ex) when (ex.Status == 404)
        {
            return null;
        }
    }

    public UserPreference? Get(string userId)
    {
        try
        {
            var response = Table.GetEntity<UserPreference>(PartitionKey, userId);
            return response.Value;
        }
        catch (RequestFailedException ex) when (ex.Status == 404)
        {
            return null;
        }
    }

    public async Task UpsertAsync(UserPreference item)
    {
        item.PartitionKey = PartitionKey;
        item.RowKey ??= item.UserId ?? Guid.NewGuid().ToString("N").Substring(0, 8);
        await Table.UpsertEntityAsync(item);
    }
}
