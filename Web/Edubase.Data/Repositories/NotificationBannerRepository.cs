using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Threading.Tasks;
using Azure;
using Azure.Data.Tables;
using Edubase.Data.Entity;
using Edubase.Data.Repositories.TableStorage;
using Microsoft.Extensions.Configuration;

namespace Edubase.Data.Repositories;

public class NotificationBannerRepository : TableStorageBase<NotificationBanner>
{
    private const string CurrentNotificationBannersCacheKey = "currentNotificationBanners";
    private readonly MemoryCache _cache = MemoryCache.Default;
    private readonly int _notificationBannerCacheExpirationInSeconds;

    public NotificationBannerRepository(IConfiguration configuration)
        : base(configuration, "DataConnectionString", "NotificationBanners")
    {
        _notificationBannerCacheExpirationInSeconds = configuration.GetValue<int?>("NotificationBannerCacheExpirationInSeconds") ?? 300;
    }

    public async Task CreateAsync(NotificationBanner entity)
    {
        entity.PartitionKey = eNotificationBannerPartition.Current.ToString();
        entity.RowKey ??= Guid.NewGuid().ToString("N").Substring(0, 8);
        await Table.AddEntityAsync(entity);
        _cache.Remove(CurrentNotificationBannersCacheKey);
    }

    public IEnumerable<NotificationBanner> GetNotificationBanners(int maximumAmount)
    {
        var now = DateTime.UtcNow;
        if (_cache[CurrentNotificationBannersCacheKey] is not IList<NotificationBanner> activeBanners)
        {
            activeBanners = GetActiveBanners(now);
            _cache.Add(CurrentNotificationBannersCacheKey,
                activeBanners,
                new DateTimeOffset(now.AddSeconds(_notificationBannerCacheExpirationInSeconds)));
        }

        return [.. activeBanners
            .Where(b => b.Start <= now && b.End >= now)
            .OrderBy(b => b.Start)
            .Take(maximumAmount)];
    }

    private IList<NotificationBanner> GetActiveBanners(DateTime forDateTime)
    {
        var expirationWindow = forDateTime.AddSeconds(_notificationBannerCacheExpirationInSeconds);

        var queryResults = Table.Query<NotificationBanner>(banner =>
            banner.PartitionKey == eNotificationBannerPartition.Current.ToString()
            && banner.Start <= expirationWindow
            && banner.End >= forDateTime
        ).Take(2);

        return queryResults.ToList();
    }

    public async Task<IEnumerable<NotificationBanner>> GetAllAsync(
        int take,
        bool excludeExpired = false,
        eNotificationBannerPartition partitionKey = eNotificationBannerPartition.Current)
    {
        var results = new List<NotificationBanner>();

        await foreach (var banner in Table.QueryAsync<NotificationBanner>(b =>
            b.PartitionKey == partitionKey.ToString() &&
            (!excludeExpired || b.End >= DateTime.UtcNow)))
        {
            results.Add(banner);
            if (results.Count >= take)
            {
                break;
            }
        }

        return results;
    }

    public async Task<NotificationBanner?> GetAsync(
        string id,
        eNotificationBannerPartition partitionKey = eNotificationBannerPartition.Current)
    {
        try
        {
            var response = await Table.GetEntityAsync<NotificationBanner>(partitionKey.ToString(), id);
            return response.Value;
        }
        catch (RequestFailedException ex) when (ex.Status == 404)
        {
            return null;
        }
    }

    public async Task DeleteAsync(string id, string auditUser)
    {
        await ArchiveAsync(id, auditUser);

        var item = await GetAsync(id);
        if (item is not null)
        {
            await Table.DeleteEntityAsync(item.PartitionKey, item.RowKey);
        }

        _cache.Remove(CurrentNotificationBannersCacheKey);
    }

    public async Task UpdateAsync(NotificationBanner item)
    {
        await ArchiveAsync(item.RowKey);
        item.Version++;
        await Table.UpdateEntityAsync(item, item.ETag, TableUpdateMode.Replace);
        _cache.Remove(CurrentNotificationBannersCacheKey);
    }

    private async Task ArchiveAsync(string id, string auditUser = "")
    {
        var item = await GetAsync(id);
        if (item is null)
        {
            return;
        }

        item.PartitionKey = eNotificationBannerPartition.Archive.ToString();
        item.RowKey = item.Version > 1
            ? Guid.NewGuid().ToString("N").Substring(0, 8)
            : item.RowKey;

        await CreateAsync(item);

        if (!string.IsNullOrEmpty(auditUser))
        {
            item.Version++;
            item.AuditEvent = eNotificationBannerEvent.Delete.ToString();
            item.AuditUser = auditUser;
            item.AuditTimestamp = DateTime.UtcNow;
            item.RowKey = Guid.NewGuid().ToString("N").Substring(0, 8);
            await CreateAsync(item);
        }

        _cache.Remove(CurrentNotificationBannersCacheKey);
    }
}
