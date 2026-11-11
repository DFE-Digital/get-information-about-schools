using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Edubase.Common;
using Edubase.Data.Entity;
using Edubase.Data.Repositories;
using Edubase.Services.Texuna;
using Edubase.Web.UI.Helpers;
using Edubase.Web.UI.Models;
using Edubase.Web.UI.Models.Notifications;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Primitives;

namespace Edubase.Web.UI.Controllers
{
    [Route("Notifications")]
    public class NotificationsController : EduBaseController
    {
        private readonly INotificationBannerRepository _BannerRepository;
        private readonly INotificationTemplateRepository _TemplateRepository;

        public NotificationsController(
            INotificationTemplateRepository TemplateRepository,
            INotificationBannerRepository BannerRepository)
        {
            _TemplateRepository = TemplateRepository;
            _BannerRepository = BannerRepository;
        }

        [HttpGet("", Name = "Notifications")]
        [MvcAuthorizeRoles(AuthorizedRoles.IsAdmin)]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet("Banners")]
        [MvcAuthorizeRoles(AuthorizedRoles.IsAdmin)]
        public async Task<ActionResult> Banners()
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

        [HttpGet("Banners/Audit")]
        [MvcAuthorizeRoles(AuthorizedRoles.IsAdmin)]
        public async Task<ActionResult> AuditBanners(string sortBy)
        {
            var result = await _BannerRepository.GetAllAsync(1000);
            var audit = await _BannerRepository.GetAllAsync(1000, null, false, eNotificationBannerPartition.Archive);
            var items = result.Items.ToList();
            items.AddRange(audit.Items);

            var distinct = items.GroupBy(x => x.Tracker)
                .Select(grp => new { tracker = grp.Key, banners = grp.OrderByDescending(x => x.Version) })
                .Select(x => x.banners.First());

            var model = new NotificationsBannersAuditViewModel(distinct, sortBy);
            return View(model);
        }

        [HttpGet("Banners/Audit/{id}")]
        [MvcAuthorizeRoles(AuthorizedRoles.IsAdmin)]
        public async Task<ActionResult> AuditBanner(string id, string sortBy)
        {
            var result = await _BannerRepository.GetAllAsync(1000);
            var audit = await _BannerRepository.GetAllAsync(1000, null, false, eNotificationBannerPartition.Archive);
            var items = result.Items.ToList();
            items.AddRange(audit.Items);

            var distinct = items.Where(x => x.Tracker == id);

            var model = new NotificationsBannerAuditViewModel(distinct, sortBy);
            return View(model);
        }

        [HttpGet("Banner/New", Name = "CreateBanner")]
        [MvcAuthorizeRoles(AuthorizedRoles.IsAdmin)]
        public async Task<ActionResult> CreateBanner()
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



        [HttpPost("Banner/New", Name = "PostCreateBanner")]
        [MvcAuthorizeRoles(AuthorizedRoles.IsAdmin)]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateBannerAsync(NotificationsBannerViewModel viewModel)
        {
            return await ProcessEditBanner(viewModel);
        }

        [HttpGet("Banner/{counter}/{id}", Name = "EditBanner")]
        [MvcAuthorizeRoles(AuthorizedRoles.IsAdmin)]
        public async Task<ActionResult> EditBannerAsync(string id, int counter)
        {
            var item = await _BannerRepository.GetAsync(id);
            if (item == null)
            {
                return NotFound();
            }

            var banners = await _BannerRepository.GetAllAsync(1000, null, true);

            if (TempData["ShowSaved"] != null)
            {
                ViewBag.ShowSaved = true;
                TempData.Remove("ShowSaved");
            }

            var model = new NotificationsBannerViewModel().Set(item);

            model.Id = id;
            model.Counter = counter;
            model.Start = new DateTimeViewModel(item.Start, item.Start);
            model.StartOriginal = item.Start;
            model.End = new DateTimeViewModel(item.End, item.End);
            model.Importance = (eNotificationBannerImportance) item.Importance;

            // Content is already set in set() - Do not change or the user will see raw HTML
            //   model.Content = item.Content;
            model.TotalBanners = banners.Items.Count();
            model.TotalLiveBanners = banners.Items.Count(x => x.Visible);

            return View("EditBanner", model);
        }

