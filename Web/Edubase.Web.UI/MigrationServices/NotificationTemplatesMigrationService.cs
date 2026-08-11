using System.Threading.Tasks;
using Edubase.Data.Repositories;
using Edubase.Web.UI.Controllers.Api;
using Microsoft.WindowsAzure.Storage.Table;

namespace Edubase.Web.UI.MigrationServices
{
    public class NotificationTemplatesMigrationService
    {
        private readonly NotificationTemplateRepository _tableStorageNotificationTemplateRepository;
        private readonly ISqlNotificationTemplateRepository _sqlNotificationTemplateRepository;

        public NotificationTemplatesMigrationService(
            NotificationTemplateRepository tableStorageNotificationTemplateRepository,
            ISqlNotificationTemplateRepository sqlNotificationTemplateRepository)
        {
            _tableStorageNotificationTemplateRepository = tableStorageNotificationTemplateRepository;
            _sqlNotificationTemplateRepository = sqlNotificationTemplateRepository;
        }

        public async Task<int> MigrateAsync()
        {
            var migrated = 0;
            TableContinuationToken continuationToken = null;

            do
            {
                var page = await _tableStorageNotificationTemplateRepository.GetAllAsync(int.MaxValue, continuationToken);
                foreach (var item in page.Items)
                {
                    await _sqlNotificationTemplateRepository.UpsertAsync(new Models.SqlNotificationTemplate
                    {
                        PartitionKey = item.PartitionKey,
                        RowKey = item.RowKey,
                        Content = item.Content
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
