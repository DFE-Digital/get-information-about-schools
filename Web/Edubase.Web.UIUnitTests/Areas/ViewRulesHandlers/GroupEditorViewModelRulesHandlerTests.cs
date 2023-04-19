using System.Collections.Generic;
using System.Security.Principal;
using Edubase.Services.Enums;
using Edubase.Services.Security;
using Edubase.Web.UI.Areas.Groups.Models;
using Edubase.Web.UI.Areas.Groups.Models.CreateEdit;
using Edubase.Web.UI.Areas.Groups.ViewRulesHandlers;
using Moq;
using Xunit;

namespace Edubase.Web.UIUnitTests.Areas.ViewRulesHandlers
{
    public class GroupEditorViewModelRulesHandlerTests
    {
        private readonly Mock<IPrincipal> mockPrincipal = new Mock<IPrincipal>(MockBehavior.Strict);
        private readonly Mock<IIdentity> mockIdentity = new Mock<IIdentity>(MockBehavior.Strict);

        public GroupEditorViewModelRulesHandlerTests()
        {
            mockPrincipal.SetupGet(x => x.Identity)
                .Returns(mockIdentity.Object);
        }

        [Theory]
        [InlineData(eLookupGroupType.MultiacademyTrust, eLookupGroupStatus.Open, EdubaseRoles.ROLE_BACKOFFICE)]
        [InlineData(eLookupGroupType.SingleacademyTrust, eLookupGroupStatus.Open, EdubaseRoles.ROLE_BACKOFFICE)]
        [InlineData(eLookupGroupType.SchoolSponsor, eLookupGroupStatus.Open, EdubaseRoles.ROLE_BACKOFFICE)]
        [InlineData(eLookupGroupType.Federation, eLookupGroupStatus.Open, EdubaseRoles.ROLE_BACKOFFICE)]
        [InlineData(eLookupGroupType.SecureSingleAcademyTrust, eLookupGroupStatus.Open, EdubaseRoles.ROLE_BACKOFFICE)]
        [InlineData(eLookupGroupType.SecureSingleAcademyTrust, eLookupGroupStatus.Open, EdubaseRoles.YCS)]
        public void SetEditPermissions_GroupViewModel_CanUserCloseAndMarkAsCreatedInError_True(eLookupGroupType groupType, eLookupGroupStatus statusId, string role)
        {
            mockPrincipal.Setup(x => x.IsInRole(It.IsAny<string>())).Returns(false);
            mockPrincipal.Setup(x => x.IsInRole(role)).Returns(true);
            var viewModel = new GroupEditorViewModel()
            {
                GroupTypeId = (int) groupType,
                StatusId = (int) statusId
            };

            var result = GroupEditorViewModelRulesHandler.SetEditPermissions(viewModel, mockPrincipal.Object);

            Assert.True(result.CanUserCloseAndMarkAsCreatedInError);
        }

        [Theory]
        [InlineData(eLookupGroupType.MultiacademyTrust, eLookupGroupStatus.CreatedInError, EdubaseRoles.ROLE_BACKOFFICE)]
        [InlineData(eLookupGroupType.MultiacademyTrust, eLookupGroupStatus.Closed, EdubaseRoles.ROLE_BACKOFFICE)]
        [InlineData(eLookupGroupType.SingleacademyTrust, eLookupGroupStatus.CreatedInError, EdubaseRoles.ROLE_BACKOFFICE)]
        [InlineData(eLookupGroupType.SingleacademyTrust, eLookupGroupStatus.Closed, EdubaseRoles.ROLE_BACKOFFICE)]
        [InlineData(eLookupGroupType.SchoolSponsor, eLookupGroupStatus.CreatedInError, EdubaseRoles.ROLE_BACKOFFICE)]
        [InlineData(eLookupGroupType.SchoolSponsor, eLookupGroupStatus.Closed, EdubaseRoles.ROLE_BACKOFFICE)]
        [InlineData(eLookupGroupType.Federation, eLookupGroupStatus.CreatedInError, EdubaseRoles.ROLE_BACKOFFICE)]
        [InlineData(eLookupGroupType.Federation, eLookupGroupStatus.Closed, EdubaseRoles.ROLE_BACKOFFICE)]
        [InlineData(eLookupGroupType.SecureSingleAcademyTrust, eLookupGroupStatus.CreatedInError, EdubaseRoles.ROLE_BACKOFFICE)]
        [InlineData(eLookupGroupType.SecureSingleAcademyTrust, eLookupGroupStatus.Closed, EdubaseRoles.ROLE_BACKOFFICE)]
        [InlineData(eLookupGroupType.SecureSingleAcademyTrust, eLookupGroupStatus.CreatedInError, EdubaseRoles.YCS)]
        [InlineData(eLookupGroupType.SecureSingleAcademyTrust, eLookupGroupStatus.Closed, EdubaseRoles.YCS)]
        [InlineData(eLookupGroupType.MultiacademyTrust, eLookupGroupStatus.CreatedInError, EdubaseRoles.YCS)]
        [InlineData(eLookupGroupType.MultiacademyTrust, eLookupGroupStatus.Closed, EdubaseRoles.YCS)]
        [InlineData(eLookupGroupType.SingleacademyTrust, eLookupGroupStatus.CreatedInError, EdubaseRoles.YCS)]
        [InlineData(eLookupGroupType.SingleacademyTrust, eLookupGroupStatus.Closed, EdubaseRoles.YCS)]
        [InlineData(eLookupGroupType.SchoolSponsor, eLookupGroupStatus.CreatedInError, EdubaseRoles.YCS)]
        [InlineData(eLookupGroupType.SchoolSponsor, eLookupGroupStatus.Closed, EdubaseRoles.YCS)]
        [InlineData(eLookupGroupType.Federation, eLookupGroupStatus.CreatedInError, EdubaseRoles.YCS)]
        [InlineData(eLookupGroupType.Federation, eLookupGroupStatus.Closed, EdubaseRoles.YCS)]
        public void SetEditPermissions_GroupViewModel_CanUserCloseAndMarkAsCreatedInError_False(eLookupGroupType groupType, eLookupGroupStatus statusId, string role)
        {
            mockPrincipal.Setup(x => x.IsInRole(It.IsAny<string>())).Returns(false);
            mockPrincipal.Setup(x => x.IsInRole(role)).Returns(true);
            var viewModel = new GroupEditorViewModel()
            {
                GroupTypeId = (int) groupType,
                StatusId = (int) statusId
            };

            var result = GroupEditorViewModelRulesHandler.SetEditPermissions(viewModel, mockPrincipal.Object);

            Assert.False(result.CanUserCloseAndMarkAsCreatedInError);
        }

