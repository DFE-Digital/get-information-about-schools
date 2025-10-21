using System.Collections.Generic;
using System.Threading.Tasks;
using Edubase.Data.Entity;

namespace Edubase.Services.DataQuality;

public interface IDataQualityReadService
{
    Task<IEnumerable<DataQualityStatus>> GetDataQualityStatus();
}
