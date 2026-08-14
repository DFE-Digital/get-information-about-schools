using System.Threading.Tasks;
using Edubase.Data.Repositories;
using Edubase.Web.UI.Controllers.Api;
using Microsoft.WindowsAzure.Storage.Table;

namespace Edubase.Web.UI.MigrationServices
{
    public class FaqGroupsMigrationService
    {
        private readonly FaqGroupRepository _tableStorageFaqGroupRepository;
        private readonly ISqlFaqGroupRepository _sqlFaqGroupRepository;

        public FaqGroupsMigrationService(
            FaqGroupRepository tableStorageFaqGroupRepository,
            ISqlFaqGroupRepository sqlFaqGroupRepository)
        {
            _tableStorageFaqGroupRepository = tableStorageFaqGroupRepository;
            _sqlFaqGroupRepository = sqlFaqGroupRepository;
        }

        public async Task<int> MigrateAsync()
        {
            var migrated = 0;
            TableContinuationToken continuationToken = null;

            do
            {
                var page = await _tableStorageFaqGroupRepository.GetAllAsync(int.MaxValue, continuationToken);
                foreach (var group in page.Items)
                {
                    await _sqlFaqGroupRepository.UpsertAsync(new Models.SqlFaqGroup
                    {
                        PartitionKey = group.PartitionKey,
                        RowKey = group.RowKey,
                        GroupName = group.GroupName,
                        DisplayOrder = group.DisplayOrder
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
