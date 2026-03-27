using System;
using System.Security.Principal;
using System.Threading.Tasks;
using Edubase.Services.Domain;
using Edubase.Services.Enums;
using Edubase.Services.Governors.Models;
using Edubase.Web.UIUnitTests.Areas.Governors.TestBase.Edubase.Web.UIUnitTests.Areas.Governors.TestBase;
using Moq;
using Xunit;

namespace Edubase.Web.UIUnitTests.Areas.Governors.Controllers.ReplaceChair
{
    public class ReplaceChairReinstateControllerPostTests : GovernorControllerTestBase
    {
        [Theory]
        [InlineData(eLookupGovernorRole.ChairOfGovernors, eLookupGovernorRole.Governor)]
        [InlineData(eLookupGovernorRole.ChairOfLocalGoverningBody, eLookupGovernorRole.LocalGovernor)]
        [InlineData(eLookupGovernorRole.ChairOfTrustees, eLookupGovernorRole.Trustee)]
        public async Task ReInstateChairAsNonChairAsync_MapsChairRoleToCorrectTargetRole(
            eLookupGovernorRole currentRole,
            eLookupGovernorRole expectedTargetRole)
        {
            // Arrange
            var controller = BuildController();
            var gid = 42;
            var start = new DateTime(2025, 1, 1);
            var end = new DateTime(2025, 12, 31);

            var existing = new GovernorModel
            {
                Id = gid,
                RoleId = (int) currentRole,
                EmailAddress = "old@example.com",
                TelephoneNumber = "0123456789"
            };

            mockGovernorsReadService
                .Setup(g => g.GetGovernorAsync(gid, It.IsAny<IPrincipal>()))
                .Returns(Task.FromResult(existing));

            GovernorModel savedModel = null;

            mockGovernorsWriteService
                .Setup(g => g.SaveAsync(It.IsAny<GovernorModel>(), It.IsAny<IPrincipal>()))
                .Callback<GovernorModel, IPrincipal>((m, _) => savedModel = m)
                .Returns(Task.FromResult(new ApiResponse<int>(true)));

            // Act
            await controller.ReInstateChairAsNonChairAsync(gid, start, end, currentRole);

            // Assert
            mockGovernorsReadService.Verify(
                g => g.GetGovernorAsync(gid, It.IsAny<IPrincipal>()),
                Times.Once);

            mockGovernorsWriteService.Verify(
                g => g.SaveAsync(It.IsAny<GovernorModel>(), It.IsAny<IPrincipal>()),
                Times.Once);

            Assert.NotNull(savedModel);
            Assert.Equal((int) expectedTargetRole, savedModel.RoleId);
            Assert.Null(savedModel.Id);
            Assert.Equal(start, savedModel.AppointmentStartDate);
            Assert.Equal(end, savedModel.AppointmentEndDate);
            Assert.Null(savedModel.EmailAddress);
            Assert.Null(savedModel.TelephoneNumber);
        }

        [Fact]
        public async Task ReInstateChairAsNonChairAsync_InvalidRole_ThrowsException()
        {
            // Arrange
            var controller = BuildController();
            var gid = 123;
            var invalidRole = eLookupGovernorRole.Governor;
            var appointmentStart = new DateTime(2025, 1, 1);
            var appointmentEnd = new DateTime(2025, 2, 1);

            mockGovernorsReadService
                .Setup(s => s.GetGovernorAsync(gid, It.IsAny<IPrincipal>()))
                .ReturnsAsync(new GovernorModel
                {
                    Id = gid,
                    RoleId = (int) invalidRole
                });

            // Act + Assert
            var ex = await Assert.ThrowsAsync<Exception>(() =>
                controller.ReInstateChairAsNonChairAsync(
                    gid,
                    appointmentStart,
                    appointmentEnd,
                    invalidRole
                ));

            Assert.Contains("You cannot demote from role", ex.Message);
            Assert.Contains(invalidRole.ToString(), ex.Message);

            // Ensure SaveAsync was NOT called
            mockGovernorsWriteService.Verify(
                w => w.SaveAsync(It.IsAny<GovernorModel>(), It.IsAny<IPrincipal>()),
                Times.Never);
        }
    }
}
