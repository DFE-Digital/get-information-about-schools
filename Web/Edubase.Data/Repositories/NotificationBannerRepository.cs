using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.Caching;
using System.Threading.Tasks;
using Azure.Data.Tables;
using Azure;
using Edubase.Data.Entity;
using Edubase.Data.Repositories.TableStorage;
using Microsoft.WindowsAzure.Storage.Table;
using MoreLinq;

namespace Edubase.Data.Repositories;

public class NotificationBannerRepository : TableStorageBase<NotificationBanner> // INotificationBannerRepository
{
    private const string CurrentNotificationBannersCacheKey = "currentNotificationBanners";
    private readonly MemoryCache _cache = MemoryCache.Default;
    private readonly int _notificationBannerCacheExpirationInSeconds;

    public NotificationBannerRepository()
        : base("DataConnectionString")
    {
        if (!int.TryParse(ConfigurationManager.AppSettings["NotificationBannerCacheExpirationInSeconds"],
                out _notificationBannerCacheExpirationInSeconds))
        {
            _notificationBannerCacheExpirationInSeconds = 300;
        }
    }

    public async Task CreateAsync(NotificationBanner entity)
    {
        await Table.AddEntityAsync(entity);
        _cache.Remove(CurrentNotificationBannersCacheKey);
    }

    /// <summary>
    ///     Synchronously get active notification banners for a given datetime and for a maximum number of banners
    ///     Only use this method for notification banners on the template.
    ///     Other processes should use async methods if possible
    /// </summary>
    /// <param name="maximumAmount">The maximum number of banners to be returned</param>
    /// <returns>An IEnumerable of NotificationBanner</returns>
    public IEnumerable<NotificationBanner> GetNotificationBanners(int maximumAmount)
    {
        var now = DateTime.Now;
        if (_cache[CurrentNotificationBannersCacheKey] is not IList<NotificationBanner> activeBanners)
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

    /// <summary>
    ///     Gets notification banners that are active during the cache period
    /// </summary>
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
        var query = Table.QueryAsync<NotificationBanner>(
            (banner)
                => banner.PartitionKey == partitionKey.ToString() &&
                    (!excludeExpired || banner.End >= DateTime.UtcNow));

        List<NotificationBanner> results = [];

        await foreach (var banner in query)
        {
            results.Add(banner);

            if (results.Count >= take)
            {
                return results.Take(take);
            }
        }

        return results;
    }


    public async Task<NotificationBanner> GetAsync(
        string id,
        eNotificationBannerPartition partitionKey = eNotificationBannerPartition.Current)
    {
        try
        {
            var response = await Table.GetEntityAsync<NotificationBanner>(partitionKey: partitionKey.ToString(), rowKey: id);
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

        if (item != null)
        {
            await Table.DeleteEntityAsync(item.PartitionKey, item.RowKey);
        }
        _cache.Remove(CurrentNotificationBannersCacheKey);
    }

    private async Task ArchiveAsync(string id, string auditUser = "")
    {
        var item = await GetAsync(id);
        item.PartitionKey = eNotificationBannerPartition.Archive.ToString();
        if (item.Version > 1)
        {
            item.RowKey = Guid.NewGuid().ToString("N").Substring(0, 8);
        }

        await CreateAsync(item);


        if (!string.IsNullOrEmpty(auditUser))
        {
            // if this is triggered as part of a delete, once we've ported the original entry over to the audit, we want to create a final one which is the delete entry
            item.Version++;
            item.AuditEvent = eNotificationBannerEvent.Delete.ToString();
            item.AuditUser = auditUser;
            item.AuditTimestamp = DateTime.Now;
            item.RowKey = Guid.NewGuid().ToString("N").Substring(0, 8);
            await CreateAsync(item);
        }

        _cache.Remove(CurrentNotificationBannersCacheKey);
    }

    public async Task UpdateAsync(NotificationBanner item)
    {
        // archive the existing one first so we have a snapshot
        await ArchiveAsync(item.RowKey);

        // now update the record
        item.Version++;
        await Table.UpdateEntityAsync(item, item.ETag, TableUpdateMode.Replace);

        _cache.Remove(CurrentNotificationBannersCacheKey);
    }
}
