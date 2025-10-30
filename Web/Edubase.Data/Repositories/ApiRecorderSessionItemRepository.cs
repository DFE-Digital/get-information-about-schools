using System.Threading.Tasks;
using Edubase.Data.Entity;
using Edubase.Data.Repositories.TableStorage;
using Microsoft.Extensions.Configuration;


namespace Edubase.Data.Repositories;

public class ApiRecorderSessionItemRepository : TableStorageBase<ApiRecorderSessionItem>
{
    public ApiRecorderSessionItemRepository(IConfiguration configuration)
        : base(configuration, "DataConnectionString", "ApiRecorderSessionItems")
    {
    }

    public async Task CreateAsync(ApiRecorderSessionItem message)
    {
        await Table.AddEntityAsync(message);
    }
}

