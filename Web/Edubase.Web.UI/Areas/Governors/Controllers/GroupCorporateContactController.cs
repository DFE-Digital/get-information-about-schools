﻿using System.Threading.Tasks;
using System.Web.Mvc;
using Edubase.Services.Groups;
using Edubase.Services.Groups.Models;
using Edubase.Web.UI.Areas.Governors.Models;
using Edubase.Web.UI.Areas.Governors.Models.Validators;
using Edubase.Web.UI.Filters;
using Edubase.Web.UI.Helpers;

namespace Edubase.Web.UI.Areas.Governors.Controllers
{
    [RouteArea("Governors")]
    public class GroupCorporateContactController : Controller
    {
        private const string GroupEditCorporateContactRoute = "~/Groups/Group/Edit/{groupUId:int}/CorporateContact";

        private readonly IGroupReadService _groupReadService;
        private readonly IGroupsWriteService _groupWriteService;
        private readonly LayoutHelper _layoutHelper;

        public GroupCorporateContactController(
            IGroupReadService groupReadService, 
            IGroupsWriteService groupWriteService,
            LayoutHelper layoutHelper)
        {
            _groupReadService = groupReadService;
            _groupWriteService = groupWriteService;
            _layoutHelper = layoutHelper;
        }

        [HttpGet, EdubaseAuthorize]
        [Route(GroupEditCorporateContactRoute, Name = "GroupEditCorporateContact")]
        public async Task<ActionResult> GroupEditCorporateContact(int groupUId)
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

        [HttpPost, EdubaseAuthorize]
        [Route(GroupEditCorporateContactRoute, Name = "GroupEditCorporateContactPost")]
        public async Task<ActionResult> GroupEditCorporateContact(EditGroupCorporateContactViewModel model)
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
                    await _groupWriteService.SaveAsync(updatedGroup, User);
                }


                var url = Url.RouteUrl("GroupDetails", new { id = model.GroupUId, saved = true });
                return Redirect($"{url}#governance");
            }

            result.EduBaseAddToModelState(ModelState, null);
            await _layoutHelper.PopulateLayoutProperties(model, null, model.GroupUId, User);
            return View(model);
        }
    }
}