using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Threading.Tasks;
using Edubase.Services.Domain;
using Edubase.Services.Establishments;
using Edubase.Services.Establishments.Models;
using Edubase.Services.Lookup;
using Edubase.Web.UI.Models;
using Edubase.Web.UI.Models.Tools;
using Moq;
using Xunit;
using Microsoft.AspNetCore.Mvc;

namespace Edubase.Web.UI.Controllers.UnitTests
{
    public class AmalgamateMergeControllerTests: IDisposable
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

        private readonly ApiResponse<AmalgamateMergeResult, AmalgamateMergeValidationEnvelope[]> amalgamateMergeApiResponse =
            new ApiResponse<AmalgamateMergeResult, AmalgamateMergeValidationEnvelope[]>(true)
            {
                Response = new AmalgamateMergeResult() { AmalgamateNewEstablishmentUrn = 101 }
            };
        private bool disposedValue;

        public AmalgamateMergeControllerTests()
        {
            controller = new AmalgamateMergeController(
                mockEstablishmentReadService.Object, mockEstablishmentWriteService.Object, mockCachedLookupService.Object);


            mockEstablishmentReadService.Setup(x => x.GetAsync(It.IsAny<int>(), It.IsAny<IPrincipal>()))
                .ReturnsAsync(establishmentServiceResultNull);

            mockEstablishmentReadService.Setup(x => x.GetAsync(It.IsInRange<int>(100, 110, Range.Inclusive), It.IsAny<IPrincipal>()))
                .ReturnsAsync(establishmentServiceResultTestEstablishement);

            mockEstablishmentWriteService.Setup(x => x.AmalgamateOrMergeAsync(It.IsAny<AmalgamateMergeRequest>(), It.IsAny<IPrincipal>()))
                .ReturnsAsync(amalgamateMergeApiResponse);

            mockCachedLookupService.Setup(x => x.LocalAuthorityGetAllAsync())
                .ReturnsAsync(new List<LookupDto> { new LookupDto { Id = 1, Code = "TEST", Name = "TEST", DisplayOrder = 1 } });
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
            int? year,
            int? month,
            int? day,
            bool includeLeadEstablishmentUrn,
            bool includeUrns,
            bool errorsInApiResponse,
            bool successExpected)
        {
            var model = new MergeEstablishmentsModel();

            if (year == 0 || month == 0 || day == 0)
            {
                model.MergeDate = new DateTimeViewModel();
            }
            else if (year != null && month != null && day != null)
            {
                model.MergeDate = new DateTimeViewModel { Year = year, Month = month, Day = day, };
            }

            if (includeLeadEstablishmentUrn)
            {
                model.LeadEstablishmentUrn = 100;
            }

            if (includeUrns)
            {
                model.Establishment1Urn = 1;
                model.Establishment2Urn = 2;
                model.Establishment3Urn = 3;
            }

            if (errorsInApiResponse)
            {

                var errors = new List<ApiError> { new ApiError { Code = "T35T", Message = "TEST", Fields = "TESTFIELD" } };
                amalgamateMergeApiResponse.Errors = errors.ToArray();
            }

            var expectedViewName = successExpected ? @"~/Views/Tools/Mergers/MergerComplete.cshtml" : @"~/Views/Tools/Mergers/ConfirmMerger.cshtml";

            var result = await controller.ProcessMergeAsync(model) as ViewResult;

            Assert.NotNull(result);
            Assert.Equal(expectedViewName, result.ViewName);
            Assert.True(successExpected == result.ViewData.ModelState.IsValid);
        }

        public static IEnumerable<object[]> GetProcessMergeAsyncTestData()
        {
            var allData = new List<object[]>
            {
                new object[] { null, null, null, false,  false, false, false },
                new object[] { 0, 0, 0, true, true, false, false },
                new object[] { 2020, 02, 31, true, true, false, false },
                new object[] { 2021, 05, 06, true, false, false, false },
                new object[] { 2021, 05, 06, false, true, false, false },
                new object[] { 2021, 05, 06, true, true, true, false },
                new object[] { 2021, 05, 06, true, true, false, true },
            };
            return allData;
        }

