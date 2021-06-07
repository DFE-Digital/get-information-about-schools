using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.UI;
using Edubase.Common;
using Edubase.Data.Entity;
using Edubase.Data.Repositories;
using Edubase.Web.UI.Filters;
using Edubase.Web.UI.Helpers;
using Edubase.Web.UI.Models;
using Edubase.Web.UI.Models.Notifications;

namespace Edubase.Web.UI.Controllers
{
    [RoutePrefix("Notifications"), Route("{action=index}")]
    public class NotificationsController : EduBaseController
    {
        private readonly NotificationTemplateRepository _TemplateRepository;
        private readonly NotificationBannerRepository _BannerRepository;

        public NotificationsController(NotificationTemplateRepository TemplateRepository, NotificationBannerRepository BannerRepository)
        {
            _TemplateRepository = TemplateRepository;
            _BannerRepository = BannerRepository;
        }

        [Route(Name = "Notifications"), EdubaseAuthorize(Roles = AuthorizedRoles.IsAdmin)]
        public ActionResult Index()
        {
            return View();
        }

        [Route("Banners"), EdubaseAuthorize(Roles = AuthorizedRoles.IsAdmin)]
        public async Task<ActionResult> Banners()
        {
            var result = await _BannerRepository.GetAllAsync(2);

            var model = new NotificationsBannersViewModel(result.Items);
            return View(model);
        }


        [Route("Banner/New", Name = "CreateBanner"), HttpGet, EdubaseAuthorize(Roles = AuthorizedRoles.IsAdmin)]
        public async Task<ActionResult> CreateBanner()
        {
            var banners = await _BannerRepository.GetAllAsync(1000);
            var newBanner = new NotificationsBannerViewModel
            {
                TotalBanners = banners.Items.Count()
            };
            return View("EditBanner", newBanner);
        } 


        [Route("Banner/New", Name = "PostCreateBanner"), HttpPost, EdubaseAuthorize(Roles = AuthorizedRoles.IsAdmin)]
        public async Task<ActionResult> CreateBannerAsync(NotificationsBannerViewModel viewModel)
        {
            return await ProcessEditBanner(viewModel);
        }

        [Route("Banner/{id}", Name = "EditBanner"), HttpGet, EdubaseAuthorize(Roles = AuthorizedRoles.IsAdmin)]
        public async Task<ActionResult> EditBannerAsync(string id)
        {
            var item = await _BannerRepository.GetAsync(id);
            if (item == null) return HttpNotFound();

            var banners = await _BannerRepository.GetAllAsync(1000);

            return View("EditBanner", new NotificationsBannerViewModel
            {
                Id = id,
                Start = new DateTimeViewModel(item.Start, item.Start),
                End = new DateTimeViewModel(item.End, item.End),
                Visible = item.Visible,
                Importance = (eNotificationBannerImportance)item.Importance,
                Content = item.Content,
                TotalBanners = banners.Items.Count()
            });
        }

        [Route("Banner/{id}", Name = "PostEditBanner"), HttpPost, EdubaseAuthorize(Roles = AuthorizedRoles.IsAdmin)]
        public async Task<ActionResult> EditBannerAsync(NotificationsBannerViewModel viewModel)
        {
            var item = await _BannerRepository.GetAsync(viewModel.Id);
            if (item == null) return HttpNotFound();

            return await ProcessEditBanner(viewModel, item);
        }

        private async Task<ActionResult> ProcessEditBanner(NotificationsBannerViewModel viewModel, NotificationBanner originalBanner = null)
        {
            if (ModelState.IsValid)
            {
                if (viewModel.Action == eNotificationBannerAction.Step2)
                {
                    // skip straight to the final review if it's being switched off - but only for an existing banner.
                    if (viewModel.Visible == false && !string.IsNullOrEmpty(viewModel.Id))
                    {
                        ModelState.Remove(nameof(viewModel.Action));
                        viewModel.Action = eNotificationBannerAction.Step6;
                        return View("EditBanner", viewModel);
                    }

                    // populate the templates
                    var result = await _TemplateRepository.GetAllAsync(1000);
                    viewModel.Templates = result.Items;
                }

                if (viewModel.Action == eNotificationBannerAction.Step3)
                {
                    if (!string.IsNullOrEmpty(viewModel.TemplateSelected))
                    {
                        // if one of the templates was selected, populate the content with the text from the template
                        var result = await _TemplateRepository.GetAsync(viewModel.TemplateSelected);
                        viewModel.Content = result.Content;
                        ModelState.Remove(nameof(viewModel.Content));
                    }
                }

                if (viewModel.Action == eNotificationBannerAction.Step6)
                {
                    if (string.IsNullOrEmpty(viewModel.Id))
                    {
                        var item = viewModel.ToBanner();
                        await _BannerRepository.CreateAsync(item);
                    }
                    else
                    {
                        var item = viewModel.ToBanner(originalBanner);
                        await _BannerRepository.UpdateAsync(item);
                    }
                    return RedirectToAction(nameof(Banners));
                }

                ModelState.Remove(nameof(viewModel.Action));
                viewModel.Action = (eNotificationBannerAction) ((int) viewModel.Action + 1);
            }

            return View("EditBanner", viewModel);
        }


