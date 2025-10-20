using System;
using System.Linq;
using System.Threading.Tasks;
using Edubase.Common;
using Edubase.Data.Entity;
using Edubase.Data.Repositories;
using Edubase.Services.Texuna;
using Edubase.Web.UI.Helpers;
using Edubase.Web.UI.Models;
using Edubase.Web.UI.Models.Notifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Edubase.Web.UI.Controllers
{
    [ApiController]
    [Route("notifications")]
    [Authorize(Roles = AuthorizedRoles.IsAdmin)]
    public class NotificationsController : EduBaseController
    {
        private readonly NotificationBannerRepository _BannerRepository;
        private readonly NotificationTemplateRepository _TemplateRepository;

        public NotificationsController(
            NotificationTemplateRepository TemplateRepository,
            NotificationBannerRepository BannerRepository)
        {
            _TemplateRepository = TemplateRepository;
            _BannerRepository = BannerRepository;
        }

        [HttpGet("", Name = "Notifications")]
        public IActionResult Index() => View();

        [HttpGet("banners")]
        public async Task<IActionResult> Banners()
        {
            var result = await _BannerRepository.GetAllAsync(2, null, true);
            var model = new NotificationsBannersViewModel(result.Items);

            if (TempData["ShowSaved"] != null)
            {
                ViewBag.ShowSaved = true;
                TempData.Remove("ShowSaved");
            }

            return View(model);
        }

        [HttpGet("banners/audit")]
        public async Task<IActionResult> AuditBanners(string sortBy)
        {
            var result = await _BannerRepository.GetAllAsync(1000);
            var audit = await _BannerRepository.GetAllAsync(1000, null, false, eNotificationBannerPartition.Archive);
            var items = result.Items.Concat(audit.Items).ToList();

            var distinct = items.GroupBy(x => x.Tracker)
                .Select(grp => grp.OrderByDescending(x => x.Version).First());

            var model = new NotificationsBannersAuditViewModel(distinct, sortBy);
            return View(model);
        }

        [HttpGet("banners/audit/{id}")]
        public async Task<IActionResult> AuditBanner(string id, string sortBy)
        {
            var result = await _BannerRepository.GetAllAsync(1000);
            var audit = await _BannerRepository.GetAllAsync(1000, null, false, eNotificationBannerPartition.Archive);
            var items = result.Items.Concat(audit.Items).Where(x => x.Tracker == id);

            var model = new NotificationsBannerAuditViewModel(items, sortBy);
            return View(model);
        }

        [HttpGet("banner/new", Name = "CreateBanner")]
        public async Task<IActionResult> CreateBanner()
        {
            var banners = await _BannerRepository.GetAllAsync(1000, null, true);
            var newBanner = new NotificationsBannerViewModel
            {
                TotalBanners = banners.Items.Count(),
                TotalLiveBanners = banners.Items.Count(x => x.Visible),
                Counter = banners.Items.Count() + 1
            };
            return View("EditBanner", newBanner);
        }

        [HttpPost("banner/new", Name = "PostCreateBanner")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateBannerAsync(NotificationsBannerViewModel viewModel)
        {
            return await ProcessEditBanner(viewModel);
        }

        [HttpGet("banner/{counter:int}/{id}", Name = "EditBanner")]
        public async Task<IActionResult> EditBannerAsync(string id, int counter)
        {
            var item = await _BannerRepository.GetAsync(id);
            if (item == null) return NotFound();

            var banners = await _BannerRepository.GetAllAsync(1000, null, true);

            if (TempData["ShowSaved"] != null)
            {
                ViewBag.ShowSaved = true;
                TempData.Remove("ShowSaved");
            }

            return View("EditBanner", new NotificationsBannerViewModel
            {
                Id = id,
                Counter = counter,
                Start = new DateTimeViewModel(item.Start, item.Start),
                StartOriginal = item.Start,
                End = new DateTimeViewModel(item.End, item.End),
                Importance = (eNotificationBannerImportance) item.Importance,
                Content = item.Content,
                TotalBanners = banners.Items.Count(),
                TotalLiveBanners = banners.Items.Count(x => x.Visible)
            });
        }

        [HttpPost("banner/{counter:int}/{id}", Name = "PostEditBanner")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditBannerAsync(NotificationsBannerViewModel viewModel)
        {
            var item = await _BannerRepository.GetAsync(viewModel.Id);
            if (item == null) return NotFound();

            return await ProcessEditBanner(viewModel, item);
        }

        private async Task<IActionResult> ProcessEditBanner(
            NotificationsBannerViewModel viewModel,
            NotificationBanner originalBanner = null)
        {
            if (viewModel.GoBack)
            {
                viewModel.Action = (eNotificationBannerAction) ((int) viewModel.Action - 1);
                foreach (var modelValue in ModelState.Values)
                {
                    modelValue.Errors.Clear();
                }
            }

            if (ModelState.IsValid)
            {
                if (viewModel.Action == eNotificationBannerAction.Start ||
                    viewModel.Action == eNotificationBannerAction.TypeChoice)
                {
                    var result = await _TemplateRepository.GetAllAsync(1000);
                    viewModel.Templates = result.Items;
                }

                if (viewModel.Action == eNotificationBannerAction.TypeChoice &&
                    !string.IsNullOrEmpty(viewModel.TemplateSelected))
                {
                    var result = await _TemplateRepository.GetAsync(viewModel.TemplateSelected);
                    viewModel.Content = result.Content;
                    ModelState.Remove(nameof(viewModel.Content));
                }

                if (viewModel.Action == eNotificationBannerAction.Review)
                {
                    if (string.IsNullOrEmpty(viewModel.Id))
                    {
                        var item = viewModel.ToBanner();
                        item.AuditTimestamp = DateTime.Now;
                        item.AuditUser = User.GetUserId();
                        item.AuditEvent = eNotificationBannerEvent.Create.ToString();
                        await _BannerRepository.CreateAsync(item);
                    }
                    else
                    {
                        var item = viewModel.ToBanner(originalBanner);
                        item.AuditTimestamp = DateTime.Now;
                        item.AuditUser = User.GetUserId();
                        item.AuditEvent = eNotificationBannerEvent.Update.ToString();
                        await _BannerRepository.UpdateAsync(item);
                    }

                    TempData["ShowSaved"] = true;

                    return viewModel.Counter == 0
                        ? RedirectToAction(nameof(EditBannerAsync), new { counter = 0, id = viewModel.Id })
                        : RedirectToAction(nameof(Banners));
                }

                ModelState.Remove(nameof(viewModel.Action));
                if (!viewModel.GoBack)
                {
                    viewModel.Action = (eNotificationBannerAction) ((int) viewModel.Action + 1);
                }
            }

            return View("EditBanner", viewModel);
        }

        [HttpGet("banner/{counter:int}/{id}/delete", Name = "DeleteBanner")]
        public async Task<IActionResult> DeleteBannerAsync(NotificationsBannerViewModel viewModel)
        {
            var item = await _BannerRepository.GetAsync(viewModel.Id);
            if (item == null) return NotFound();

            return View("ConfirmDeleteBanner", viewModel.Set(item));
        }

        [HttpGet("banner/{counter:int}/{id}/delete/confirm", Name = "DeleteBannerConfirmed")]
        public async Task<IActionResult> DeleteBannerConfirmedAsync(NotificationsBannerViewModel viewModel)
        {
            var item = await _BannerRepository.GetAsync(viewModel.Id);
            if (item == null) return NotFound();

            await _BannerRepository.DeleteAsync(viewModel.Id, User.GetUserId());
            TempData["ShowSaved"] = true;
            return RedirectToAction(nameof(Banners));
        }

        [HttpGet("templates")]
        public async Task<IActionResult> Templates()
        {
            var result = await _TemplateRepository.GetAllAsync(1000);
            var model = new NotificationsTemplatesViewModel(result.Items);

            if (TempData["ShowSaved"] != null)
            {
                ViewBag.ShowSaved = true;
                TempData.Remove("ShowSaved");
            }

            return View(model);
        }

        [HttpGet("template/new", Name = "CreateTemplate")]
        public IActionResult CreateTemplate() =>
            View("EditTemplate", new NotificationsTemplateViewModel());

        [HttpPost("template/new", Name = "PostCreateTemplate")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateTemplateAsync(NotificationsTemplateViewModel viewModel) =>
            await ProcessEditTemplate(viewModel);

        [HttpGet("template/{id}", Name = "EditTemplate")]
        public async Task<IActionResult> EditTemplateAsync(string id)
        {
            var item = await _TemplateRepository.GetAsync(id);
            if (item == null) return NotFound();

            return View("EditTemplate", new NotificationsTemplateViewModel
            {
                Id = id,
                Content = item.Content,
                OriginalContent = item.Content
            });
        }

        [HttpPost("template/{id}", Name = "PostEditTemplate")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditTemplateAsync(NotificationsTemplateViewModel viewModel)
        {
            var item = await _TemplateRepository.GetAsync(viewModel.Id);
            if (item == null) return NotFound();

            return await ProcessEditTemplate(viewModel, item);
        }

        private async Task<IActionResult> ProcessEditTemplate(
            NotificationsTemplateViewModel viewModel,
            NotificationTemplate oldModel = null)
        {
            if (viewModel.GoBack)
            {
                viewModel.Action = (eNotificationsTemplateAction) ((int) viewModel.Action - 1);
                foreach (var modelValue in ModelState.Values)
                {
                    modelValue.Errors.Clear();
                }
            }

            if (ModelState.IsValid)
            {
                if (viewModel.Action == eNotificationsTemplateAction.Review)
                {
                    if (string.IsNullOrEmpty(viewModel.Id))
                    {
                        var item = new NotificationTemplate { Content = viewModel.Content };
                        await _TemplateRepository.CreateAsync(item);
                    }
                    else
                    {
                        var item = oldModel;
                        item.Content = viewModel.Content;
                        await _TemplateRepository.UpdateAsync(item);
                    }

                    TempData["ShowSaved"] = true;
                    return RedirectToAction(nameof(Templates));
                }

                ModelState.Remove(nameof(viewModel.Action));
                if (!viewModel.GoBack)
                {
                    viewModel.Action = (eNotificationsTemplateAction) ((int) viewModel.Action + 1);
                }
            }

            return View("EditTemplate", viewModel);
        }

        [HttpGet("template/{id}/delete", Name = "DeleteTemplate")]
        public async Task<IActionResult> DeleteTemplateAsync(NotificationsTemplateViewModel viewModel)
        {
            var item = await _TemplateRepository.GetAsync(viewModel.Id);
            if (item == null) return NotFound();

            return View("ConfirmDeleteTemplate", new NotificationsTemplateViewModel
            {
                Id = item.RowKey,
                Content = item.Content
            });
        }

        [HttpGet("template/{id}/delete/confirm", Name = "DeleteTemplateConfirmed")]
        public async Task<IActionResult> DeleteTemplateConfirmedAsync(NotificationsTemplateViewModel viewModel)
        {
            var item = await _TemplateRepository.GetAsync(viewModel.Id);
            if (item == null) return NotFound();

            await _TemplateRepository.DeleteAsync(viewModel.Id);
            TempData["ShowSaved"] = true;
            return RedirectToAction(nameof(Templates));
        }

        [HttpGet("banners-partial")]
        [AllowAnonymous]
        public IActionResult BannersPartial()
        {
            var notificationBanners = _BannerRepository.GetNotificationBanners(2);
            var model = new NotificationsBannersViewModel(notificationBanners);
            return PartialView("_NotificationsBannersPartial", model);
        }
    }
}
