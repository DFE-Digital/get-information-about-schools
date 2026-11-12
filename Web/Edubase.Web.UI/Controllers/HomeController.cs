using System;
using System.Linq;
using System.Threading.Tasks;
using Edubase.Common.Cache;
using Edubase.Data;
using Edubase.Data.Repositories;
using Edubase.Services;
using Edubase.Services.Establishments;
using Edubase.Services.Lookup;
using Edubase.Web.UI.Helpers;
using Edubase.Web.UI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Edubase.Web.UI.Controllers
{
    [Route("Home")]
    public class HomeController : EduBaseController
    {
        private const string UserPrefsCookieName = "analytics_preferences";

        private readonly ILookupService _lookup;
        private readonly IBlobService _blobService;
        private readonly ICacheAccessor _cacheAccessor;
        private readonly INewsArticleRepository _newsRepository;
        private readonly IConfiguration _configuration;

        public HomeController(
            ILookupService lookup,
            IBlobService blobService,
            ICacheAccessor cacheAccessor,
            INewsArticleRepository newsRepository,
            IConfiguration configuration)
        {
            _lookup = lookup;
            _blobService = blobService;
            _cacheAccessor = cacheAccessor;
            _newsRepository = newsRepository;
            _configuration = configuration;
        }

        [HttpGet("~/")]
        public async Task<ActionResult> Index()
        {
            var results = await _newsRepository.GetAllAsync(1000);
            var items = results.Items.Where(x => x.Visible).OrderByDescending(x => x.ArticleDate).Take(2);
            return View(new HomepageViewModel(items));
        }

        [HttpGet("~/about")]
        public ActionResult About() => View();

        [HttpGet("~/accessibility")]
        public ActionResult Accessibility() => View();

        [HttpGet("~/accessibility/report")]
        public ActionResult AccessibilityReport() => View();

        [HttpGet("~/content")]
        [Filters.EdubaseAuthorize]
        public async Task<ActionResult> Container(string file)
        {
            return await GetFileFromContainer("content", file);
        }

        [HttpGet("~/content/guidance")]
        [Filters.EdubaseAuthorize]
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

        [HttpGet("~/cookies")]
        public ActionResult Cookies() => View();

        [HttpPost("~/CookieChoices")]
        [ValidateAntiForgeryToken]
        public ActionResult CookieChoices(bool acceptAnalyticsCookies = false)
        {
            var cookieDomain = GetCookieDomain();
            var returnTo = Request.Form["OriginatingPage"].ToString();

            Response.Cookies.Append(UserPrefsCookieName, acceptAnalyticsCookies.ToString(), new CookieOptions
            {
                Expires = DateTime.Today.AddDays(28),
                SameSite = SameSiteMode.Lax,
                Domain = cookieDomain
            });
            TempData["CookiesPrefsSaved"] = acceptAnalyticsCookies;

            if (string.IsNullOrWhiteSpace(returnTo))
            {
                return RedirectToAction("cookies");
            }

            returnTo = Uri.UnescapeDataString(returnTo);

            if (Uri.IsWellFormedUriString(returnTo, UriKind.RelativeOrAbsolute) &&
                !returnTo.Contains("\n") &&
                !returnTo.Contains("\r") &&
                Url.IsLocalUrl(returnTo))
            {
                return Redirect(returnTo);
            }

            return RedirectToAction("cookies");
        }

        [HttpPost("~/CookieChoicesAjax")]
        public ActionResult CookieChoicesAjax(bool acceptAnalyticsCookies = false)
        {
            var cookieDomain = GetCookieDomain();
            Response.Cookies.Append(UserPrefsCookieName, acceptAnalyticsCookies.ToString(), new CookieOptions
            {
                Expires = DateTime.Today.AddDays(28),
                SameSite = SameSiteMode.Lax,
                Domain = cookieDomain
            });

            return Json(new { success = true, analyticsPref = acceptAnalyticsCookies }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet("~/responsibilities")]
        public ActionResult Responsibilities() => View();

        [HttpGet("~/privacy")]
        public ActionResult Privacy() => View();

        [HttpGet("~/help")]
        public ActionResult Help() => View();

        //Appears that this was used to test the error handling.  It can only be accessed if you are signed in so no problem keeping it here for now.
        [HttpGet("~/8bg594ghfdgh5t90-throwex")]
        [Filters.EdubaseAuthorize]
        public ActionResult ThrowException() => throw new Exception("Test exception - to test exception reporting");

        [HttpGet("~/service-wsdl")]
        [HttpGet("~/service.wsdl")]
        public ActionResult ServiceWSDL()
        {
            //using (var client = new HttpClient())
            //{
            //    var result = (await client.GetAsync("http://ea-edubase-api-prod.azurewebsites.net/edubase/service.wsdl")).EnsureSuccessStatusCode();
            //    return Content(await result.Content.ReadAsStringAsync(), "text/xml");
            //}

            //Commented out the above and replaced this with a redirect as this is not a good way to create the HttpClient and could cause socket exhaustion
            return Redirect("https://ea-edubase-api-prod.azurewebsites.net/edubase/service.wsdl");
        }

        [HttpGet("~/Contact")]
        public ActionResult Contact() => View();

        [HttpGet("~/AcceptableUsePolicy")]
        public ActionResult AcceptableUsePolicy() => View();

        private string GetCookieDomain()
        {
            var request = HttpContext.Request;
            var requestHeaderHost = request.Headers["X-Forwarded-Host"].ToString();
            var host = request.Host.HasValue ? request.Host.Host : string.Empty;

            if (!string.IsNullOrWhiteSpace(requestHeaderHost))
            {
                var allowedForwardedHostNames = (_configuration["AllowedForwardedHostNames"] ?? string.Empty)
                    .Split(',')
                    .Select(x => x.Trim())
                    .Where(x => !string.IsNullOrWhiteSpace(x));

                if (allowedForwardedHostNames.Contains(requestHeaderHost, StringComparer.OrdinalIgnoreCase))
                {
                    host = requestHeaderHost;
                }
                else
                {
                    throw new InvalidOperationException($"The `X-Forwarded-Host` header contains an invalid value: {requestHeaderHost}");
                }
            }

            return string.Concat(".", host);
        }
    }
}
