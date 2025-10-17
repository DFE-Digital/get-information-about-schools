using System.Collections.Generic;
using System.Threading.Tasks;
using Edubase.Data.Entity;
using Edubase.Data.Repositories;

namespace Edubase.Services.DataQuality;

public class DataQualityReadService : IDataQualityReadService
{
    public DataQualityReadService(IDataQualityStatusRepository repository)
    {
        Repository = repository;
    }

    protected IDataQualityStatusRepository Repository { get; }

    public async Task<IEnumerable<DataQualityStatus>> GetDataQualityStatus()
    {
        return await Repository.GetAllAsync();
    }
}
