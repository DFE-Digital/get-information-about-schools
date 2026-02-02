using System;
using System.Threading.Tasks;
using Azure;
using Azure.Data.Tables;
using Edubase.Data.Entity;

namespace Edubase.Data.Repositories;

public class UserPreferenceRepository : IUserPreferenceRepository
{
    private const string PartitionKey = "UserPreference";
    private const string TableNameKey = "UserPreferences";

    private readonly TableClient _userPreferenceTableClient;

    public UserPreferenceRepository(TableServiceClient tableServiceClient)
    {
        _userPreferenceTableClient = tableServiceClient.GetTableClient(TableNameKey);
    }

    public async Task<UserPreference?> GetAsync(string userId)
    {
        try
        {
            var response = await _userPreferenceTableClient.GetEntityAsync<UserPreference>(PartitionKey, userId);
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
            var response = _userPreferenceTableClient.GetEntity<UserPreference>(PartitionKey, userId);
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
        await _userPreferenceTableClient.UpsertEntityAsync(item);
    }
}
