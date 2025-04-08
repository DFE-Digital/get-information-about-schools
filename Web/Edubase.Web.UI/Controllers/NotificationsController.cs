using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Edubase.Common;
using Edubase.Data.Entity;
using Edubase.Data.Repositories;
using Edubase.Services.Texuna;
using Edubase.Web.UI.Filters;
using Edubase.Web.UI.Helpers;
using Edubase.Web.UI.Models;
using Edubase.Web.UI.Models.Notifications;

namespace Edubase.Web.UI.Controllers
{
    [RoutePrefix("Notifications")]
    [Route("{action=index}")]
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

        [Route(Name = "Notifications")]
        [EdubaseAuthorize(Roles = AuthorizedRoles.IsAdmin)]
        public ActionResult Index()
        {
            return View();
        }

        [Route("Banners")]
        [EdubaseAuthorize(Roles = AuthorizedRoles.IsAdmin)]
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

        [Route("Banners/Audit")]
        [EdubaseAuthorize(Roles = AuthorizedRoles.IsAdmin)]
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

        [Route("Banners/Audit/{id}")]
        [EdubaseAuthorize(Roles = AuthorizedRoles.IsAdmin)]
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

        [Route("Banner/New", Name = "CreateBanner")]
        [HttpGet]
        [EdubaseAuthorize(Roles = AuthorizedRoles.IsAdmin)]
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



