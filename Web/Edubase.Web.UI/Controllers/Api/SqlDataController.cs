using System.Configuration;
using Edubase.Common.Config;
using Edubase.Data.Repositories;
using Microsoft.Data.SqlClient;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.UI;
using AzureTableLogger;
using AzureTableLogger.LogMessages;
using Edubase.Data.Entity;

namespace Edubase.Web.UI.Controllers.Api
{
    [Authorize]
    public class SqlDataController : ApiController
    {
        private readonly IAzLogger _logger;
        private readonly IUserPreferenceRepository _tableStorageUserPreferenceRepository;
        private readonly ISqlUserPreferenceRepository _sqlUserPreferenceRepository;
        private readonly NotificationTemplateRepository _tableStorageNotificationTemplateRepository;
        private readonly ISqlNotificationTemplateRepository _sqlNotificationTemplateRepository;
        private readonly NotificationBannerRepository _tableStorageNotificationBannerRepository;
        private readonly ISqlNotificationBannerRepository _sqlNotificationBannerRepository;
        private readonly FaqItemRepository _tableStorageFaqItemRepository;
        private readonly ISqlFaqItemRepository _sqlFaqItemRepository;

        public SqlDataController(
            IAzLogger logger,
            IUserPreferenceRepository tableStorageUserPreferenceRepository,
            ISqlUserPreferenceRepository sqlUserPreferenceRepository,
            NotificationTemplateRepository tableStorageNotificationTemplateRepository,
            ISqlNotificationTemplateRepository sqlNotificationTemplateRepository,
            NotificationBannerRepository tableStorageNotificationBannerRepository,
            ISqlNotificationBannerRepository sqlNotificationBannerRepository,
            FaqItemRepository tableStorageFaqItemRepository,
            ISqlFaqItemRepository sqlFaqItemRepository)
        {
            _logger = logger;
            _tableStorageUserPreferenceRepository = tableStorageUserPreferenceRepository;
            _sqlUserPreferenceRepository = sqlUserPreferenceRepository;
            _tableStorageNotificationTemplateRepository = tableStorageNotificationTemplateRepository;
            _sqlNotificationTemplateRepository = sqlNotificationTemplateRepository;
            _tableStorageNotificationBannerRepository = tableStorageNotificationBannerRepository;
            _sqlNotificationBannerRepository = sqlNotificationBannerRepository;
            _tableStorageFaqItemRepository = tableStorageFaqItemRepository;
            _sqlFaqItemRepository = sqlFaqItemRepository;
        }


        [Route("api/sql"), HttpGet]
        public async Task<IHttpActionResult> Sql()
        {
            var serverName = ConfigurationManager.AppSettings["SQLServer"];
            var databaseName = ConfigurationManager.AppSettings["SQLDatabase"];
            var connectionString =
                $"Server=tcp:{serverName},1433;" +
                $"database={databaseName};" +
                "authentication=Active Directory Default;" +
                "encrypt=True;TrustServerCertificate=False;";

            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new SqlCommand("SELECT GETUTCDATE();", connection))
                    {
                        var result = await command.ExecuteScalarAsync();
                        return Ok(new { serverDate = result, status = "Connected" });
                    }
                }
            }
            catch (SqlException ex)
            {
                _logger.Log(new WebLogMessage
                {
                    Level = LogMessage.LogLevel.ERROR,
                    Environment = ConfigurationManager.AppSettings["Environment"],
                    Message = $"[api/sql] connection failed. SQLServer='{serverName}', " +
                              $"SQLDatabase='{databaseName}', {ex.GetType().Name}: {ex.Message}",
                    Exception = ex.ToString()
                });
                return StatusCode(HttpStatusCode.ServiceUnavailable);
            }
        }

        [Route("api/migrate-user-preferences"), HttpPost]
        public async Task<IHttpActionResult> MigrateUserPreferencesAsync()
        {
            if (!Feature.IsEnabled("Feature_UserPreferencesMigration"))
            {
                return NotFound();
            }

            var migrated = 0;
            Microsoft.WindowsAzure.Storage.Table.TableContinuationToken continuationToken = null;

            do
            {
                var page = await _tableStorageUserPreferenceRepository.GetAllAsync(skip: continuationToken);
                foreach (var pref in page.Items)
                {
                    await _sqlUserPreferenceRepository.UpsertAsync(new Models.SqlUserPreference
                    {
                        PartitionKey = pref.PartitionKey,
                        RowKey = pref.RowKey,
                        SavedSearchToken = pref.SavedSearchToken
                    });
                    migrated++;
                }
                continuationToken = page.TableContinuationToken;
            }
            while (continuationToken != null);

            return Ok(new { migrated });
        }

        [Route("api/migrate-notification-templates"), HttpPost]
        public async Task<IHttpActionResult> MigrateNotificationTemplatesAsync()
        {
            if (!Feature.IsEnabled("Feature_NotificationTemplatesMigration"))
            {
                return NotFound();
            }

            var migrated = 0;
            Microsoft.WindowsAzure.Storage.Table.TableContinuationToken continuationToken = null;

            do
            {
                var page = await _tableStorageNotificationTemplateRepository.GetAllAsync(int.MaxValue, continuationToken);
                foreach (var pref in page.Items)
                {
                    await _sqlNotificationTemplateRepository.UpsertAsync(new Models.SqlNotificationTemplate
                    {
                        PartitionKey = pref.PartitionKey,
                        RowKey = pref.RowKey,
                        Content = pref.Content
                    });
                    migrated++;
                }
                continuationToken = page.TableContinuationToken;
            }
            while (continuationToken != null);

            return Ok(new { migrated });
        }

        [Route("api/migrate-notification-banners"), HttpPost]
        public async Task<IHttpActionResult> MigrateNotificationBannerAsync()
        {
            if (!Feature.IsEnabled("Feature_NotificationBannersMigration"))
            {
                return NotFound();
            }

            var migrated = 0;
            var partitions = new[] { eNotificationBannerPartition.Current, eNotificationBannerPartition.Archive };

            foreach (var partition in partitions)
            {
                Microsoft.WindowsAzure.Storage.Table.TableContinuationToken continuationToken = null;
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
                            Importance = (byte)banner.Importance,
                            Start = banner.Start,
                            End = banner.End,
                            Version = (byte)banner.Version,
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

            return Ok(new { migrated });
        }

        [Route("api/migrate-faq-items"), HttpPost]
        public async Task<IHttpActionResult> MigrateFaqItemsAsync()
        {
            if (!Feature.IsEnabled("Feature_FaqItemsMigration"))
            {
                return NotFound();
            }

            var migrated = 0;
            Microsoft.WindowsAzure.Storage.Table.TableContinuationToken continuationToken = null;

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

            return Ok(new { migrated });
        }
    }
}
