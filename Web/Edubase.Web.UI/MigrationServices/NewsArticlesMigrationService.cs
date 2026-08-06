using System.Threading.Tasks;
using Edubase.Data.Entity;
using Edubase.Data.Repositories;
using Edubase.Web.UI.Controllers.Api;
using Microsoft.WindowsAzure.Storage.Table;

namespace Edubase.Web.UI.MigrationServices
{
    public class NewsArticlesMigrationService
    {
        private readonly NewsArticleRepository _tableStorageNewsArticleRepository;
        private readonly ISqlNewsArticleRepository _sqlNewsArticleRepository;

        public NewsArticlesMigrationService(
            NewsArticleRepository tableStorageNewsArticleRepository,
            ISqlNewsArticleRepository sqlNewsArticleRepository)
        {
            _tableStorageNewsArticleRepository = tableStorageNewsArticleRepository;
            _sqlNewsArticleRepository = sqlNewsArticleRepository;
        }

        public async Task<int> MigrateAsync()
        {
            var migrated = 0;
            var partitions = new[] { eNewsArticlePartition.Current, eNewsArticlePartition.Archive };

            foreach (var partition in partitions)
            {
                TableContinuationToken continuationToken = null;
                do
                {
                    var page = await _tableStorageNewsArticleRepository.GetAllAsync(int.MaxValue, false, null, continuationToken, partition);
                    foreach (var article in page.Items)
                    {
                        await _sqlNewsArticleRepository.UpsertAsync(new Models.SqlNewsArticle
                        {
                            PartitionKey = article.PartitionKey,
                            RowKey = article.RowKey,
                            Title = article.Title,
                            ArticleDate = article.ArticleDate,
                            ShowDate = article.ShowDate,
                            Content = article.Content,
                            Version = (byte) article.Version,
                            Tracker = article.Tracker,
                            AuditUser = int.TryParse(article.AuditUser, out var auditUserId) ? auditUserId : 0,
                            AuditEvent = article.AuditEvent,
                            AuditTimeStamp = article.AuditTimestamp
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
