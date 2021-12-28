using Edubase.Web.UI.Controllers;
using System.Web.Mvc;
using Edubase.Services.Establishments;
using Edubase.Services.Lookup;
using Moq;
using Xunit;
using System.Threading.Tasks;
using System.Security.Principal;
using Edubase.Services.Domain;
using Edubase.Services.Establishments.Models;
using System.Collections.Generic;
using Edubase.Web.UI.Models.Tools;
using Edubase.Web.UI.Models;

namespace Edubase.Web.UI.Controllers.Tests
{
    public class AmalgamateMergeControllerTests
    {
        private readonly Mock<IEstablishmentReadService> mockEstablishmentReadService = new Mock<IEstablishmentReadService>();
        private readonly Mock<IEstablishmentWriteService> mockEstablishmentWriteService = new Mock<IEstablishmentWriteService>();
        private readonly Mock<ICachedLookupService> mockCachedLookupService = new Mock<ICachedLookupService>();

        private readonly AmalgamateMergeController controller;
        private readonly ServiceResultDto<EstablishmentModel> establishmentServiceResultNull =
            new ServiceResultDto<EstablishmentModel>(eServiceResultStatus.Success);
        private readonly ServiceResultDto<EstablishmentModel> establishmentServiceResultTestEstablishement =
            new ServiceResultDto<EstablishmentModel>(eServiceResultStatus.Success)
            {
                ReturnValue = new EstablishmentModel()
                {
                    Name = "test establishment"
                }
            };

        public AmalgamateMergeControllerTests()
        {
            controller = new AmalgamateMergeController(
                mockEstablishmentReadService.Object, mockEstablishmentWriteService.Object, mockCachedLookupService.Object);


            mockEstablishmentReadService.Setup(x => x.GetAsync(It.IsAny<int>(), It.IsAny<IPrincipal>()))
                .ReturnsAsync(establishmentServiceResultNull);
            mockEstablishmentReadService.Setup(x => x.GetAsync(It.IsInRange<int>(100, 110, Range.Inclusive), It.IsAny<IPrincipal>()))
                .ReturnsAsync(establishmentServiceResultTestEstablishement);
        }

        [Fact()]
        public void AmalgamateMergeControllerTest()
        {
            Assert.NotNull(controller);
        }

        [Theory()]
        [InlineData("Merger", "MergeEstablishments")]
        [InlineData("Amalgamation", "AmalgamateEstablishments")]
        public void SelectMergerTypeTest_Redirections(string mergerType, string expectedAction)
        {
            var result = controller.SelectMergerType(mergerType) as RedirectToRouteResult;
            var action = result.RouteValues["action"];

            Assert.NotNull(result);
            Assert.Equal(expectedAction, action);
        }

        [Theory()]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("something-unlikely")]
        public void SelectMergerTypeTest_ErrorAddedIfNotMergerOrAmalagamation(string mergerType)
        {
            var result = controller.SelectMergerType(mergerType) as ViewResult;

            Assert.NotNull(result);
            Assert.True(result.ViewData.ModelState.TryGetValue("MergerType", out var modelState));
            Assert.True(modelState.Errors.Count > 0);
        }

        [Fact()]
        public void AmalgamateEstablishmentsTest()
        {
            var result = controller.AmalgamateEstablishments();
            Assert.IsType<ViewResult>(result);
        }

        [Fact()]
        public void MergeEstablishmentsTest()
        {
            var result = controller.MergeEstablishments();
            Assert.IsType<ViewResult>(result);
        }

        [Theory()]
        [MemberData(nameof(GetProcessMergeEstablishmentsAsyncTestData))]
        public async Task ProcessMergeEstablishmentsAsyncTestAsync(
            bool leadUrnHasErrors,
            bool est1HasErrors,
            bool est2HasErrors,
            bool est3HasErrors,
            int? leadUrn,
            int? est1Urn,
            int? est2Urn,
            int? est3Urn,
            bool successExpected
            )
        {
            var expectedViewName =
                successExpected ? @"~/Views/Tools/Mergers/ConfirmMerger.cshtml" : @"~/Views/Tools/Mergers/MergeEstablishments.cshtml";

            if (leadUrnHasErrors)
            {
                controller.ViewData.ModelState.AddModelError("LeadEstablishmentUrn", "TEST ERROR");
            }

            if (est1HasErrors)
            {
                controller.ViewData.ModelState.AddModelError("Establishment1Urn", "TEST ERROR");
            }

            if (est2HasErrors)
            {
                controller.ViewData.ModelState.AddModelError("Establishment2Urn", "TEST ERROR");
            }

            if (est3HasErrors)
            {
                controller.ViewData.ModelState.AddModelError("Establishment3Urn", "TEST ERROR");
            }

            var model = new MergeEstablishmentsModel()
            {
                LeadEstablishmentUrn = leadUrn,
                Establishment1Urn = est1Urn,
                Establishment2Urn = est2Urn,
                Establishment3Urn = est3Urn
            };

            var result = await controller.ProcessMergeEstablishmentsAsync(model) as ViewResult;
            Assert.NotNull(result);
            Assert.Equal(expectedViewName, result.ViewName);
            Assert.True(successExpected == result.ViewData.ModelState.IsValid);
        }

