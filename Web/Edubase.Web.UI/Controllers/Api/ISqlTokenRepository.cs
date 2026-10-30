using System.Collections.Generic;
using System.Threading.Tasks;
using Edubase.Web.UI.Models;

namespace Edubase.Web.UI.Controllers.Api
{
    public interface ISqlTokenRepository
    {
        Task UpsertAsync(SqlToken item);
        SqlToken Get(string partitionKey, string rowKey);
        Task<SqlToken> GetAsync(string partitionKey, string rowKey);
        Task<IEnumerable<SqlToken>> GetAllAsync();
    }
}
