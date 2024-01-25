using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Security.Claims;
using System.Security.Principal;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Edubase.Common;
using Edubase.Services.Core;
using Edubase.Services.Domain;
using Edubase.Services.Enums;
using Edubase.Services.Establishments;
using Edubase.Services.Establishments.Models;
using Edubase.Services.Exceptions;
using Edubase.Services.ExternalLookup;
using Edubase.Services.Governors;
using Edubase.Services.Governors.Models;
using Edubase.Services.Groups;
using Edubase.Services.Groups.Models;
using Edubase.Services.IntegrationEndPoints.CompaniesHouse;
using Edubase.Services.Lookup;
using Edubase.Services.Nomenclature;
using Edubase.Services.Security;
using Edubase.Services.Texuna;
using Edubase.Web.UI.Areas.Governors.Models;
using Edubase.Web.UI.Areas.Groups.Models;
using Edubase.Web.UI.Areas.Groups.Models.CreateEdit;
using Edubase.Web.UI.Areas.Groups.ViewRulesHandlers;
using Edubase.Web.UI.Exceptions;
using Edubase.Web.UIUnitTests;
using Faker;
using Moq;
using Xunit;
using Xunit.Abstractions;
using static Edubase.Web.UI.Areas.Groups.Models.CreateEdit.GroupEditorViewModel;
using static Edubase.Web.UI.Areas.Groups.Models.CreateEdit.GroupEditorViewModelBase;

namespace Edubase.Web.UI.Areas.Groups.Controllers.UnitTests
{
    public class GroupControllerTests : IDisposable
    {
        private readonly GroupController controller;
        private readonly Mock<ICachedLookupService> mockCachedLookupService;
        private readonly ITestOutputHelper output;

        private readonly Mock<IEstablishmentReadService> mockEstablishmentReadService = new Mock<IEstablishmentReadService>(MockBehavior.Strict);
        private readonly Mock<IGroupReadService> mockGroupReadService = new Mock<IGroupReadService>(MockBehavior.Loose);
        private readonly Mock<IGroupsWriteService> mockGroupsWriteService = new Mock<IGroupsWriteService>(MockBehavior.Strict);
        private readonly Mock<IGovernorsReadService> mockGovernorsReadService = new Mock<IGovernorsReadService>(MockBehavior.Strict);
        private readonly Mock<ICompaniesHouseService> mockCompaniesHouseService = new Mock<ICompaniesHouseService>(MockBehavior.Strict);
        private readonly Mock<ISecurityService> mockSecurityService = new Mock<ISecurityService>(MockBehavior.Strict);
        private readonly Mock<IExternalLookupService> mockExternalLookupService = new Mock<IExternalLookupService>(MockBehavior.Strict);
        private readonly Mock<NomenclatureService> mockNomenclatureService = new Mock<NomenclatureService>(MockBehavior.Strict);
        private readonly Mock<UrlHelper> mockUrlHelper = new Mock<UrlHelper>(MockBehavior.Loose);
        private readonly Mock<ControllerContext> mockControllerContext = new Mock<ControllerContext>(MockBehavior.Strict);
        private readonly Mock<HttpRequestBase> mockHttpRequestBase = new Mock<HttpRequestBase>(MockBehavior.Strict);
        private readonly Mock<HttpContextBase> mockHttpContextBase = new Mock<HttpContextBase>(MockBehavior.Strict);
        private readonly Mock<IPrincipal> mockPrincipal = new Mock<IPrincipal>(MockBehavior.Strict);
        private readonly Mock<IIdentity> mockIdentity = new Mock<IIdentity>(MockBehavior.Strict);
        private readonly Mock<IGovernorsGridViewModelFactory> mockGovernorGridViewModelFactory = new Mock<IGovernorsGridViewModelFactory>(MockBehavior.Loose);
        private bool disposedValue;

        public GroupControllerTests(ITestOutputHelper output)
        {
            this.output = output;
            mockCachedLookupService = MockHelper.SetupCachedLookupService();

            mockEstablishmentReadService.Setup(e => e.GetEstabType2EducationPhaseMap())
                .Returns(new Dictionary<eLookupEstablishmentType, eLookupEducationPhase[]>());

            mockUrlHelper.Setup(u => u.RouteUrl(It.IsAny<string>(), It.IsAny<object>()))
                .Returns("fake url");

            controller = new GroupController(
                mockCachedLookupService.Object,
                mockSecurityService.Object,
                mockGroupReadService.Object,
                mockEstablishmentReadService.Object,
                mockGroupsWriteService.Object,
                mockCompaniesHouseService.Object,
                mockNomenclatureService.Object,
                mockGovernorsReadService.Object,
                mockExternalLookupService.Object,
                mockGovernorGridViewModelFactory.Object);

            SetupController();
        }

        protected void SetupController()
        {
            SetupHttpRequest();
            controller.ControllerContext = mockControllerContext.Object;
            mockControllerContext.SetupGet(c => c.Controller).Returns(controller);
            controller.Url = mockUrlHelper.Object;
        }

        private void SetupHttpRequest()
        {
            mockHttpRequestBase.SetupGet(x => x.QueryString)
                .Returns(HttpUtility.ParseQueryString(string.Empty));
            mockHttpContextBase.SetupGet(x => x.Request)
                .Returns(mockHttpRequestBase.Object);
            mockHttpContextBase.SetupGet(x => x.User)
                .Returns(mockPrincipal.Object);
            mockControllerContext.SetupGet(x => x.HttpContext)
                .Returns(mockHttpContextBase.Object);
            mockControllerContext.SetupGet(x => x.IsChildAction)
                .Returns(false);
            mockControllerContext.SetupGet(x => x.RouteData)
                .Returns(new System.Web.Routing.RouteData());
            mockPrincipal.SetupGet(x => x.Identity)
                .Returns(mockIdentity.Object);
        }

        private void InjectBasicLAsAndGroupTypes()
        {
            var cls = mockCachedLookupService;
            cls.Setup(x => x.LocalAuthorityGetAllAsync()).ReturnsAsync(new LookupDto[] { new LookupDto { Name = "[placeholder_la]" } });
            cls.Setup(x => x.GroupTypesGetAllAsync()).ReturnsAsync(new LookupDto[] { new LookupDto { Name = "[placeholder_grouptype]", Id = 1 } });
            cls.Setup(x => x.GetNameAsync(It.IsAny<Expression<Func<int?>>>(), It.IsAny<string>())).ReturnsAsync("placeholder");
        }

        private GroupModel CreateGroupModel(eLookupGroupType groupType)
        {
            return new GroupModel
            {
                Address = new AddressDto
                {
                    CityOrTown = Faker.Address.City(),
                    Line1 = Faker.Address.StreetAddress(),
                    Line2 = Faker.Address.SecondaryAddress(),
                    Line3 = Faker.Address.UkCounty(),
                    PostCode = Faker.Address.UkPostCode()
                },
                ClosedDate = DateTime.Now,
                CompaniesHouseNumber = "67829662",
                ConfirmationUpToDateGovernanceRequired = true,
                ConfirmationUpToDateGovernance_LastConfirmationDate = DateTime.Now,
                DelegationInformation = "delinf",
                GroupId = "123",
                GroupTypeId = (int) groupType,
                GroupUId = 123,
                HeadFirstName = Faker.Name.First(),
                HeadLastName = Faker.Name.Last(),
                HeadTitleId = 1,
                LocalAuthorityId = 1,
                ManagerEmailAddress = Faker.Internet.Email(),
                Name = $"I am {groupType.ToString()}",
                OpenDate = DateTime.Now.Subtract(TimeSpan.FromDays(10)),
                StatusId = (int) eLookupGroupStatus.Open
            };
        }

