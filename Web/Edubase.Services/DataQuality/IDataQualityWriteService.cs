using System;
using System.Threading.Tasks;
using Edubase.Data.Entity;

namespace Edubase.Services.DataQuality
{
    public interface IDataQualityWriteService : IDataQualityReadService
    {
        Task UpdateDataQualityDate(DataQualityStatus.DataQualityEstablishmentType establishmentType, DateTime updateTime);
    }
}