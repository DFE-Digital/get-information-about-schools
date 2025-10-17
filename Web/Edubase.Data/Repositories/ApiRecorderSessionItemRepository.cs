using Edubase.Data.Entity;
using Edubase.Data.Repositories.TableStorage;
using Microsoft.WindowsAzure.Storage.Table;
using System.Threading.Tasks;

namespace Edubase.Data.Repositories;

public class ApiRecorderSessionItemRepository : TableStorageBase<ApiRecorderSessionItem>
{
    public ApiRecorderSessionItemRepository()
        : base("DataConnectionString")
    {
    }

    public async Task CreateAsync(ApiRecorderSessionItem message) => await Table.AddEntityAsync(message);
}
