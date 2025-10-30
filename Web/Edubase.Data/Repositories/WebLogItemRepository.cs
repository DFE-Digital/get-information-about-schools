using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Edubase.Data.Entity;
using Edubase.Data.Repositories.TableStorage;
using Microsoft.Extensions.Configuration;

namespace Edubase.Data.Repositories;

public class WebLogItemRepository : TableStorageBase<AZTLoggerMessages>
{
    private const int LogsLimit = 1000;

    public WebLogItemRepository(IConfiguration configuration)
        : base(configuration, "DataConnectionString", "WebLogItems")
    {
    }

    public async Task<IEnumerable<AZTLoggerMessages>> GetById(string value)
    {
        var items = new List<AZTLoggerMessages>();

        if (string.IsNullOrWhiteSpace(value) || value.Length < 8)
        {
            return items;
        }

        var rowKey = value[..^8];
        var partitionKey = value[^8..];

        await foreach (var msg in Table.QueryAsync<AZTLoggerMessages>(m =>
            m.RowKey == rowKey && m.PartitionKey == partitionKey))
        {
            items.Add(msg);
            if (items.Count >= LogsLimit)
            {
                break;
            }
        }

        return items;
    }

    public async Task<IEnumerable<AZTLoggerMessages>> GetWithinDateRange(DateTime startDateTime, DateTime endDateTime)
    {
        var items = new List<AZTLoggerMessages>();

        await foreach (var msg in Table.QueryAsync<AZTLoggerMessages>(m =>
            m.DateUtc >= startDateTime && m.DateUtc <= endDateTime))
        {
            items.Add(msg);
            if (items.Count >= LogsLimit)
            {
                break;
            }
        }

        return items;
    }

    public static List<AZTLoggerMessages> FilterPurgeZeroLogsMessage(List<AZTLoggerMessages> messages, bool includeZeroLogs)
    {
        return includeZeroLogs
            ? messages
            :  [.. messages.Where(m => !string.Equals(m.Message, "LOG PURGE REPORT: There were 0 logs purged from storage.", StringComparison.OrdinalIgnoreCase))];
    }

    private static bool ContainsIgnoreCase(string value, string query) =>
        !string.IsNullOrWhiteSpace(value) && value.Contains(query, StringComparison.OrdinalIgnoreCase);

    public static List<AZTLoggerMessages> FilterByAllTextColumns(List<AZTLoggerMessages> messages, string query)
    {
        return string.IsNullOrWhiteSpace(query)
            ? messages
            :  [.. messages.Where(m =>
            ContainsIgnoreCase(m.Id, query) ||
            ContainsIgnoreCase(m.ClientIpAddress, query) ||
            ContainsIgnoreCase(m.Environment, query) ||
            ContainsIgnoreCase(m.Exception, query) ||
            ContainsIgnoreCase(m.HttpMethod, query) ||
            ContainsIgnoreCase(m.Level, query) ||
            ContainsIgnoreCase(m.Message, query) ||
            ContainsIgnoreCase(m.ReferrerUrl, query) ||
            ContainsIgnoreCase(m.RequestJsonBody, query) ||
            ContainsIgnoreCase(m.Url, query) ||
            ContainsIgnoreCase(m.UserAgent, query) ||
            ContainsIgnoreCase(m.UserId, query) ||
            ContainsIgnoreCase(m.UserName, query)
        )];
    }
}
