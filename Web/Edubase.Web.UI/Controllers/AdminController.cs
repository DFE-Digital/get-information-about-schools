﻿using Autofac;
using Edubase.Common;
using Edubase.Common.Cache;
using Edubase.Services;
using Edubase.Services.Security;
using Edubase.Web.UI.Helpers;
using Edubase.Web.UI.Models.Admin;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Edubase.Web.UI.Controllers
{
    [RoutePrefix("Admin"), Route("{action=Logs}"), MvcAuthorizeRoles(EdubaseRoles.ROLE_BACKOFFICE)]
    public class AdminController : EduBaseController
    {
        [Route("Logs")]
        public async Task<ActionResult> Logs(string date, string skipToken)
        {
            var dto = await new LogMessageReadService().GetAllAsync(10, skipToken, date.ToDateTime("yyyyMMdd"));
            var viewModel = new LogMessagesViewModel(dto) { DateFilter = date };
            return View(viewModel);
        }

        [Route("LogDetail/{id}")]
        public async Task<ActionResult> LogDetail(string id)
        {
            var message = await new LogMessageReadService().GetAsync(id);
            if (message == null) return Content("Log message not found.  Please note it may take up to 30 seconds for a log message to become available at this URL.", "text/plain");
            return View(message);
        }
        
        [HttpGet, Route("GetPendingErrors")]
        public ActionResult GetPendingErrors(string pwd)
        {
            if (pwd == "c7634") return Json(DependencyResolver.Current
                .GetService<IMessageLoggingService>().GetPending());
            else return new EmptyResult();
        }

        [HttpGet, Route("GetPendingErrorsId")]
        public ActionResult GetPendingErrorsId()
        {
            return Json(DependencyResolver.Current
                .GetService<IMessageLoggingService>().InstanceId);
        }
        
        [Route("ClearCache")]
        public async Task<ActionResult> ClearCache()
        {
            await IocConfig.AutofacDependencyResolver.ApplicationContainer.Resolve<ICacheAccessor>().ClearAsync();   
            return Content("Redis cache and MemoryCache cleared successfully.", "text/plain");
        }
        
        public async Task FlushErrors() => await DependencyResolver.Current
                .GetService<IMessageLoggingService>().FlushAsync();
    }
}