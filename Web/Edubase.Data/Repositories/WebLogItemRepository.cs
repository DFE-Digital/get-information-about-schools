using System;
using System.Collections.Generic;
using System.Linq;
using Edubase.Data.Entity;
using Edubase.Data.Repositories.TableStorage;
using Microsoft.WindowsAzure.Storage.Table;
using System.Threading.Tasks;
using Azure.Data.Tables;

namespace Edubase.Data.Repositories;

public class WebLogItemRepository : TableStorageBase<AZTLoggerMessages>
{
    private const int LogsLimit = 1000;
    public WebLogItemRepository()
        : base("DataConnectionString")
    {
    }

    public async Task<IEnumerable<AZTLoggerMessages>> GetById(string value)
    {
        List<AZTLoggerMessages> items = [];

        // ID format is {rowKey}{partitionKey}, where the partition key is eight digits YYYYMMDD

        // If the query is far too short, it's not a valid ID and we can exit early
        if (value.Length < 8)
        {
            return items;
        }

        // Additional validation _could_ be done here, but for our purposes it is not necessary
        var rowKey = value.Substring(0, value.Length - 8);
        var partitionKey = value.Substring(value.Length - 8);

        var query =
            Table.QueryAsync<AZTLoggerMessages>(
                (msg) =>
                    msg.RowKey == rowKey &&
                    msg.PartitionKey == partitionKey);

        await foreach (var msg in query)
        {
            items.Add(msg);

            if (items.Count >= LogsLimit) // mimic legacy TakeCount limit
            {
                return items.Take(LogsLimit);
            }
        }

        return items;
    }

    public async Task<IEnumerable<AZTLoggerMessages>> GetWithinDateRange(DateTime startDateTime, DateTime endDateTime)
    {
        var items = new List<AZTLoggerMessages>();

        var query = Table.QueryAsync<AZTLoggerMessages>(
            (msg) =>
                msg.DateUtc >= startDateTime &&
                msg.DateUtc <= endDateTime);

        await foreach (var msg in query)
        {
            items.Add(msg);

            if (items.Count >= LogsLimit) //limit to 1000 rows to avoid performance problems
            {
                return items.Take(LogsLimit);
            }
        }

        return items;
    }


    public static List<AZTLoggerMessages> FilterPurgeZeroLogsMessage(List<AZTLoggerMessages> webLogMessages, bool includePurgeZeroLogsMessage)
    {
        if (includePurgeZeroLogsMessage)
        {
            return webLogMessages;
        }

        webLogMessages = webLogMessages
            .Where(m => !m.Message.Equals("LOG PURGE REPORT: There were 0 logs purged from storage."))
            .ToList();

        return webLogMessages;
    }

    private static bool ValueIsNotNullAndNotWhitespaceAndContainsCaseInsensitive(string value, string query)
    {
        return !string.IsNullOrWhiteSpace(value) && value.ToLowerInvariant().Contains(query.ToLowerInvariant());
    }

    public static List<AZTLoggerMessages> FilterByAllTextColumns(
        List<AZTLoggerMessages> webLogMessages,
        string queryString
    )
    {
        if (string.IsNullOrWhiteSpace(queryString))
        {
            return webLogMessages;
        }

        webLogMessages = webLogMessages.Where(m =>
            ValueIsNotNullAndNotWhitespaceAndContainsCaseInsensitive(m.Id, queryString)
            || ValueIsNotNullAndNotWhitespaceAndContainsCaseInsensitive(m.ClientIpAddress, queryString)
            || ValueIsNotNullAndNotWhitespaceAndContainsCaseInsensitive(m.Environment, queryString)
            || ValueIsNotNullAndNotWhitespaceAndContainsCaseInsensitive(m.Exception, queryString)
            || ValueIsNotNullAndNotWhitespaceAndContainsCaseInsensitive(m.HttpMethod, queryString)
            || ValueIsNotNullAndNotWhitespaceAndContainsCaseInsensitive(m.Level, queryString)
            || ValueIsNotNullAndNotWhitespaceAndContainsCaseInsensitive(m.Message, queryString)
            || ValueIsNotNullAndNotWhitespaceAndContainsCaseInsensitive(m.ReferrerUrl, queryString)
            || ValueIsNotNullAndNotWhitespaceAndContainsCaseInsensitive(m.RequestJsonBody, queryString)
            || ValueIsNotNullAndNotWhitespaceAndContainsCaseInsensitive(m.Url, queryString)
            || ValueIsNotNullAndNotWhitespaceAndContainsCaseInsensitive(m.UserAgent, queryString)
            || ValueIsNotNullAndNotWhitespaceAndContainsCaseInsensitive(m.UserId, queryString)
            || ValueIsNotNullAndNotWhitespaceAndContainsCaseInsensitive(m.UserName, queryString)
        ).ToList();

        return webLogMessages;
    }
}
