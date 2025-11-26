using System.Collections.Generic;
using System.Security.Principal;
using System.Threading.Tasks;
using Edubase.Services.Domain;
using Edubase.Services.Enums;
using Edubase.Services.Establishments;
using Edubase.Services.Establishments.Models;
using Edubase.Services.Governors;
using Edubase.Services.Governors.DisplayPolicies;
using Edubase.Services.Governors.Models;
using Edubase.Services.Groups;
using Edubase.Services.Groups.Models;
using Edubase.Services.Lookup;
using Edubase.Web.UI.Areas;
using Moq;
using Xunit;

namespace Edubase.Web.UIUnitTests.Areas
{
    public class GovernorsGridViewModelFactoryTests
    {
        private readonly GovernorsGridViewModelFactory _governorsGridViewModelFactory;

        private readonly Mock<IGovernorsReadService> _governorsReadService = new Mock<IGovernorsReadService>();
        private readonly Mock<ICachedLookupService> _cachedLookupService = new Mock<ICachedLookupService>();
        private readonly Mock<IEstablishmentReadService> _establishmentReadService = new Mock<IEstablishmentReadService>();
        private readonly Mock<IGroupReadService> _groupReadService = new Mock<IGroupReadService>();

        public GovernorsGridViewModelFactoryTests()
        {
            _governorsGridViewModelFactory = new GovernorsGridViewModelFactory(
                _governorsReadService.Object,
                _cachedLookupService.Object,
                _establishmentReadService.Object,
                _groupReadService.Object);
        }

        [Theory]
        [InlineData((int) eLookupGroupType.MultiacademyTrust, true)]
        [InlineData((int) eLookupGroupType.SecureSingleAcademyTrust, false)]
        public async Task CreateGovernorsViewModel_groupUIdSpecified(int groupTypeId, bool expectedShowDelegationAndCorpContactInformation)
        {
            var groupUId = It.IsAny<int>();

            var appointingBodieslookups = new List<LookupDto> { It.IsAny<LookupDto>() };
            _cachedLookupService.Setup(x => x.GovernorAppointingBodiesGetAllAsync())
                .ReturnsAsync(() => appointingBodieslookups);

            var governorsDetailsDto = new GovernorsDetailsDto
            {
                ApplicableRoles = new List<eLookupGovernorRole> { eLookupGovernorRole.AccountingOfficer, eLookupGovernorRole.Governor },
                RoleDisplayPolicies = new Dictionary<eLookupGovernorRole, GovernorDisplayPolicy>
                {
                    { eLookupGovernorRole.AccountingOfficer, new GovernorDisplayPolicy() },
                    { eLookupGovernorRole.Governor, new GovernorDisplayPolicy() }
                },
                CurrentGovernors = new List<GovernorModel>(),
                HistoricalGovernors = new List<GovernorModel>()
            };

            var governorPermissions = new GovernorPermissions()
            {
                Add = true,
                Remove = true,
                Update = true
            };

            var groupModel = new GroupModel
            {
                DelegationInformation = "delegation info",
                CorporateContact = "corporate contact info",
                GroupTypeId = groupTypeId
            };

            _governorsReadService.Setup(x => x.GetGovernorListAsync(null, groupUId, It.IsAny<IPrincipal>()))
                .ReturnsAsync(() => governorsDetailsDto);
            _governorsReadService.Setup(x => x.GetGovernorPermissions(null, groupUId, It.IsAny<IPrincipal>()))
                .ReturnsAsync(() => governorPermissions);
            _groupReadService.Setup(x => x.GetAsync((int) groupUId, It.IsAny<IPrincipal>()))
                .ReturnsAsync(new ServiceResultDto<GroupModel>(groupModel));

            var result = await _governorsGridViewModelFactory.CreateGovernorsViewModel(groupUId, null, null, It.IsAny<IPrincipal>());

            Assert.NotNull(result);
            Assert.Equal(expectedShowDelegationAndCorpContactInformation, result.ShowDelegationAndCorpContactInformation);
            Assert.Equal(groupUId, result.GroupUId);
            Assert.Equal(groupModel.GroupTypeId, result.GroupTypeId);
            Assert.Equal(groupModel.CorporateContact, result.CorporateContact);
            Assert.Null(result.EstablishmentUrn);
            Assert.Equal(governorsDetailsDto, result.DomainModel);
            Assert.False(result.EditMode);
            Assert.Equal(appointingBodieslookups, result.AppointingBodies);
            Assert.Equal(governorPermissions, result.GovernorPermissions);
            // need to test the Grid creation but no time at the moment
        }

        [Theory]
        [InlineData(true)]
        public async Task CreateGovernorsViewModel_EstablishmentUrnSpecified(bool establishmentModelNull)
        {
            var establishmentUrn = It.IsAny<int>();

            var appointingBodieslookups = new List<LookupDto> { It.IsAny<LookupDto>() };
            _cachedLookupService.Setup(x => x.GovernorAppointingBodiesGetAllAsync())
                .ReturnsAsync(() => appointingBodieslookups);

            var permissableLocalGovernors = new List<LookupDto> { It.IsAny<LookupDto>() };

            var governorsDetailsDto = new GovernorsDetailsDto
            {
                ApplicableRoles = new List<eLookupGovernorRole> { eLookupGovernorRole.AccountingOfficer, eLookupGovernorRole.Governor },
                RoleDisplayPolicies = new Dictionary<eLookupGovernorRole, GovernorDisplayPolicy>
                {
                    { eLookupGovernorRole.AccountingOfficer, new GovernorDisplayPolicy() },
                    { eLookupGovernorRole.Governor, new GovernorDisplayPolicy() }
                },
                CurrentGovernors = new List<GovernorModel>(),
                HistoricalGovernors = new List<GovernorModel>()
            };

            var governorPermissions = new GovernorPermissions()
            {
                Add = true,
                Remove = true,
                Update = true
            };

            var establishmentModel = new EstablishmentModel()
            {
                GovernanceModeId = (int) eGovernanceMode.LocalGovernors
            };

            _governorsReadService.Setup(x => x.GetGovernorListAsync(establishmentUrn, null, It.IsAny<IPrincipal>()))
                .ReturnsAsync(() => governorsDetailsDto);
            _governorsReadService.Setup(x => x.GetGovernorPermissions(establishmentUrn, null, It.IsAny<IPrincipal>()))
                .ReturnsAsync(() => governorPermissions);
            _establishmentReadService.Setup(x => x.GetAsync(establishmentUrn, It.IsAny<IPrincipal>()))
                .ReturnsAsync(() => new ServiceResultDto<EstablishmentModel>(establishmentModel));
            _establishmentReadService.Setup(x => x.GetPermissibleLocalGovernorsAsync(establishmentUrn, It.IsAny<IPrincipal>()))
                .ReturnsAsync(() => permissableLocalGovernors);

            var result = await _governorsGridViewModelFactory.CreateGovernorsViewModel(null, establishmentUrn, establishmentModelNull ? null : establishmentModel, It.IsAny<IPrincipal>());

            Assert.Equal(establishmentModel.GovernanceMode, result.GovernanceMode);
            Assert.Equal(establishmentUrn, result.EstablishmentUrn);
            Assert.Equal(governorsDetailsDto, result.DomainModel);
            Assert.False(result.EditMode);
            Assert.Equal(appointingBodieslookups, result.AppointingBodies);
            Assert.Equal(governorPermissions, result.GovernorPermissions);
            Assert.Equal(establishmentModel.GovernanceModeId, (int)result.GovernanceMode);
        }
    }
}
