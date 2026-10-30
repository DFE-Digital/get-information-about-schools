using System.Collections.Generic;
using System.Threading.Tasks;
using Edubase.Web.UI.Models;

namespace Edubase.Web.UI.Controllers.Api
{
    public interface ISqlLocalAuthoritySetRepository
    {
        Task UpsertAsync(SqlLocalAuthoritySet item);
        SqlLocalAuthoritySet Get(string partitionKey, string rowKey);
        Task<SqlLocalAuthoritySet> GetAsync(string partitionKey, string rowKey);
        Task<IEnumerable<SqlLocalAuthoritySet>> GetAllAsync();
    }
}