        [Theory]
        [InlineData((int) eLookupGroupType.ChildrensCentresCollaboration, EdubaseRoles.ROLE_BACKOFFICE)]
        [InlineData((int) eLookupGroupType.ChildrensCentresGroup, EdubaseRoles.ROLE_BACKOFFICE)]
        public void SetEditPermissions_GroupViewModel_IsLocalAuthorityEditable_True(int groupType, string role)
        {
            mockPrincipal.Setup(x => x.IsInRole(It.IsAny<string>())).Returns(false);
            mockPrincipal.Setup(x => x.IsInRole(role)).Returns(true);
            var viewModel = new GroupEditorViewModel()
            {
                GroupTypeId = groupType,
                LinkedEstablishments = new GroupLinkedEstablishmentsViewModel() { Establishments = new List<EstablishmentGroupViewModel>() }
            };

            var result = GroupEditorViewModelRulesHandler.SetEditPermissions(viewModel, mockPrincipal.Object);

            Assert.True(result.IsLocalAuthorityEditable);
        }

        [Theory]
        [InlineData((int) eLookupGroupType.SecureSingleAcademyTrust, EdubaseRoles.ROLE_BACKOFFICE)]
        [InlineData((int) eLookupGroupType.ChildrensCentresGroup, EdubaseRoles.YCS)]
        [InlineData((int) eLookupGroupType.ChildrensCentresCollaboration, EdubaseRoles.YCS)]
        public void SetEditPermissions_GroupViewModel_IsLocalAuthorityEditable_False(int groupType, string role)
        {
            mockPrincipal.Setup(x => x.IsInRole(It.IsAny<string>())).Returns(false);
            mockPrincipal.Setup(x => x.IsInRole(role)).Returns(true);
            var viewModel = new GroupEditorViewModel()
            {
                GroupTypeId = groupType,
                LinkedEstablishments = new GroupLinkedEstablishmentsViewModel() { Establishments = new List<EstablishmentGroupViewModel>() }
            };

            var result = GroupEditorViewModelRulesHandler.SetEditPermissions(viewModel, mockPrincipal.Object);

            Assert.False(result.IsLocalAuthorityEditable);
        }

        [Theory]
        [InlineData((int) eLookupGroupType.MultiacademyTrust, EdubaseRoles.ROLE_BACKOFFICE)]
        [InlineData((int) eLookupGroupType.MultiacademyTrust, EdubaseRoles.EDUBASE)]
        [InlineData((int) eLookupGroupType.MultiacademyTrust, EdubaseRoles.EDUBASE_CMT)]
        [InlineData((int) eLookupGroupType.MultiacademyTrust, EdubaseRoles.AP_AOS)]
        [InlineData((int) eLookupGroupType.SingleacademyTrust, EdubaseRoles.ROLE_BACKOFFICE)]
        [InlineData((int) eLookupGroupType.SingleacademyTrust, EdubaseRoles.EDUBASE)]
        [InlineData((int) eLookupGroupType.SingleacademyTrust, EdubaseRoles.EDUBASE_CMT)]
        [InlineData((int) eLookupGroupType.SingleacademyTrust, EdubaseRoles.AP_AOS)]
        [InlineData((int) eLookupGroupType.SecureSingleAcademyTrust, EdubaseRoles.ROLE_BACKOFFICE)]
        [InlineData((int) eLookupGroupType.SecureSingleAcademyTrust, EdubaseRoles.YCS)]

