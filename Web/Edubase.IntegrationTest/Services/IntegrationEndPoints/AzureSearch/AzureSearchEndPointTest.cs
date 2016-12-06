using Edubase.Common.Config;
using Edubase.Services.Establishments;
using Edubase.Services.Establishments.Search;
using Edubase.Services.Governors.Search;
using Edubase.Services.Groups.Search;
using Edubase.Services.IntegrationEndPoints.AzureSearch;
using Edubase.Services.IntegrationEndPoints.AzureSearch.Models;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Edubase.Common;
using Edubase.Services.Establishments.Models;

namespace Edubase.IntegrationTest.Services.IntegrationEndPoints.AzureSearch
{
    [TestFixture]
    public class AzureSearchEndPointTest
    {
        private const string CONNSTR = "Microsoft.Azure.Search.ConnectionString";
        private const string SQLCONNSTRREMOTE_DEV = "EdubaseSqlDb_devremote";

        [Test]
        public async Task Create()
        {
            await SetupAzureSearch(new EstablishmentsSearchIndex().CreateModel(), "Establishment");
            await SetupAzureSearch(new GovernorsSearchIndex().CreateModel(), "Governor");
            await SetupAzureSearch(new GroupsSearchIndex().CreateModel(), "GroupCollection");
        }

        [Test]
        public async Task DeleteAZSItemsWorks()
        {
            await DeleteAzureSearchItems(EstablishmentsSearchIndex.INDEX_NAME);
            await DeleteAzureSearchItems(GovernorsSearchIndex.INDEX_NAME);
            await DeleteAzureSearchItems(GroupsSearchIndex.INDEX_NAME);
        }

        [Test]
        public async Task SuggestEstablishment()
        {
            var subject = new AzureSearchEndPoint(GetAZSConnStr());
            var items = await subject.SuggestAsync<EstablishmentSuggestionItem>(EstablishmentsSearchIndex.INDEX_NAME, EstablishmentsSearchIndex.SUGGESTER_NAME, "school");
            Assert.IsTrue(items.Count() > 0);
            Assert.IsTrue(items.All(x => x.Name.Clean() != null));
        }

        [Test]
        public async Task SuggestGroup()
        {
            var subject = new AzureSearchEndPoint(GetAZSConnStr());
            var items = await subject.SuggestAsync<GroupSuggestionItem>(GroupsSearchIndex.INDEX_NAME, GroupsSearchIndex.SUGGESTER_NAME, "academy");
            Assert.IsTrue(items.Count() > 0);
            Assert.IsTrue(items.All(x => x.Name.Clean() != null));
        }

        [Test]
        public async Task SuggestGovernor()
        {
            var subject = new AzureSearchEndPoint(GetAZSConnStr());
            var items = await subject.SuggestAsync<GovernorSuggestionItem>(GovernorsSearchIndex.INDEX_NAME, GovernorsSearchIndex.SUGGESTER_NAME, "john");
            Assert.IsTrue(items.Count() > 0);
            Assert.IsTrue(items.All(x => x.Text.Clean() != null));
        }



        [Test]
        public async Task SearchEstablishments()
        {
            var subject = new AzureSearchEndPoint(GetAZSConnStr());
            var r = await subject.SearchAsync<SearchEstablishmentDocument>(EstablishmentsSearchIndex.INDEX_NAME, null, null, 0, 10, null);
            Assert.AreEqual(10, r.Items.Count);
            Assert.IsTrue(r.Count > 1000);
            Assert.IsTrue(r.Items.All(x => x.Name != null));
        }
        
        private async Task SetupAzureSearch(SearchIndex index, string tableName)
        {
            var subject = new AzureSearchEndPoint(GetAZSConnStr());
            await subject.CreateOrUpdateIndexAsync(index);
            await subject.CreateOrUpdateDataSource(index.Name + "-ds", GetRemoteSqlDEVConnStr(), tableName);
            await subject.CreateOrUpdateIndexerAsync(index.Name + "-indexer", index.Name + "-ds", index.Name);
        }

        private async Task DeleteAzureSearchItems(string indexName)
        {
            var subject = new AzureSearchEndPoint(GetAZSConnStr());
            await subject.DeleteIndexAsync(indexName);
            await subject.DeleteDataSourceAsync(indexName + "-ds");
            await subject.DeleteIndexerAsync(indexName + "-indexer");
        }




        public static string GetAZSConnStr() => ConfigurationManager.ConnectionStrings[CONNSTR]?.ConnectionString;

        public static string GetRemoteSqlDEVConnStr() => ConfigurationManager.ConnectionStrings[SQLCONNSTRREMOTE_DEV]?.ConnectionString;
    }
}
