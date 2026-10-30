using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Edubase.Web.UI.Models;

namespace Edubase.Web.UI.Controllers.Api
{
    public interface ISqlNewsArticleRepository
    {
        Task UpsertAsync(SqlNewsArticle item);
        SqlNewsArticle Get(string partitionKey, string rowKey);
        Task<SqlNewsArticle> GetAsync(string partitionKey, string rowKey);
        Task<IEnumerable<SqlNewsArticle>> GetAllAsync();
    }
}
