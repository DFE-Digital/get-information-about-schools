using Edubase.Common.Cache;
using Edubase.Services;
using Edubase.Services.Lookup;
using System;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Edubase.Web.UI.Controllers
{
    [RoutePrefix("Home"), Route("{action=index}")]
    public class HomeController : EduBaseController
    {
        private readonly ILookupService _lookup;
        private readonly IBlobService _blobService;
        private readonly ICacheAccessor _cacheAccessor;

        public HomeController(ILookupService lookup, IBlobService blobService, ICacheAccessor cacheAccessor)
        {
            _lookup = lookup;
            _blobService = blobService;
            _cacheAccessor = cacheAccessor;
        }

        [Route]
        public ActionResult Index()
        {
            var model = new Models.HomepageViewModel();
            model.AllowApprovals = User.Identity.IsAuthenticated;
            model.AllowSchoolCreation = User.Identity.IsAuthenticated;
            model.AllowTrustCreation = User.Identity.IsAuthenticated;
            return View(model);
        }

        [Route("~/cookies")]
        public ActionResult Cookies()
        {
            return View();
        }

        [Route("~/guidance")]
        public ActionResult Guidance()
        {
            return View();
        }
        
        [Route("~/news")]
        public async Task<ActionResult> News()
        {
            var blob = _blobService.GetBlobReference("content", "news.html");
            var html = await _cacheAccessor.GetAsync<string>("news");
            if (html == null)
            {
                html = await blob.DownloadTextAsync();
                await _cacheAccessor.SetAsync("news", html, TimeSpan.FromHours(1));
            }
            return View(new MvcHtmlString(html));
        }

        [Route("~/8bg594ghfdgh5t90-throwex"), Filters.EdubaseAuthorize]
        public ActionResult ThrowException() { throw new Exception("Test exception - to test exception reporting"); }
        

    }
}