        public static IEnumerable<object[]> GetProcessMergeEstablishmentsAsyncTestData()
        {
            var allData = new List<object[]>
            {
                new object[] { false, false, false, true, 101, 102, 103, 104, false},
                new object[] { false, false, true, false, 101, 102, 103, 104, false},
                new object[] { false, true, false, false, 101, 102, 103, 104, false},
                new object[] { true, false, false, false, 101, 102, 103, 104, false},
                new object[] { true, true, false, false, 101, 102, 103, 104, false},
                new object[] { true, false, true, false, 101, 102, 103, 104, false},
                new object[] { true, false, false, true, 101, 102, 103, 104, false},
                new object[] { false, true, true, false, 101, 102, 103, 104, false},
                new object[] { false, true, false, true, 101, 102, 103, 104, false},
                new object[] { false, false, true, true, 101, 102, 103, 104, false},
                new object[] { true, true, true, false, 101, 102, 103, 104, false},
                new object[] { true, true, false, true, 101, 102, 103, 104, false},
                new object[] { true, false, true, true, 101, 102, 103, 104, false},
                new object[] { false, true, true, true, 101, 102, 103, 104, false},
                new object[] { true, true, true, true, 101, 102, 103, 104, false},
                new object[] { false, false, false, false, null, null, null, null, false},
                new object[] { false, false, false, false, null, 102, 103, 104, false},
                new object[] { false, false, false, false, 101, null, null, null, false},
                new object[] { false, false, false, false, 1, 2, 3, 4, false},
                new object[] { false, false, false, false, 1, 102, 103, 104, false},
                new object[] { false, false, false, false, 101, 2, 103, 104, false},
                new object[] { false, false, false, false, 101, 102, 3, 104, false},
                new object[] { false, false, false, false, 101, 102, 103, 4, false},
                new object[] { false, false, false, false, 101, 101, 101, 101, false},
                new object[] { false, false, false, false, 101, 101, 103, 104, false},
                new object[] { false, false, false, false, 101, 102, 101, 104, false},
                new object[] { false, false, false, false, 101, 102, 103, 101, false},
                new object[] { false, false, false, false, 101, 102, 102, 104, false},
                new object[] { false, false, false, false, 101, 102, 103, 102, false},
                new object[] { false, false, false, false, 101, 102, 103, 103, false},
                new object[] { false, false, false, false, 101, 102, null, null, true},
                new object[] { false, false, false, false, 101, null, 103, null, true},
                new object[] { false, false, false, false, 101, null, null, 104, true},
                new object[] { false, false, false, false, 101, 102, 103, null, true},
                new object[] { false, false, false, false, 101, null, 103, 104, true},
                new object[] { false, false, false, false, 101, 102, 103, 104, true}
            };
            return allData;
        }

        [Theory()]
        [MemberData(nameof(GetProcessMergeAsyncTestData))]
        public async Task ProcessMergeAsyncTestAsync(
            DateTimeViewModel mergeDate,
            int? urn1,
            int? urn2,
            int? urn3,
            bool successExpected )
        {
            var model = new MergeEstablishmentsModel()
            {
                MergeDate = mergeDate,
                Establishment1Urn = urn1,
                Establishment2Urn = urn2,
                Establishment3Urn = urn3,
            };
            var expectedViewName = @"~/Views/Tools/Mergers/ConfirmMerger.cshtml";

            var result = await controller.ProcessMergeAsync(model) as ViewResult;

            Assert.NotNull(result);
            Assert.Equal(expectedViewName, result.ViewName);
            Assert.True(successExpected == result.ViewData.ModelState.IsValid);
        }

        public static IEnumerable<object[]> GetProcessMergeAsyncTestData()
        {
            var allData = new List<object[]>
            {
                new object[] { null, null, null, null, false },
                new object[] { new DateTimeViewModel(), null, null, null, false },
                new object[] { new DateTimeViewModel() { Year = 2020, Month = 02, Day = 31}, null, null, null, false },
                new object[] { new DateTimeViewModel() { Year = 2021, Month = 05, Day = 06}, null, null, null, false },
                new object[] { new DateTimeViewModel() { Year = 2021, Month = 05, Day = 06}, 1, null, null, false },
                new object[] { new DateTimeViewModel() { Year = 2021, Month = 05, Day = 06}, 1, 2, null, false },
                new object[] { new DateTimeViewModel() { Year = 2021, Month = 05, Day = 06}, 1, null, 3, false },
                new object[] { new DateTimeViewModel() { Year = 2021, Month = 05, Day = 06}, 1, 2, 3, false }
            };
            return allData;
        }
    }
}
