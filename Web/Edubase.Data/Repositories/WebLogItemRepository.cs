using System;
using System.Collections.Generic;
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
    }
}
