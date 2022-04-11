using System.Collections.Generic;
using System.Threading.Tasks;
using Edubase.Data.Entity;
using Edubase.Data.Repositories.TableStorage;
using Microsoft.WindowsAzure.Storage.Table;

namespace Edubase.Data.Repositories
{
    public interface INotificationTemplateRepository
    {
        Task CreateAsync(IEnumerable<NotificationTemplate> entities);
        Task CreateAsync(NotificationTemplate entity);
        Task CreateAsync(params NotificationTemplate[] entities);
        Task DeleteAsync(string id);
        Task<Page<NotificationTemplate>> GetAllAsync(int take, TableContinuationToken skip = null);
        Task<NotificationTemplate> GetAsync(string id);
        Task UpdateAsync(NotificationTemplate item);
    }
}
