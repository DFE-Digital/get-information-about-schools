using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Edubase.Data.Entity;
using Edubase.Data.Repositories;
using Edubase.Services.Texuna;
using Edubase.Web.UI.Helpers;
using Edubase.Web.UI.Models;
using Edubase.Web.UI.Models.News;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Edubase.Web.UI.Controllers
{
    [ApiController]
    [Route("news")]
    public class NewsController : EduBaseController
    {
        private readonly NewsArticleRepository _newsRepository;

        public NewsController(NewsArticleRepository newsRepository)
        {
            _newsRepository = newsRepository;
        }

        [HttpGet("", Name = "News")]
        public async Task<IActionResult> Index(int? year)
        {
            var lookupYear = year ?? DateTime.Now.Year;
            var result = await _newsRepository.GetAllAsync(1000, true, lookupYear);
            var model = new NewsArticlesViewModel(result.Items, lookupYear);
            return View(model);
        }

        [HttpGet("article/{id}", Name = "Article")]
        public async Task<IActionResult> Article(string id, string auditRoute)
        {
            var item = await _newsRepository.GetAsync(id);
            if (item == null) return NotFound();

            var model = new NewsArticlesViewModel(new List<NewsArticle> { item }, item.ArticleDate.Year, false, false, auditRoute);
            return View(model);
        }

        [HttpGet("article/audit/{id}", Name = "ArticleAudit")]
        public async Task<IActionResult> ArticleAudit(string id, string auditRoute = nameof(AuditArticle))
        {
            var item = await _newsRepository.GetAsync(id, eNewsArticlePartition.Archive);
            if (item == null) return NotFound();

            var model = new NewsArticlesViewModel(new List<NewsArticle> { item }, item.ArticleDate.Year, false, false, auditRoute);
            return View(nameof(Article), model);
        }

        [HttpGet("manage", Name = "ManageNews")]
        [Authorize(Roles = AuthorizedRoles.IsAdmin)]
        public IActionResult Manage() => View();

        [HttpGet("preview")]
        [Authorize(Roles = AuthorizedRoles.IsAdmin)]
        public async Task<IActionResult> Preview(int? year)
        {
            var lookupYear = year ?? DateTime.Now.Year;
            var result = await _newsRepository.GetAllAsync(1000, false, lookupYear);
            var model = new NewsArticlesViewModel(result.Items, lookupYear, true);
            return View(nameof(Index), model);
        }

        [HttpGet("edit")]
        [Authorize(Roles = AuthorizedRoles.IsAdmin)]
        public async Task<IActionResult> Edit(int? year)
        {
            var lookupYear = year ?? DateTime.Now.Year;
            var result = await _newsRepository.GetAllAsync(1000, false, lookupYear);
            var model = new NewsArticlesViewModel(result.Items, lookupYear, true, true);
            return View(nameof(Index), model);
        }

        [HttpGet("article/new", Name = "CreateArticle")]
        [Authorize(Roles = AuthorizedRoles.IsAdmin)]
        public IActionResult CreateArticle() => View("EditArticle", new NewsArticleViewModel());

        [HttpPost("article/new", Name = "PostCreateArticle")]
        [Authorize(Roles = AuthorizedRoles.IsAdmin)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateArticleAsync(NewsArticleViewModel viewModel) =>
            await ProcessEditArticle(viewModel);

        [HttpGet("article/{id}/edit", Name = "EditArticle")]
        [Authorize(Roles = AuthorizedRoles.IsAdmin)]
        public async Task<IActionResult> EditArticleAsync(string id)
        {
            var item = await _newsRepository.GetAsync(id);
            if (item == null) return NotFound();

            return View("EditArticle", new NewsArticleViewModel
            {
                Id = id,
                Title = item.Title,
                Content = item.Content,
                ArticleDate = new DateTimeViewModel(item.ArticleDate, item.ArticleDate),
                ShowDate = item.ShowDate
            });
        }

        [HttpPost("article/{id}/edit", Name = "PostEditArticle")]
        [Authorize(Roles = AuthorizedRoles.IsAdmin)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditArticleAsync(NewsArticleViewModel viewModel)
        {
            var item = await _newsRepository.GetAsync(viewModel.Id);
            if (item == null) return NotFound();

            return await ProcessEditArticle(viewModel, item);
        }

        private async Task<IActionResult> ProcessEditArticle(NewsArticleViewModel viewModel, NewsArticle originalArticle = null)
        {
            if (viewModel.GoBack)
            {
                viewModel.Action = (eNewsArticleAction) ((int) viewModel.Action - 1);
                foreach (var modelValue in ModelState.Values)
                {
                    modelValue.Errors.Clear();
                }
            }

            if (ModelState.IsValid)
            {
                if (viewModel.Action == eNewsArticleAction.Review)
                {
                    var item = string.IsNullOrEmpty(viewModel.Id)
                        ? viewModel.ToArticle()
                        : viewModel.ToArticle(originalArticle);

                    item.AuditTimestamp = DateTime.Now;
                    item.AuditUser = User.GetUserId();
                    item.AuditEvent = string.IsNullOrEmpty(viewModel.Id)
                        ? eNewsArticleEvent.Create.ToString()
                        : eNewsArticleEvent.Update.ToString();

                    if (string.IsNullOrEmpty(viewModel.Id))
                        await _newsRepository.CreateAsync(item);
                    else
                        await _newsRepository.UpdateAsync(item);

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

        [HttpGet("article/{id}/delete", Name = "DeleteArticle")]
        [Authorize(Roles = AuthorizedRoles.IsAdmin)]
        public async Task<IActionResult> DeleteArticleAsync(NewsArticleViewModel viewModel)
        {
            var item = await _newsRepository.GetAsync(viewModel.Id);
            if (item == null) return NotFound();
            return View("ConfirmDeleteArticle", viewModel.Set(item));
        }

        [HttpGet("article/{id}/delete/confirm", Name = "DeleteArticleConfirmed")]
        [Authorize(Roles = AuthorizedRoles.IsAdmin)]
        public async Task<IActionResult> DeleteArticleConfirmedAsync(NewsArticleViewModel viewModel)
        {
            var item = await _newsRepository.GetAsync(viewModel.Id);
            if (item == null) return NotFound();

            await _newsRepository.DeleteAsync(viewModel.Id, User.GetUserId());
            TempData["ShowSaved"] = true;
            return RedirectToAction(nameof(Manage));
        }

        [HttpGet("audit")]
        [Authorize(Roles = AuthorizedRoles.IsAdmin)]
        public async Task<IActionResult> AuditArticles(string sortBy)
        {
            var result = await _newsRepository.GetAllAsync(1000, false);
            var audit = await _newsRepository.GetAllAsync(1000, false, null, null, eNewsArticlePartition.Archive);
            var items = result.Items.Concat(audit.Items).ToList();

            var distinct = items.GroupBy(x => x.Tracker)
                .Select(grp => grp.OrderByDescending(x => x.Version).First());

            var model = new NewsArticlesAuditViewModel(distinct, sortBy);
            return View(model);
        }

        [HttpGet("audit/{id}")]
        [Authorize(Roles = AuthorizedRoles.IsAdmin)]
        public async Task<IActionResult> AuditArticle(string id, string sortBy)
        {
            var result = await _newsRepository.GetAllAsync(1000, false);
            var audit = await _newsRepository.GetAllAsync(1000, false, null, null, eNewsArticlePartition.Archive);
            var items = result.Items.Concat(audit.Items).Where(x => x.Tracker == id);

            var model = new NewsArticleAuditViewModel(items, sortBy);
            return View(model);
        }
    }
}
