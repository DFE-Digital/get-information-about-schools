using Edubase.Common.Cache;
using Edubase.Services;
using Edubase.Services.Lookup;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Edubase.Web.UI.Controllers
{
    [RoutePrefix("Home"), Route("{action=index}")]
    public class HomeController : EduBaseController
    {
        const string NewsBlobNameHtml = "newsblog.html";
        public const string NewsBlobETag = "newsblog-etag";
        
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
        public ActionResult Cookies() => View();

        [Route("~/guidance")]
        public ActionResult Guidance() => View();

        [Route("~/news")]
        public async Task<ActionResult> News(bool? refresh)
        {
            var blob = _blobService.GetBlobReference("content", NewsBlobNameHtml);
            var html = await _cacheAccessor.GetAsync<string>(NewsBlobNameHtml);
            var etag = await _cacheAccessor.GetAsync<string>(NewsBlobETag);

            if((html == null && await blob.ExistsAsync()) || refresh.GetValueOrDefault())
            {
                await blob.FetchAttributesAsync();
                html = await blob.DownloadTextAsync();
                etag = CleanETag(blob.Properties.ETag);

                await _cacheAccessor.SetAsync(NewsBlobNameHtml, html, TimeSpan.FromHours(1));
                await _cacheAccessor.SetAsync(NewsBlobETag, etag, TimeSpan.FromHours(1));
            }

            Response.Cookies.Set(new HttpCookie(NewsBlobETag, etag) { Expires = DateTime.MaxValue });

            return View(new MvcHtmlString(html));
        }

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

        public static string GetNewsPageETag() => DependencyResolver.Current.GetService<ICacheAccessor>().Get<string>(NewsBlobETag);

        private static string CleanETag(string s) => s.Replace("\"", string.Empty);
    }
}