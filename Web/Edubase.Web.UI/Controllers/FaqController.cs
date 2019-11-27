using Edubase.Data.Entity;
using Edubase.Data.Repositories;
using Edubase.Services.Security;
using Edubase.Web.UI.Filters;
using Edubase.Web.UI.Models;
using System.Threading.Tasks;
using System.Web.Mvc;
using Edubase.Web.UI.Helpers;

namespace Edubase.Web.UI.Controllers
{
    [RoutePrefix("Faq"), Route("{action=index}")]
    public class FaqController : Controller
    {
        private readonly FaqRepository _FaqRepository;

        public FaqController(FaqRepository FaqRepository)
        {
            _FaqRepository = FaqRepository;
        }

        [Route(Name = "Faq")]
        public async Task<ActionResult> Index()
        {
            var result = await _FaqRepository.GetAllAsync(1000);
            return View(new FaqViewModel(result.Items) { UserCanEdit = User.IsInRole(AuthorizedRoles.CanEdit) });
        }

        [Route("Create", Name = "CreateFaqItem"), HttpGet, EdubaseAuthorize(Roles = AuthorizedRoles.CanEdit)]
        public ActionResult Create() => View("CreateEdit", new FaqItemViewModel());
        
        [Route("Edit/{id}", Name = "EditFaqItem"), HttpGet, EdubaseAuthorize(Roles = AuthorizedRoles.CanEdit)]
        public async Task<ActionResult> EditAsync(string id)
        {
            var item = await _FaqRepository.GetAsync(id);
            if (item == null) return HttpNotFound();
            else return View("CreateEdit", new FaqItemViewModel
            {
                Id = id,
                Content = item.Content,
                Title = item.Title,
                DisplayOrder = item.DisplayOrder,
                TitleFontSize = item.TitleFontSize
            });
        }

        [Route("Edit/{id}", Name = "PostEditFaqItem"), HttpPost, EdubaseAuthorize(Roles = AuthorizedRoles.CanEdit)]
        public async Task<ActionResult> EditAsync(FaqItemViewModel viewModel)
        {
            var item = await _FaqRepository.GetAsync(viewModel.Id);
            if (item == null) return HttpNotFound();

            if (viewModel.IsDeleting)
            {
                await _FaqRepository.DeleteAsync(viewModel.Id);
                return RedirectToAction(nameof(Index));
            }

            if (ModelState.IsValid)
            {
                item.Title = viewModel.Title;
                item.Content = viewModel.Content;
                item.DisplayOrder = viewModel.DisplayOrder;
                item.TitleFontSize = viewModel.TitleFontSize;
                await _FaqRepository.UpdateAsync(item);
                return RedirectToAction(nameof(Index));
            }
            else return View("CreateEdit", viewModel);
        }

        [Route("Create", Name = "PostCreateFaqItem"), HttpPost, EdubaseAuthorize(Roles = AuthorizedRoles.CanEdit)]
        public async Task<ActionResult> CreateAsync(FaqItemViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var item = new FaqItem
                {
                    Title = viewModel.Title,
                    Content = viewModel.Content,
                    DisplayOrder = viewModel.DisplayOrder,
                    TitleFontSize = viewModel.TitleFontSize
                };
                await _FaqRepository.CreateAsync(item);
                return RedirectToAction(nameof(Index));
            }
            else return View("CreateEdit", viewModel);
        }

    }
}
