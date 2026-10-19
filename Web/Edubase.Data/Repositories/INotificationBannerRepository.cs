using System.Collections.Generic;
using System.Threading.Tasks;
using Edubase.Data.Entity;
using Edubase.Data.Repositories.TableStorage;
using Microsoft.WindowsAzure.Storage.Table;

namespace Edubase.Data.Repositories
{
    public interface INotificationBannerRepository
    {
        Task CreateAsync(NotificationBanner entity);
        Task CreateAsync(params NotificationBanner[] entities);
        Task CreateAsync(IEnumerable<NotificationBanner> entities);
        Task<Page<NotificationBanner>> GetAllAsync(int take, TableContinuationToken skip = null, bool excludeExpired = false, eNotificationBannerPartition partitionKey = eNotificationBannerPartition.Current);
        Task<NotificationBanner> GetAsync(string id, eNotificationBannerPartition partitionKey = eNotificationBannerPartition.Current);
        Task DeleteAsync(string id, string auditUser);
        Task UpdateAsync(NotificationBanner item);
        IEnumerable<NotificationBanner> GetNotificationBanners(int maximumAmount);
    }
}
