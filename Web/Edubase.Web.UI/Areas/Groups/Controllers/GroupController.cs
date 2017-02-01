using Edubase.Services.Establishments;
using Edubase.Services.Groups;
using Edubase.Services.Lookup;
using Edubase.Services.Security;
using Edubase.Web.UI.Areas.Groups.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Edubase.Web.UI.Areas.Groups.Controllers
{
    using Common;
    using Services.Governors;
    using Services.Groups.Models;
    using static GroupDetailViewModel;
    using GT = Services.Enums.eLookupGroupType;

    [RouteArea("Groups"), RoutePrefix("Group")]
    public class GroupController : Controller
    {
        ICachedLookupService _lookup;
        IEstablishmentReadService _establishmentReadService;
        IGroupReadService _groupReadService;
        ISecurityService _securityService;
        IGovernorsReadService _governorsReadService;

        public GroupController(
            ICachedLookupService cachedLookupService, 
            ISecurityService securityService,
            IGroupReadService groupReadService,
            IEstablishmentReadService establishmentReadService,
            IGovernorsReadService governorsReadService)
        {
            _lookup = cachedLookupService;
            _securityService = securityService;
            _groupReadService = groupReadService;
            _establishmentReadService = establishmentReadService;
            _governorsReadService = governorsReadService;
        }


        [Route(nameof(Details) + "/{id:int}"), HttpGet]
        public async Task<ActionResult> Details(int id)
        {
            var viewModel = new GroupDetailViewModel();
            var result = await _groupReadService.GetAsync(id, User);

            var model = result.GetResult();
            viewModel.Group = model;

            if (model.GroupTypeId.HasValue) viewModel.GroupTypeName = (await _lookup.GetNameAsync(() => model.GroupTypeId));
            if (model.LocalAuthorityId.HasValue) viewModel.LocalAuthorityName = (await _lookup.GetNameAsync(() => model.LocalAuthorityId));
            if (model.StatusId.HasValue) viewModel.GroupStatusName = (await _lookup.GetNameAsync(() => model.StatusId, "Group"));

            if (model.GroupTypeId.Equals((int)GT.ChildrensCentresGroup)) viewModel.Address = "!FROM LEAD CENTRE!"; // TODO: get from the 'lead centre'; need IsLeadCentre on the estabgroup table
            else if(model.GroupTypeId.OneOfThese(GT.SingleacademyTrust, GT.MultiacademyTrust)) viewModel.Address = model.Address;

            viewModel.CanUserEdit = _securityService.GetEditGroupPermission(User).CanEdit(model.GroupUID, model.GroupTypeId.Value);
            viewModel.IsUserLoggedOn = User.Identity.IsAuthenticated;

            await PopulateEstablishmentList(viewModel, model);

            viewModel.HistoricalGovernors = await _governorsReadService.GetHistoricalByGroupUID(id);
            viewModel.Governors = await _governorsReadService.GetCurrentByGroupUID(id);

            return View(viewModel);
        }

        private async Task PopulateEstablishmentList(GroupDetailViewModel viewModel, GroupModel model)
        {
            var establishmentGroups = await _groupReadService.GetEstablishmentGroupsAsync(model.GroupUID);
            foreach (var establishmentGroup in establishmentGroups)
            {
                var result = await _establishmentReadService.GetAsync(establishmentGroup.EstablishmentUrn, User);
                if (result.Success)
                {
                    var estabModel = result.GetResult();
                    viewModel.Establishments.Add(new EstablishmentGroupViewModel
                    {
                        Address = estabModel.GetAddress(),
                        HeadFirstName = estabModel.HeadFirstName,
                        HeadLastName = estabModel.HeadLastName,
                        Name = estabModel.Name,
                        Urn = estabModel.Urn.Value,
                        TypeName = await _lookup.GetNameAsync(() => estabModel.LocalAuthorityId),
                        HeadTitleName = await _lookup.GetNameAsync(() => estabModel.HeadTitleId),
                        JoinedDate = establishmentGroup.JoinedDate
                    });
                }
            }
        }
    }
}