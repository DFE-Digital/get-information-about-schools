using System.Threading.Tasks;
using Azure.Data.Tables;
using Edubase.Data.Entity;

namespace Edubase.Data.Repositories;

public class ApiRecorderSessionItemRepository
{
    private const string TableNameKey = "ApiRecorderSessionItems";

    private readonly TableClient _apiRecorderSessionTableClient;

    public ApiRecorderSessionItemRepository(TableServiceClient tableServiceClient)
    {
        _apiRecorderSessionTableClient = tableServiceClient.GetTableClient(TableNameKey);
    }

    public async Task CreateAsync(ApiRecorderSessionItem message)
    {
        await _apiRecorderSessionTableClient.AddEntityAsync(message);
    }
}

