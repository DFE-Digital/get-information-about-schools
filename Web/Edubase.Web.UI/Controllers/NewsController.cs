using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Edubase.Common.Cache;
using Edubase.Data.Repositories;
using Edubase.Web.UI.Filters;
using Edubase.Web.UI.Helpers;
using Edubase.Web.UI.Models.News;

namespace Edubase.Web.UI.Controllers
{
    [RoutePrefix("News"), Route("{action=index}")]
    public class NewsController : EduBaseController
    {
        public const string NewsBlobETag = "newsblog-etag";
        private readonly NewsArticleRepository _newsRepository;
        private readonly ICacheAccessor _cacheAccessor;

        public NewsController(NewsArticleRepository newsRepository, ICacheAccessor cacheAccessor)
        {
            _newsRepository = newsRepository;
            _cacheAccessor = cacheAccessor;
        }

        [Route(Name = "News")]
        public async Task<ActionResult> Index()
        {
            var result = await _newsRepository.GetAllAsync(1000);
            var model = new NewsArticlesViewModel(result.Items.Where(x => x.Visible && x.ArticleDate >= DateTime.Now.AddMonths(-6)));
            return View(model);
        }

        [Route("Refresh"), EdubaseAuthorize(Roles = AuthorizedRoles.IsAdmin)]
        public async Task<ActionResult> Refresh()
        {
            await _cacheAccessor.DeleteAsync(NewsBlobETag);
            return RedirectToAction(nameof(Index));
        }

        [Route("Archive")]
        public async Task<ActionResult> Archive()
        {
            var result = await _newsRepository.GetAllAsync(1000);
            var model = new NewsArticlesViewModel(result.Items.Where(x => x.Visible && x.ArticleDate < DateTime.Now.AddMonths(-6)));
            return View(model);
        }
    }
}