        [HttpPost("Banner/{counter}/{id}", Name = "PostEditBanner")]
        [MvcAuthorizeRoles(AuthorizedRoles.IsAdmin)]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditBannerAsync(NotificationsBannerViewModel viewModel)
        {
            var item = await _BannerRepository.GetAsync(viewModel.Id);
            if (item == null)
            {
                return NotFound();
            }

            return await ProcessEditBanner(viewModel, item);
        }

        private async Task<ActionResult> ProcessEditBanner(NotificationsBannerViewModel viewModel, NotificationBanner originalBanner = null)
        {
            if (viewModel.GoBack)
            {
                viewModel.Action = (eNotificationBannerAction) ((int) viewModel.Action - 1);

                // if we're going back, we dont really care about any validation errors
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
                    // populate the templates, we need to do this the usual route through, and also if they have clicked the back button
                    var result = await _TemplateRepository.GetAllAsync(1000);
                    viewModel.Templates = result.Items;
                }

                if (viewModel.Action == eNotificationBannerAction.TypeChoice)
                {
                    if (!string.IsNullOrEmpty(viewModel.TemplateSelected))
                    {
                        // if one of the templates was selected, populate the content with the text from the template
                        var result = await _TemplateRepository.GetAsync(viewModel.TemplateSelected);
                        viewModel.Content = result.Content;
                        ModelState.Remove(nameof(viewModel.Content));
                    }
                }

                if (viewModel.Action == eNotificationBannerAction.Review)
                {
                    var updatedBanner = viewModel.ToBanner(originalBanner);

                    updatedBanner.AuditTimestamp = DateTime.Now;
                    updatedBanner.AuditUser = User.GetUserId();
                    updatedBanner.AuditEvent = string.IsNullOrEmpty(viewModel.Id)
                        ? eNotificationBannerEvent.Create.ToString()
                            : eNotificationBannerEvent.Update.ToString();

                    if (string.IsNullOrEmpty(viewModel.Id))
                    {
                        await _BannerRepository.CreateAsync(updatedBanner);
                    }
                    else
                    {
                        await _BannerRepository.UpdateAsync(updatedBanner);
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

            if (!ModelState.IsValid)
            {
                ModelState.SetModelValue(nameof(viewModel.LinkUrl1), new ValueProviderResult(new StringValues(viewModel.LinkUrl1 ?? string.Empty), CultureInfo.CurrentCulture));
                ModelState.SetModelValue(nameof(viewModel.LinkText1), new ValueProviderResult(new StringValues(viewModel.LinkText1 ?? string.Empty), CultureInfo.CurrentCulture));
                ModelState.SetModelValue(nameof(viewModel.LinkUrl2), new ValueProviderResult(new StringValues(viewModel.LinkUrl2 ?? string.Empty), CultureInfo.CurrentCulture));
                ModelState.SetModelValue(nameof(viewModel.LinkText2), new ValueProviderResult(new StringValues(viewModel.LinkText2 ?? string.Empty), CultureInfo.CurrentCulture));
            }

            return View("EditBanner", viewModel);
        }

        [HttpGet("Banner/{counter}/{id}/Delete", Name = "DeleteBanner")]
        [MvcAuthorizeRoles(AuthorizedRoles.IsAdmin)]
        public async Task<ActionResult> DeleteBannerAsync(NotificationsBannerViewModel viewModel)
        {
            var item = await _BannerRepository.GetAsync(viewModel.Id);
            if (item == null)
            {
                return NotFound();
            }

            return View("ConfirmDeleteBanner", viewModel.Set(item));
        }

        [HttpGet("Banner/{counter}/{id}/Delete/Confirm", Name = "DeleteBannerConfirmed")]
        [MvcAuthorizeRoles(AuthorizedRoles.IsAdmin)]
        public async Task<ActionResult> DeleteBannerConfirmedAsync(NotificationsBannerViewModel viewModel)
        {
            var item = await _BannerRepository.GetAsync(viewModel.Id);
            if (item == null)
            {
                return NotFound();
            }

            await _BannerRepository.DeleteAsync(viewModel.Id, User.GetUserId());
            TempData["ShowSaved"] = true;
            return RedirectToAction(nameof(Banners));
        }


        [HttpGet("Templates")]
        [MvcAuthorizeRoles(AuthorizedRoles.IsAdmin)]
        public async Task<ActionResult> Templates()
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

        [HttpGet("Template/New", Name = "CreateTemplate")]
        [MvcAuthorizeRoles(AuthorizedRoles.IsAdmin)]
        public ActionResult CreateTemplate()
        {
            return View("EditTemplate", new NotificationsTemplateViewModel());
        }


        [HttpPost("Template/New", Name = "PostCreateTemplate")]
        [MvcAuthorizeRoles(AuthorizedRoles.IsAdmin)]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateTemplateAsync(NotificationsTemplateViewModel viewModel)
        {
            return await ProcessEditTemplate(viewModel);
        }


        [HttpGet("Template/{id}", Name = "EditTemplate")]
        [MvcAuthorizeRoles(AuthorizedRoles.IsAdmin)]
        public async Task<ActionResult> EditTemplateAsync(string id)
        {
            var item = await _TemplateRepository.GetAsync(id);
            if (item == null)
            {
                return NotFound();
            }

            return View("EditTemplate", new NotificationsTemplateViewModel { Id = id, Content = item.Content, OriginalContent = item.Content });
        }

        [HttpPost("Template/{id}", Name = "PostEditTemplate")]
        [MvcAuthorizeRoles(AuthorizedRoles.IsAdmin)]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditTemplateAsync(NotificationsTemplateViewModel viewModel)
        {
            var item = await _TemplateRepository.GetAsync(viewModel.Id);
            if (item == null)
            {
                return NotFound();
            }

            return await ProcessEditTemplate(viewModel, item);
        }

        private async Task<ActionResult> ProcessEditTemplate(NotificationsTemplateViewModel viewModel, NotificationTemplate oldModel = null)
        {
            if (viewModel.GoBack)
            {
                viewModel.Action = (eNotificationsTemplateAction) ((int) viewModel.Action - 1);

                // if we're going back, we dont really care about any validation errors
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


        [HttpGet("Template/{id}/Delete", Name = "DeleteTemplate")]
        [MvcAuthorizeRoles(AuthorizedRoles.IsAdmin)]
        public async Task<ActionResult> DeleteTemplateAsync(NotificationsTemplateViewModel viewModel)
        {
            var item = await _TemplateRepository.GetAsync(viewModel.Id);
            if (item == null)
            {
                return NotFound();
            }

            return View("ConfirmDeleteTemplate", new NotificationsTemplateViewModel { Id = item.RowKey, Content = item.Content });
        }

        [HttpGet("Template/{id}/Delete/Confirm", Name = "DeleteTemplateConfirmed")]
        [MvcAuthorizeRoles(AuthorizedRoles.IsAdmin)]
        public async Task<ActionResult> DeleteTemplateConfirmedAsync(NotificationsTemplateViewModel viewModel)
        {
            var item = await _TemplateRepository.GetAsync(viewModel.Id);
            if (item == null)
            {
                return NotFound();
            }

            await _TemplateRepository.DeleteAsync(viewModel.Id);
            TempData["ShowSaved"] = true;
            return RedirectToAction(nameof(Templates));
        }

        [HttpGet("BannersPartial")]
        public ActionResult BannersPartial()
        {
            var notificationBanners = _BannerRepository.GetNotificationBanners(2);
            var model = new NotificationsBannersViewModel(notificationBanners);
            return PartialView("_NotificationsBannersPartial", model);
        }
    }
}
