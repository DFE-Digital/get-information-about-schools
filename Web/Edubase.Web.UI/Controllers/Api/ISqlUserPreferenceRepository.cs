using System.Collections.Generic;
using System.Threading.Tasks;
using Edubase.Web.UI.Models;

namespace Edubase.Web.UI.Controllers.Api
{
    public interface ISqlUserPreferenceRepository
    {
        Task UpsertAsync(SqlUserPreference item);
        SqlUserPreference Get(string partitionKey, string rowKey);
        Task<SqlUserPreference> GetAsync(string partitionKey, string rowKey);
        Task<IEnumerable<SqlUserPreference>> GetAllAsync();
    }
}