        [Route("Banner/New", Name = "PostCreateBanner"), HttpPost, EdubaseAuthorize(Roles = AuthorizedRoles.IsAdmin), ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateBannerAsync(NotificationsBannerViewModel viewModel)
        {
            return await ProcessEditBanner(viewModel);
        }

        [Route("Banner/{counter}/{id}", Name = "EditBanner")]
        [HttpGet]
        [EdubaseAuthorize(Roles = AuthorizedRoles.IsAdmin)]
        public async Task<ActionResult> EditBannerAsync(string id, int counter)
        {
            var item = await _BannerRepository.GetAsync(id);
            if (item == null)
            {
                return HttpNotFound();
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

        [Route("Banner/{counter}/{id}", Name = "PostEditBanner"), HttpPost, EdubaseAuthorize(Roles = AuthorizedRoles.IsAdmin), ValidateAntiForgeryToken]
        public async Task<ActionResult> EditBannerAsync(NotificationsBannerViewModel viewModel)
        {
            var item = await _BannerRepository.GetAsync(viewModel.Id);
            if (item == null)
            {
                return HttpNotFound();
            }

            return await ProcessEditBanner(viewModel, item);
        }

        private async Task<ActionResult> ProcessEditBanner(NotificationsBannerViewModel viewModel,
            NotificationBanner originalBanner = null)
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
                ModelState.SetModelValue(nameof(viewModel.LinkUrl1), new ValueProviderResult(viewModel.LinkUrl1, viewModel.LinkUrl1, CultureInfo.CurrentCulture));
                ModelState.SetModelValue(nameof(viewModel.LinkText1), new ValueProviderResult(viewModel.LinkText1, viewModel.LinkText1, CultureInfo.CurrentCulture));
                ModelState.SetModelValue(nameof(viewModel.LinkUrl2), new ValueProviderResult(viewModel.LinkUrl2, viewModel.LinkUrl2, CultureInfo.CurrentCulture));
                ModelState.SetModelValue(nameof(viewModel.LinkText2), new ValueProviderResult(viewModel.LinkText2, viewModel.LinkText2, CultureInfo.CurrentCulture));
            }

            return View("EditBanner", viewModel);
        }


        [Route("Banner/{counter}/{id}/Delete", Name = "DeleteBanner")]
        [HttpGet]
        [EdubaseAuthorize(Roles = AuthorizedRoles.IsAdmin)]
        public async Task<ActionResult> DeleteBannerAsync(NotificationsBannerViewModel viewModel)
        {
            var item = await _BannerRepository.GetAsync(viewModel.Id);
            if (item == null)
            {
                return HttpNotFound();
            }

            return View("ConfirmDeleteBanner", viewModel.Set(item));
        }

        [Route("Banner/{counter}/{id}/Delete/Confirm", Name = "DeleteBannerConfirmed")]
        [HttpGet]
        [EdubaseAuthorize(Roles = AuthorizedRoles.IsAdmin)]
        public async Task<ActionResult> DeleteBannerConfirmedAsync(NotificationsBannerViewModel viewModel)
        {
            var item = await _BannerRepository.GetAsync(viewModel.Id);
            if (item == null)
            {
                return HttpNotFound();
            }

            await _BannerRepository.DeleteAsync(viewModel.Id, User.GetUserId());
            TempData["ShowSaved"] = true;
            return RedirectToAction(nameof(Banners));
        }


        [Route("Templates")]
        [EdubaseAuthorize(Roles = AuthorizedRoles.IsAdmin)]
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

        [Route("Template/New", Name = "CreateTemplate")]
        [HttpGet]
        [EdubaseAuthorize(Roles = AuthorizedRoles.IsAdmin)]
        public ActionResult CreateTemplate()
        {
            return View("EditTemplate", new NotificationsTemplateViewModel());
        }


        [Route("Template/New", Name = "PostCreateTemplate"), HttpPost, EdubaseAuthorize(Roles = AuthorizedRoles.IsAdmin), ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateTemplateAsync(NotificationsTemplateViewModel viewModel)
        {
            return await ProcessEditTemplate(viewModel);
        }


        [Route("Template/{id}", Name = "EditTemplate")]
        [HttpGet]
        [EdubaseAuthorize(Roles = AuthorizedRoles.IsAdmin)]
        public async Task<ActionResult> EditTemplateAsync(string id)
        {
            var item = await _TemplateRepository.GetAsync(id);
            if (item == null)
            {
                return HttpNotFound();
            }

            return View("EditTemplate",
                new NotificationsTemplateViewModel { Id = id, Content = item.Content, OriginalContent = item.Content });
        }

        [Route("Template/{id}", Name = "PostEditTemplate"), HttpPost, EdubaseAuthorize(Roles = AuthorizedRoles.IsAdmin), ValidateAntiForgeryToken]
        public async Task<ActionResult> EditTemplateAsync(NotificationsTemplateViewModel viewModel)
        {
            var item = await _TemplateRepository.GetAsync(viewModel.Id);
            if (item == null)
            {
                return HttpNotFound();
            }

            return await ProcessEditTemplate(viewModel, item);
        }

        private async Task<ActionResult> ProcessEditTemplate(NotificationsTemplateViewModel viewModel,
            NotificationTemplate oldModel = null)
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


        [Route("Template/{id}/Delete", Name = "DeleteTemplate")]
        [HttpGet]
        [EdubaseAuthorize(Roles = AuthorizedRoles.IsAdmin)]
        public async Task<ActionResult> DeleteTemplateAsync(NotificationsTemplateViewModel viewModel)
        {
            var item = await _TemplateRepository.GetAsync(viewModel.Id);
            if (item == null)
            {
                return HttpNotFound();
            }

            return View("ConfirmDeleteTemplate",
                new NotificationsTemplateViewModel { Id = item.RowKey, Content = item.Content });
        }

        [Route("Template/{id}/Delete/Confirm", Name = "DeleteTemplateConfirmed")]
        [HttpGet]
        [EdubaseAuthorize(Roles = AuthorizedRoles.IsAdmin)]
        public async Task<ActionResult> DeleteTemplateConfirmedAsync(NotificationsTemplateViewModel viewModel)
        {
            var item = await _TemplateRepository.GetAsync(viewModel.Id);
            if (item == null)
            {
                return HttpNotFound();
            }

            await _TemplateRepository.DeleteAsync(viewModel.Id);
            TempData["ShowSaved"] = true;
            return RedirectToAction(nameof(Templates));
        }

        [Route("BannersPartial")]
        public ActionResult BannersPartial()
        {
            var notificationBanners =
                _BannerRepository.GetNotificationBanners(2);
            var model = new NotificationsBannersViewModel(notificationBanners);
            return PartialView("_NotificationsBannersPartial", model);
        }
    }
}