        public void SetEditPermissions_GroupViewModel_CanUserEditClosedDate_CanUserEditStatus_True(int groupType, string role)
        {
            mockPrincipal.Setup(x => x.IsInRole(It.IsAny<string>())).Returns(false);
            mockPrincipal.Setup(x => x.IsInRole(role)).Returns(true);

            var viewModel = new GroupEditorViewModel()
            {
                GroupTypeId = groupType
            };

            var result = GroupEditorViewModelRulesHandler.SetEditPermissions(viewModel, mockPrincipal.Object);

            Assert.True(result.CanUserEditClosedDate);
            Assert.True(result.CanUserEditStatus);
        }

        [Theory]
        [InlineData((int) eLookupGroupType.MultiacademyTrust, EdubaseRoles.YCS)]
        [InlineData((int) eLookupGroupType.SingleacademyTrust, EdubaseRoles.YCS)]
        [InlineData((int) eLookupGroupType.SecureSingleAcademyTrust, EdubaseRoles.EDUBASE)]
        [InlineData((int) eLookupGroupType.SecureSingleAcademyTrust, EdubaseRoles.EDUBASE_CMT)]
        [InlineData((int) eLookupGroupType.SecureSingleAcademyTrust, EdubaseRoles.AP_AOS)]
        public void SetEditPermissions_GroupViewModel_CanUserEditClosedDate_CanUserEditStatus_False(int groupType, string role)
        {
            mockPrincipal.Setup(x => x.IsInRole(It.IsAny<string>())).Returns(false);
            mockPrincipal.Setup(x => x.IsInRole(role)).Returns(true);

            var viewModel = new GroupEditorViewModel()
            {
                GroupTypeId = groupType
            };

            var result = GroupEditorViewModelRulesHandler.SetEditPermissions(viewModel, mockPrincipal.Object);

            Assert.False(result.CanUserEditClosedDate);
            Assert.False(result.CanUserEditStatus);
        }

        [Theory]
        [InlineData((int) eLookupGroupType.MultiacademyTrust, EdubaseRoles.ROLE_BACKOFFICE)]
        [InlineData((int) eLookupGroupType.SingleacademyTrust, EdubaseRoles.ROLE_BACKOFFICE)]
        [InlineData((int) eLookupGroupType.SecureSingleAcademyTrust, EdubaseRoles.ROLE_BACKOFFICE)]
        [InlineData((int) eLookupGroupType.SecureSingleAcademyTrust, EdubaseRoles.UKRLP)]
        public void SetEditPermissions_GroupViewModel_CanUserEditUkprn_True(int groupType, string role)
        {
            mockPrincipal.Setup(x => x.IsInRole(It.IsAny<string>())).Returns(false);
            mockPrincipal.Setup(x => x.IsInRole(role)).Returns(true);

            var viewModel = new GroupEditorViewModel()
            {
                GroupTypeId = groupType
            };
            
            var result = GroupEditorViewModelRulesHandler.SetEditPermissions(viewModel, mockPrincipal.Object);

            Assert.True(result.CanUserEditUkprn);
        }

        [Theory]
        [InlineData((int) eLookupGroupType.MultiacademyTrust, EdubaseRoles.UKRLP)]
        [InlineData((int) eLookupGroupType.SingleacademyTrust, EdubaseRoles.UKRLP)]
        [InlineData((int) eLookupGroupType.MultiacademyTrust, EdubaseRoles.YCS)]
        [InlineData((int) eLookupGroupType.SingleacademyTrust, EdubaseRoles.YCS)]
        [InlineData((int) eLookupGroupType.ChildrensCentresCollaboration, EdubaseRoles.ROLE_BACKOFFICE)]
        public void SetEditPermissions_GroupViewModel_CanUserEditUkprn_False(int groupType, string role)
        {
            mockPrincipal.Setup(x => x.IsInRole(It.IsAny<string>())).Returns(false);
            mockPrincipal.Setup(x => x.IsInRole(role)).Returns(true);

            var viewModel = new GroupEditorViewModel()
            {
                GroupTypeId = groupType
            };

            var result = GroupEditorViewModelRulesHandler.SetEditPermissions(viewModel, mockPrincipal.Object);

            Assert.False(result.CanUserEditUkprn);
        }
    }
}
