using System.Threading.Tasks;
using Edubase.Data.Entity;
using Edubase.Data.Repositories;
using Edubase.Web.UI.Controllers.Api;
using Microsoft.WindowsAzure.Storage.Table;

namespace Edubase.Web.UI.MigrationServices
{
    public class NotificationBannersMigrationService
    {
        private readonly NotificationBannerRepository _tableStorageNotificationBannerRepository;
        private readonly ISqlNotificationBannerRepository _sqlNotificationBannerRepository;

        public NotificationBannersMigrationService(
            NotificationBannerRepository tableStorageNotificationBannerRepository,
            ISqlNotificationBannerRepository sqlNotificationBannerRepository)
        {
            _tableStorageNotificationBannerRepository = tableStorageNotificationBannerRepository;
            _sqlNotificationBannerRepository = sqlNotificationBannerRepository;
        }

        public async Task<int> MigrateAsync()
        {
            var migrated = 0;
            var partitions = new[] { eNotificationBannerPartition.Current, eNotificationBannerPartition.Archive };

            foreach (var partition in partitions)
            {
                TableContinuationToken continuationToken = null;

                do
                {
                    var page = await _tableStorageNotificationBannerRepository.GetAllAsync(int.MaxValue, continuationToken, false, partition);
                    foreach (var banner in page.Items)
                    {
                        await _sqlNotificationBannerRepository.UpsertAsync(new Models.SqlNotificationBanner
                        {
                            PartitionKey = banner.PartitionKey,
                            RowKey = banner.RowKey,
                            Content = banner.Content,
                            Importance = (byte) banner.Importance,
                            Start = banner.Start,
                            End = banner.End,
                            Version = (byte) banner.Version,
                            Tracker = banner.Tracker,
                            AuditUser = int.TryParse(banner.AuditUser, out var auditUserId) ? auditUserId : 0,
                            AuditEvent = banner.AuditEvent,
                            AuditTimeStamp = banner.AuditTimestamp
                        });
                        migrated++;
                    }
                    continuationToken = page.TableContinuationToken;
                }
                while (continuationToken != null);
            }

            return migrated;
        }
    }
}
