using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Edubase.Data.Repositories;
using Edubase.Web.UI.Controllers.Api;
using Microsoft.WindowsAzure.Storage.Table;

namespace Edubase.Web.UI.MigrationServices
{
    public class GlossaryItemsMigrationService
    {
        private readonly GlossaryRepository _tableStorageGlossaryRepository;
        private readonly ISqlGlossaryItemRepository _sqlGlossaryItemRepository;

        public GlossaryItemsMigrationService(
            GlossaryRepository tableStorageGlossaryRepository,
            ISqlGlossaryItemRepository sqlGlossaryItemRepository)
        {
            _tableStorageGlossaryRepository = tableStorageGlossaryRepository;
            _sqlGlossaryItemRepository = sqlGlossaryItemRepository;
        }

        public async Task<int> MigrateAsync()
        {
            var migrated = 0;
            TableContinuationToken continuationToken = null;

            do
            {
                var page = await _tableStorageGlossaryRepository.GetAllAsync(int.MaxValue, continuationToken);
                foreach (var item in page.Items)
                {
                    await _sqlGlossaryItemRepository.UpsertAsync(new Models.SqlGlossaryItem
                    {
                        PartitionKey = item.PartitionKey,
                        RowKey = item.RowKey,
                        Title = item.Title,
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
