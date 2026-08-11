using System.Threading.Tasks;
using Edubase.Data.Repositories;
using Edubase.Web.UI.Controllers.Api;
using Microsoft.WindowsAzure.Storage.Table;

namespace Edubase.Web.UI.MigrationServices
{
    public class LocalAuthoritySetsMigrationService
    {
        private readonly ILocalAuthoritySetRepository _tableStorageLocalAuthoritySetRepository;
        private readonly ISqlLocalAuthoritySetRepository _sqlLocalAuthoritySetRepository;

        public LocalAuthoritySetsMigrationService(
            ILocalAuthoritySetRepository tableStorageLocalAuthoritySetRepository,
            ISqlLocalAuthoritySetRepository sqlLocalAuthoritySetRepository)
        {
            _tableStorageLocalAuthoritySetRepository = tableStorageLocalAuthoritySetRepository;
            _sqlLocalAuthoritySetRepository = sqlLocalAuthoritySetRepository;
        }

        public async Task<int> MigrateAsync()
        {
            var migrated = 0;
            TableContinuationToken continuationToken = null;

            do
            {
                var page = await _tableStorageLocalAuthoritySetRepository.GetAllAsync(int.MaxValue, continuationToken);
                foreach (var set in page.Items)
                {
                    await _sqlLocalAuthoritySetRepository.UpsertAsync(new Models.SqlLocalAuthoritySet
                    {
                        PartitionKey = set.PartitionKey,
                        RowKey = set.RowKey,
                        Title = set.Title,
                        IdData = set.IdData
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
