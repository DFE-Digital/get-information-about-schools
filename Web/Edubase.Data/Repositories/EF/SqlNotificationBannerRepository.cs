using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Runtime.Caching;
using System.Threading.Tasks;
using Edubase.Data.Entity;
using Edubase.Data.Repositories.TableStorage;
using Microsoft.WindowsAzure.Storage.Table;

namespace Edubase.Data.Repositories.EF
{
    public class SqlNotificationBannerRepository : INotificationBannerRepository
    {
        private const string CurrentNotificationBannersCacheKey = "currentNotificationBanners";
        private readonly MemoryCache _cache = MemoryCache.Default;
        private readonly int _notificationBannerCacheExpirationInSeconds;
        private readonly FrontEndDbContext _context;

        public SqlNotificationBannerRepository(FrontEndDbContext context)
        {
            _context = context;

            if (!int.TryParse(ConfigurationManager.AppSettings["NotificationBannerCacheExpirationInSeconds"],
                    out _notificationBannerCacheExpirationInSeconds))
            {
                _notificationBannerCacheExpirationInSeconds = 300;
            }
        }

        public async Task CreateAsync(NotificationBanner entity)
        {
            _context.NotificationBanners.Add(entity);
            await _context.SaveChangesAsync();
            _cache.Remove(CurrentNotificationBannersCacheKey);
        }

        public async Task CreateAsync(params NotificationBanner[] entities)
        {
            _context.NotificationBanners.AddRange(entities);
            await _context.SaveChangesAsync();
            _cache.Remove(CurrentNotificationBannersCacheKey);
        }

        public async Task CreateAsync(IEnumerable<NotificationBanner> entities)
        {
            await CreateAsync(entities.ToArray());
        }

        public IEnumerable<NotificationBanner> GetNotificationBanners(int maximumAmount)
        {
            var now = DateTime.Now;
            if (!(_cache[CurrentNotificationBannersCacheKey] is IList<NotificationBanner> activeBanners))
            {
                activeBanners = GetActiveBanners(now);
                _cache.Add(CurrentNotificationBannersCacheKey,
                    activeBanners,
                    new DateTimeOffset(now.AddSeconds(_notificationBannerCacheExpirationInSeconds)));
            }

            var result = (from banner in activeBanners
                where banner.Start <= now
                      && banner.End >= now
                orderby banner.Start
                select banner).Take(maximumAmount).ToList();

            return result;
        }

        private IList<NotificationBanner> GetActiveBanners(DateTime forDateTime)
        {
            var cacheWindowEnd = forDateTime.AddSeconds(_notificationBannerCacheExpirationInSeconds);
                return _context.NotificationBanners
                    .Where(x => x.PartitionKey == nameof(eNotificationBannerPartition.Current)
                                && x.Start <= cacheWindowEnd
                                && x.End >= forDateTime)
                    .Take(2)
                    .ToList();
        }

        public async Task<Page<NotificationBanner>> GetAllAsync(int take, TableContinuationToken skip = null,
            bool excludeExpired = false, eNotificationBannerPartition partitionKey = eNotificationBannerPartition.Current)
        {
            IQueryable<NotificationBanner> query = _context.NotificationBanners
                .Where(x => x.PartitionKey == partitionKey.ToString());

            if (excludeExpired)
            {
                query = query.Where(x => x.End >= DateTime.Now);
            }

            query = query.Take(take);

            var results = await query.ToListAsync();
            return new Page<NotificationBanner>(results, null);
        }

        public async Task<NotificationBanner> GetAsync(string id,
            eNotificationBannerPartition partitionKey = eNotificationBannerPartition.Current)
        {
            return await _context.NotificationBanners.FindAsync(partitionKey.ToString(), id);
        }

        private async Task ArchiveAsync(string id, string auditUser = "")
        {
            var item = await GetAsync(id);

            var archive = Clone(item);
            archive.PartitionKey = nameof(eNotificationBannerPartition.Archive);
            if (archive.Version > 1)
            {
                archive.RowKey = Guid.NewGuid().ToString("N").Substring(0, 8);
            }
            _context.NotificationBanners.Add(archive);

            if (!string.IsNullOrEmpty(auditUser))
            {
                var deleteEntry = Clone(item);
                deleteEntry.PartitionKey = nameof(eNotificationBannerPartition.Archive);
                deleteEntry.Version++;
                deleteEntry.AuditEvent = nameof(eNotificationBannerEvent.Delete);
                deleteEntry.AuditUser = auditUser;
                deleteEntry.AuditTimestamp = DateTime.Now;
                deleteEntry.RowKey = Guid.NewGuid().ToString("N").Substring(0, 8);
                _context.NotificationBanners.Add(deleteEntry);
            }

            await _context.SaveChangesAsync();
            _cache.Remove(CurrentNotificationBannersCacheKey);
        }

        public async Task DeleteAsync(string id, string auditUser)
        {
            await ArchiveAsync(id, auditUser);

            var row = await _context.NotificationBanners.FindAsync(nameof(eNotificationBannerPartition.Current), id);
            if (row != null)
            {
                _context.NotificationBanners.Remove(row);
                await _context.SaveChangesAsync();
            }

            _cache.Remove(CurrentNotificationBannersCacheKey);
        }

        public async Task UpdateAsync(NotificationBanner item)
        {
            await ArchiveAsync(item.RowKey);

            var existing = await _context.NotificationBanners.FindAsync(nameof(eNotificationBannerPartition.Current), item.RowKey);
            if (existing != null)
            {
                existing.Importance = item.Importance;
                existing.Content = item.Content;
                existing.Start = item.Start;
                existing.End = item.End;
                existing.Version++;
                existing.AuditUser = item.AuditUser;
                existing.AuditEvent = item.AuditEvent;
                existing.AuditTimestamp = item.AuditTimestamp;

                await _context.SaveChangesAsync();
            }
            _cache.Remove(CurrentNotificationBannersCacheKey);
        }

        private static NotificationBanner Clone(NotificationBanner item) => new NotificationBanner
        {
            PartitionKey = item.PartitionKey,
            RowKey = item.RowKey,
            Importance = item.Importance,
            Content = item.Content,
            Start = item.Start,
            End = item.End,
            Version = item.Version,
            Tracker = item.Tracker,
            AuditUser = item.AuditUser,
            AuditEvent = item.AuditEvent,
            AuditTimestamp = item.AuditTimestamp,
            LinkUrl1 = item.LinkUrl1,
            LinkText1 = item.LinkText1,
            LinkUrl2 = item.LinkUrl2,
            LinkText2 = item.LinkText2
        };
    }
}
