using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Edubase.Data.Entity;

namespace Edubase.Data.Repositories
{
    public interface IDataQualityStatusRepository
    {
        Task<List<DataQualityStatus>> GetAllAsync();
        Task UpdateDataQualityAsync(DataQualityStatus.DataQualityEstablishmentType establishmentType, DateTime lastUpdated);
        Task UpdateDataQualityDataOwnerDetailsAsync(DataQualityStatus.DataQualityEstablishmentType establishmentType, string dataOwnerName, string dataOwnerEmail);
    }
}
