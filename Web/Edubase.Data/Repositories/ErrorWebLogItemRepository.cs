using System;
using System.Collections.Generic;
using Edubase.Data.Entity;
using Edubase.Data.Repositories.TableStorage;
using Microsoft.WindowsAzure.Storage.Table;
using System.Threading.Tasks;

namespace Edubase.Data.Repositories
{
    public class ErrorWebLogItemRepository : TableStorageBase<AZTLoggerMessages>
    {
        public ErrorWebLogItemRepository()
            : base("DataConnectionString")
        {
        }

        public async Task<List<AZTLoggerMessages>> Get(DateTime dateTime, int days = 28, bool includePurgeZeroLogsMessage = false)
        {
            var items = new List<AZTLoggerMessages>();

            // loop over the items in the table, and combine paginated results
            TableContinuationToken currentToken = null;
            do
            {
                var query = new TableQuery<AZTLoggerMessages>().Where(
                    TableQuery.GenerateFilterConditionForDate(
                        "DateUtc",
                        QueryComparisons.GreaterThanOrEqual,
                        dateTime.AddDays(days * -1)
                    )
                );

                if (!includePurgeZeroLogsMessage)
                {
                    query = query.Where(
                        TableQuery.GenerateFilterCondition(
                            "Message",
                            QueryComparisons.NotEqual,
                            "LOG PURGE REPORT: There were 0 logs purged from storage."
                        )
                    );
                }

                var segment = await Table.ExecuteQuerySegmentedAsync(query, currentToken);
                items.AddRange(segment.Results);
                currentToken = segment.ContinuationToken;
            } while (currentToken != null);

            return items;
        }
    }
}
