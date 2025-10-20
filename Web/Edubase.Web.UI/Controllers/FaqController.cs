using System.Linq;
using System.Threading.Tasks;
using Edubase.Data.Entity;
using Edubase.Data.Repositories;
using Edubase.Web.UI.Filters;
using Edubase.Web.UI.Helpers;
using Edubase.Web.UI.Models;
using Edubase.Web.UI.Models.Faq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Edubase.Web.UI.Controllers
{
    [ApiController]
    [Route("faq")]
    public class FaqController : Controller
    {
        private readonly FaqItemRepository _FaqItemRepository;
        private readonly FaqGroupRepository _FaqGroupRepository;

        public FaqController(FaqItemRepository FaqItemRepository, FaqGroupRepository FaqGroupRepository)
        {
            _FaqItemRepository = FaqItemRepository;
            _FaqGroupRepository = FaqGroupRepository;
        }

        [HttpGet("faq", Name = "Faq")]
        public async Task<ActionResult> Index()
        {
            var faqs = await _FaqItemRepository.GetAllAsync(1000);
            var groups = await _FaqGroupRepository.GetAllAsync(1000);
            return View(new FaqViewModel(faqs.Items, groups.Items) { UserCanEdit = User.IsInRole(AuthorizedRoles.IsAdmin) });
        }

        [Route("Create", Name = "CreateItem"), HttpGet, Authorize(Roles = AuthorizedRoles.IsAdmin)]
        public async Task<ActionResult> Create()
        {
            var groups = await _FaqGroupRepository.GetAllAsync(1000);
            return View("CreateEdit", new FaqItemViewModel(groups.Items));
        }

        [Route("Create", Name = "PostCreateItem"), HttpPost, Authorize(Roles = AuthorizedRoles.IsAdmin), ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateAsync(FaqItemViewModel viewModel)
        {
            return await ProcessEditItem(viewModel);
        }

        [Route("Edit/{id}", Name = "EditItem"), HttpGet, Authorize(Roles = AuthorizedRoles.IsAdmin)]
        public async Task<ActionResult> EditAsync(string id)
        {
            var item = await _FaqItemRepository.GetAsync(id);
            var groups = await _FaqGroupRepository.GetAllAsync(1000);
            if (item == null) return NotFound();
            else return View("CreateEdit", new FaqItemViewModel
            {
                Id = id,
                Content = item.Content,
                Title = item.Title,
                GroupId = item.GroupId,
                Groups = groups.Items
            });
        }

        [Route("Edit/{id}", Name = "PostEditItem"), HttpPost, Authorize(Roles = AuthorizedRoles.IsAdmin), ValidateAntiForgeryToken]
        public async Task<ActionResult> EditAsync(FaqItemViewModel viewModel)
        {
            var item = await _FaqItemRepository.GetAsync(viewModel.Id);
            if (item == null) return NotFound();

            if (viewModel.IsDeleting)
            {
                await _FaqItemRepository.DeleteAsync(viewModel.Id);
                await ReorderList(item.GroupId, item.DisplayOrder);
                return RedirectToAction(nameof(Index));
            }

            return await ProcessEditItem(viewModel, item);
        }

        [Route("Edit/{id}/{order}", Name = "EditItemOrder"), HttpGet, Authorize(Roles = AuthorizedRoles.IsAdmin)]
        public async Task<ActionResult> EditItemOrderAsync(string id, string order)
        {
            var item = await _FaqItemRepository.GetAsync(id);
            if (item == null) return NotFound();

            var newOrder = item.DisplayOrder + (order == "down" ? 1 : -1);
            var list = await _FaqItemRepository.GetAllAsync(1000);
            var swap = list.Items.FirstOrDefault(x => x.GroupId == item.GroupId && x.DisplayOrder == newOrder);
            if (swap == null) return NotFound();

            swap.DisplayOrder = item.DisplayOrder;
            await _FaqItemRepository.UpdateAsync(swap);

            item.DisplayOrder = newOrder;
            await _FaqItemRepository.UpdateAsync(item);

            return RedirectToAction(nameof(Index));
        }


        private async Task<ActionResult> ProcessEditItem(FaqItemViewModel viewModel, FaqItem oldModel = null)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(viewModel.Id))
                {
                    var item = new FaqItem()
                    {
                        Title = viewModel.Title,
                        Content = viewModel.Content.Trim(),
                        GroupId = viewModel.GroupId,
                        DisplayOrder = (await _FaqItemRepository.GetAllAsync(1000)).Items.Count(x => x.GroupId == viewModel.GroupId) + 1
                    };
                    await _FaqItemRepository.CreateAsync(item);
                }
                else
                {
                    var item = oldModel.ShallowCopy();
                    item.Title = viewModel.Title;
                    item.Content = viewModel.Content.Trim();
                    item.GroupId = viewModel.GroupId;

                    if (oldModel.GroupId != viewModel.GroupId)
                    {
                        item.DisplayOrder =
                            (await _FaqItemRepository.GetAllAsync(1000)).Items.Count(x => x.GroupId == viewModel.GroupId) + 1;
                    }

                    await _FaqItemRepository.UpdateAsync(item);

                    if (oldModel.GroupId != viewModel.GroupId)
                    {
                        await ReorderList(oldModel.GroupId, oldModel.DisplayOrder);
                    }
                }
                TempData["ShowSaved"] = true;
                return RedirectToAction(nameof(Index));
            }

            var groups = await _FaqGroupRepository.GetAllAsync(1000);
            viewModel.Groups = groups.Items;
            return View("CreateEdit", viewModel);
        }

        private async Task<bool> ReorderList(string groupId, int reorderFrom)
        {
            var allItems = (await _FaqItemRepository.GetAllAsync(1000)).Items;

            if (allItems.Any(x => x.GroupId == groupId))
            {
                // reorder the old list
                foreach (var faqItem in allItems.Where(x => x.GroupId == groupId && x.DisplayOrder > reorderFrom).OrderBy(x => x.DisplayOrder))
                {
                    faqItem.DisplayOrder -= 1;
                    await _FaqItemRepository.UpdateAsync(faqItem);
                }
            }

            return true;
        }


        [Route("Groups"), EdubaseAuthorize(Roles = AuthorizedRoles.IsAdmin)]
        public async Task<ActionResult> Groups()
        {
            var result = await _FaqGroupRepository.GetAllAsync(1000);

            var model = new FaqGroupsViewModel(result.Items);

            if (TempData["ShowSaved"] != null)
            {
                ViewBag.ShowSaved = true;
                TempData.Remove("ShowSaved");
            }
            return View(model);
        }


        [Route("Groups/New", Name = "CreateGroup"), HttpGet, Authorize(Roles = AuthorizedRoles.IsAdmin)]
        public ActionResult CreateGroup() => View("EditGroup", new FaqGroupViewModel());


        [Route("Groups/New", Name = "PostCreateGroup"), HttpPost, Authorize(Roles = AuthorizedRoles.IsAdmin), ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateGroupAsync(FaqGroupViewModel viewModel)
        {
            return await ProcessEditGroup(viewModel);
        }

        [Route("Group/{id}", Name = "EditGroup"), HttpGet, Authorize(Roles = AuthorizedRoles.IsAdmin)]
        public async Task<ActionResult> EditGroupAsync(string id)
        {
            var item = await _FaqGroupRepository.GetAsync(id);
            if (item == null) return NotFound();
            var empty = (await _FaqItemRepository.GetAllAsync(1000)).Items.All(x => x.GroupId != item.RowKey);

            return View("EditGroup", new FaqGroupViewModel
            {
                Id = id,
                DisplayOrder = item.DisplayOrder,
                GroupName = item.GroupName,
                CanDelete = empty
            });
        }

        [Route("Group/{id}", Name = "PostEditGroup"), HttpPost, Authorize(Roles = AuthorizedRoles.IsAdmin), ValidateAntiForgeryToken]
        public async Task<ActionResult> EditGroupAsync(FaqGroupViewModel viewModel)
        {
            var item = await _FaqGroupRepository.GetAsync(viewModel.Id);
            if (item == null)
            {
                return NotFound();
            }

            if (viewModel.IsDeleting)
            {
                await _FaqGroupRepository.DeleteAsync(viewModel.Id);
                await ReorderGroup(item.DisplayOrder);
                return RedirectToAction(nameof(Index));
            }

            return await ProcessEditGroup(viewModel, item);
        }

        [Route("Group/{id}/{order}", Name = "EditGroupOrder"), HttpGet, Authorize(Roles = AuthorizedRoles.IsAdmin)]
        public async Task<ActionResult> EditGroupOrderAsync(string id, string order)
        {
            var item = await _FaqGroupRepository.GetAsync(id);
            if (item == null) return NotFound();

            var newOrder = item.DisplayOrder + (order == "down" ? 1 : -1);
            var groups = await _FaqGroupRepository.GetAllAsync(1000);
            var swap = groups.Items.FirstOrDefault(x => x.DisplayOrder == newOrder);
            if (swap == null) return NotFound();

            swap.DisplayOrder = item.DisplayOrder;
            await _FaqGroupRepository.UpdateAsync(swap);

            item.DisplayOrder = newOrder;
            await _FaqGroupRepository.UpdateAsync(item);

            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> ReorderGroup(int reorderFrom)
        {
            var allItems = (await _FaqGroupRepository.GetAllAsync(1000)).Items;

            foreach (var group in allItems.Where(x => x.DisplayOrder > reorderFrom).OrderBy(x => x.DisplayOrder))
            {
                group.DisplayOrder -= 1;
                await _FaqGroupRepository.UpdateAsync(group);
            }

            return true;
        }

        private async Task<ActionResult> ProcessEditGroup(FaqGroupViewModel viewModel, FaqGroup oldModel = null)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(viewModel.Id))
                {
                    var item = new FaqGroup()
                    {
                        GroupName = viewModel.GroupName,
                        DisplayOrder = (await _FaqGroupRepository.GetAllAsync(1000)).Items.Count() + 1
                    };
                    await _FaqGroupRepository.CreateAsync(item);
                }
                else
                {
                    var item = oldModel.ShallowCopy();
                    item.GroupName = viewModel.GroupName;
                    await _FaqGroupRepository.UpdateAsync(item);
                }
                TempData["ShowSaved"] = true;
                return RedirectToAction(nameof(Index));
            }

            return View("EditGroup", viewModel);
        }

    }
}