        [Route("Banner/{id}/Delete", Name = "DeleteBanner"), HttpGet, EdubaseAuthorize(Roles = AuthorizedRoles.IsAdmin)]
        public async Task<ActionResult> DeleteBannerAsync(NotificationsBannerViewModel viewModel)
        {
            var item = await _BannerRepository.GetAsync(viewModel.Id);
            if (item == null) return HttpNotFound();
            return View("ConfirmDeleteBanner", new NotificationsBannerViewModel(item));
        }

        [Route("Banner/{id}/Delete/Confirm", Name = "DeleteBannerConfirmed"), HttpGet, EdubaseAuthorize(Roles = AuthorizedRoles.IsAdmin)]
        public async Task<ActionResult> DeleteBannerConfirmedAsync(NotificationsBannerViewModel viewModel)
        {
            var item = await _BannerRepository.GetAsync(viewModel.Id);
            if (item == null)
            {
                return HttpNotFound();
            }

            await _BannerRepository.DeleteAsync(viewModel.Id);
            return RedirectToAction(nameof(Banners));
        }




        [Route("Templates"), EdubaseAuthorize(Roles = AuthorizedRoles.IsAdmin)]
        public async Task<ActionResult> Templates()
        {
            var result = await _TemplateRepository.GetAllAsync(1000);

            var model = new NotificationsTemplatesViewModel(result.Items);
            return View(model);
        }

        [Route("Template/New", Name = "CreateTemplate"), HttpGet, EdubaseAuthorize(Roles = AuthorizedRoles.IsAdmin)]
        public ActionResult CreateTemplate() => View("EditTemplate", new NotificationsTemplateViewModel());


        [Route("Template/New", Name = "PostCreateTemplate"), HttpPost, EdubaseAuthorize(Roles = AuthorizedRoles.IsAdmin)]
        public async Task<ActionResult> CreateTemplateAsync(NotificationsTemplateViewModel viewModel)
        {
            return await ProcessEditTemplate(viewModel);
        }


        [Route("Template/{id}", Name= "EditTemplate"), HttpGet, EdubaseAuthorize(Roles = AuthorizedRoles.IsAdmin)]
        public async Task<ActionResult> EditTemplateAsync(string id)
        {
            var item = await _TemplateRepository.GetAsync(id);
            if (item == null) return HttpNotFound();

            return View("EditTemplate", new NotificationsTemplateViewModel
            {
                Id = id,
                Content = item.Content,
                OriginalContent = item.Content
            });
        }

        [Route("Template/{id}", Name = "PostEditTemplate"), HttpPost, EdubaseAuthorize(Roles = AuthorizedRoles.IsAdmin)]
        public async Task<ActionResult> EditTemplateAsync(NotificationsTemplateViewModel viewModel)
        {
            var item = await _TemplateRepository.GetAsync(viewModel.Id);
            if (item == null)
            {
                return HttpNotFound();
            }

            return await ProcessEditTemplate(viewModel, item);
        }

        private async Task<ActionResult> ProcessEditTemplate(NotificationsTemplateViewModel viewModel, NotificationTemplate oldModel = null)
        {
            if (ModelState.IsValid)
            {
                if (viewModel.Action == eNotificationsTemplateAction.Step2)
                {
                    if (string.IsNullOrEmpty(viewModel.Id))
                    {
                        var item = new NotificationTemplate()
                        {
                            Content = viewModel.Content,
                        };
                        await _TemplateRepository.CreateAsync(item);
                    }
                    else
                    {
                        var item = oldModel;
                        item.Content = viewModel.Content;
                        await _TemplateRepository.UpdateAsync(item);
                    }
                    return RedirectToAction(nameof(Templates));
                }

                ModelState.Remove(nameof(viewModel.Action));
                viewModel.Action = (eNotificationsTemplateAction) ((int) viewModel.Action + 1);
            }

            return View("EditTemplate", viewModel);
        }


        [Route("Template/{id}/Delete", Name = "DeleteTemplate"), HttpGet, EdubaseAuthorize(Roles = AuthorizedRoles.IsAdmin)]
        public async Task<ActionResult> DeleteTemplateAsync(NotificationsTemplateViewModel viewModel)
        {
            var item = await _TemplateRepository.GetAsync(viewModel.Id);
            if (item == null)
            {
                return HttpNotFound();
            }
            return View("ConfirmDeleteTemplate", new NotificationsTemplateViewModel {Id = item.RowKey, Content = item.Content});
        }
        
        [Route("Template/{id}/Delete/Confirm", Name = "DeleteTemplateConfirmed"), HttpGet, EdubaseAuthorize(Roles = AuthorizedRoles.IsAdmin)]
        public async Task<ActionResult> DeleteTemplateConfirmedAsync(NotificationsTemplateViewModel viewModel)
        {
            var item = await _TemplateRepository.GetAsync(viewModel.Id);
            if (item == null)
            {
                return HttpNotFound();
            }

            await _TemplateRepository.DeleteAsync(viewModel.Id);
            return RedirectToAction(nameof(Templates));
        }


        [HttpGet, Route("BannersPartial")]
        public ActionResult BannersPartial()
        {
            var result = Task.Run(() => _BannerRepository.GetAllAsync(1000).Result.Items.Where(x => x.Visible == true && x.Start < DateTime.Now && x.End > DateTime.Now));
            var model = new NotificationsBannersViewModel(result.Result);
            return PartialView("_NotificationsBannersPartial", model);
        }
    }
}
