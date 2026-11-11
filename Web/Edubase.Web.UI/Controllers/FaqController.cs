using System.Linq;
using System.Threading.Tasks;
using Edubase.Data.Entity;
using Edubase.Data.Repositories;
using Edubase.Web.UI.Helpers;
using Edubase.Web.UI.Models;
using Edubase.Web.UI.Models.Faq;
using Microsoft.AspNetCore.Mvc;

namespace Edubase.Web.UI.Controllers
{
    [Route("Faq")]
    public class FaqController : Controller
    {
        private readonly IFaqItemRepository _FaqItemRepository;
        private readonly IFaqGroupRepository _FaqGroupRepository;

        public FaqController(IFaqItemRepository FaqItemRepository, IFaqGroupRepository FaqGroupRepository)
        {
            _FaqItemRepository = FaqItemRepository;
            _FaqGroupRepository = FaqGroupRepository;
        }

        [HttpGet("", Name = "Faq")]
        [HttpGet("Index")]
        public async Task<ActionResult> Index()
        {
            var faqs = await _FaqItemRepository.GetAllAsync(1000);
            var groups = await _FaqGroupRepository.GetAllAsync(1000);
            return View(new FaqViewModel(faqs.Items, groups.Items) { UserCanEdit = User.IsInRole(AuthorizedRoles.IsAdmin) });
        }

        [HttpGet("Create", Name = "CreateItem"), MvcAuthorizeRoles(AuthorizedRoles.IsAdmin)]
        public async Task<ActionResult> Create()
        {
            var groups = await _FaqGroupRepository.GetAllAsync(1000);
            return View("CreateEdit", new FaqItemViewModel(groups.Items));
        }

        [HttpPost("Create", Name = "PostCreateItem"), MvcAuthorizeRoles(AuthorizedRoles.IsAdmin), ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateAsync(FaqItemViewModel viewModel)
        {
            return await ProcessEditItem(viewModel);
        }

        [HttpGet("Edit/{id}", Name = "EditItem"), MvcAuthorizeRoles(AuthorizedRoles.IsAdmin)]
        public async Task<ActionResult> EditAsync(string id)
        {
            var item = await _FaqItemRepository.GetAsync(id);
            var groups = await _FaqGroupRepository.GetAllAsync(1000);
            if (item == null) return NotFound();

            return View("CreateEdit", new FaqItemViewModel
            {
                Id = id,
                Content = item.Content,
                Title = item.Title,
                GroupId = item.GroupId,
                Groups = groups.Items
            });
        }

        [HttpPost("Edit/{id}", Name = "PostEditItem"), MvcAuthorizeRoles(AuthorizedRoles.IsAdmin), ValidateAntiForgeryToken]
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

        [HttpGet("Edit/{id}/{order}", Name = "EditItemOrder"), MvcAuthorizeRoles(AuthorizedRoles.IsAdmin)]
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
                foreach (var faqItem in allItems.Where(x => x.GroupId == groupId && x.DisplayOrder > reorderFrom).OrderBy(x => x.DisplayOrder))
                {
                    faqItem.DisplayOrder -= 1;
                    await _FaqItemRepository.UpdateAsync(faqItem);
                }
            }

            return true;
        }

        [HttpGet("Groups"), MvcAuthorizeRoles(AuthorizedRoles.IsAdmin)]
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

        [HttpGet("Groups/New", Name = "CreateGroup"), MvcAuthorizeRoles(AuthorizedRoles.IsAdmin)]
        public ActionResult CreateGroup() => View("EditGroup", new FaqGroupViewModel());

        [HttpPost("Groups/New", Name = "PostCreateGroup"), MvcAuthorizeRoles(AuthorizedRoles.IsAdmin), ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateGroupAsync(FaqGroupViewModel viewModel)
        {
            return await ProcessEditGroup(viewModel);
        }

        [HttpGet("Group/{id}", Name = "EditGroup"), MvcAuthorizeRoles(AuthorizedRoles.IsAdmin)]
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

        [HttpPost("Group/{id}", Name = "PostEditGroup"), MvcAuthorizeRoles(AuthorizedRoles.IsAdmin), ValidateAntiForgeryToken]
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

        [HttpGet("Group/{id}/{order}", Name = "EditGroupOrder"), MvcAuthorizeRoles(AuthorizedRoles.IsAdmin)]
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
