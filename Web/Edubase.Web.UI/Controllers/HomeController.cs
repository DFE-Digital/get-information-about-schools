using System;
using System.Linq;
using System.Threading.Tasks;
using Edubase.Common.Cache;
using Edubase.Data.Repositories;
using Edubase.Services;
using Edubase.Services.Lookup;
using Edubase.Web.UI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Edubase.Web.UI.Controllers
{
    [ApiController]
    [Route("")]
    public class HomeController : EduBaseController
    {
        private const string UserPrefsCookieName = "analytics_preferences";

        private readonly ILookupService _lookup;
        private readonly IBlobService _blobService;
        private readonly ICacheAccessor _cacheAccessor;
        private readonly NewsArticleRepository _newsRepository;

        public HomeController(
            ILookupService lookup,
            IBlobService blobService,
            ICacheAccessor cacheAccessor,
            NewsArticleRepository newsRepository)
        {
            _lookup = lookup;
            _blobService = blobService;
            _cacheAccessor = cacheAccessor;
            _newsRepository = newsRepository;
        }

        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            var results = await _newsRepository.GetAllAsync(1000);
            var items = results.Where(x => x.Visible).OrderByDescending(x => x.ArticleDate).Take(2);
            return View(new HomepageViewModel(items));
        }

        [HttpGet("about")]
        public IActionResult About() => View();

        [HttpGet("accessibility")]
        public IActionResult Accessibility() => View();

        [HttpGet("accessibility/report")]
        public IActionResult AccessibilityReport() => View();

        [HttpGet("content")]
        [Authorize]
        public async Task<IActionResult> Container(string file) =>
            await GetFileFromContainer("content", file);

        [HttpGet("content/guidance")]
        [Authorize]
        public async Task<IActionResult> Guidance(string file) =>
            await GetFileFromContainer("guidance", file);

        private async Task<IActionResult> GetFileFromContainer(string container, string file)
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

            return NotFound("File not available");
        }

        [HttpGet("cookies")]
        public IActionResult Cookies() => View();

        [HttpPost("cookie-choices")]
        [ValidateAntiForgeryToken]
        public IActionResult CookieChoices(bool acceptAnalyticsCookies = false)
        {
            var returnTo = Request.Form["OriginatingPage"];
            Response.Cookies.Append(UserPrefsCookieName, acceptAnalyticsCookies.ToString(), new CookieOptions
            {
                Expires = DateTimeOffset.Now.AddDays(28),
                SameSite = SameSiteMode.Lax,
                Secure = true
            });

            TempData["CookiesPrefsSaved"] = acceptAnalyticsCookies;

            if (string.IsNullOrWhiteSpace(returnTo))
                return RedirectToAction("Cookies");

            returnTo = Uri.UnescapeDataString(returnTo);

            if (Uri.IsWellFormedUriString(returnTo, UriKind.RelativeOrAbsolute) &&
                !returnTo.Contains("\n") && !returnTo.Contains("\r"))
            {
                return Redirect(returnTo);
            }

            return RedirectToAction("Cookies");
        }

        [HttpPost("cookie-choices-ajax")]
        public IActionResult CookieChoicesAjax(bool acceptAnalyticsCookies = false)
        {
            Response.Cookies.Append(UserPrefsCookieName, acceptAnalyticsCookies.ToString(), new CookieOptions
            {
                Expires = DateTimeOffset.Now.AddDays(28),
                SameSite = SameSiteMode.Lax,
                Secure = true
            });

            return Json(new { success = true, analyticsPref = acceptAnalyticsCookies });
        }

        [HttpGet("responsibilities")]
        public IActionResult Responsibilities() => View();

        [HttpGet("privacy")]
        public IActionResult Privacy() => View();

        [HttpGet("help")]
        public IActionResult Help() => View();

        [HttpGet("8bg594ghfdgh5t90-throwex")]
        [Authorize]
        public IActionResult ThrowException() => throw new Exception("Test exception - to test exception reporting");

        [HttpGet("service-wsdl")]
        [HttpGet("service.wsdl")]
        public IActionResult ServiceWSDL() =>
            Redirect("https://ea-edubase-api-prod.azurewebsites.net/edubase/service.wsdl");

        [HttpGet("contact")]
        public IActionResult Contact() => View();
    }
}
