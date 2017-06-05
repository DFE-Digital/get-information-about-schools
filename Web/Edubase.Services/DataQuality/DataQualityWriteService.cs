using System;
using System.Threading.Tasks;
using Edubase.Data.Entity;
using Edubase.Data.Repositories;

namespace Edubase.Services.DataQuality
{
    public class DataQualityWriteService : DataQualityReadService, IDataQualityWriteService
    {
        public DataQualityWriteService(IDataQualityStatusRepository repository) : base(repository)
        {
        }

        public async Task UpdateDataQualityDate(DataQualityStatus.DataQualityEstablishmentType establishmentType,
            DateTime updateTime)
        {
            await Repository.UpdateDataQualityAsync(establishmentType, updateTime);
        }
    }
}
