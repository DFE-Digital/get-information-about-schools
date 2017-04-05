using System.Collections.Generic;
using System.Threading.Tasks;
using Edubase.Services.IntegrationEndPoints.AzureSearch.Models;
using System;

namespace Edubase.Services.IntegrationEndPoints.AzureSearch
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
        Task<IEnumerable<T>> SuggestAsync<T>(string indexName, string suggesterName, string text, string odataFilter = null, int take = 5) where T : class;

        /// <summary>
        /// Searches the index according to the parameters specified
        /// </summary>
        /// <typeparam name="T">Only fields that exist on the return type will be returned</typeparam>
        /// <param name="indexName"></param>
        /// <param name="text"></param>
        /// <param name="filter"></param>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <param name="fullTextSearchFields"></param>
        /// <returns></returns>
        Task<ApiSearchResult<T>> SearchAsync<T>(string indexName, string text = null, string filter = null, int skip = 0,
            int take = 10, IList<string> fullTextSearchFields = null, IList<string> orderBy = null) where T : class;

        Task<Tuple<string, long>> GetStatusAsync(string indexName);
    }
}