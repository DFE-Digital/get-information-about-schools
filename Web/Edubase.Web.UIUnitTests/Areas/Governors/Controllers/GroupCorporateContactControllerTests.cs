using System.Security.Principal;
using System.Threading.Tasks;
using System.Web.Mvc;
using Edubase.Services.Domain;
using Edubase.Services.Groups;
using Edubase.Services.Groups.Models;
using Edubase.Web.UI.Areas.Governors.Controllers;
using Edubase.Web.UI.Areas.Governors.Models;
using Edubase.Web.UI.Helpers;
using Moq;
using Xunit;

namespace Edubase.Web.UIUnitTests.Areas.Governors.Controllers
{
    public class GroupCorporateContactControllerTests
    {
        private readonly GroupCorporateContactController controller;
        private readonly Mock<IGroupReadService> mockGroupReadService = new Mock<IGroupReadService>();
        private readonly Mock<IGroupsWriteService> mockGroupsWriteService = new Mock<IGroupsWriteService>();
        private readonly Mock<UrlHelper> mockUrlHelper = new Mock<UrlHelper>(MockBehavior.Loose);
        private readonly Mock<ILayoutHelper> mockLayoutHelper = new Mock<ILayoutHelper>(MockBehavior.Loose);

        public GroupCorporateContactControllerTests()
        {
            mockUrlHelper.Setup(u => u.RouteUrl(It.IsAny<string>(), It.IsAny<object>())).Returns("test-url");
            controller = new GroupCorporateContactController(mockGroupReadService.Object, mockGroupsWriteService.Object, mockLayoutHelper.Object)
            {
                Url = mockUrlHelper.Object
            };
        }

        [Fact]
        public async Task GroupEditCorporateContact_Success()
        {                                
            var viewModel = new EditGroupCorporateContactViewModel
            {
                GroupUId = 999,
                CorporateContact = "a corporate contact"
            };

            var groupModel = new GroupModel();
            mockGroupReadService.Setup(x => x.GetAsync(999, It.IsAny<IPrincipal>()))
                .ReturnsAsync(() => new ServiceResultDto<GroupModel>
                {
                    Status = eServiceResultStatus.Success,
                    ReturnValue = groupModel
                });

            mockGroupsWriteService.Setup(x => x.SaveAsync(It.IsAny<SaveGroupDto>(), It.IsAny<IPrincipal>()))
                .ReturnsAsync(() => new ApiResponse(true));
                      
            var actionResult = await controller.GroupEditCorporateContact(viewModel);
            var result = Assert.IsType<RedirectResult>(actionResult);

            Assert.Equal("test-url#governance", result.Url);
        }

        [Fact]
        public async Task GroupEditCorporateContact_Validator_Error()
        {
            var viewModel = new EditGroupCorporateContactViewModel
            {
                GroupUId = 999,
                CorporateContact = new string('x', 151)
            };
          
            var result = await controller.GroupEditCorporateContact(viewModel) as ViewResult;

            Assert.NotNull(result);
            Assert.False(result.ViewData.ModelState.IsValid);
            Assert.True(result.ViewData.ModelState.ContainsKey("CorporateContact"));
            var errors = result.ViewData.ModelState["CorporateContact"].Errors;
            Assert.Single(errors);
            Assert.Equal("Corporate contact must be 150 characters or less", errors[0].ErrorMessage);            
        }

    }
}
