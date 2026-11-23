using System.Threading.Tasks;
using Edubase.Services.Groups;
using Edubase.Services.Security;
using Edubase.Web.UI.Areas.Groups.Controllers;
using Edubase.Web.UI.Areas.Groups.Models.CreateEdit;
using Edubase.Web.UI.Areas.Groups.Models.Validators;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Edubase.Web.UIUnitTests.Areas.Groups.Controllers
{
    public class GroupApiControllerTests
    {
        [Fact]
        public async Task ValidateChildrensCentreGroup_InvalidInput_ReturnsValidationErrorsWithoutCallingBackend()
        {
            var groupReadService = new Mock<IGroupReadService>(MockBehavior.Strict);
            var validator = new ValidateChildrensCentreStep2Validator(groupReadService.Object,
                Mock.Of<ISecurityService>());
            var controller = new GroupApiController(Mock.Of<IGroupsWriteService>(), validator);

            var result = await controller.ValidateChildrensCentreGroup(new ValidateChildrensCentreStep2
            {
                LocalAuthorityId = 1
            });

            Assert.IsType<JsonResult>(result);
            Assert.False(controller.ModelState.IsValid);
            Assert.True(controller.ModelState.ContainsKey(nameof(ValidateChildrensCentreStep2.OpenDate)));
            groupReadService.VerifyNoOtherCalls();
        }
    }
}
