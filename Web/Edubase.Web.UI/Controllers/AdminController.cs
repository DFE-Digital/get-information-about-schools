using Edubase.Data.Repositories;
using Edubase.Services;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Edubase.Common;
using Edubase.Web.UI.Models.Admin;
using Edubase.Services.Security;
using Autofac;
using Edubase.Services.IntegrationEndPoints.Smtp;
using Edubase.Common.Cache;
using Edubase.Data.Repositories.Establishments;
using System.Security.Claims;

namespace Edubase.Web.UI.Controllers
{
    [Authorize(Roles = EdubaseRoles.Admin)]
    public class AdminController : EduBaseController
    {
        // GET: Admin
        public ActionResult Index() => View();
        
        public ActionResult Dashboard()
        {
            var cache = DependencyResolver.Current.GetService<CacheAccessor>();
            ViewBag.SmtpEndPointName = DependencyResolver.Current.GetService<ISmtpEndPoint>().GetType().FullName;
            ViewBag.RedisStatus = cache.Status.ToString();
            ViewBag.MemoryCacheSize = cache.GetMemoryCacheApproximateSize().ToString();
            return View();   
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

        [HttpGet]
        public async Task<ActionResult> ClearCache()
        {
            await DependencyResolver.Current.GetService<CacheAccessor>().ClearAsync();
            return Content($"Cleared cache");
        }

        [HttpGet]
        public async Task<ActionResult> WarmEstabRepo(int maxBatchSize = 1000, int maxConcurrency = 40, int? maxTotalRecords = null)
        {
            var report = await DependencyResolver.Current
                .GetService<ICachedEstablishmentReadRepository>().WarmAsync(maxBatchSize, maxConcurrency, maxTotalRecords).ConfigureAwait(false);
            return Content($"Warmed establishments repository\r\n" + report, "text/plain");
        }

        public async Task FlushErrors() => await DependencyResolver.Current
                .GetService<IMessageLoggingService>().FlushAsync();
    }
}