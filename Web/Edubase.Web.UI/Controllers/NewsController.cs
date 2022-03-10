using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;
using Edubase.Data.Entity;
using Edubase.Data.Repositories;
using Edubase.Web.UI.Filters;
using Edubase.Web.UI.Helpers;
using Edubase.Web.UI.Models.News;

namespace Edubase.Web.UI.Controllers
{
    [RoutePrefix("News"), Route("{action=index}")]
    public class NewsController : EduBaseController
    {
        private readonly NewsArticleRepository _newsRepository;

        public NewsController(NewsArticleRepository newsRepository)
        {
            _newsRepository = newsRepository;
        }

        [Route(Name = "News")]
        public async Task<ActionResult> Index(int? year)
        {
            var lookupYear = year ?? DateTime.Now.Year;
            var result = await _newsRepository.GetAllAsync(1000, true, lookupYear);
            var model = new NewsArticlesViewModel(result.Items, lookupYear);
            return View(model);
        }
        
        [Route("Article/{id}", Name = "Article"), HttpGet]
        public async Task<ActionResult> Article(string id)
        {
            var item = await _newsRepository.GetAsync(id);
            if (item == null) return HttpNotFound();

            var model = new NewsArticlesViewModel(new List<NewsArticle> { item }, item.ArticleDate.Year);
            return View(model);
        }

        [Route("Manage"), EdubaseAuthorize(Roles = AuthorizedRoles.IsAdmin)]
        public async Task<ActionResult> Manage()
        {
            return View();
        }

        [Route("Preview"), EdubaseAuthorize(Roles = AuthorizedRoles.IsAdmin)]
        public async Task<ActionResult> Preview(int? year)
        {
            var lookupYear = year ?? DateTime.Now.Year;
            var result = await _newsRepository.GetAllAsync(1000, false, lookupYear);
            var model = new NewsArticlesViewModel(result.Items, lookupYear, true);
            return View(nameof(Index), model);
        }
    }
}
