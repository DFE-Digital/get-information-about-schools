using System.Security.Principal;
using System.Threading.Tasks;
using Edubase.Services.Domain;
using Edubase.Services.Enums;
using Edubase.Services.Establishments;
using Edubase.Services.Establishments.Models;
using Edubase.Services.Groups;
using Edubase.Services.Security;
using Edubase.Web.UI.Areas.Groups.Models.CreateEdit;
using Edubase.Web.UI.Areas.Groups.Models.Validators;
using Moq;
using Xunit;

namespace Edubase.Web.UIUnitTests.Areas.Groups.Models
{
    public class GroupEditorViewModelValidatorTests
    {
        private readonly Mock<IEstablishmentReadService> _establishmentReadService = new(MockBehavior.Strict);
        private readonly Mock<IGroupReadService> _groupReadService = new(MockBehavior.Strict);

        private GroupEditorViewModelValidator CreateValidator()
        {
            return new GroupEditorViewModelValidator(_groupReadService.Object,
                _establishmentReadService.Object, Mock.Of<IPrincipal>(), Mock.Of<ISecurityService>());
        }

        [Fact]
        public async Task ValidateAsync_EmptyGroupId_DoesNotCheckBackend()
        {
            var model = new GroupEditorViewModel
            {
                Action = GroupEditorViewModel.ActionSave,
                SaveMode = GroupEditorViewModel.eSaveMode.Details,
                GroupTypeId = (int) eLookupGroupType.MultiacademyTrust,
                GroupId = ""
            };

            var result = await CreateValidator().ValidateAsync(model);

            Assert.Contains(result.Errors, error => error.PropertyName == nameof(model.GroupId)
                && error.ErrorMessage == "Please enter a Group ID");
            _groupReadService.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData("")]
        [InlineData("invalid")]
        [InlineData("2147483648")]
        public async Task ValidateAsync_InvalidUrn_DoesNotCallBackend(string urn)
        {
            var model = new GroupEditorViewModel { Action = GroupEditorViewModel.ActionLinkedEstablishmentSearch };
            model.LinkedEstablishments.LinkedEstablishmentSearch.Urn = urn;

            var result = await CreateValidator().ValidateAsync(model);

            var error = Assert.Single(result.Errors);
            Assert.Equal("LinkedEstablishments.LinkedEstablishmentSearch.Urn", error.PropertyName);
            Assert.Equal("Please enter a valid URN", error.ErrorMessage);
            _establishmentReadService.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(eLookupGroupType.MultiacademyTrust)]
        [InlineData(eLookupGroupType.ChildrensCentresGroup)]
        public async Task ValidateAsync_UnmatchedUrn_ReturnsNotFoundMessage(eLookupGroupType groupType)
        {
            var model = new GroupEditorViewModel
            {
                Action = GroupEditorViewModel.ActionLinkedEstablishmentSearch,
                GroupTypeId = (int) groupType
            };
            model.LinkedEstablishments.LinkedEstablishmentSearch.Urn = "123456";
            _establishmentReadService.Setup(x => x.GetAsync(123456, It.IsAny<IPrincipal>()))
                .ReturnsAsync(new ServiceResultDto<EstablishmentModel>(eServiceResultStatus.NotFound));

            var result = await CreateValidator().ValidateAsync(model);

            Assert.Equal("The establishment was not found", Assert.Single(result.Errors).ErrorMessage);
            _establishmentReadService.Verify(x => x.GetAsync(123456, It.IsAny<IPrincipal>()), Times.Once);
        }

        [Fact]
        public async Task ValidateAsync_MatchedUrn_IsValid()
        {
            var model = new GroupEditorViewModel
            {
                Action = GroupEditorViewModel.ActionLinkedEstablishmentSearch,
                GroupTypeId = (int) eLookupGroupType.MultiacademyTrust
            };
            model.LinkedEstablishments.LinkedEstablishmentSearch.Urn = "123456";
            _establishmentReadService.Setup(x => x.GetAsync(123456, It.IsAny<IPrincipal>()))
                .ReturnsAsync(new ServiceResultDto<EstablishmentModel>(new EstablishmentModel { Urn = 123456 }));

            var result = await CreateValidator().ValidateAsync(model);

            Assert.True(result.IsValid);
            _establishmentReadService.Verify(x => x.GetAsync(123456, It.IsAny<IPrincipal>()), Times.Once);
        }
    }
}
