using System.Collections.Generic;
using System.Threading.Tasks;
using Edubase.Web.UI.Models;

namespace Edubase.Web.UI.Controllers.Api
{
    public interface ISqlUserPreferenceRepository
    {
        Task UpsertAsync(SqlUserPreference item);
        SqlUserPreference Get(string userId);
        Task<SqlUserPreference> GetAsync(string userId);
        Task<IEnumerable<SqlUserPreference>> GetAllAsync();
    }
}