        private List<EstablishmentGroupModel> CreateEstabList()
        {
            var y = Faker.RandomNumber.Next(1, 10); // what index should be CCIsLeadCentre == true
            return Enumerable.Range(1, 10).Select(x => new EstablishmentGroupModel
            {
                Address = new AddressDto
                {
                    CityOrTown = Faker.Address.City(),
                    Line1 = Faker.Address.StreetAddress(),
                    Line2 = Faker.Address.SecondaryAddress(),
                    Line3 = Faker.Address.UkCounty(),
                    PostCode = Faker.Address.UkPostCode()
                },
                CCIsLeadCentre = x == y,
                Urn = Faker.RandomNumber.Next(100000, 400000),
                HeadFirstName = Faker.Name.First(),
                HeadLastName = Faker.Name.Last(),
                Id = Faker.RandomNumber.Next(100, 1000000),
                HeadTitle = Faker.Name.Prefix(),
                Name = string.Join(" ", Faker.Lorem.Words(2)) + " school",
                JoinedDate = DateTime.Now,
                LAESTAB = string.Concat(Faker.RandomNumber.Next(100, 999), "/", Faker.RandomNumber.Next(1000, 9999)),
                LocalAuthorityName = string.Join(" ", Faker.Lorem.Words(2)),
                PhaseName = Faker.Lorem.GetFirstWord(),
                StatusName = Faker.Lorem.GetFirstWord(),
                TypeName = Faker.Lorem.GetFirstWord()
            }).ToList();
        }

        private List<EstablishmentGroupViewModel> CreateEstablishmentGroupViewModelList(int count)
        {
            var y = Faker.RandomNumber.Next(0, count); // what index should be CCIsLeadCentre == true
            return Enumerable.Range(1, count).Select(x => new EstablishmentGroupViewModel
            {
                Address = Faker.Address.StreetAddress(),
                CCIsLeadCentre = x == y,
                Urn = Faker.RandomNumber.Next(100000, 400000),
                HeadFirstName = Faker.Name.First(),
                HeadLastName = Faker.Name.Last(),
                Id = Faker.RandomNumber.Next(100, 1000000),
                HeadTitleName = Faker.Name.Prefix(),
                Name = string.Join(" ", Faker.Lorem.Words(2)) + " school",
                JoinedDate = DateTime.Now,
                LAESTAB = string.Concat(Faker.RandomNumber.Next(100, 999), "/", Faker.RandomNumber.Next(1000, 9999)),
                LocalAuthorityName = string.Join(" ", Faker.Lorem.Words(2)),
                PhaseName = Faker.Lorem.GetFirstWord(),
                StatusName = Faker.Lorem.GetFirstWord(),
                TypeName = Faker.Lorem.GetFirstWord()
            }).ToList();
        }

        [Theory]
        [InlineData(true, true, "Group Detail; User logged on and can edit")]
        [InlineData(true, false, "Group Detail; on and cannot edit")]
        [InlineData(false, false, "Group Detail; Anonymous user")]
        public async Task Group_Details_WithValidRecord(bool isUserLoggedOn, bool canUserEdit, string testName)
        {
            output.WriteLine(testName);

            var grs = mockGroupReadService;
            var govrs = mockGovernorsReadService;
            var ext = mockExternalLookupService;
            var id = mockIdentity;
            var estabList = CreateEstabList();


            ext.Setup(x => x.FscpdCheckExists(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<bool>())).ReturnsAsync(true);
            id.Setup(x => x.IsAuthenticated).Returns(isUserLoggedOn);
            grs.Setup(x => x.GetAsync(It.IsAny<int>(), It.IsAny<IPrincipal>())).ReturnsAsync(new ServiceResultDto<GroupModel>(new GroupModel { GroupUId = 1, Name = "grp" }));
            grs.Setup(x => x.GetLinksAsync(It.IsAny<int>(), It.IsAny<IPrincipal>())).ReturnsAsync(Enumerable.Empty<LinkedGroupModel>());
            grs.Setup(x => x.CanEditAsync(It.IsAny<int>(), It.IsAny<IPrincipal>())).ReturnsAsync(canUserEdit);
            grs.Setup(x => x.CanEditGovernanceAsync(It.IsAny<int>(), It.IsAny<IPrincipal>())).ReturnsAsync(canUserEdit);
            grs.Setup(x => x.GetChangeHistoryAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<IPrincipal>())).ReturnsAsync(new PaginatedResult<GroupChangeDto>());
            grs.Setup(x => x.GetEstablishmentGroupsAsync(It.IsAny<int>(), It.IsAny<IPrincipal>(), true)).ReturnsAsync(estabList);
            govrs.Setup(x => x.GetGovernorPermissions(null, It.IsAny<int>(), It.IsAny<IPrincipal>())).ReturnsAsync(() => new GovernorPermissions { Add = true, Update = true, Remove = true });
            var response = (ViewResult) await controller.Details(1);

            ext.Verify(x => x.FscpdCheckExists(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<bool>()), Times.AtLeastOnce);

