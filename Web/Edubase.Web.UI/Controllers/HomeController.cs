using Edubase.Common.Cache;
using Edubase.Services;
using Edubase.Services.Lookup;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;
using Edubase.Data.Repositories;
using Edubase.Services.Establishments;
using Edubase.Web.UI.Helpers;
using Edubase.Web.UI.Models;

namespace Edubase.Web.UI.Controllers
{
    [RoutePrefix("Home"), Route("{action=index}")]
    public class HomeController : EduBaseController
    {
        public const string NewsBlobETag = "newsblog-etag";
        private const string UserPrefsCookieName = "analytics_preferences";

        private readonly ILookupService _lookup;
        private readonly IBlobService _blobService;
        private readonly ICacheAccessor _cacheAccessor;

        public HomeController(ILookupService lookup, IBlobService blobService, ICacheAccessor cacheAccessor)
        {
            _lookup = lookup;
            _blobService = blobService;
            _cacheAccessor = cacheAccessor;
        }

        [Route("~/")]
        public ActionResult Index() => View(new HomepageViewModel());

        [Route("~/about")]
        public ActionResult About() => View();


        [Route("~/accessibility")]
        public ActionResult Accessibility() => View();


        [Route("~/accessibility/report")]
        public ActionResult AccessibilityReport() => View();


        [Route("~/content"), Filters.EdubaseAuthorize]
        public async Task<ActionResult> Container(string file)
        {
            return await GetFileFromContainer("content", file);
        }

        [Route("~/content/guidance"), Filters.EdubaseAuthorize]
        public async Task<ActionResult> Guidance(string file)
        {
            return await GetFileFromContainer("guidance", file);
        }

        private async Task<ActionResult> GetFileFromContainer(string container, string file)
        {
            var blob = _blobService.GetBlobReference(container, file);
            if (await blob.ExistsAsync())
            {
                var stream = await blob.OpenReadAsync();
                return new FileStreamResult(stream, blob.Properties.ContentType)
                {
                    FileDownloadName = blob.Name
                };
            }
            throw new Exception("File not available");
        }

        [Route("~/cookies")]
        public ActionResult Cookies() => View();

        [HttpPost, Route("~/CookieChoices")]
        public ActionResult CookieChoices(bool acceptAnalyticsCookies)
        {
            var returnTo = Request.Form["OriginatingPage"];
            var cookieDomain = string.Concat(".", Request.Url.Host);
            Response.Cookies.Set(new HttpCookie(UserPrefsCookieName, acceptAnalyticsCookies.ToString()) { Expires = DateTime.Today.AddDays(28), SameSite = SameSiteMode.Lax, Domain = cookieDomain });
            TempData["CookiesPrefsSaved"] = acceptAnalyticsCookies;
            if (returnTo != null)
            {
                return Redirect(returnTo);
            }
            return RedirectToAction("cookies");
        }

        [HttpPost, Route("~/CookieChoicesAjax")]
        public ActionResult CookieChoicesAjax(bool acceptAnalyticsCookies)
        {
            var cookieDomain = string.Concat(".", Request.Url.Host);
            Response.Cookies.Set(new HttpCookie(UserPrefsCookieName, acceptAnalyticsCookies.ToString()) { Expires = DateTime.Today.AddDays(28), SameSite = SameSiteMode.Lax, Domain = cookieDomain});
            return Json(new { success = true , analyticsPref = acceptAnalyticsCookies}, JsonRequestBehavior.AllowGet);
        }

        [Route("~/responsibilities")]
        public ActionResult Responsibilities() => View();

        [Route("~/privacy")]
        public ActionResult Privacy() => View();
        
        [Route("~/help")]
        public ActionResult Help() => View();

        [Route("~/8bg594ghfdgh5t90-throwex"), Filters.EdubaseAuthorize]
        public ActionResult ThrowException() { throw new Exception("Test exception - to test exception reporting"); }

        [Route("~/service-wsdl"), Route("~/service.wsdl")]
        public async Task<ActionResult> ServiceWSDL()
        {
            using (var client = new HttpClient())
            {
                var result = (await client.GetAsync("http://ea-edubase-api-prod.azurewebsites.net/edubase/service.wsdl")).EnsureSuccessStatusCode();
                return Content(await result.Content.ReadAsStringAsync(), "text/xml");
            }
        }

        [Route("~/Contact")]
        public ActionResult Contact() => View();

        public static string GetNewsPageETag()
        {
            var cache = DependencyResolver.Current.GetService<ICacheAccessor>();
            var newsRepo = DependencyResolver.Current.GetService<NewsArticleRepository>();

            var etag = cache.Get<string>(NewsBlobETag);
            if (etag == null)
            {
                var results = newsRepo.GetAll(1000);
                var latestItem = results.Items.Where(x => x.Visible).OrderByDescending(x => x.ArticleDate)
                    .First();
                etag = latestItem.RowKey;
                cache.Set(NewsBlobETag, etag, TimeSpan.FromHours(1));
            }

            return etag;
        }
    }
}
