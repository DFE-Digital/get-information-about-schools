using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Web;
using Edubase.Services.Enums;
using Edubase.Services.Establishments.Models;
using Edubase.Services.Establishments;
using Edubase.Services.Governors;
using Edubase.Services.Groups;
using Edubase.Web.UI.Areas.Governors.Models;
using static Microsoft.ApplicationInsights.MetricDimensionNames.TelemetryContext;
using Edubase.Services.Lookup;

namespace Edubase.Web.UI.Areas
{
    public class GovernorsGridViewModelFactory : IGovernorsGridViewModelFactory
    {
        private readonly IGovernorsReadService _governorsReadService;
        private readonly ICachedLookupService _cachedLookupService;
        private readonly IEstablishmentReadService _establishmentReadService;
        private readonly IGroupReadService _groupReadService;

        public GovernorsGridViewModelFactory(
            IGovernorsReadService governorsReadService, ICachedLookupService cachedLookupService,
            IEstablishmentReadService establishmentReadService, IGroupReadService groupReadService)
        {
            _governorsReadService = governorsReadService;
            _cachedLookupService = cachedLookupService;
            _establishmentReadService = establishmentReadService;
            _groupReadService = groupReadService;
        }

        public async Task<GovernorsGridViewModel> CreateGovernorsViewModel(int? groupUId = null,
            int? establishmentUrn = null, EstablishmentModel establishmentModel = null, IPrincipal user = null)
        {
            establishmentUrn = establishmentUrn ?? establishmentModel?.Urn;

            var domainModelTask = _governorsReadService.GetGovernorListAsync(establishmentUrn, groupUId, user);
            var governorPermissionsTask =
                _governorsReadService.GetGovernorPermissions(establishmentUrn, groupUId, user);

            await Task.WhenAll(domainModelTask, governorPermissionsTask);

            var domainModel = domainModelTask.Result;
            var governorPermissions = governorPermissionsTask.Result;

            var viewModel = new GovernorsGridViewModel(domainModel,
                false,
                groupUId,
                establishmentUrn,
                await _cachedLookupService.NationalitiesGetAllAsync(),
                await _cachedLookupService.GovernorAppointingBodiesGetAllAsync(),
                await _cachedLookupService.TitlesGetAllAsync(),
                governorPermissions);

            if (establishmentUrn.HasValue || establishmentModel != null) // governance view for an establishment
            {
                var estabDomainModel = establishmentModel ??
                                       (await _establishmentReadService.GetAsync(establishmentUrn.Value, user))
                                       .GetResult();
                var items = await _establishmentReadService.GetPermissibleLocalGovernorsAsync(establishmentUrn.Value,
                    user); // The API uses 1 as a default value, hence we have to call another API to deduce whether to show the Governance mode UI section
                viewModel.GovernanceMode = items.Any() ? estabDomainModel.GovernanceMode : null;
            }

            if (groupUId.HasValue) // governance view for a group
            {
                var groupModel = (await _groupReadService.GetAsync(groupUId.Value, user)).GetResult();
                viewModel.ShowDelegationAndCorpContactInformation =
                    groupModel.GroupTypeId == (int) eLookupGroupType.MultiacademyTrust;
                viewModel.DelegationInformation = groupModel.DelegationInformation;
                viewModel.CorporateContact = groupModel.CorporateContact;
                viewModel.GroupTypeId = groupModel.GroupTypeId;
            }

            return viewModel;
        }
    }
}