            if (!isUserLoggedOn)
            {
                grs.Verify(x => x.GetChangeHistoryAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<IPrincipal>()), Times.Never());
            }
            else
            {
                grs.Verify(x => x.GetChangeHistoryAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<IPrincipal>()), Times.Once());
            }

            var viewModel = (GroupDetailViewModel) response.Model;
            Assert.Equal(canUserEdit, viewModel.CanUserEdit);
            Assert.Equal(estabList.Count, viewModel.Establishments.Count);

            for (var i = 0; i < estabList.Count; i++)
            {
                Assert.Equal(estabList[i].Address.ToString(), viewModel.Establishments[i].Address);
                Assert.Equal(estabList[i].Name, viewModel.Establishments[i].Name);
                Assert.Equal(estabList[i].CCIsLeadCentre, viewModel.Establishments[i].CCIsLeadCentre);
                Assert.False(viewModel.Establishments[i].EditMode);
                Assert.Equal(estabList[i].HeadFirstName, viewModel.Establishments[i].HeadFirstName);
                Assert.Equal(estabList[i].HeadLastName, viewModel.Establishments[i].HeadLastName);
                Assert.Equal(estabList[i].HeadTitle, viewModel.Establishments[i].HeadTitleName);
                Assert.Equal(estabList[i].Id, viewModel.Establishments[i].Id);
                Assert.Equal(estabList[i].JoinedDate, viewModel.Establishments[i].JoinedDate);
                Assert.Equal(estabList[i].LAESTAB, viewModel.Establishments[i].LAESTAB);
                Assert.Equal(estabList[i].LocalAuthorityName, viewModel.Establishments[i].LocalAuthorityName);
                Assert.Equal(estabList[i].PhaseName, viewModel.Establishments[i].PhaseName);
                Assert.Equal(estabList[i].StatusName, viewModel.Establishments[i].StatusName);
                Assert.Equal(estabList[i].TypeName, viewModel.Establishments[i].TypeName);
                Assert.Equal(estabList[i].Urn, viewModel.Establishments[i].Urn);
            }
        }

        [Fact]
        public async Task Group_CreateNewGroup_InvalidType() =>
            await Assert.ThrowsAsync<InvalidParameterException>(() => controller.CreateNewGroup("invalidtype"));

        [Fact]
        public async Task Group_CreateNewGroup_PermissionDenied()
        {
            mockPrincipal.Setup(x => x.IsInRole(It.IsAny<string>())).Returns(true);
            InjectBasicLAsAndGroupTypes();
            mockSecurityService.Setup(x => x.GetCreateGroupPermissionAsync(It.IsAny<IPrincipal>()))
                .ReturnsAsync(new CreateGroupPermissionDto { GroupTypes = new eLookupGroupType[0] });
            await Assert.ThrowsAsync<PermissionDeniedException>(() => controller.CreateNewGroup("Federation"));
        }


        [Theory]
        [InlineData("ChildrensCentre", 899, "Group_CreateNewGroup_ChildrensCentre_WithLA")]
        [InlineData("ChildrensCentre", null, "Group_CreateNewGroup_ChildrensCentre_NoLA")]
        [InlineData("Federation", null, "Group_CreateNewGroup_Federation")]
        [InlineData("Trust", null, "Group_CreateNewGroup_Trust")]
        [InlineData("Sponsor", null, "Group_CreateNewGroup_Sponsor")]
        public async Task Group_CreateNewGroup(string type, int? localAuthorityId, string testName)
        {
            output.WriteLine(testName);

            mockSecurityService.Setup(x => x.GetCreateGroupPermissionAsync(It.IsAny<IPrincipal>()))
                .ReturnsAsync(new CreateGroupPermissionDto
                {
                    GroupTypes = new eLookupGroupType[] {
                        eLookupGroupType.ChildrensCentresCollaboration, eLookupGroupType.Federation, eLookupGroupType.Trust, eLookupGroupType.SchoolSponsor },
                    CCLocalAuthorityId = localAuthorityId
                });

            InjectBasicLAsAndGroupTypes();

            mockPrincipal.Setup(x => x.IsInRole(It.IsAny<string>())).Returns(false);

            var result = (ViewResult) await controller.CreateNewGroup(type);
            var model = (GroupEditorViewModel) result.Model;

            if (type == "ChildrensCentre")
            {
                Assert.Equal((int) eLookupGroupType.ChildrensCentresCollaboration, model.GroupTypeId);
                Assert.Equal(eSaveMode.DetailsAndLinks, model.SaveMode);
                Assert.Equal("CreateChildrensCentre", result.ViewName);

                if (localAuthorityId.HasValue)
                {
                    Assert.False(model.IsLocalAuthorityEditable);
                    Assert.Equal(localAuthorityId, model.LocalAuthorityId);
                    Assert.Equal("placeholder", model.LocalAuthorityName);
                }
                else
                {
                    Assert.True(model.IsLocalAuthorityEditable);
                }
            }
            else
            {
                Assert.Equal(eSaveMode.Details, model.SaveMode);
                Assert.False(model.IsLocalAuthorityEditable);
                Assert.Equal("Create", result.ViewName);
            }


            switch (type)
            {
                case "Federation":
                    Assert.Equal((int) eLookupGroupType.Federation, model.GroupTypeId);
                    break;
                case "Trust":
                    Assert.Equal((int) eLookupGroupType.Trust, model.GroupTypeId);
                    break;
                case "Sponsor":
                    Assert.Equal((int) eLookupGroupType.SchoolSponsor, model.GroupTypeId);
                    break;
            }
        }

        [Fact]
        public async Task Group_Create()
        {
            InjectBasicLAsAndGroupTypes();
            mockGroupsWriteService.Setup(x => x.ValidateAsync(It.IsAny<SaveGroupDto>(), It.IsAny<IPrincipal>())).ReturnsAsync(new ValidationEnvelopeDto());
            mockGroupsWriteService.Setup(x => x.SaveNewAsync(It.IsAny<SaveGroupDto>(), It.IsAny<IPrincipal>())).ReturnsAsync(new ApiResponse<NumericResultDto>(true) { Response = new NumericResultDto { Value = 123 } });
            mockGroupReadService.Setup(x => x.ExistsAsync(It.IsAny<IPrincipal>(), It.IsAny<CompaniesHouseNumber?>(), It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<string>(), It.IsAny<int?>())).ReturnsAsync(false);
            mockSecurityService.Setup(x => x.CreateAnonymousPrincipal()).Returns(new GenericPrincipal(new GenericIdentity(""), new string[0]));

            var result = (RedirectToRouteResult) await controller.Create(new GroupEditorViewModel
            {
                GroupName = "test group",
                Action = ActionSave,
                GroupTypeId = (int) eLookupGroupType.Federation,
                OpenDate = new Web.UI.Models.DateTimeViewModel(DateTime.Now)
            }, ActionSave);

            Assert.Equal("Details", result.RouteValues["action"]);
            Assert.Equal(123, result.RouteValues["id"]);
        }

        [Fact]
        public async Task Group_Create_WithSimilarNameWarning()
        {
            InjectBasicLAsAndGroupTypes();
            mockGroupsWriteService.Setup(x => x.ValidateAsync(It.IsAny<SaveGroupDto>(), It.IsAny<IPrincipal>())).ReturnsAsync(new ValidationEnvelopeDto { Warnings = new List<ApiWarning> { new ApiWarning { Code = ApiWarningCodes.GROUP_WITH_SIMILAR_NAME_FOUND, Message = "similar" } } });
            mockGroupsWriteService.Setup(x => x.SaveNewAsync(It.IsAny<SaveGroupDto>(), It.IsAny<IPrincipal>())).ReturnsAsync(new ApiResponse<NumericResultDto>(true) { Response = new NumericResultDto { Value = 123 } });
            mockGroupReadService.Setup(x => x.ExistsAsync(It.IsAny<IPrincipal>(), It.IsAny<CompaniesHouseNumber?>(), It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<string>(), It.IsAny<int?>())).ReturnsAsync(false);
            mockSecurityService.Setup(x => x.CreateAnonymousPrincipal()).Returns(new GenericPrincipal(new GenericIdentity(""), new string[0]));

            var result = (ViewResult) await controller.Create(new GroupEditorViewModel
            {
                GroupName = "test group",
                Action = ActionSave,
                GroupTypeId = (int) eLookupGroupType.Federation,
                OpenDate = new Web.UI.Models.DateTimeViewModel(DateTime.Now)
            }, ActionSave);
            var model = (GroupEditorViewModel) result.Model;
            Assert.True(model.WarningsToProcess.Any());

            // Then dismiss the warnings...
            var result2 = (RedirectToRouteResult) await controller.Create(new GroupEditorViewModel
            {
                GroupName = "test group",
                Action = ActionSave,
                GroupTypeId = (int) eLookupGroupType.Federation,
                OpenDate = new Web.UI.Models.DateTimeViewModel(DateTime.Now),
                ProcessedWarnings = true
            }, ActionSave);

            Assert.Equal("Details", result2.RouteValues["action"]);
            Assert.Equal(123, result2.RouteValues["id"]);
        }


        [Fact]
        public async Task Group_Create_WithUnknownWarning_IgnoresIt()
        {
            InjectBasicLAsAndGroupTypes();
            mockGroupsWriteService.Setup(x => x.ValidateAsync(It.IsAny<SaveGroupDto>(), It.IsAny<IPrincipal>())).ReturnsAsync(new ValidationEnvelopeDto { Warnings = new List<ApiWarning> { new ApiWarning { Code = "unknown", Message = "similar" } } });
            mockGroupsWriteService.Setup(x => x.SaveNewAsync(It.IsAny<SaveGroupDto>(), It.IsAny<IPrincipal>())).ReturnsAsync(new ApiResponse<NumericResultDto>(true) { Response = new NumericResultDto { Value = 123 } });
            mockGroupReadService.Setup(x => x.ExistsAsync(It.IsAny<IPrincipal>(), It.IsAny<CompaniesHouseNumber?>(), It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<string>(), It.IsAny<int?>())).ReturnsAsync(false);
            mockSecurityService.Setup(x => x.CreateAnonymousPrincipal()).Returns(new GenericPrincipal(new GenericIdentity(""), new string[0]));

            var result = (RedirectToRouteResult) await controller.Create(new GroupEditorViewModel
            {
                GroupName = "test group",
                Action = ActionSave,
                GroupTypeId = (int) eLookupGroupType.Federation,
                OpenDate = new Web.UI.Models.DateTimeViewModel(DateTime.Now),
                ProcessedWarnings = true
            }, ActionSave);

            Assert.Equal("Details", result.RouteValues["action"]);
            Assert.Equal(123, result.RouteValues["id"]);
        }


        [Fact]
        public async Task Group_Federation_EditDetails()
        {
            var grs = mockGroupReadService;
            var estabList = CreateEstabList();

            mockPrincipal.Setup(x => x.IsInRole(It.IsAny<string>())).Returns(true);

            InjectBasicLAsAndGroupTypes();

            var domainModel = CreateGroupModel(eLookupGroupType.Federation);

            grs.Setup(x => x.GetAsync(It.IsAny<int>(), It.IsAny<IPrincipal>())).ReturnsAsync(new ServiceResultDto<GroupModel>(domainModel));
            grs.Setup(x => x.GetEstablishmentGroupsAsync(It.IsAny<int>(), It.IsAny<IPrincipal>(), true)).ReturnsAsync(estabList);

            var response = (ViewResult) await controller.EditDetails(123);
            var vm = (GroupEditorViewModel) response.Model;
            Assert.Equal("EditDetails", response.ViewName);
            Assert.Equal("I am Federation", vm.GroupName);
            Assert.Equal("123", vm.GroupId);
            Assert.Equal(eLookupGroupType.Federation, vm.GroupType);
            Assert.Equal((int) eLookupGroupType.Federation, vm.GroupTypeId);
            Assert.Equal(eGroupTypeMode.Federation, vm.GroupTypeMode);
            Assert.Equal("placeholder", vm.GroupTypeName);
            Assert.Equal(123, vm.GroupUId);
            Assert.True(vm.InEditMode);
            Assert.False(vm.IsLocalAuthorityEditable);
            Assert.Equal(10, vm.LinkedEstablishments.Establishments.Count);
            Assert.Equal("Schools", vm.ListOfEstablishmentsPluralName);
            Assert.Equal(1, vm.LocalAuthorityId);
            Assert.Equal("placeholder", vm.LocalAuthorityName);
            Assert.Equal(domainModel.ManagerEmailAddress, vm.ManagerEmailAddress);
            Assert.Equal(domainModel.OpenDate.GetValueOrDefault().Date, vm.OpenDate.ToDateTime().GetValueOrDefault().Date);
            Assert.Equal(domainModel.ClosedDate.GetValueOrDefault().Date, vm.ClosedDate.ToDateTime().GetValueOrDefault().Date);
            Assert.Equal("Open date", vm.OpenDateLabel);
            Assert.Equal("Edit federation", vm.PageTitle);
            Assert.Equal((int) eLookupGroupStatus.Open, vm.StatusId);
            Assert.Equal(domainModel.Address.ToString(), vm.Address);
            Assert.True(vm.CanUserCloseAndMarkAsCreatedInError);
            Assert.Equal(estabList.Single(x => x.CCIsLeadCentre == true).Urn, vm.CCLeadCentreUrn);
        }


        [Fact]
        public async Task Group_Trust_EditDetails()
        {
            var grs = mockGroupReadService;
            var estabList = CreateEstabList();

            mockPrincipal.Setup(x => x.IsInRole(It.IsAny<string>())).Returns(true);

            InjectBasicLAsAndGroupTypes();

            var domainModel = CreateGroupModel(eLookupGroupType.Trust);

            grs.Setup(x => x.GetAsync(It.IsAny<int>(), It.IsAny<IPrincipal>())).ReturnsAsync(new ServiceResultDto<GroupModel>(domainModel));
            grs.Setup(x => x.GetEstablishmentGroupsAsync(It.IsAny<int>(), It.IsAny<IPrincipal>(), true)).ReturnsAsync(estabList);

            var response = (ViewResult) await controller.EditDetails(123);
            var vm = (GroupEditorViewModel) response.Model;
            Assert.Equal("EditDetails", response.ViewName);
            Assert.Equal("I am Trust", vm.GroupName);
            Assert.Equal("123", vm.GroupId);
            Assert.Equal(eLookupGroupType.Trust, vm.GroupType);
            Assert.Equal((int) eLookupGroupType.Trust, vm.GroupTypeId);
            Assert.Equal(eGroupTypeMode.Trust, vm.GroupTypeMode);
            Assert.Equal("placeholder", vm.GroupTypeName);
            Assert.Equal(123, vm.GroupUId);
            Assert.True(vm.InEditMode);
            Assert.False(vm.IsLocalAuthorityEditable);
            Assert.Equal(10, vm.LinkedEstablishments.Establishments.Count);
            Assert.Equal("Schools", vm.ListOfEstablishmentsPluralName);
            Assert.Equal(1, vm.LocalAuthorityId);
            Assert.Equal("placeholder", vm.LocalAuthorityName);
            Assert.Equal(domainModel.ManagerEmailAddress, vm.ManagerEmailAddress);
            Assert.Equal(domainModel.OpenDate.GetValueOrDefault().Date, vm.OpenDate.ToDateTime().GetValueOrDefault().Date);
            Assert.Equal(domainModel.ClosedDate.GetValueOrDefault().Date, vm.ClosedDate.ToDateTime().GetValueOrDefault().Date);
            Assert.Equal("Open date", vm.OpenDateLabel);
            Assert.Equal("Edit foundation trust", vm.PageTitle);
            Assert.Equal((int) eLookupGroupStatus.Open, vm.StatusId);
            Assert.Equal(domainModel.Address.ToString(), vm.Address);
            Assert.False(vm.CanUserCloseAndMarkAsCreatedInError);
            Assert.Equal(estabList.Single(x => x.CCIsLeadCentre == true).Urn, vm.CCLeadCentreUrn);
        }

        [Theory]
        [InlineData(eLookupGroupType.SingleacademyTrust, "Single-academy trust name", "Close this single-academy trust and mark as created in error", "Group_EditDetails_DynamicLabels_SingleacademyTrust")]
        [InlineData(eLookupGroupType.MultiacademyTrust, "Multi-academy trust name", "Close this multi-academy trust and mark as created in error", "Group_EditDetails_DynamicLabels_MultiacademyTrust")]
        [InlineData(eLookupGroupType.ChildrensCentresCollaboration, "Children's centres collaboration name", "", "Group_EditDetails_DynamicLabels_ChildrensCentresCollaboration")]
        [InlineData(eLookupGroupType.ChildrensCentresGroup, "Children's centres group name", "", "Group_EditDetails_DynamicLabels_ChildrensCentresGroup")]
        [InlineData(eLookupGroupType.Federation, "Federation name", "Close this federation and mark as created in error", "Group_EditDetails_DynamicLabels_Federation")]
        [InlineData(eLookupGroupType.Trust, "Foundation trust name", "", "Group_EditDetails_DynamicLabels_Trust")]
        [InlineData(eLookupGroupType.SchoolSponsor, "Academy sponsor name", "Close this academy sponsor and mark as created in error", "Group_EditDetails_DynamicLabels_SchoolSponsor")]
        [InlineData(eLookupGroupType.SecureSingleAcademyTrust, "Secure single-academy trust name", "Close this secure single-academy trust and mark as created in error", "Group_EditDetails_DynamicLabels_SecureSingleAcademyTrust")]
        public async Task Group_EditDetails_DynamicLabels(eLookupGroupType groupType, string groupNameLabelText,
            string closeAndMarkAsCreatedInErrorLabelText, string testName)
        {
            output.WriteLine(testName);

            var grs = mockGroupReadService;
            var estabList = CreateEstabList();

            mockPrincipal.Setup(x => x.IsInRole(It.IsAny<string>())).Returns(true);

            InjectBasicLAsAndGroupTypes();

            var domainModel = CreateGroupModel(groupType);

            grs.Setup(x => x.GetAsync(It.IsAny<int>(), It.IsAny<IPrincipal>())).ReturnsAsync(new ServiceResultDto<GroupModel>(domainModel));
            grs.Setup(x => x.GetEstablishmentGroupsAsync(It.IsAny<int>(), It.IsAny<IPrincipal>(), true)).ReturnsAsync(estabList);

            var response = (ViewResult) await controller.EditDetails(123);
            var vm = (GroupEditorViewModel) response.Model;

            Assert.Equal(groupNameLabelText, vm.GroupNameLabel);
            if (vm.CanUserCloseAndMarkAsCreatedInError)
            {
                Assert.Equal(closeAndMarkAsCreatedInErrorLabelText, vm.CloseAndMarkAsCreatedInErrorLabel);
            }
            else
            {
                Assert.Null(vm.CloseAndMarkAsCreatedInErrorLabel);
            }
        }


        [Fact]
        public async Task Group_Federation_EditLinks()
        {
            var grs = mockGroupReadService;
            var estabList = CreateEstabList();

            InjectBasicLAsAndGroupTypes();

            var domainModel = CreateGroupModel(eLookupGroupType.Federation);

            grs.Setup(x => x.GetAsync(It.IsAny<int>(), It.IsAny<IPrincipal>())).ReturnsAsync(new ServiceResultDto<GroupModel>(domainModel));
            grs.Setup(x => x.GetEstablishmentGroupsAsync(It.IsAny<int>(), It.IsAny<IPrincipal>(), true)).ReturnsAsync(estabList);

            var response = (ViewResult) await controller.EditLinks(123);
            var vm = (GroupEditorViewModel) response.Model;
            Assert.Equal(string.Empty, response.ViewName);
            Assert.Equal("I am Federation", vm.GroupName);
            Assert.Equal(123, vm.GroupUId);
            Assert.Equal(eLookupGroupType.Federation, vm.GroupType);
            Assert.Equal((int) eLookupGroupType.Federation, vm.GroupTypeId);
            Assert.Equal(eGroupTypeMode.Federation, vm.GroupTypeMode);
            Assert.Equal("placeholder", vm.GroupTypeName);
            Assert.Equal(estabList.Single(x => x.CCIsLeadCentre == true).Urn, vm.CCLeadCentreUrn);
            Assert.Equal(eSaveMode.Links, vm.SaveMode);
            Assert.True(vm.InEditMode);
            Assert.Equal(10, vm.LinkedEstablishments.Establishments.Count);
            Assert.Equal("Schools", vm.ListOfEstablishmentsPluralName);
            Assert.Equal("links", vm.SelectedTabName);
            Assert.Equal("Open date", vm.OpenDateLabel);
            Assert.Equal("Edit federation", vm.PageTitle);
            Assert.False(vm.CanUserCloseAndMarkAsCreatedInError);
        }

        [Fact]
        public async Task Group_Trust_EditLinks()
        {
            var grs = mockGroupReadService;
            var estabList = CreateEstabList();

            InjectBasicLAsAndGroupTypes();

            var domainModel = CreateGroupModel(eLookupGroupType.Trust);

            grs.Setup(x => x.GetAsync(It.IsAny<int>(), It.IsAny<IPrincipal>())).ReturnsAsync(new ServiceResultDto<GroupModel>(domainModel));
            grs.Setup(x => x.GetEstablishmentGroupsAsync(It.IsAny<int>(), It.IsAny<IPrincipal>(), true)).ReturnsAsync(estabList);

            var response = (ViewResult) await controller.EditLinks(123);
            var vm = (GroupEditorViewModel) response.Model;
            Assert.Equal(string.Empty, response.ViewName);
            Assert.Equal("I am Trust", vm.GroupName);
            Assert.Equal(123, vm.GroupUId);
            Assert.Equal(eLookupGroupType.Trust, vm.GroupType);
            Assert.Equal((int) eLookupGroupType.Trust, vm.GroupTypeId);
            Assert.Equal(eGroupTypeMode.Trust, vm.GroupTypeMode);
            Assert.Equal("placeholder", vm.GroupTypeName);
            Assert.Equal(estabList.Single(x => x.CCIsLeadCentre == true).Urn, vm.CCLeadCentreUrn);
            Assert.Equal(eSaveMode.Links, vm.SaveMode);
            Assert.True(vm.InEditMode);
            Assert.Equal(10, vm.LinkedEstablishments.Establishments.Count);
            Assert.Equal("Schools", vm.ListOfEstablishmentsPluralName);
            Assert.Equal("links", vm.SelectedTabName);
            Assert.Equal("Open date", vm.OpenDateLabel);
            Assert.Equal("Edit foundation trust", vm.PageTitle);
            Assert.False(vm.CanUserCloseAndMarkAsCreatedInError);
        }

        [Fact]
        public void Group_Convert()
        {
            var response = (ViewResult) controller.Convert();
            var vm = (ConvertSATViewModel) response.Model;
            Assert.Equal(string.Empty, response.ViewName);
            Assert.IsType<ConvertSATViewModel>(response.Model);
        }

        [Fact]
        public async Task Group_Convert_FindNonExistentSAT()
        {
            var grs = mockGroupReadService;
            grs.Setup(x => x.SearchByIdsAsync(It.IsAny<string>(), It.Is<int?>(i => i == 1000), It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<IPrincipal>())).ReturnsAsync(() => new ApiPagedResult<SearchGroupDocument>(0, new List<SearchGroupDocument>()));

            var response = (ViewResult) await controller.Convert(new ConvertSATViewModel
            {
                ActionName = "find",
                Text = "1000"
            });

            Assert.Equal("We were unable to find a single-academy trust matching those details", controller.ModelState["Text"].Errors[0].ErrorMessage);
            Assert.Equal(string.Empty, response.ViewName);
        }

        [Fact]
        public async Task Group_Convert_FindInvalidGroupType()
        {
            var grs = mockGroupReadService;
            grs.Setup(x => x.SearchByIdsAsync(It.IsAny<string>(), It.Is<int?>(i => i == 1000), It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<IPrincipal>()))
                .ReturnsAsync(() => new ApiPagedResult<SearchGroupDocument>(1, new List<SearchGroupDocument>(){
                new SearchGroupDocument{ Name="Group 1000", GroupUId = 1000, GroupTypeId=(int)eLookupGroupType.MultiacademyTrust }
            }));

            var response = (ViewResult) await controller.Convert(new ConvertSATViewModel
            {
                ActionName = "find",
                Text = "1000"
            });

            Assert.Equal("That's an invalid group because it's of the wrong type.", controller.ModelState["Text"].Errors[0].ErrorMessage);
            Assert.Equal(string.Empty, response.ViewName);
        }

        [Fact]
        public async Task Group_Convert_FindValidGroup()
        {
            mockCachedLookupService.Setup(x => x.GetNameAsync(It.IsAny<Expression<Func<int?>>>(), It.IsAny<string>())).ReturnsAsync("placeholder");
            mockGroupReadService.Setup(x => x.SearchByIdsAsync(It.IsAny<string>(), It.Is<int?>(i => i == 1000), It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<IPrincipal>())).ReturnsAsync(() => new ApiPagedResult<SearchGroupDocument>(1, new List<SearchGroupDocument>(){
                new SearchGroupDocument{ Name="Group 1000", GroupUId = 1000, GroupTypeId=(int)eLookupGroupType.SingleacademyTrust }
            }));

            var vm = new ConvertSATViewModel
            {
                ActionName = "find",
                Text = "1000"
            };
            var response = (ViewResult) await controller.Convert(vm);
            Assert.Equal("Group 1000", vm.Details.Name);
            Assert.Equal(1000, vm.Details.GroupUId);
            Assert.Equal((int) eLookupGroupType.SingleacademyTrust, vm.Details.GroupTypeId);
            Assert.Equal("placeholder", vm.CountryName);
            Assert.Equal("placeholder", vm.CountyName);
            Assert.NotNull(vm.Token);
            Assert.Equal(string.Empty, response.ViewName);
        }

        [Fact]
        public async Task Group_Convert_Confirm_Success()
        {
            var domainModel = new SearchGroupDocument { Name = "Group 1000", GroupUId = 1000, GroupTypeId = (int) eLookupGroupType.SingleacademyTrust };
            mockGroupsWriteService.Setup(x => x.ConvertSAT2MAT(It.Is<int>(i => i == 1000), It.Is<bool>(b => b == true), It.IsAny<IPrincipal>())).ReturnsAsync(new ApiResponse<NumericResultDto>(true) { Response = new NumericResultDto { Value = 200 } });

            var vm = new ConvertSATViewModel
            {
                Token = UriHelper.SerializeToUrlToken(domainModel),
                ActionName = "confirm",
                CopyGovernanceInfo = true
            };
            var response = (RedirectToRouteResult) await controller.Convert(vm);
            Assert.Equal("GroupDetails", response.RouteName);
            Assert.Equal(200, response.RouteValues["id"]);
            mockGroupsWriteService.Verify(x => x.ConvertSAT2MAT(It.Is<int>(i => i == 1000), It.Is<bool>(b => b == true), It.IsAny<IPrincipal>()), Times.Once);
        }

        [Fact]
        public async Task Group_Convert_Confirm_WithValidationError()
        {
            var domainModel = new SearchGroupDocument { Name = "Group 1000", GroupUId = 1000, GroupTypeId = (int) eLookupGroupType.SingleacademyTrust };
            mockGroupsWriteService.Setup(x => x.ConvertSAT2MAT(It.Is<int>(i => i == 1000), It.Is<bool>(b => b == true), It.IsAny<IPrincipal>())).ReturnsAsync(new ApiResponse<NumericResultDto>(false) { Errors = new[] { new ApiError { Code = "test", Message = "msg" } } });

            var vm = new ConvertSATViewModel
            {
                Token = UriHelper.SerializeToUrlToken(domainModel),
                ActionName = "confirm",
                CopyGovernanceInfo = true
            };
            var response = (ViewResult) await controller.Convert(vm);
            Assert.Equal(string.Empty, response.ViewName);
            Assert.Equal("msg", controller.ModelState[""].Errors[0].ErrorMessage);
            mockGroupsWriteService.Verify(x => x.ConvertSAT2MAT(It.Is<int>(i => i == 1000), It.Is<bool>(b => b == true), It.IsAny<IPrincipal>()), Times.Once);
        }

        [Fact]
        public async Task Group_EditDetails_Post_Success()
        {
            mockGroupsWriteService.Setup(x => x.ValidateAsync(It.IsAny<SaveGroupDto>(), It.IsAny<IPrincipal>())).ReturnsAsync(new ValidationEnvelopeDto());
            mockGroupReadService.Setup(x => x.ExistsAsync(It.IsAny<IPrincipal>(), It.IsAny<CompaniesHouseNumber?>(), It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<string>(), It.IsAny<int?>())).ReturnsAsync(false);
            mockGroupReadService.Setup(x => x.GetModelChangesAsync(It.IsAny<GroupModel>(), It.IsAny<IPrincipal>()))
                .ReturnsAsync(new List<ChangeDescriptorDto>()
                {
                    // Changes are made, exact details not important for this test
                    new ChangeDescriptorDto(),
                });
            mockSecurityService.Setup(x => x.CreateAnonymousPrincipal()).Returns(new GenericPrincipal(new GenericIdentity(""), new string[0]));
            mockGroupsWriteService.Setup(x => x.SaveAsync(It.IsAny<SaveGroupDto>(), It.IsAny<IPrincipal>())).ReturnsAsync(new ApiResponse(true));
            mockGroupReadService.Setup(x =>
                x.GetEstablishmentGroupsAsync(It.IsAny<int>(), It.IsAny<IPrincipal>(), It.IsAny<bool>())).ReturnsAsync(new List<EstablishmentGroupModel>());
            InjectBasicLAsAndGroupTypes();

            var viewModel = new GroupEditorViewModel
            {
                GroupUId = 123,
                GroupName = "This is a test",
                Action = ActionSave,
                GroupTypeId = (int) eLookupGroupType.Federation
            };

            // Changes made, so we should be redirected to the details page
            var result = (RedirectToRouteResult) await controller.EditDetails(viewModel);
            Assert.Equal("Details", result.RouteValues["action"]);
            Assert.Equal(123, result.RouteValues["id"]);

            mockGroupsWriteService.Verify(x => x.SaveAsync(It.Is<SaveGroupDto>(v => v.LinkedEstablishments == null && v.GroupUId == 123 && v.Group != null), It.IsAny<IPrincipal>()), Times.Once);
        }

        [Fact]
        public async Task Group_EditDetails_Post_NoChanges()
        {
            mockGroupsWriteService.Setup(x => x.ValidateAsync(It.IsAny<SaveGroupDto>(), It.IsAny<IPrincipal>())).ReturnsAsync(new ValidationEnvelopeDto());
            mockGroupReadService.Setup(x => x.ExistsAsync(It.IsAny<IPrincipal>(), It.IsAny<CompaniesHouseNumber?>(), It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<string>(), It.IsAny<int?>())).ReturnsAsync(false);
            mockGroupReadService.Setup(x => x.GetModelChangesAsync(It.IsAny<GroupModel>(), It.IsAny<IPrincipal>()))
                .ReturnsAsync(new List<ChangeDescriptorDto>()
                {
                    // No changes are made
                });
            mockSecurityService.Setup(x => x.CreateAnonymousPrincipal()).Returns(new GenericPrincipal(new GenericIdentity(""), new string[0]));
            mockGroupsWriteService.Setup(x => x.SaveAsync(It.IsAny<SaveGroupDto>(), It.IsAny<IPrincipal>())).ReturnsAsync(new ApiResponse(true));
            mockGroupReadService.Setup(x => x.GetEstablishmentGroupsAsync(It.IsAny<int>(), It.IsAny<IPrincipal>(), It.IsAny<bool>())).ReturnsAsync(new List<EstablishmentGroupModel>());

            InjectBasicLAsAndGroupTypes();

            var viewModel = new GroupEditorViewModel
            {
                GroupUId = 123,
                GroupName = "This is a test",
                Action = ActionSave,
                GroupTypeId = (int) eLookupGroupType.Federation
            };
            var result = (ViewResult) await controller.EditDetails(viewModel);

            // No changes made, so we should be shown the "no changes made" page
            Assert.Equal("EditDetailsEmpty", result.ViewName);
            Assert.Equal(viewModel, result.Model);
        }

        [Fact]
        public async Task Group_Federation_EditLinks_Post_Success()
        {
            var domainModel = CreateGroupModel(eLookupGroupType.Federation);
            var gws = mockGroupsWriteService;
            var grs = mockGroupReadService;
            grs.Setup(x => x.GetAsync(It.IsAny<int>(), It.IsAny<IPrincipal>())).ReturnsAsync(new ServiceResultDto<GroupModel>(domainModel));
            gws.Setup(x => x.ValidateAsync(It.IsAny<SaveGroupDto>(), It.IsAny<IPrincipal>())).ReturnsAsync(new ValidationEnvelopeDto());
            grs.Setup(x => x.ExistsAsync(It.IsAny<IPrincipal>(), It.IsAny<CompaniesHouseNumber?>(), It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<string>(), It.IsAny<int?>())).ReturnsAsync(false);
            mockSecurityService.Setup(x => x.CreateAnonymousPrincipal()).Returns(new GenericPrincipal(new GenericIdentity(""), new string[0]));
            gws.Setup(x => x.SaveAsync(It.IsAny<SaveGroupDto>(), It.IsAny<IPrincipal>())).ReturnsAsync(new ApiResponse(true));
            InjectBasicLAsAndGroupTypes();

            var viewModel = new GroupEditorViewModel
            {
                GroupUId = 123,
                GroupName = "This is a test",
                Action = ActionSave,
                SaveMode = eSaveMode.Links,
                GroupTypeId = (int) eLookupGroupType.Federation,
                LinkedEstablishments = new GroupLinkedEstablishmentsViewModel
                {
                    Establishments = CreateEstablishmentGroupViewModelList(1)
                }
            };
            var result = (RedirectToRouteResult) await controller.EditLinks(viewModel);
            Assert.Equal("Details", result.RouteValues["action"]);
            Assert.Equal(123, result.RouteValues["id"]);

            mockGroupsWriteService.Verify(x => x.SaveAsync(It.Is<SaveGroupDto>(v => v.LinkedEstablishments != null && v.GroupUId == 123 && v.Group == null), It.IsAny<IPrincipal>()), Times.Once);
        }


        [Fact]
        public async Task Group_Federation_EditLinks_RemoveEstablishment_Success()
        {
            var domainModel = CreateGroupModel(eLookupGroupType.Federation);

            var grs = mockGroupReadService;
            grs.Setup(x => x.GetAsync(It.IsAny<int>(), It.IsAny<IPrincipal>())).ReturnsAsync(new ServiceResultDto<GroupModel>(domainModel));
            grs.Setup(x => x.ExistsAsync(It.IsAny<IPrincipal>(), It.IsAny<CompaniesHouseNumber?>(), It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<string>(), It.IsAny<int?>())).ReturnsAsync(false);
            mockGroupsWriteService.Setup(x => x.ValidateAsync(It.IsAny<SaveGroupDto>(), It.IsAny<IPrincipal>())).ReturnsAsync(new ValidationEnvelopeDto());
            mockSecurityService.Setup(x => x.CreateAnonymousPrincipal()).Returns(new GenericPrincipal(new GenericIdentity(""), new string[0]));
            mockGroupsWriteService.Setup(x => x.SaveAsync(It.IsAny<SaveGroupDto>(), It.IsAny<IPrincipal>())).ReturnsAsync(new ApiResponse(true));
            InjectBasicLAsAndGroupTypes();

            var estabs = CreateEstablishmentGroupViewModelList(3);
            var viewModel = new GroupEditorViewModel
            {
                GroupUId = 123,
                GroupName = "This is a test",
                Action = ActionLinkedEstablishmentRemove + estabs[0].Urn,
                SaveMode = eSaveMode.Links,
                GroupTypeId = (int) eLookupGroupType.Federation,
                LinkedEstablishments = new GroupLinkedEstablishmentsViewModel
                {
                    Establishments = estabs
                }
            };
            var result = (ViewResult) await controller.EditLinks(viewModel);
            Assert.Equal(2, viewModel.LinkedEstablishments.Establishments.Count);
        }

        [Fact]
        public async Task Group_Federation_EditLinks_CancelEditMode()
        {
            var domainModel = CreateGroupModel(eLookupGroupType.Federation);

            var grs = mockGroupReadService;
            grs.Setup(x => x.GetAsync(It.IsAny<int>(), It.IsAny<IPrincipal>())).ReturnsAsync(new ServiceResultDto<GroupModel>(domainModel));
            mockGroupsWriteService.Setup(x => x.ValidateAsync(It.IsAny<SaveGroupDto>(), It.IsAny<IPrincipal>())).ReturnsAsync(new ValidationEnvelopeDto());
            mockGroupReadService.Setup(x => x.ExistsAsync(It.IsAny<IPrincipal>(), It.IsAny<CompaniesHouseNumber?>(), It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<string>(), It.IsAny<int?>())).ReturnsAsync(false);
            mockSecurityService.Setup(x => x.CreateAnonymousPrincipal()).Returns(new GenericPrincipal(new GenericIdentity(""), new string[0]));
            mockGroupsWriteService.Setup(x => x.SaveAsync(It.IsAny<SaveGroupDto>(), It.IsAny<IPrincipal>())).ReturnsAsync(new ApiResponse(true));
            InjectBasicLAsAndGroupTypes();

            var viewModel = new GroupEditorViewModel
            {
                GroupUId = 123,
                GroupName = "This is a test",
                Action = ActionLinkedEstablishmentCancelEdit,
                SaveMode = eSaveMode.Links,
                GroupTypeId = (int) eLookupGroupType.Federation,
                LinkedEstablishments = new GroupLinkedEstablishmentsViewModel
                {
                    Establishments = CreateEstablishmentGroupViewModelList(3)
                }
            };
            viewModel.LinkedEstablishments.Establishments[0].EditMode = true;
            var result = (ViewResult) await controller.EditLinks(viewModel);
            Assert.False(viewModel.LinkedEstablishments.Establishments[0].EditMode);
        }


        [Fact]
        public async Task Group_Federation_EditLinks_SetEditMode_Success()
        {
            var domainModel = CreateGroupModel(eLookupGroupType.Federation);

            var grs = mockGroupReadService;
            grs.Setup(x => x.GetAsync(It.IsAny<int>(), It.IsAny<IPrincipal>())).ReturnsAsync(new ServiceResultDto<GroupModel>(domainModel));
            grs.Setup(x => x.ExistsAsync(It.IsAny<IPrincipal>(), It.IsAny<CompaniesHouseNumber?>(), It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<string>(), It.IsAny<int?>())).ReturnsAsync(false);
            mockGroupsWriteService.Setup(x => x.ValidateAsync(It.IsAny<SaveGroupDto>(), It.IsAny<IPrincipal>())).ReturnsAsync(new ValidationEnvelopeDto());
            mockSecurityService.Setup(x => x.CreateAnonymousPrincipal()).Returns(new GenericPrincipal(new GenericIdentity(""), new string[0]));
            mockGroupsWriteService.Setup(x => x.SaveAsync(It.IsAny<SaveGroupDto>(), It.IsAny<IPrincipal>())).ReturnsAsync(new ApiResponse(true));
            InjectBasicLAsAndGroupTypes();

            var estabs = CreateEstablishmentGroupViewModelList(3);
            var viewModel = new GroupEditorViewModel
            {
                GroupUId = 123,
                GroupName = "This is a test",
                Action = ActionLinkedEstablishmentEdit + estabs[0].Urn,
                SaveMode = eSaveMode.Links,
                GroupTypeId = (int) eLookupGroupType.Federation,
                LinkedEstablishments = new GroupLinkedEstablishmentsViewModel
                {
                    Establishments = estabs
                }
            };
            var result = (ViewResult) await controller.EditLinks(viewModel);
            Assert.True(viewModel.LinkedEstablishments.Establishments[0].EditMode);
            Assert.Equal(viewModel.LinkedEstablishments.Establishments[0].JoinedDate.Value.Date,
                viewModel.LinkedEstablishments.Establishments[0].JoinedDateEditable.ToDateTime().Value.Date);
        }

        [Fact]
        public async Task Group_Federation_EditLinks_SaveJoinedDate_Success()
        {
            var domainModel = CreateGroupModel(eLookupGroupType.Federation);

            var grs = mockGroupReadService;
            grs.Setup(x => x.GetAsync(It.IsAny<int>(), It.IsAny<IPrincipal>())).ReturnsAsync(new ServiceResultDto<GroupModel>(domainModel));
            grs.Setup(x => x.ExistsAsync(It.IsAny<IPrincipal>(), It.IsAny<CompaniesHouseNumber?>(), It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<string>(), It.IsAny<int?>())).ReturnsAsync(false);
            mockGroupsWriteService.Setup(x => x.ValidateAsync(It.IsAny<SaveGroupDto>(), It.IsAny<IPrincipal>())).ReturnsAsync(new ValidationEnvelopeDto());
            mockSecurityService.Setup(x => x.CreateAnonymousPrincipal()).Returns(new GenericPrincipal(new GenericIdentity(""), new string[0]));
            mockGroupsWriteService.Setup(x => x.SaveAsync(It.IsAny<SaveGroupDto>(), It.IsAny<IPrincipal>())).ReturnsAsync(new ApiResponse(true));
            InjectBasicLAsAndGroupTypes();

            var estabs = CreateEstablishmentGroupViewModelList(3);
            var viewModel = new GroupEditorViewModel
            {
                GroupUId = 123,
                GroupName = "This is a test",
                Action = ActionLinkedEstablishmentSave,
                SaveMode = eSaveMode.Links,
                GroupTypeId = (int) eLookupGroupType.Federation,
                LinkedEstablishments = new GroupLinkedEstablishmentsViewModel
                {
                    Establishments = estabs
                }
            };
            estabs[0].JoinedDateEditable = new Web.UI.Models.DateTimeViewModel(DateTime.Now.AddDays(1));
            estabs[0].EditMode = true;
            var result = (ViewResult) await controller.EditLinks(viewModel);
            Assert.False(viewModel.LinkedEstablishments.Establishments[0].EditMode);
            Assert.Equal(viewModel.LinkedEstablishments.Establishments[0].JoinedDateEditable.ToDateTime().Value.Date,
                viewModel.LinkedEstablishments.Establishments[0].JoinedDate.Value.Date);
        }

        [Fact]
        public async Task Group_SaveNewAcademyTrust()
        {
            var gws = mockGroupsWriteService;
            gws.Setup(x => x.ValidateAsync(It.IsAny<SaveGroupDto>(), It.IsAny<IPrincipal>())).ReturnsAsync(new ValidationEnvelopeDto());
            gws.Setup(x => x.SaveNewAsync(It.IsAny<SaveGroupDto>(), It.IsAny<IPrincipal>())).ReturnsAsync(new ApiResponse<NumericResultDto>(true) { Response = new NumericResultDto { Value = 123 } });

            mockSecurityService.Setup(x => x.GetCreateGroupPermissionAsync(It.IsAny<IPrincipal>()))
                .ReturnsAsync(new CreateGroupPermissionDto { GroupTypes = new eLookupGroupType[] { eLookupGroupType.MultiacademyTrust } });


            var vm = new CreateAcademyTrustViewModel
            {
                CompaniesHouseAddressToken = UriHelper.SerializeToUrlToken(new AddressDto { Line1 = "line1", CityOrTown = "Bobville" }),
                CompaniesHouseNumber = "67362546543",
                TypeId = (int) eLookupGroupType.MultiacademyTrust,
                Name = "Multi acad",
                OpenDate = DateTime.Now,
                GroupId = "54243"
            };
            var result = (RedirectToRouteResult) await controller.SaveNewAcademyTrust(vm,"academy-trust");
            Assert.Equal("Details", result.RouteValues["action"]);
            Assert.Equal(123, result.RouteValues["id"]);
        }

        [Fact]
        public async Task dets()
        {
            var id = 12;
            var search = "search";
            var searchsource = eLookupSearchSource.Groups;
            var skip = 0;
            var sortBy = "requestedDateUTC-desc";
            var saved = false;

            var mockUser = new Mock<IPrincipal>();

            mockHttpContextBase.SetupGet(x => x.User).Returns(mockUser.Object);
            mockIdentity.Setup(x => x.IsAuthenticated).Returns(true);
            mockPrincipal.Setup(x => x.Identity).Returns(mockIdentity.Object);
            mockExternalLookupService.Setup(x => x.FscpdCheckExists(It.IsAny<int?>(), It.IsAny<string>(), It.IsAny<bool>()))
                .ReturnsAsync(true);
            mockSecurityService.Setup(x => x.GetCreateGroupPermissionAsync(It.IsAny<IPrincipal>()))
                .ReturnsAsync(new CreateGroupPermissionDto { GroupTypes = new eLookupGroupType[] { eLookupGroupType.MultiacademyTrust } });

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = mockHttpContextBase.Object
            };


            var expectedGroup = new GroupModel { GroupTypeId = (int?) eLookupGroupType.MultiacademyTrust };
            mockGroupReadService.Setup(x => x.GetAsync(It.IsAny<int>(), It.IsAny<IPrincipal>()))
                .ReturnsAsync(new ServiceResultDto<GroupModel> { ReturnValue = expectedGroup });

            var result = await controller.Details(id, search, searchsource, skip, sortBy, saved);

            var viewResult = Assert.IsType<ViewResult>(result);
            var viewModel = Assert.IsAssignableFrom<GroupDetailViewModel>(viewResult.Model);
            Assert.NotNull(viewModel.GovernorsGridViewModel);
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
