using System.Threading.Tasks;
using Edubase.Data.Repositories;
using Edubase.Web.UI.Controllers.Api;
using Microsoft.WindowsAzure.Storage.Table;

namespace Edubase.Web.UI.MigrationServices
{
    public class TokensMigrationService
    {
        private readonly ITokenRepository _tableStorageTokenRepository;
        private readonly ISqlTokenRepository _sqlTokenRepository;

        public TokensMigrationService(
            ITokenRepository tableStorageTokenRepository,
            ISqlTokenRepository sqlTokenRepository)
        {
            _tableStorageTokenRepository = tableStorageTokenRepository;
            _sqlTokenRepository = sqlTokenRepository;
        }

        public async Task<int> MigrateAsync()
        {
            var migrated = 0;
            TableContinuationToken continuationToken = null;

            do
            {
                var page = await _tableStorageTokenRepository.GetAllAsync(int.MaxValue, continuationToken);
                foreach (var item in page.Items)
                {
                    await _sqlTokenRepository.UpsertAsync(new Models.SqlToken
                    {
                        PartitionKey = item.PartitionKey,
                        RowKey = item.RowKey,
                        Data = item.Data
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
