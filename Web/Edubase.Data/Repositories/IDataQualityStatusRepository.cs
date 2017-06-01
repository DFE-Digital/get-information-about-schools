using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Edubase.Data.Entity;

namespace Edubase.Data.Repositories
{
    public interface IDataQualityStatusRepository
    {
        Task<List<DataQualityStatus>> GetAllAsync();
        Task UpdateDataQuality(DataQualityStatus.DataQualityEstablishmentType establishmentType, DateTime lastUpdated);
    }
}