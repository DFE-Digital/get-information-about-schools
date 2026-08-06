using System.Threading.Tasks;
using Edubase.Data.Repositories;
using Edubase.Web.UI.Controllers.Api;
using Microsoft.WindowsAzure.Storage.Table;

namespace Edubase.Web.UI.MigrationServices
{
    public class UserPreferencesMigrationService
    {
        private readonly UserPreferenceRepository _tableStorageUserPreferenceRepository;
        private readonly ISqlUserPreferenceRepository _sqlUserPreferenceRepository;

        public UserPreferencesMigrationService(
            UserPreferenceRepository tableStorageUserPreferenceRepository,
            ISqlUserPreferenceRepository sqlUserPreferenceRepository)
        {
            _tableStorageUserPreferenceRepository = tableStorageUserPreferenceRepository;
            _sqlUserPreferenceRepository = sqlUserPreferenceRepository;
        }

        public async Task<int> MigrateAsync()
        {
            var migrated = 0;
            TableContinuationToken continuationToken = null;

            do
            {
                var page = await _tableStorageUserPreferenceRepository.GetAllAsync(int.MaxValue, continuationToken);
                foreach (var item in page.Items)
                {
                    await _sqlUserPreferenceRepository.UpsertAsync(new Models.SqlUserPreference
                    {
                        PartitionKey = item.PartitionKey,
                        RowKey = item.RowKey,
                        SavedSearchToken = item.SavedSearchToken
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
