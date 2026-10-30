using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Edubase.Web.UI.Models;

namespace Edubase.Web.UI.Controllers.Api
{
    public interface ISqlGlossaryItemRepository
    {
        Task UpsertAsync(SqlGlossaryItem item);
        SqlGlossaryItem Get(string partitionKey, string rowKey);
        Task<SqlGlossaryItem> GetAsync(string partitionKey, string rowKey);
        Task<IEnumerable<SqlGlossaryItem>> GetAllAsync();
    }
}
