using System.Threading.Tasks;
using Edubase.Services.Groups;
using Edubase.Services.Groups.Models;
using Edubase.Web.UI.Areas.Governors.Models;
using Edubase.Web.UI.Areas.Governors.Models.Validators;
using Edubase.Web.UI.Filters;
using Edubase.Web.UI.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Edubase.Web.UI.Areas.Governors.Controllers
{
    [Route("governors/governance-delegation")]
    public class GovernanceDelegationController : Controller
    {
        private const string GROUP_EDIT_DELEGATION = "~/Groups/Group/Edit/{groupUId:int}/Governance/Delegation";

        private readonly IGroupReadService _groupReadService;
        private readonly IGroupsWriteService _groupWriteService;
        private readonly LayoutHelper _layoutHelper;

        public GovernanceDelegationController(
            IGroupReadService groupReadService, 
            IGroupsWriteService groupWriteService,
            LayoutHelper layoutHelper)
        {
            _groupReadService = groupReadService;
            _groupWriteService = groupWriteService;
            _layoutHelper = layoutHelper;
        }

        [HttpGet, EdubaseAuthorize]
        [Route(GROUP_EDIT_DELEGATION, Name = "GroupEditDelegation")]
        public async Task<ActionResult> GroupEditDelegation(int groupUId)
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

        [HttpPost, EdubaseAuthorize, ValidateAntiForgeryToken]
        [Route(GROUP_EDIT_DELEGATION)]
        public async Task<ActionResult> GroupEditDelegation(EditGroupDelegationInformationViewModel model)
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
