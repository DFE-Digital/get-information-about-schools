using System.Threading.Tasks;
using Edubase.Data.Entity;
using Edubase.Data.Repositories;
using Edubase.Web.UI.Helpers;
using Edubase.Web.UI.Models;
using Microsoft.AspNetCore.Mvc;

namespace Edubase.Web.UI.Controllers
{
    [Route("glossary")]
    public class GlossaryController : Controller
    {
        private readonly IGlossaryRepository _glossaryRepository;

        public GlossaryController(IGlossaryRepository glossaryRepository)
        {
            _glossaryRepository = glossaryRepository;
        }

        [HttpGet("", Name = "Glossary")]
        [HttpGet("Index")]
        public async Task<ActionResult> Index()
        {
            var result = await _glossaryRepository.GetAllAsync(1000);
            return View(new GlossaryViewModel(result.Items) { UserCanEdit = User.IsInRole(AuthorizedRoles.IsAdmin) });
        }

        [HttpGet("Create", Name = "CreateGlossaryItem"), MvcAuthorizeRoles(AuthorizedRoles.IsAdmin)]
        public ActionResult Create() => View("CreateEdit", new GlossaryItemViewModel());

        [HttpGet("Edit/{id}", Name = "EditGlossaryItem"), MvcAuthorizeRoles(AuthorizedRoles.IsAdmin)]
        public async Task<ActionResult> EditAsync(string id)
        {
            var item = await _glossaryRepository.GetAsync(id);
            if (item == null) return NotFound();

            return View("CreateEdit", new GlossaryItemViewModel
            {
                Id = id,
                Content = item.Content,
                Title = item.Title
            });
        }

        [HttpPost("Edit/{id}", Name = "PostEditGlossaryItem"), MvcAuthorizeRoles(AuthorizedRoles.IsAdmin), ValidateAntiForgeryToken]
        public async Task<ActionResult> EditAsync(GlossaryItemViewModel viewModel)
        {
            var item = await _glossaryRepository.GetAsync(viewModel.Id);
            if (item == null) return NotFound();

            if (viewModel.IsDeleting)
            {
                await _glossaryRepository.DeleteAsync(viewModel.Id);
                return RedirectToAction(nameof(Index));
            }

            if (ModelState.IsValid)
            {
                item.Title = viewModel.Title;
                item.Content = viewModel.Content;
                await _glossaryRepository.UpdateAsync(item);
                return RedirectToAction(nameof(Index));
            }

            return View("CreateEdit", viewModel);
        }

        [HttpPost("Create", Name = "PostCreateGlossaryItem"), MvcAuthorizeRoles(AuthorizedRoles.IsAdmin), ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateAsync(GlossaryItemViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var item = new GlossaryItem
                {
                    Title = viewModel.Title,
                    Content = viewModel.Content
                };

                await _glossaryRepository.CreateAsync(item);
                return RedirectToAction(nameof(Index));
            }

            return View("CreateEdit", viewModel);
        }
    }
}
