using System.Threading.Tasks;
using Edubase.Data.Repositories;
using Edubase.Web.UI.Controllers.Api;
using Microsoft.WindowsAzure.Storage.Table;

namespace Edubase.Web.UI.MigrationServices
{
    public class FaqItemsMigrationService
    {
        private readonly FaqItemRepository _tableStorageFaqItemRepository;
        private readonly ISqlFaqItemRepository _sqlFaqItemRepository;

        public FaqItemsMigrationService(
            FaqItemRepository tableStorageFaqItemRepository,
            ISqlFaqItemRepository sqlFaqItemRepository)
        {
            _tableStorageFaqItemRepository = tableStorageFaqItemRepository;
            _sqlFaqItemRepository = sqlFaqItemRepository;
        }

        public async Task<int> MigrateAsync()
        {
            var migrated = 0;
            TableContinuationToken continuationToken = null;

            do
            {
                var page = await _tableStorageFaqItemRepository.GetAllAsync(int.MaxValue, continuationToken);
                foreach (var item in page.Items)
                {
                    await _sqlFaqItemRepository.UpsertAsync(new Models.SqlFaqItem
                    {
                        PartitionKey = item.PartitionKey,
                        RowKey = item.RowKey,
                        Title = item.Title,
                        Content = item.Content,
                        DisplayOrder = item.DisplayOrder,
                        GroupId = item.GroupId
                    });
                    migrated++;
                }
                continuationToken = page.TableContinuationToken;
            }
            while (continuationToken != null);

            return migrated;
        }
    }
}
