using Edubase.Common;
using Edubase.Common.Cache;
using Edubase.Data.DbContext;
using Edubase.Data.Entity;
using Edubase.Data.Repositories.Establishments;
using Edubase.Services;
using Edubase.Services.Establishments.Search;
using Edubase.Services.Governors.Search;
using Edubase.Services.Groups.Search;
using Edubase.Services.IntegrationEndPoints.AzureSearch;
using Edubase.Services.IntegrationEndPoints.AzureSearch.Models;
using Edubase.Services.IntegrationEndPoints.Smtp;
using Edubase.Services.Security;
using Edubase.Web.UI.Filters;
using Edubase.Web.UI.Models.Admin;
using System.Configuration;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Hosting;
using System.Web.Mvc;
using Autofac;

namespace Edubase.Web.UI.Controllers
{
    [EdubaseAuthorize(Roles = EdubaseRoles.Admin)]
    public class AdminController : EduBaseController
    {
        private IAzureSearchEndPoint _azureSearchEndPoint;

        public AdminController(IAzureSearchEndPoint azureSearchEndPoint)
        {
            _azureSearchEndPoint = azureSearchEndPoint;
        }

        // GET: Admin
        public ActionResult Index() => View();
        
        public async Task<ActionResult> Dashboard(string message = null)
        {
            ViewBag.Message = message;

            using (var scope = IocConfig.Container.BeginLifetimeScope())
            {
                var cache = scope.Resolve<ICacheAccessor>();
                ViewBag.SmtpEndPointName = scope.Resolve<ISmtpEndPoint>().GetType().FullName;
                ViewBag.RedisStatus = cache.Status.ToString();
                ViewBag.MemoryCacheSize = cache.GetMemoryCacheApproximateSize().ToString();

                var lines = cache.GetRedisMemoryUsage().SelectMany(x => x.Select(v => string.Concat(v.Key, " = ", v.Value)));
                ViewBag.RedisReport = string.Join("<br/>", lines);

                ViewBag.DbName = new System.Data.SqlClient.SqlConnectionStringBuilder(ConfigurationManager.ConnectionStrings["EdubaseSqlDb"].ConnectionString).InitialCatalog;

                ViewBag.AZSEstablishmentsStatus = await _azureSearchEndPoint.GetStatusAsync(EstablishmentsSearchIndex.INDEX_NAME);
                ViewBag.AZSGovernorsStatus = await _azureSearchEndPoint.GetStatusAsync(GovernorsSearchIndex.INDEX_NAME);
                ViewBag.AZSGroupsStatus = await _azureSearchEndPoint.GetStatusAsync(GroupsSearchIndex.INDEX_NAME);

                ViewBag.EstabWarmUpStatus = await cache.GetAsync<string>(
                    scope.Resolve<ICachedEstablishmentReadRepository>().GetWarmUpProgressCacheKey());

                return View(nameof(Dashboard));
            }   
        }

        public async Task<ActionResult> Logs(string date, string skipToken)
        {
            var dto = await new LogMessageReadService().GetAllAsync(10, skipToken, date.ToDateTime("yyyyMMdd"));
            var viewModel = new LogMessagesViewModel(dto) { DateFilter = date };
            return View(viewModel);
        }

        public async Task<ActionResult> LogDetail(string id)
        {
            var message = await new LogMessageReadService().GetAsync(id);
            return View(message);
        }


        public ActionResult DoException() { throw new System.Exception("This is a test exception"); }

        [HttpGet]
        public ActionResult GetPendingErrors(string pwd)
        {
            if (pwd == "c7634") return Json(DependencyResolver.Current
                .GetService<IMessageLoggingService>().GetPending());
            else return new EmptyResult();
        }

        [HttpGet]
        public ActionResult GetPendingErrorsId()
        {
            return Json(DependencyResolver.Current
                .GetService<IMessageLoggingService>().InstanceId);
        }

        [Authorize]
        public ActionResult Secure() => View((User.Identity as ClaimsIdentity).Claims);

        public async Task<ActionResult> ClearCache()
        {
            using (var scope = IocConfig.Container.BeginLifetimeScope())
            {
                await scope.Resolve<ICacheAccessor>().ClearAsync();
            }
            return RedirectToAction(nameof(Dashboard), new { message = "Redis cache and MemoryCache cleared successfully." });
        }

        [HttpPost]
        public ActionResult WarmEstabRepo(int maxBatchSize = 1000, 
            int maxConcurrency = 40, 
            int? maxTotalRecords = null)
        {
            HostingEnvironment.QueueBackgroundWorkItem(async x =>
            {
                using (var scope = IocConfig.Container.BeginLifetimeScope())
                {
                    var repo = scope.Resolve<ICachedEstablishmentReadRepository>();
                    await repo.WarmAsync(maxBatchSize, maxConcurrency, maxTotalRecords);
                }
            });

            return RedirectToAction(nameof(Dashboard), new { message = "Establishments cache is now warming." });
        }
        
        [HttpPost]
        public async Task<ActionResult> ResetAzureSearch()
        {
            await DeleteAzureSearchItems(EstablishmentsSearchIndex.INDEX_NAME);
            await DeleteAzureSearchItems(GovernorsSearchIndex.INDEX_NAME);
            await DeleteAzureSearchItems(GroupsSearchIndex.INDEX_NAME);
            await ConfigureAzureSearch();

            if (Request.AcceptTypes.Contains("application/json"))
            {
                return Json(new { success = true });
            }
            else
            {
                return RedirectToAction(nameof(Dashboard), new { message = "Re-created azure search indexes, indexers and data sources." });
            }
        }

        private async Task ConfigureAzureSearch()
        {
            var connStringName = ConfigurationManager.AppSettings["environment"] == "localdev" 
                ? "EdubaseSqlDb_remotedev" : "EdubaseSqlDb";
            var sqlConnectionString = ConfigurationManager.ConnectionStrings[connStringName]?.ConnectionString;

            if (string.IsNullOrWhiteSpace(sqlConnectionString)) throw new System.Exception("The sql connection string is empty");

            await SetupAzureSearch(new EstablishmentsSearchIndex().CreateModel(),
                nameof(Establishment), sqlConnectionString);

            await SetupAzureSearch(new GovernorsSearchIndex().CreateModel(),
                nameof(Governor), sqlConnectionString);

            await SetupAzureSearch(new GroupsSearchIndex().CreateModel(),
                nameof(GroupCollection), sqlConnectionString);
        }



        private async Task SetupAzureSearch(SearchIndex index, string tableName, string sqlConnectionString)
        {
            await _azureSearchEndPoint.CreateOrUpdateIndexAsync(index);
            await _azureSearchEndPoint.CreateOrUpdateDataSource(index.Name + "-ds", sqlConnectionString, tableName);
            await _azureSearchEndPoint.CreateOrUpdateIndexerAsync(index.Name + "-indexer", index.Name + "-ds", index.Name);
        }

        private async Task DeleteAzureSearchItems(string indexName)
        {
            await _azureSearchEndPoint.DeleteIndexAsync(indexName);
            await _azureSearchEndPoint.DeleteDataSourceAsync(indexName + "-ds");
            await _azureSearchEndPoint.DeleteIndexerAsync(indexName + "-indexer");
        }

        public async Task FlushErrors() => await DependencyResolver.Current
                .GetService<IMessageLoggingService>().FlushAsync();
    }
}