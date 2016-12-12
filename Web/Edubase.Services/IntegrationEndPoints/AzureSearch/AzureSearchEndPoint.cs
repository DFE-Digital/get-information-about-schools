using Edubase.Common;
using Edubase.Common.Reflection;
using Edubase.Services.Establishments.Models;
using Edubase.Services.IntegrationEndPoints.AzureSearch.Models;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Edubase.Services.IntegrationEndPoints.AzureSearch
{
    public class AzureSearchEndPoint : IAzureSearchEndPoint
    {
        public const string ODATA_FILTER_DELETED = "IsDeleted eq false";
        private string _connectionString;
        private Dictionary<Type, IList<string>> _fieldLists = new Dictionary<Type, IList<string>>();
        
        public struct ConnectionString
        {
            public string ApiKey { get; set; }
            public string Name { get; set; }
            public static ConnectionString Parse(string text)
            {
                var parts = text.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries).Select(x => new Tuple<string, string>(x.GetPart("=", 0), x.GetPart("=", 1)));

                var retVal = new ConnectionString();
                retVal.Name = parts.FirstOrDefault(x => x.Item1.Equals("name", StringComparison.OrdinalIgnoreCase))?.Item2;
                retVal.ApiKey = parts.FirstOrDefault(x => x.Item1.Equals("apikey", StringComparison.OrdinalIgnoreCase))?.Item2;
                return retVal;
            }
        }

        private ConnectionString _connection;


        public AzureSearchEndPoint(string connectionString)
        {
            _connectionString = connectionString;
            _connection = ConnectionString.Parse(_connectionString);
        }
        
        public async Task DeleteIndexAsync(string name) => await GetClient().Indexes.DeleteAsync(name.ToLower());
        public async Task DeleteIndexerAsync(string name) => await GetClient().Indexers.DeleteAsync(name.ToLower());
        public async Task DeleteDataSourceAsync(string name) => await GetClient().DataSources.DeleteAsync(name.ToLower());
        public async Task CreateOrUpdateDataSource(string name, string sqlConnectionString, string tableName, string description = null)
        {
            var c = GetClient();
            await c.DataSources.CreateOrUpdateAsync(DataSource.AzureSql(name, sqlConnectionString, tableName, 
                new SqlIntegratedChangeTrackingPolicy(), description));
        }
        public async Task CreateOrUpdateIndexerAsync(string name, string dataSourceName, string targetIndexName, string description = null)
        {
            var c = GetClient();
            await c.Indexers.CreateOrUpdateAsync(new Indexer()
            {
                DataSourceName = dataSourceName,
                Name = name,
                TargetIndexName = targetIndexName,
                Description = description
            });
        }
        public async Task CreateOrUpdateIndexAsync(string name, IList<SearchIndexField> fields, string suggesterName = null)
        {
            name = name.ToLower();

            var suggesters = new List<Suggester>();
            if (suggesterName != null && fields.Any(x => x.IncludeInSuggester == true))
                suggesters.Add(new Suggester(suggesterName, SuggesterSearchMode.AnalyzingInfixMatching, fields.Where(x => x.IncludeInSuggester == true).Select(x => x.Name).ToArray()));

            var client = GetClient();
            await client.Indexes.CreateOrUpdateAsync(new Index(name, fields.Cast<Field>().ToList(), suggesters: suggesters));
        }

        public async Task<AzureSearchResult<T>> SearchAsync<T>(string indexName, string text = null, string filter = null, int skip = 0, int take = 10, IList<string> fullTextSearchFields = null, IList<string> orderBy = null) where T : class
        {
            if (skip > 100000) throw new Exception($"The skip parameter cannot be greater than 100,000");
            if (take == 0) throw new Exception($"Argument {nameof(take)} cannot be zero.");
            var fields = _fieldLists.Get(typeof(T), () => ReflectionHelper.GetProperties(typeof(T), writeableOnly: true));

            var result = await GetIndexClient(indexName).Documents.SearchAsync<T>(text, new SearchParameters()
            {
                Skip = skip,
                Top = take,
                SearchMode = SearchMode.All,
                Filter = filter,
                OrderBy = orderBy,
                IncludeTotalResultCount = (skip == 0),
                Select = fields,
                SearchFields = fullTextSearchFields
            });

            var retVal = new AzureSearchResult<T>(result);
            if (skip > 0) retVal.CountAccessor = () => { throw new Exception("Count is not populated when the Skip value is greater than 0"); };
            return retVal;
        }
        

        public async Task CreateOrUpdateIndexAsync(SearchIndex index) => await CreateOrUpdateIndexAsync(index.Name, index.Fields, index.SuggesterName);


        public async Task<IEnumerable<T>> SuggestAsync<T>(string indexName, string suggesterName, string text, string odataFilter = null, int take = 5) where T : class
        {
            var fields = _fieldLists.Get(typeof(T), () => ReflectionHelper.GetProperties(typeof(T), writeableOnly: true));
            
            var c = GetIndexClient(indexName);
            var result = await c.Documents.SuggestAsync<T>(text, suggesterName, new SuggestParameters() { Select = fields, Top = take, Filter = odataFilter ?? ODATA_FILTER_DELETED });
            return result.Results.Select(x => x.Document);
        }



        
        private SearchServiceClient GetClient() => new SearchServiceClient(_connection.Name, new SearchCredentials(_connection.ApiKey));

        private SearchIndexClient GetIndexClient(string indexName) => new SearchIndexClient(_connection.Name, indexName, new SearchCredentials(_connection.ApiKey));

    }
}
