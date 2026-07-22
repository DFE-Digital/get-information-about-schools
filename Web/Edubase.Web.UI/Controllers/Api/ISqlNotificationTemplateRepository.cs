using System.Collections.Generic;
using System.Threading.Tasks;
using Edubase.Web.UI.Models;

namespace Edubase.Web.UI.Controllers.Api
{
    public interface ISqlNotificationTemplateRepository
    {
        Task UpsertAsync(SqlNotificationTemplate item);
        SqlNotificationTemplate Get(string partitionKey, string rowKey);
        Task<SqlNotificationTemplate> GetAsync(string partitionKey, string rowKey);
        Task<IEnumerable<SqlNotificationTemplate>> GetAllAsync();
    }
}
