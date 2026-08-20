using System.Threading.Tasks;
using Edubase.Data.Repositories;
using Edubase.Web.UI.Controllers.Api;
using Edubase.Web.UI.Models;

namespace Edubase.Web.UI.MigrationServices
{
    public class DataQualityStatusMigrationService
    {
        private readonly IDataQualityStatusRepository _tableStorageDataQualityStatusRepository;
        private readonly ISqlDataQualityStatusRepository  _sqlDataQualityStatusRepository;

        public DataQualityStatusMigrationService(
            IDataQualityStatusRepository tableStorageDataQualityStatusRepository,
            ISqlDataQualityStatusRepository sqlDataQualityStatusRepository)
        {
            _tableStorageDataQualityStatusRepository = tableStorageDataQualityStatusRepository;
            _sqlDataQualityStatusRepository = sqlDataQualityStatusRepository;
        }

        public async Task<int> MigrateAsync()
        {
            var items = await _tableStorageDataQualityStatusRepository.GetAllAsync();
            var migrated = 0;

            foreach (var item in items)
            {
                await _sqlDataQualityStatusRepository.UpsertAsync(new SqlDataQualityStatus()
                {
                    PartitionKey = item.PartitionKey,
                    RowKey = item.RowKey,
                    LastUpdated = item.LastUpdated,
                    DataOwner = item.DataOwner,
                    Email = item.Email
                });
                migrated++;
            }
            return migrated;
        }
    }
}