        [Theory()]
        [MemberData(nameof(GetProcessAmalgamationEstablishmentsAsyncTestTestData))]
        public async Task ProcessAmalgamationEstablishmentsAsyncTest(
            bool est0HasErrors,
            bool est1HasErrors,
            int? est0Urn,
            int? est1Urn,
            int? est2Urn,
            int? est3Urn,
            bool modelHasErrors,
            bool successExpected)
        {
            var model = new AmalgamateEstablishmentsModel
            {
                Establishment0Urn = est0Urn,
                Establishment1Urn = est1Urn,
                Establishment2Urn = est2Urn,
                Establishment3Urn = est3Urn
            };

            if (est0HasErrors)
            {
                controller.ViewData.ModelState.AddModelError("Establishment0Urn", "test error");
            }

            if (est1HasErrors)
            {
                controller.ViewData.ModelState.AddModelError("Establishment1Urn", "test error");
            }

            if (modelHasErrors)
            {
                controller.ViewData.ModelState.AddModelError("test", "test error");
            }

            var expectedViewName = successExpected ? @"~/Views/Tools/Mergers/ConfirmAmalgamation.cshtml"
                : @"~/Views/Tools/Mergers/AmalgamateEstablishments.cshtml";

            var result = await controller.ProcessAmalgamationEstablishmentsAsync(model) as ViewResult;

            Assert.NotNull(result);
            Assert.True(successExpected == result.ViewData.ModelState.IsValid);
            Assert.Equal(expectedViewName, result.ViewName);
        }

        public static IEnumerable<object[]> GetProcessAmalgamationEstablishmentsAsyncTestTestData()
        {
            var allData = new List<object[]>
            {
                //              est0Errors  est1Errors  est0Urn est1Urn est2Urn est3Urn modelError  success
                //everything wrong
                new object[] {  true,       true,       null,   null,   null,   null,   true,       false },

                //one thing wrong
                new object[] {  true,       false,      101,    102,    103,    104,    false,      false },
                new object[] {  false,      true,       101,    102,    103,    104,    false,      false },
                new object[] {  false,      false,      null,   102,    103,    104,    false,      false },
                new object[] {  false,      false,      101,    null,   null,   null,   false,      false },
                new object[] {  false,      false,      101,    102,    103,    104,    true,       false },

                //Not found Ids
                new object[] {  false,      false,      1,      102,    103,    104,    false,      false },
                new object[] {  false,      false,      101,    2,      103,    104,    false,      false },
                new object[] {  false,      false,      101,    102,    3,      104,    false,      false },
                new object[] {  false,      false,      101,    102,    103,    4,      false,      false },
                new object[] {  false,      false,      1,      2,      103,    104,    false,      false },
                new object[] {  false,      false,      1,      102,    3,      104,    false,      false },
                new object[] {  false,      false,      1,      102,    103,    4,      false,      false },
                new object[] {  false,      false,      101,    2,      3,      104,    false,      false },
                new object[] {  false,      false,      101,    2,      103,    4,      false,      false },
                new object[] {  false,      false,      101,    102,    3,      4,      false,      false },
                new object[] {  false,      false,      1,      2,      3,      104,    false,      false },
                new object[] {  false,      false,      1,      2,      103,    4,      false,      false },
                new object[] {  false,      false,      1,      102,    3,      4,      false,      false },
                new object[] {  false,      false,      101,    2,      3,      4,      false,      false },
                new object[] {  false,      false,      1,      2,      3,      4,      false,      false },

                //duplicate IDS
                new object[] {  false,      false,      101,    101,    103,    104,    false,      false },
                new object[] {  false,      false,      101,    102,    101,    104,    false,      false },
                new object[] {  false,      false,      101,    102,    103,    101,    false,      false },
                new object[] {  false,      false,      101,    102,    102,    104,    false,      false },
                new object[] {  false,      false,      101,    102,    103,    102,    false,      false },
                new object[] {  false,      false,      101,    102,    103,    103,    false,      false },
                new object[] {  false,      false,      101,    101,    101,    104,    false,      false },
                new object[] {  false,      false,      101,    101,    103,    101,    false,      false },
                new object[] {  false,      false,      101,    102,    101,    101,    false,      false },
                new object[] {  false,      false,      101,    102,    102,    102,    false,      false },
                new object[] {  false,      false,      101,    101,    101,    101,    false,      false },

                //should be valid
                new object[] {  false,      false,      101,    102,    103,    104,    false,      true },
                new object[] {  false,      false,      101,    102,    103,    null,   false,      true },
                new object[] {  false,      false,      101,    102,    null,   104,    false,      true },
                new object[] {  false,      false,      101,    null,   103,    104,    false,      true },
                new object[] {  false,      false,      101,    102,    null,   null,   false,      true },
                new object[] {  false,      false,      101,    null,   103,    null,   false,      true },
                new object[] {  false,      false,      101,    null,   null,   104,    false,      true },
            };
            return allData;
        }

