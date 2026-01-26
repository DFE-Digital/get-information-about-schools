using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure.Data.Tables;
using Edubase.Common.Logging;
using Microsoft.Extensions.Configuration;

namespace Edubase.Data.Repositories
{
    /// <summary>
    /// Provides access to WebLogItem records stored in Azure Table Storage.
    /// Supports lookup by ID and date range, and includes helper methods for filtering.
    /// </summary>
    public class WebLogItemRepository
    {
        /// <summary>
        /// The name of the connection string used to access table storage.
        /// </summary>
        private const string ConnectionStringName = "DataConnectionString";

        /// <summary>
        /// The name of the Azure Table Storage table containing log entries.
        /// </summary>
        private const string TableName = "WebLogItems";

        /// <summary>
        /// Maximum number of log items returned by any query.
        /// </summary>
        private const int LogsLimit = 1000;

        private readonly TableClient _table;

        /// <summary>
        /// Creates a new instance of <see cref="WebLogItemRepository"/> using the provided configuration.
        /// </summary>
        /// <param name="config">Application configuration containing the storage connection string.</param>
        /// <exception cref="ArgumentException">Thrown when the connection string is missing or empty.</exception>
        public WebLogItemRepository(IConfiguration config)
        {
            string? connectionString = config.GetConnectionString(ConnectionStringName);

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new ArgumentException(
                    $"The connection string '{ConnectionStringName}' is missing or empty.");
            }

            _table = new TableClient(connectionString, TableName);
            _table.CreateIfNotExists();
        }

        /// <summary>
        /// Retrieves log entries by their combined RowKey + PartitionKey identifier.
        /// </summary>
        /// <param name="value">The combined identifier string.</param>
        /// <returns>A collection of matching <see cref="WebLogMessage"/> items.</returns>
        public async Task<IEnumerable<WebLogMessage>> GetById(string value)
        {
            List<WebLogMessage> items = [];

            if (string.IsNullOrWhiteSpace(value) || value.Length < 8)
            {
                return items;
            }

            string rowKey = value[..^8];
            string partitionKey = value[^8..];

            await foreach (TableEntity entity in _table.QueryAsync<TableEntity>(e =>
                e.RowKey == rowKey &&
                e.PartitionKey == partitionKey))
            {
                items.Add(Map(entity));

                if (items.Count >= LogsLimit)
                {
                    break;
                }
            }

            return items;
        }

        /// <summary>
        /// Retrieves log entries whose <c>DateUtc</c> value falls within the specified range.
        /// </summary>
        /// <param name="start">Start of the date range.</param>
        /// <param name="end">End of the date range.</param>
        /// <returns>A collection of matching <see cref="WebLogMessage"/> items.</returns>
        public async Task<IEnumerable<WebLogMessage>> GetWithinDateRange(DateTime start, DateTime end)
        {
            List<WebLogMessage> items = [];

            await foreach (TableEntity entity in _table.QueryAsync<TableEntity>(e =>
                e.GetDateTime(WebLogItemColumns.DateUtc) >= start &&
                e.GetDateTime(WebLogItemColumns.DateUtc) <= end))
            {
                items.Add(Map(entity));

                if (items.Count >= LogsLimit)
                {
                    break;
                }
            }

            return items;
        }

