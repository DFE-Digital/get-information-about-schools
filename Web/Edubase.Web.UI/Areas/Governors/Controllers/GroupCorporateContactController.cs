using System.Threading.Tasks;
using Edubase.Services.Groups;
using Edubase.Services.Groups.Models;
using Edubase.Web.UI.Areas.Governors.Models;
using Edubase.Web.UI.Areas.Governors.Models.Validators;
using Edubase.Web.UI.Helpers;
using Edubase.Web.UI.Validation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Edubase.Web.UI.Areas.Governors.Controllers
{
    [Route("Governors/GroupCorporateContact")]
    public class GroupCorporateContactController : Controller
    {
        private readonly IGroupReadService _groupReadService;
        private readonly IGroupsWriteService _groupWriteService;
        private readonly ILayoutHelper _layoutHelper;

        public GroupCorporateContactController(
            IGroupReadService groupReadService,
            IGroupsWriteService groupWriteService,
            ILayoutHelper layoutHelper)
        {
            _groupReadService = groupReadService;
            _groupWriteService = groupWriteService;
            _layoutHelper = layoutHelper;
        }

        [HttpGet("Groups/Group/Edit/{groupUId:int}/CorporateContact", Name = "GroupEditCorporateContact")]
        [Authorize(Policy = "EdubasePolicy")]
        public async Task<IActionResult> GroupEditCorporateContact(int groupUId)
        {
            var group = await _groupReadService.GetAsync(groupUId, User);
            if (group.Success)
            {
                var model = new EditGroupCorporateContactViewModel
                {
                    CorporateContact = group.ReturnValue.CorporateContact
                };

                await _layoutHelper.PopulateLayoutProperties(model, null, groupUId, User);
                return View(model);
            }

            return RedirectToRoute("GroupEditGovernance", new { GroupUId = groupUId });
        }

        [HttpPost("Groups/Group/Edit/{groupUId:int}/CorporateContact", Name = "GroupEditCorporateContactPost")]
        [Authorize(Policy = "EdubasePolicy")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GroupEditCorporateContact(EditGroupCorporateContactViewModel model)
        {
            var result = await new EditGroupCorporateContactViewModelValidator().ValidateAsync(model);

            if (ModelState.IsValid)
            {
                var groupResult = await _groupReadService.GetAsync(model.GroupUId.Value, User);
                if (groupResult.Success)
                {
                    var group = groupResult.ReturnValue;
                    group.CorporateContact = model.CorporateContact;
                    var updatedGroup = new SaveGroupDto(group);
                    var validation = await _groupWriteService.SaveAsync(updatedGroup, User);

                    if (!validation.HasErrors)
                    {
                        var url = Url.RouteUrl("GroupDetails", new { id = model.GroupUId, saved = true });
                        return Redirect($"{url}#governance");
                    }

                    validation.ApplyToModelState(ControllerContext);
                }
            }
            else
            {
                result.EduBaseAddToModelState(ModelState, null);
            }

            await _layoutHelper.PopulateLayoutProperties(model, null, model.GroupUId, User);
            return View(model);
        }
    }
}
