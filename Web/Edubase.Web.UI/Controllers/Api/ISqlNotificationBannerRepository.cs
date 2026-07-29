using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Edubase.Web.UI.Models;

namespace Edubase.Web.UI.Controllers.Api
{
    public interface ISqlNotificationBannerRepository
    {
        Task UpsertAsync(SqlNotificationBanner item);
        SqlNotificationBanner Get(string partitionKey, string rowKey);
        Task<SqlNotificationBanner> GetAsync(string partitionKey, string rowKey);
        Task<IEnumerable<SqlNotificationBanner>> GetAllAsync();
    }
}