        [Theory()]
        [MemberData(nameof(GetProcessAmalgamationAsyncTestData))]
        public async Task ProcessAmalgamationAsyncTest(
            int? mergeDateYear,
            int? mergeDateMonth,
            int? mergeDateDay,
            string newEstablishmentName,
            string establishmentType,
            int? establishmentPhase,
            string localAuthorityId,
            bool modelIsValid,
            int? est0Urn,
            int? est1Urn,
            int? est2Urn,
            int? est3Urn,
            bool resultHasErrors,
            bool successExpected
            )
        {
            var unsuccessfulViewName = @"~/Views/Tools/Mergers/ConfirmAmalgamation.cshtml";
            var successfulViewName = @"~/Views/Tools/Mergers/AmalgamationComplete.cshtml";
            var expectedViewName = successExpected ? successfulViewName : unsuccessfulViewName;

            var model = new AmalgamateEstablishmentsModel()
            {
                MergeDate = new DateTimeViewModel() { Year = mergeDateYear, Month = mergeDateMonth, Day = mergeDateDay },
                NewEstablishmentName = newEstablishmentName,
                EstablishmentType = establishmentType,
                EstablishmentPhase = establishmentPhase,
                LocalAuthorityId = localAuthorityId,
                Establishment0Urn = est0Urn,
                Establishment1Urn = est1Urn,
                Establishment2Urn = est2Urn,
                Establishment3Urn = est3Urn,
            };

            if(!modelIsValid)
            {
                controller.ViewData.ModelState.AddModelError("TEST", "TEST ERROR");
            }

            if(resultHasErrors)
            {
                var errors = new List<ApiError> { new ApiError { Code = "T35T", Message = "TEST", Fields = "TESTFIELD" } };
                amalgamateMergeApiResponse.Errors = errors.ToArray();
            }

            var result = await controller.ProcessAmalgamationAsync(model) as ViewResult;

            Assert.NotNull(result);
            Assert.Equal(successExpected, result.ViewData.ModelState.IsValid);
            Assert.Equal(expectedViewName, result.ViewName);
        }

