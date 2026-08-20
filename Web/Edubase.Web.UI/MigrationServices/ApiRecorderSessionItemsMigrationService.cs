using System.Linq;
using System.Threading.Tasks;
using Edubase.Data.Repositories;
using Edubase.Web.UI.Controllers.Api;
using Microsoft.WindowsAzure.Storage.Table;

namespace Edubase.Web.UI.MigrationServices
{
    public class ApiRecorderSessionItemsMigrationService
    {
        private readonly ApiRecorderSessionItemRepository _tableStorageApiRecorderSessionItemRepository;
        private readonly ISqlApiRecorderSessionItemRepository _sqlApiRecorderSessionItemRepository;

        public ApiRecorderSessionItemsMigrationService(
            ApiRecorderSessionItemRepository tableStorageApiRecorderSessionItemRepository,
            ISqlApiRecorderSessionItemRepository sqlApiRecorderSessionItemRepository)
        {
            _tableStorageApiRecorderSessionItemRepository = tableStorageApiRecorderSessionItemRepository;
            _sqlApiRecorderSessionItemRepository = sqlApiRecorderSessionItemRepository;
        }

        public async Task<int> MigrateAsync()
        {
            var migrated = 0;
            TableContinuationToken continuationToken = null;

            do
            {
                var page = await _tableStorageApiRecorderSessionItemRepository.GetAllAsync(int.MaxValue, continuationToken);
                var batch = page.Items.Select(item => new Models.SqlApiRecorderSessionItem
                {
                    PartitionKey = item.PartitionKey,
                    RowKey = item.RowKey,
                    HttpMethod = item.HttpMethod,
                    Path = item.Path,
                    RequestHeaders = item.RequestHeaders,
                    ResponseHeaders = item.ResponseHeaders,
                    RawRequestBody = item.RawRequestBody,
                    RawResponseBody = item.RawResponseBody,
                    ElapsedTimeSpan = item.ElapsedTimeSpan,
                    ElapsedMS = item.ElapsedMS
                }).ToList();

                await _sqlApiRecorderSessionItemRepository.UpsertBatchAsync(batch);
                migrated += batch.Count;
                continuationToken = page.TableContinuationToken;
            }
            while (continuationToken != null);

            return migrated;
        }
    }
}
