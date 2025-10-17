using Azure;
using Azure.Data.Tables;
using Edubase.Data.Entity;
using Edubase.Data.Repositories.TableStorage;
using MoreLinq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Edubase.Data.Repositories;

public class UserPreferenceRepository : TableStorageBase<UserPreference>, IUserPreferenceRepository
{
    public UserPreferenceRepository() : base("DataConnectionString") { }

    public async Task<UserPreference> GetAsync(string userId)
    {
        try
        {
            var response = await Table.GetEntityAsync<UserPreference>(partitionKey: string.Empty, rowKey: userId);
            return response.Value;
        }
        catch (RequestFailedException ex) when (ex.Status == 404)
        {
            return null;
        }
    }

    public UserPreference Get(string userId)
    {
        try
        {
            var response = Table.GetEntity<UserPreference>(partitionKey: string.Empty, rowKey: userId);
            return response.Value;
        }
        catch (RequestFailedException ex) when (ex.Status == 404)
        {
            return null;
        }
    }

    public async Task UpsertAsync(UserPreference item) => await Table.UpsertEntityAsync(item);
    
}
