using System.Configuration;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using AzureTableLogger;
using AzureTableLogger.LogMessages;
using Edubase.Common.Config;
using Edubase.Data.Repositories;
using Edubase.Web.UI.MigrationServices;
using Microsoft.Data.SqlClient;

namespace Edubase.Web.UI.Controllers.Api
{
    [Authorize]
    public class SqlDataController : ApiController
    {
        private readonly IAzLogger _logger;
        private readonly IUserPreferenceRepository _tableStorageUserPreferenceRepository;
        private readonly ISqlUserPreferenceRepository _sqlUserPreferenceRepository;

        private readonly GlossaryItemsMigrationService _glossaryItemsMigrationService;
        private readonly FaqGroupsMigrationService _faqGroupsMigrationService;
        private readonly FaqItemsMigrationService _faqItemsMigrationService;
        private readonly LocalAuthoritySetsMigrationService _localAuthoritySetsMigrationService;
        private readonly NewsArticlesMigrationService _sqlNewsArticleMigrationService;
        private readonly NotificationBannersMigrationService _sqlNotificationsBannersMigrationService;
        private readonly NotificationTemplatesMigrationService _sqlNotificationsTemplatesMigrationService;
        private readonly UserPreferencesMigrationService _userPreferencesMigrationService;
        private readonly DataQualityStatusMigrationService _dataQualityStatusMigrationService;
        public SqlDataController(
            IAzLogger logger,

            GlossaryItemsMigrationService glossaryItemsMigrationService,
            FaqGroupsMigrationService faqGroupsMigrationService,
            FaqItemsMigrationService faqItemsMigrationService,
            LocalAuthoritySetsMigrationService localAuthoritySetsMigrationService,
            NewsArticlesMigrationService sqlNewsArticleMigrationService,
            NotificationBannersMigrationService sqlNotificationsBannersMigrationService,
            NotificationTemplatesMigrationService sqlNotificationsTemplatesMigrationService,
            UserPreferencesMigrationService userPreferencesMigrationService,
            DataQualityStatusMigrationService dataQualityStatusMigrationService)
        {
            _logger = logger;

            _glossaryItemsMigrationService = glossaryItemsMigrationService;
            _faqGroupsMigrationService = faqGroupsMigrationService;
            _faqItemsMigrationService = faqItemsMigrationService;
            _localAuthoritySetsMigrationService = localAuthoritySetsMigrationService;
            _sqlNewsArticleMigrationService = sqlNewsArticleMigrationService;
            _sqlNotificationsBannersMigrationService = sqlNotificationsBannersMigrationService;
            _sqlNotificationsTemplatesMigrationService = sqlNotificationsTemplatesMigrationService;
            _userPreferencesMigrationService = userPreferencesMigrationService;
            _dataQualityStatusMigrationService = dataQualityStatusMigrationService;
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

            var migrated = await _userPreferencesMigrationService.MigrateAsync();
            return Ok(new { migrated });
        }

        [Route("api/migrate-notification-templates"), HttpPost]
        public async Task<IHttpActionResult> MigrateNotificationTemplatesAsync()
        {
            if (!Feature.IsEnabled("Feature_NotificationTemplatesMigration"))
            {
                return NotFound();
            }

            var migrated = await _sqlNotificationsTemplatesMigrationService.MigrateAsync();
            return Ok(new { migrated });
        }

        [Route("api/migrate-notification-banners"), HttpPost]
        public async Task<IHttpActionResult> MigrateNotificationBannerAsync()
        {
            if (!Feature.IsEnabled("Feature_NotificationBannersMigration"))
            {
                return NotFound();
            }

            var migrated = await _sqlNotificationsBannersMigrationService.MigrateAsync();
            return Ok(new { migrated });
        }

        [Route("api/migrate-news-articles"), HttpPost]
        public async Task<IHttpActionResult> MigrateNewsArticlesAsync()
        {
            if (!Feature.IsEnabled("Feature_NewsArticlesMigration"))
            {
                return NotFound();
            }

            var migrated = await _sqlNewsArticleMigrationService.MigrateAsync();
            return Ok(new { migrated });
        }

        [Route("api/migrate-local-authority-sets"), HttpPost]
        public async Task<IHttpActionResult> MigrateLocalAuthoritySetsAsync()
        {
            if (!Feature.IsEnabled("Feature_LocalAuthoritySetsMigration"))
            {
                return NotFound();
            }

            var migrated = await _localAuthoritySetsMigrationService.MigrateAsync();
            return Ok(new { migrated });
        }

        [Route("api/migrate-faq-items"), HttpPost]
        public async Task<IHttpActionResult> MigrateFaqItemsAsync()
        {
            if (!Feature.IsEnabled("Feature_FaqItemsMigration"))
            {
                return NotFound();
            }

            var migrated = await _faqItemsMigrationService.MigrateAsync();
            return Ok(new { migrated });
        }

        [Route("api/migrate-faq-groups"), HttpPost]
        public async Task<IHttpActionResult> MigrateFaqGroupsAsync()
        {
            if (!Feature.IsEnabled("Feature_FaqGroupsMigration"))
            {
                return NotFound();
            }

            var migrated = await _faqGroupsMigrationService.MigrateAsync();
            return Ok(new { migrated });
        }

        [Route("api/migrate-glossary-items"), HttpPost]
        public async Task<IHttpActionResult> MigrateGlossaryItemsAsync()
        {
            if (!Feature.IsEnabled("Feature_GlossaryItemsMigration"))
            {
                return NotFound();
            }

            var migrated = await _glossaryItemsMigrationService.MigrateAsync();
            return Ok(new { migrated });
        }

        [Route("api/migrate-data-quality-status"), HttpPost]
        public async Task<IHttpActionResult> MigrateDataQualityStatusAsync()
        {
            if (!Feature.IsEnabled("Feature_DataQualityStatusMigration"))
            {
                return NotFound();
            }

            var migrated = await _dataQualityStatusMigrationService.MigrateAsync();
            return Ok(new { migrated });
        }
    }
}