        public static IEnumerable<object[]> GetProcessAmalgamationAsyncTestData()
        {
            var allData = new List<object[]>
            {
                //              mergeDate       newEstName  estType estPhase    locAuthId   modelValid  est0Urn est1Urn est2Urn est3Urn resultError succesful
                //all correct
                new object[] {  2021, 01, 20,   "test sch", "1",    1,          "1",        true,       101,    102,    103,    104,    false,      true},
                //dates                                             
                new object[] {  2021, 13, 01,   "test sch", "1",    1,          "1",        true,       101,    102,    103,    104,    false,      false},
                new object[] {  2021, 01, 32,   "test sch", "1",    1,          "1",        true,       101,    102,    103,    104,    false,      false},
                new object[] {  2021, 02, 29,   "test sch", "1",    1,          "1",        true,       101,    102,    103,    104,    false,      false},
                new object[] {  2020, 02, 29,   "test sch", "1",    1,          "1",        true,       101,    102,    103,    104,    false,      true},
                new object[] {  2100, 02, 29,   "test sch", "1",    1,          "1",        true,       101,    102,    103,    104,    false,      false},
                new object[] { null, null, null,"test sch", "1",    1,          "1",        true,       101,    102,    103,    104,    false,      false},
                new object[] {  2021, 01, null, "test sch", "1",    1,          "1",        true,       101,    102,    103,    104,    false,      false},
                new object[] {  2021, null, 08, "test sch", "1",    1,          "1",        true,       101,    102,    103,    104,    false,      false},
                new object[] {  null, null, 08, "test sch", "1",    1,          "1",        true,       101,    102,    103,    104,    false,      false},
                new object[] {  null, 01, null, "test sch", "1",    1,          "1",        true,       101,    102,    103,    104,    false,      false},
                new object[] { 2021, null, null,"test sch", "1",    1,          "1",        true,       101,    102,    103,    104,    false,      false},
                //estName                                           
                new object[] {  2021, 01, 20,   null,       "1",    1,          "1",        true,       101,    102,    103,    104,    false,      false},
                new object[] {  2021, 01, 20,   "        ", "1",    1,          "1",        true,       101,    102,    103,    104,    false,      false},
                new object[] {  2021, 01, 20,   "",         "1",    1,          "1",        true,       101,    102,    103,    104,    false,      false},
                //estType                                           
                new object[] {  2021, 01, 20,   "test sch", null,   1,          "1",        true,       101,    102,    103,    104,    false,      false},
                new object[] {  2021, 01, 20,   "test sch", "",     1,          "1",        true,       101,    102,    103,    104,    false,      false},
                new object[] {  2021, 01, 20,   "test sch", " ",    1,          "1",        true,       101,    102,    103,    104,    false,      false},
                new object[] {  2021, 01, 20,   "test sch", "text", 1,          "1",        true,       101,    102,    103,    104,    false,      true},
                //estPhase
                new object[] {  2021, 01, 20,   "test sch", "1",    null,       "1",        true,       101,    102,    103,    104,    false,      false},
                //localAuthorityId
                new object[] {  2021, 01, 20,   "test sch", "1",    1,          null,       true,       101,    102,    103,    104,    false,      false},
                new object[] {  2021, 01, 20,   "test sch", "1",    1,          " ",        true,       101,    102,    103,    104,    false,      false},
                new object[] {  2021, 01, 20,   "test sch", "1",    1,          "",         true,       101,    102,    103,    104,    false,      false},
                new object[] {  2021, 01, 20,   "test sch", "1",    1,          "text",     true,       101,    102,    103,    104,    false,      true},
                //modelValid                                        
                new object[] {  2021, 01, 20,   "test sch", "1",    1,          "1",        false,      101,    102,    103,    104,    false,      false},
                //Urns-accepatable                                  
                new object[] {  2021, 01, 20,   "test sch", "1",    1,          "1",        true,       101,    102,    null,   null,   false,      true},
                new object[] {  2021, 01, 20,   "test sch", "1",    1,          "1",        true,       101,    null,   103,    null,   false,      true},
                new object[] {  2021, 01, 20,   "test sch", "1",    1,          "1",        true,       101,    null,   null,   104,    false,      true},
                new object[] {  2021, 01, 20,   "test sch", "1",    1,          "1",        true,       101,    102,    103,    null,   false,      true},
                new object[] {  2021, 01, 20,   "test sch", "1",    1,          "1",        true,       101,    102,    null,   104,    false,      true},
                new object[] {  2021, 01, 20,   "test sch", "1",    1,          "1",        true,       101,    null,   103,    104,    false,      true},
                new object[] {  2021, 01, 20,   "test sch", "1",    1,          "1",        true,       null,   102,    103,    null,   false,      true},
                new object[] {  2021, 01, 20,   "test sch", "1",    1,          "1",        true,       null,   102,    null,   104,    false,      true},
                new object[] {  2021, 01, 20,   "test sch", "1",    1,          "1",        true,       null,   null,   103,    104,    false,      true},
                new object[] {  2021, 01, 20,   "test sch", "1",    1,          "1",        true,       null,   102,    103,    104,    false,      true},
                //Urns-unacceptable
                new object[] {  2021, 01, 20,   "test sch", "1",    1,          "1",        true,       101,    null,   null,   null,   false,      false},
                new object[] {  2021, 01, 20,   "test sch", "1",    1,          "1",        true,       null,   102,    null,   null,   false,      false},
                new object[] {  2021, 01, 20,   "test sch", "1",    1,          "1",        true,       null,   null,   103,    null,   false,      false},
                new object[] {  2021, 01, 20,   "test sch", "1",    1,          "1",        true,       null,   null,   null,   104,    false,      false},
                new object[] {  2021, 01, 20,   "test sch", "1",    1,          "1",        true,       null,   null,   null,   null,   false,      false},
                //resultError
                new object[] {  2021, 01, 20,   "test sch", "1",    1,          "1",        true,       null,   null,   103,    104,    true,       false},
            };
            return allData;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    controller.Dispose();
                }

                disposedValue = true;
            }
        }


        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