        /// <summary>
        /// Maps a <see cref="TableEntity"/> to a <see cref="WebLogMessage"/> instance.
        /// </summary>
        private static WebLogMessage Map(TableEntity entity) =>
            new()
            {
                PartitionKey = entity.PartitionKey,
                RowKey = entity.RowKey,
                DateUtc = entity.GetDateTime(WebLogItemColumns.DateUtc) ?? DateTime.MinValue,
                Message = entity.GetString(WebLogItemColumns.Message),
                Id = entity.GetString(WebLogItemColumns.Id),
                ClientIpAddress = entity.GetString(WebLogItemColumns.ClientIpAddress),
                Environment = entity.GetString(WebLogItemColumns.Environment),
                Exception = entity.GetString(WebLogItemColumns.Exception),
                HttpMethod = entity.GetString(WebLogItemColumns.HttpMethod),
                Level = entity.GetString(WebLogItemColumns.Level),
                ReferrerUrl = entity.GetString(WebLogItemColumns.ReferrerUrl),
                RequestJsonBody = entity.GetString(WebLogItemColumns.RequestJsonBody),
                Url = entity.GetString(WebLogItemColumns.Url),
                UserAgent = entity.GetString(WebLogItemColumns.UserAgent),
                UserId = entity.GetString(WebLogItemColumns.UserId),
                UserName = entity.GetString(WebLogItemColumns.UserName)
            };

        /// <summary>
        /// Removes "zero logs purged" messages unless explicitly included.
        /// </summary>
        public static List<WebLogMessage> FilterPurgeZeroLogsMessage(
            List<WebLogMessage> messages, bool includeZeroLogs) =>
                includeZeroLogs
                    ? messages
                    : [.. messages.Where(m =>
                        !string.Equals(
                            m.Message,
                            "LOG PURGE REPORT: There were 0 logs purged from storage.",
                            StringComparison.OrdinalIgnoreCase))];

        /// <summary>
        /// Performs a full-text search across all text-based columns.
        /// </summary>
        public static List<WebLogMessage> FilterByAllTextColumns(
            List<WebLogMessage> messages, string query) =>
                string.IsNullOrWhiteSpace(query)
                    ? messages
                    : [.. messages.Where(message =>
                        ContainsIgnoreCase(message.Id, query) ||
                        ContainsIgnoreCase(message.ClientIpAddress, query) ||
                        ContainsIgnoreCase(message.Environment, query) ||
                        ContainsIgnoreCase(message.Exception, query) ||
                        ContainsIgnoreCase(message.HttpMethod, query) ||
                        ContainsIgnoreCase(message.Level, query) ||
                        ContainsIgnoreCase(message.Message, query) ||
                        ContainsIgnoreCase(message.ReferrerUrl, query) ||
                        ContainsIgnoreCase(message.RequestJsonBody, query) ||
                        ContainsIgnoreCase(message.Url, query) ||
                        ContainsIgnoreCase(message.UserAgent, query) ||
                        ContainsIgnoreCase(message.UserId, query) ||
                        ContainsIgnoreCase(message.UserName, query)
                    )];

        /// <summary>
        /// Determines whether the specified <paramref name="value"/> contains the given
        /// <paramref name="query"/> string, using a case‑insensitive comparison.
        /// </summary>
        /// <param name="value">The text to search within.</param>
        /// <param name="query">The text to search for.</param>
        /// <returns>
        /// <c>true</c> if <paramref name="value"/> is not null or whitespace and contains
        /// <paramref name="query"/> using <see cref="StringComparison.OrdinalIgnoreCase"/>; 
        /// otherwise, <c>false</c>.
        /// </returns>
        private static bool ContainsIgnoreCase(string value, string query) =>
            !string.IsNullOrWhiteSpace(value) &&
            value.Contains(query, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Defines the Azure Table Storage column names used by <see cref="WebLogItemRepository"/>.
    /// </summary>
    public static class WebLogItemColumns
    {
        public const string DateUtc = "DateUtc";
        public const string Message = "Message";
        public const string Id = "Id";
        public const string ClientIpAddress = "ClientIpAddress";
        public const string Environment = "Environment";
        public const string Exception = "Exception";
        public const string HttpMethod = "HttpMethod";
        public const string Level = "Level";
        public const string ReferrerUrl = "ReferrerUrl";
        public const string RequestJsonBody = "RequestJsonBody";
        public const string Url = "Url";
        public const string UserAgent = "UserAgent";
        public const string UserId = "UserId";
        public const string UserName = "UserName";
    }
}
