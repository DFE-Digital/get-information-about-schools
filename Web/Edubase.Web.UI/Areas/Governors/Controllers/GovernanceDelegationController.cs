using System.Threading.Tasks;
using Edubase.Services.Groups;
using Edubase.Services.Groups.Models;
using Edubase.Web.UI.Areas.Governors.Models;
using Edubase.Web.UI.Areas.Governors.Models.Validators;
using Edubase.Web.UI.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Edubase.Web.UI.Areas.Governors.Controllers
{
    [Route("Governors/GovernanceDelegation")]
    public class GovernanceDelegationController : Controller
    {
        private readonly IGroupReadService _groupReadService;
        private readonly IGroupsWriteService _groupWriteService;
        private readonly ILayoutHelper _layoutHelper;

        public GovernanceDelegationController(
            IGroupReadService groupReadService,
            IGroupsWriteService groupWriteService,
            ILayoutHelper layoutHelper)
        {
            _groupReadService = groupReadService;
            _groupWriteService = groupWriteService;
            _layoutHelper = layoutHelper;
        }

        [HttpGet("Groups/Group/Edit/{groupUId:int}/Governance/Delegation", Name = "GroupEditDelegation")]
        [Authorize(Policy = "EdubasePolicy")]
        public async Task<IActionResult> GroupEditDelegation(int groupUId)
        {
            var group = await _groupReadService.GetAsync(groupUId, User);
            if (group.Success)
            {
                var model = new EditGroupDelegationInformationViewModel
                {
                    DelegationInformation = group.ReturnValue.DelegationInformation
                };

                await _layoutHelper.PopulateLayoutProperties(model, null, groupUId, User);

                return View(model);
            }

            return RedirectToRoute("GroupEditGovernance", new { GroupUId = groupUId });
        }

        [HttpPost("Groups/Group/Edit/{groupUId:int}/Governance/Delegation")]
        [Authorize(Policy = "EdubasePolicy")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GroupEditDelegation(EditGroupDelegationInformationViewModel model)
        {
            var result = await new EditGroupDelegationInformationViewModelValidator().ValidateAsync(model);

            if (ModelState.IsValid)
            {
                var groupResult = await _groupReadService.GetAsync(model.GroupUId.Value, User);
                if (groupResult.Success)
                {
                    var group = groupResult.ReturnValue;
                    group.DelegationInformation = model.DelegationInformation;
                    var updatedGroup = new SaveGroupDto(group);
                    await _groupWriteService.SaveAsync(updatedGroup, User);
                }

                var url = Url.RouteUrl("GroupDetails", new { id = model.GroupUId, saved = true });
                return Redirect($"{url}#governance");
            }

            result.EduBaseAddToModelState(ModelState, null, true);
            await _layoutHelper.PopulateLayoutProperties(model, null, model.GroupUId, User);
            return View(model);
        }
    }
}
