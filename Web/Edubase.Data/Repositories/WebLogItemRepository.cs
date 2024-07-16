using System;
using System.Collections.Generic;
using System.Linq;
using Edubase.Data.Entity;
using Edubase.Data.Repositories.TableStorage;
using Microsoft.WindowsAzure.Storage.Table;
using System.Threading.Tasks;

namespace Edubase.Data.Repositories
{
    public class WebLogItemRepository : TableStorageBase<AZTLoggerMessages>
    {
        public WebLogItemRepository()
            : base("DataConnectionString")
        {
        }

        public async Task<List<AZTLoggerMessages>> GetWithinDateRange(DateTime startDateTime, DateTime endDateTime)
        {
            var items = new List<AZTLoggerMessages>();

            TableContinuationToken currentToken = null;
            do
            {
                var query = new TableQuery<AZTLoggerMessages>().Where(
                    TableQuery.CombineFilters(
                        TableQuery.GenerateFilterConditionForDate(
                            "DateUtc",
                            QueryComparisons.GreaterThanOrEqual,
                            startDateTime
                        ),
                        TableOperators.And,
                        TableQuery.GenerateFilterConditionForDate(
                            "DateUtc",
                            QueryComparisons.LessThanOrEqual,
                            endDateTime
                        )
                    )
                );

                var segment = await Table.ExecuteQuerySegmentedAsync(query, currentToken);
                items.AddRange(segment.Results);
                currentToken = segment.ContinuationToken;
            } while (currentToken != null);

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
}
