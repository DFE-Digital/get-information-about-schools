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

        public static List<AZTLoggerMessages> FilterByAllTextColumns(List<AZTLoggerMessages> webLogMessages,
            string queryString)
        {
            if (string.IsNullOrWhiteSpace(queryString))
            {
                return webLogMessages;
            }

            var lowerQueryString = queryString.ToLowerInvariant();

            webLogMessages = webLogMessages.Where(m =>
                {
                    if (!string.IsNullOrWhiteSpace(m.Id) && m.Id.ToLowerInvariant().Contains(lowerQueryString))
                    {
                        return true;
                    }

                    if (!string.IsNullOrWhiteSpace(m.Level) && m.Level.ToLowerInvariant().Contains(lowerQueryString))
                    {
                        return true;
                    }

                    if (!string.IsNullOrWhiteSpace(m.Environment) && m.Environment.ToLowerInvariant().Contains(lowerQueryString))
                    {
                        return true;
                    }

                    if (!string.IsNullOrWhiteSpace(m.Message) && m.Message.ToLowerInvariant().Contains(lowerQueryString))
                    {
                        return true;
                    }

                    if (!string.IsNullOrWhiteSpace(m.Exception) && m.Exception.ToLowerInvariant().Contains(lowerQueryString))
                    {
                        return true;
                    }

                    if (!string.IsNullOrWhiteSpace(m.Url) && m.Url.ToLowerInvariant().Contains(lowerQueryString))
                    {
                        return true;
                    }

                    if (!string.IsNullOrWhiteSpace(m.UserAgent) && m.UserAgent.ToLowerInvariant().Contains(lowerQueryString))
                    {
                        return true;
                    }

                    if (!string.IsNullOrWhiteSpace(m.ClientIpAddress) && m.ClientIpAddress.ToLowerInvariant().Contains(lowerQueryString))
                    {
                        return true;
                    }

                    if (!string.IsNullOrWhiteSpace(m.ReferrerUrl) && m.ReferrerUrl.ToLowerInvariant().Contains(lowerQueryString))
                    {
                        return true;
                    }

                    if (!string.IsNullOrWhiteSpace(m.HttpMethod) && m.HttpMethod.ToLowerInvariant().Contains(lowerQueryString))
                    {
                        return true;
                    }

                    if (!string.IsNullOrWhiteSpace(m.RequestJsonBody) && m.RequestJsonBody.ToLowerInvariant().Contains(lowerQueryString))
                    {
                        return true;
                    }

                    if (!string.IsNullOrWhiteSpace(m.UserId) && m.UserId.ToLowerInvariant().Contains(lowerQueryString))
                    {
                        return true;
                    }

                    if (!string.IsNullOrWhiteSpace(m.UserName) && m.UserName.ToLowerInvariant().Contains(lowerQueryString))
                    {
                        return true;
                    }

                    return false;
                }
            ).ToList();

            return webLogMessages;
        }
    }
}
