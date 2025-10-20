using System.Threading.Tasks;
using Edubase.Data.Entity;
using Edubase.Data.Repositories;
using Edubase.Web.UI.Helpers;
using Edubase.Web.UI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Edubase.Web.UI.Controllers
{
    [ApiController]
    [Route("glossary")]
    public class GlossaryController : Controller
    {
        private readonly GlossaryRepository _glossaryRepository;

        public GlossaryController(GlossaryRepository glossaryRepository)
        {
            _glossaryRepository = glossaryRepository;
        }

        [HttpGet("", Name = "Glossary")]
        public async Task<IActionResult> Index()
        {
            var result = await _glossaryRepository.GetAllAsync(1000);
            return View(new GlossaryViewModel(result.Items)
            {
                UserCanEdit = User.IsInRole(AuthorizedRoles.IsAdmin)
            });
        }

        [HttpGet("create", Name = "CreateGlossaryItem")]
        [Authorize(Roles = AuthorizedRoles.IsAdmin)]
        public IActionResult Create() => View("CreateEdit", new GlossaryItemViewModel());

        [HttpGet("edit/{id}", Name = "EditGlossaryItem")]
        [Authorize(Roles = AuthorizedRoles.IsAdmin)]
        public async Task<IActionResult> EditAsync(string id)
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

        [HttpPost("edit/{id}", Name = "PostEditGlossaryItem")]
        [Authorize(Roles = AuthorizedRoles.IsAdmin)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAsync(GlossaryItemViewModel viewModel)
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

        [HttpPost("create", Name = "PostCreateGlossaryItem")]
        [Authorize(Roles = AuthorizedRoles.IsAdmin)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAsync(GlossaryItemViewModel viewModel)
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
