using System.Collections.Generic;
using System.Linq;
using Edubase.Services.Governors.Models;
using Edubase.Web.UI.Areas.Governors.Models;
using Xunit;

namespace Edubase.Web.UIUnitTests.Areas.Governors.Models
{
    public sealed class ReplaceChairViewModelTests
    {
        [Fact]
        public void ReplaceChair_SelectedPreviousExistingNonChairId_AssignsSelectedNonChair()
        {
            // Arrange
            var model = new ReplaceChairViewModel
            {
                SelectedPreviousExistingNonChairId = 200
            };

            var localGovernors = new List<GovernorModel>
            {
                new GovernorModel { Id = 100, RoleId = 1 },
                new GovernorModel { Id = 200, RoleId = 2 },
                new GovernorModel { Id = 300, RoleId = 3 }
            };

            // Act
            if (model.SelectedPreviousExistingNonChairId.HasValue)
            {
                model.SelectedNonChair =
                    localGovernors.FirstOrDefault(x =>
                        x.Id == model.SelectedPreviousExistingNonChairId);
            }

            // Assert
            Assert.NotNull(model.SelectedNonChair);
            Assert.Equal(200, model.SelectedNonChair.Id);
        }
    }
}
