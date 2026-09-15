using System.Collections.Generic;
using System.Threading.Tasks;
using Edubase.Web.UI.Models;

namespace Edubase.Web.UI.Controllers.Api
{
    public interface ISqlLaNameCodeRepository
    {
        Task<IEnumerable<SqlLaNameCode>> GetAllAsync();
        Task<IEnumerable<SqlLaNameCode>> GetByGroupAsync(string groupCode);
    }
}
