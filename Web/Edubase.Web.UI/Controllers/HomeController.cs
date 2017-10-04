using Edubase.Common.Cache;
using Edubase.Services;
using Edubase.Services.Lookup;
using System;
using System.Net.Http;
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
        public async Task<ActionResult> News() => View(new MvcHtmlString(await GetHtmlBlob("newsblog.html")));

        [Route("~/known-issues")]
        public ActionResult KnownIssues() => RedirectToAction(nameof(AreasUnderDevelopment));

        [Route("~/areas-under-development")]
        public async Task<ActionResult> AreasUnderDevelopment() => View("KnownIssues", new MvcHtmlString(await GetHtmlBlob("knownissues.html")));

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

        private async Task<string> GetHtmlBlob(string name)
        {
            var blob = _blobService.GetBlobReference("content", name);
            var html = await _cacheAccessor.GetAsync<string>(name);
            if (html == null && await blob.ExistsAsync())
            {
                html = await blob.DownloadTextAsync();
                await _cacheAccessor.SetAsync(name, html, TimeSpan.FromHours(1));
            }
            return html;
        }
    }
}