using System.Collections.Generic;
using System.Threading.Tasks;
using Edubase.Web.UI.Models;

namespace Edubase.Web.UI.Controllers.Api
{
    public interface ISqlApiRecorderSessionItemRepository
    {
        Task UpsertAsync(SqlApiRecorderSessionItem item);
        SqlApiRecorderSessionItem Get(string partitionKey, string rowKey);
        Task<SqlApiRecorderSessionItem> GetAsync(string partitionKey, string rowKey);
        Task<IEnumerable<SqlApiRecorderSessionItem>> GetAllAsync();
    }
}
