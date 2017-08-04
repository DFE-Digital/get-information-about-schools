using System;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using Edubase.Services.Enums;
using Edubase.Services.Establishments;
using Edubase.Services.Establishments.Models;
using Edubase.Services.Groups;
using Edubase.Services.Groups.Models;
using Edubase.Services.Nomenclature;
using Edubase.Web.UI.Areas.Establishments.Models;
using Edubase.Web.UI.Areas.Groups.Models.CreateEdit;
using Edubase.Web.UI.Exceptions;

namespace Edubase.Web.UI.Helpers
{
    public class LayoutHelper
    {
        private const string GroupsLayout = "~/Areas/Groups/Views/Group/_EditLayoutPage.cshtml";
        private const string EstabLayout = "~/Areas/Establishments/Views/Establishment/_EditLayoutPage.cshtml";

        private readonly IEstablishmentReadService _establishmentReadService;
        private readonly IGroupReadService _groupReadService;
        private readonly NomenclatureService _nomenclatureService;

        public LayoutHelper(
            NomenclatureService nomenclatureService,
            IGroupReadService groupReadService,
            IEstablishmentReadService establishmentReadService)
        {
            _nomenclatureService = nomenclatureService;
            _establishmentReadService = establishmentReadService;
            _groupReadService = groupReadService;
        }

        internal async Task PopulateLayoutProperties(object viewModel, int? establishmentUrn, int? groupUId, IPrincipal user, Action<EstablishmentModel> processEstablishment = null, Action<GroupModel> processGroup = null)
        {
            if (establishmentUrn.HasValue && groupUId.HasValue)
                throw new InvalidParameterException("Both urn and uid cannot be populated");

            if (!establishmentUrn.HasValue && !groupUId.HasValue)
                throw new InvalidParameterException($"Both {nameof(establishmentUrn)} and {nameof(groupUId)} parameters are null");

            if (establishmentUrn.HasValue)
            {
                var domainModel = (await _establishmentReadService.GetAsync(establishmentUrn.Value, user)).GetResult();
                var displayPolicy = await _establishmentReadService.GetDisplayPolicyAsync(domainModel, user);
                var permissibleGovernanceModes = await _establishmentReadService.GetPermissibleLocalGovernorsAsync(establishmentUrn.Value, user);
                if (!permissibleGovernanceModes.Any()) domainModel.GovernanceModeId = null; // hack the model returned.
                var vm = (IEstablishmentPageViewModel)viewModel;
                vm.Layout = EstabLayout;
                vm.Name = domainModel.Name;
                vm.SelectedTab = "governance";
                vm.Urn = domainModel.Urn;
                vm.TabDisplayPolicy = new TabDisplayPolicy(domainModel, displayPolicy, user);
                processEstablishment?.Invoke(domainModel);
            }
            else if (groupUId.HasValue)
            {
                var domainModel = (await _groupReadService.GetAsync(groupUId.Value, user)).GetResult();
                var vm = (IGroupPageViewModel)viewModel;
                vm.Layout = GroupsLayout;
                vm.GroupName = domainModel.Name;
                vm.GroupTypeId = domainModel.GroupTypeId.Value;
                vm.GroupUId = groupUId;
                vm.SelectedTabName = "governance";
                vm.ListOfEstablishmentsPluralName = _nomenclatureService.GetEstablishmentsPluralName((eLookupGroupType)vm.GroupTypeId.Value);
                processGroup?.Invoke(domainModel);
            }
        }
    }
}