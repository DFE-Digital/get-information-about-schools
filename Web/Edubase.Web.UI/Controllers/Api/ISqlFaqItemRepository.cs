using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Edubase.Web.UI.Models;

namespace Edubase.Web.UI.Controllers.Api
{
    public interface ISqlFaqItemRepository
    {
        Task UpsertAsync(SqlFaqItem item);
        SqlFaqItem Get(string partitionKey, string rowKey);
        Task<SqlFaqItem> GetAsync(string partitionKey, string rowKey);
        Task<IEnumerable<SqlFaqItem>> GetAllAsync();
    }
}
