using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Edubase.Data.Entity;
using Edubase.Data.Repositories;
using Edubase.Services.Texuna;
using Edubase.Web.UI.Filters;
using Edubase.Web.UI.Helpers;
using Edubase.Web.UI.Models;
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
        public async Task<ActionResult> Article(string id, string auditRoute)
        {
            var item = await _newsRepository.GetAsync(id);
            if (item == null) return HttpNotFound();

            var model = new NewsArticlesViewModel(new List<NewsArticle> { item }, item.ArticleDate.Year, false, false, auditRoute);
            return View(model);
        }

        [Route("Article/Audit/{id}", Name = "ArticleAudit"), HttpGet]
        public async Task<ActionResult> ArticleAudit(string id, string auditRoute = nameof(AuditArticle))
        {
            var item = await _newsRepository.GetAsync(id, eNewsArticlePartition.Archive);
            if (item == null) return HttpNotFound();

            var model = new NewsArticlesViewModel(new List<NewsArticle> { item }, item.ArticleDate.Year, false, false, auditRoute);
            return View(nameof(Article), model);
        }

        [Route("Manage", Name = "ManageNews"), EdubaseAuthorize(Roles = AuthorizedRoles.IsAdmin)]
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

        [Route("Edit"), EdubaseAuthorize(Roles = AuthorizedRoles.IsAdmin)]
        public async Task<ActionResult> Edit(int? year)
        {
            var lookupYear = year ?? DateTime.Now.Year;
            var result = await _newsRepository.GetAllAsync(1000, false, lookupYear);
            var model = new NewsArticlesViewModel(result.Items, lookupYear, true, true);
            return View(nameof(Index), model);
        }

        [Route("Article/New", Name = "CreateArticle"), HttpGet, EdubaseAuthorize(Roles = AuthorizedRoles.IsAdmin)]
        public async Task<ActionResult> CreateArticle()
        {
            return View("EditArticle", new NewsArticleViewModel());
        }


        [Route("Article/New", Name = "PostCreateArticle"), HttpPost, EdubaseAuthorize(Roles = AuthorizedRoles.IsAdmin)]
        public async Task<ActionResult> CreateArticleAsync(NewsArticleViewModel viewModel)
        {
            return await ProcessEditArticle(viewModel);
        }

        [Route("Article/{id}/Edit", Name = "EditArticle"), HttpGet, EdubaseAuthorize(Roles = AuthorizedRoles.IsAdmin)]
        public async Task<ActionResult> EditArticleAsync(string id)
        {
            var item = await _newsRepository.GetAsync(id);
            if (item == null) return HttpNotFound();

            return View("EditArticle", new NewsArticleViewModel
            {
                Id = id,
                Title = item.Title,
                Content = item.Content,
                ArticleDate = new DateTimeViewModel(item.ArticleDate, item.ArticleDate),
                ShowDate = item.ShowDate
            });
        }

        [Route("Article/{id}/Edit", Name = "PostEditArticle"), HttpPost, EdubaseAuthorize(Roles = AuthorizedRoles.IsAdmin)]
        public async Task<ActionResult> EditArticleAsync(NewsArticleViewModel viewModel)
        {
            var item = await _newsRepository.GetAsync(viewModel.Id);
            if (item == null) return HttpNotFound();

            return await ProcessEditArticle(viewModel, item);
        }

        private async Task<ActionResult> ProcessEditArticle(NewsArticleViewModel viewModel, NewsArticle originalArticle = null)
        {
            if (viewModel.GoBack)
            {
                viewModel.Action = (eNewsArticleAction) ((int) viewModel.Action - 1);

                // if we're going back, we dont really care about any validation errors
                foreach (var modelValue in ModelState.Values)
                {
                    modelValue.Errors.Clear();
                }
            }

            if (ModelState.IsValid)
            {
                if (viewModel.Action == eNewsArticleAction.Review)
                {
                    if (string.IsNullOrEmpty(viewModel.Id))
                    {
                        var item = viewModel.ToArticle();
                        item.AuditTimestamp = DateTime.Now;
                        item.AuditUser = User.GetUserId();
                        item.AuditEvent = eNewsArticleEvent.Create.ToString();
                        await _newsRepository.CreateAsync(item);
                    }
                    else
                    {
                        var item = viewModel.ToArticle(originalArticle);
                        item.AuditTimestamp = DateTime.Now;
                        item.AuditUser = User.GetUserId();
                        item.AuditEvent = eNewsArticleEvent.Update.ToString();
                        await _newsRepository.UpdateAsync(item);
                    }
                    TempData["ShowSaved"] = true;
                    return RedirectToAction(nameof(Edit));
                }

                ModelState.Remove(nameof(viewModel.Action));
                if (!viewModel.GoBack)
                {
                    viewModel.Action = (eNewsArticleAction) ((int) viewModel.Action + 1);
                }
            }

            return View("EditArticle", viewModel);
        }


        [Route("Article/{id}/Delete", Name = "DeleteArticle"), HttpGet, EdubaseAuthorize(Roles = AuthorizedRoles.IsAdmin)]
        public async Task<ActionResult> DeleteArticleAsync(NewsArticleViewModel viewModel)
        {
            var item = await _newsRepository.GetAsync(viewModel.Id);
            if (item == null) return HttpNotFound();
            return View("ConfirmDeleteArticle", viewModel.Set(item));
        }

        [Route("Article/{id}/Delete/Confirm", Name = "DeleteArticleConfirmed"), HttpGet, EdubaseAuthorize(Roles = AuthorizedRoles.IsAdmin)]
        public async Task<ActionResult> DeleteArticleConfirmedAsync(NewsArticleViewModel viewModel)
        {
            var item = await _newsRepository.GetAsync(viewModel.Id);
            if (item == null)
            {
                return HttpNotFound();
            }

            await _newsRepository.DeleteAsync(viewModel.Id, User.GetUserId());
            TempData["ShowSaved"] = true;
            return RedirectToAction(nameof(Manage));
        }

        [Route("Audit"), EdubaseAuthorize(Roles = AuthorizedRoles.IsAdmin)]
        public async Task<ActionResult> AuditArticles(string sortBy)
        {
            var result = await _newsRepository.GetAllAsync(1000, false);
            var audit = await _newsRepository.GetAllAsync(1000, false,null,null, eNewsArticlePartition.Archive);
            var items = result.Items.ToList();
            items.AddRange(audit.Items);

            var distinct = items.GroupBy(x => x.Tracker)
                .Select(grp => new { tracker = grp.Key, banners = grp.OrderByDescending(x => x.Version) })
                .Select(x => x.banners.First());

            var model = new NewsArticlesAuditViewModel(distinct, sortBy);
            return View(model);
        }

        [Route("Audit/{id}"), EdubaseAuthorize(Roles = AuthorizedRoles.IsAdmin)]
        public async Task<ActionResult> AuditArticle(string id, string sortBy)
        {
            var result = await _newsRepository.GetAllAsync(1000, false);
            var audit = await _newsRepository.GetAllAsync(1000, false, null, null, eNewsArticlePartition.Archive);
            var items = result.Items.ToList();
            items.AddRange(audit.Items);

            var distinct = items.Where(x => x.Tracker == id);

            var model = new NewsArticleAuditViewModel(distinct, sortBy);
            return View(model);
        }
    }
}
