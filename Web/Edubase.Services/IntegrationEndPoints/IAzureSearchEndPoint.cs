using System.Collections.Generic;
using System.Threading.Tasks;
using Edubase.Services.IntegrationEndPoints.Models;

namespace Edubase.Services.IntegrationEndPoints
{
    public interface IAzureSearchEndPoint
    {
        Task CreateOrUpdateDataSource(string name, string sqlConnectionString, string tableName, string description = null);
        Task CreateOrUpdateIndexAsync(SearchIndex index);
        Task CreateOrUpdateIndexAsync(string name, IList<SearchIndexField> fields, string suggesterName = null);
        Task CreateOrUpdateIndexerAsync(string name, string dataSourceName, string targetIndexName, string description = null);
        Task DeleteDataSourceAsync(string name);
        Task DeleteIndexAsync(string name);
        Task DeleteIndexerAsync(string name);
        Task<IEnumerable<T>> SuggestAsync<T>(string indexName, string suggesterName, string text, int take = 5) where T : class;
    }
}