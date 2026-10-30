using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Edubase.Web.UI.Models;

namespace Edubase.Web.UI.Controllers.Api
{
    public interface ISqlFaqGroupRepository
    {
        Task UpsertAsync(SqlFaqGroup item);
        SqlFaqGroup Get(string partitionKey, string rowKey);
        Task<SqlFaqGroup> GetAsync(string partitionKey, string rowKey);
        Task<IEnumerable<SqlFaqGroup>> GetAllAsync();
    }
}
