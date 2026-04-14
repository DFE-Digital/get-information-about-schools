using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Edubase.Services.Domain;
using Edubase.Services.Establishments.Models;
using Edubase.Services.Governors;
using Edubase.Services.Governors.Models;
using Edubase.Services.Groups.Models;
using Edubase.Services.Lookup;
using Edubase.Web.UI.Areas.Governors.Models;
using Edubase.Web.UI.Helpers;
using Moq;

namespace Edubase.Web.UIUnitTests.Areas.Governors.Helpers
{
    internal static class WireEditHelper
    {
        public static void WireEdit(
            int estabId,
            GovernorsDetailsDto dto,
            Mock<IGovernorsReadService> mockRead,
            Mock<ICachedLookupService> mockCache,
            Mock<ILayoutHelper> mockLayout)
        {
            mockRead.Setup(s => s.GetGovernorListAsync(estabId, null, It.IsAny<IPrincipal>()))
                .ReturnsAsync(dto);

            mockRead.Setup(s => s.GetGovernorPermissions(estabId, null, It.IsAny<IPrincipal>()))
                .ReturnsAsync(new GovernorPermissions());

            mockCache.Setup(s => s.GovernorRolesGetAllAsync())
                .ReturnsAsync(new List<LookupDto>());
            mockCache.Setup(s => s.TitlesGetAllAsync())
                .ReturnsAsync(new List<LookupDto>());
            mockCache.Setup(s => s.NationalitiesGetAllAsync())
                .ReturnsAsync(new List<LookupDto>());
            mockCache.Setup(s => s.GovernorAppointingBodiesGetAllAsync())
                .ReturnsAsync(new List<LookupDto>());

            mockLayout.Setup(l => l.PopulateLayoutProperties(
                    It.IsAny<GovernorsGridViewModel>(),
                    estabId, null,
                    It.IsAny<IPrincipal>(),
                    It.IsAny<Action<EstablishmentModel>>(),
                    It.IsAny<Action<GroupModel>>()))
                .Returns(Task.CompletedTask);
        }
    }
}
