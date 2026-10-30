using System.Collections.Generic;
using System.Threading.Tasks;
using Edubase.Web.UI.Models;

namespace Edubase.Web.UI.Controllers.Api
{
    public interface ISqlDataQualityStatusRepository
    {
        Task UpsertAsync(SqlDataQualityStatus item);
        SqlDataQualityStatus Get(string partitionKey, string rowKey);
        Task<SqlDataQualityStatus> GetAsync(string partitionKey, string rowKey);
        Task<IEnumerable<SqlDataQualityStatus>> GetAllAsync();
    }
}
