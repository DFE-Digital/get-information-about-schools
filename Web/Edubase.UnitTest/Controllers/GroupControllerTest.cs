using Edubase.Services.Core;
using Edubase.Services.Domain;
using Edubase.Services.Enums;
using Edubase.Services.Establishments;
using Edubase.Services.Exceptions;
using Edubase.Services.Groups;
using Edubase.Services.Groups.Models;
using Edubase.Services.IntegrationEndPoints.CompaniesHouse;
using Edubase.Services.Lookup;
using Edubase.Services.Nomenclature;
using Edubase.Services.Security;
using Edubase.Web.UI.Areas.Groups.Controllers;
using Edubase.Web.UI.Areas.Groups.Models;
using Edubase.Web.UI.Areas.Groups.Models.CreateEdit;
using Edubase.Web.UI.Exceptions;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using static Edubase.Web.UI.Areas.Groups.Models.CreateEdit.GroupEditorViewModel;

namespace Edubase.UnitTest.Controllers
{
    [TestFixture]
    public class GroupControllerTest : UnitTestBase<GroupController>
    {
        [Test]
        [TestCase(true, true, TestName = "Group Detail; User logged on and can edit")]
        [TestCase(true, false, TestName = "Group Detail; on and cannot edit")]
        [TestCase(false, false, TestName = "Group Detail; Anonymous user")]
        public async Task Group_Details_WithValidRecord(bool isUserLoggedOn, bool canUserEdit)
        {
            var grs = GetMock<IGroupReadService>();
            var id = GetMock<IIdentity>();

            var estabList = Enumerable.Range(1, 10).Select(x => new EstablishmentGroupModel
            {
                Address = new AddressDto
                {
                    CityOrTown = Faker.Address.City(),
                    Line1 = Faker.Address.StreetAddress(),
                    Line2 = Faker.Address.SecondaryAddress(),
                    Line3 = Faker.Address.UkCounty(),
                    PostCode = Faker.Address.UkPostCode()
                },
                CCIsLeadCentre = Faker.RandomNumber.Next() % 2 == 0,
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

            id.Setup(x => x.IsAuthenticated).Returns(isUserLoggedOn);
            grs.Setup(x => x.GetAsync(It.IsAny<int>(), It.IsAny<IPrincipal>())).ReturnsAsync(new ServiceResultDto<GroupModel>(new GroupModel { GroupUId = 1, Name = "grp" }));
            grs.Setup(x => x.CanEditAsync(It.IsAny<int>(), It.IsAny<IPrincipal>())).ReturnsAsync(canUserEdit);
            grs.Setup(x => x.GetChangeHistoryAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<IPrincipal>())).ReturnsAsync(new PaginatedResult<GroupChangeDto>());
            grs.Setup(x => x.GetEstablishmentGroupsAsync(It.IsAny<int>(), It.IsAny<IPrincipal>(), true)).ReturnsAsync(estabList);

            var response = (ViewResult) await ObjectUnderTest.Details(1);

            if (!isUserLoggedOn)
            {
                grs.Verify(x => x.GetChangeHistoryAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<IPrincipal>()), Times.Never());
            }
            else
            {
                grs.Verify(x => x.GetChangeHistoryAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<IPrincipal>()), Times.Once());
            }

            var viewModel = (GroupDetailViewModel) response.Model;
            Assert.That(viewModel.CanUserEdit, Is.EqualTo(canUserEdit));
            Assert.That(viewModel.Establishments.Count, Is.EqualTo(estabList.Count));

            for (int i = 0; i < estabList.Count; i++)
            {
                Assert.That(viewModel.Establishments[i].Address, Is.EqualTo(estabList[i].Address.ToString()));
                Assert.That(viewModel.Establishments[i].Name, Is.EqualTo(estabList[i].Name));
                Assert.That(viewModel.Establishments[i].CCIsLeadCentre, Is.EqualTo(estabList[i].CCIsLeadCentre));
                Assert.That(viewModel.Establishments[i].EditMode, Is.EqualTo(false));
                Assert.That(viewModel.Establishments[i].HeadFirstName, Is.EqualTo(estabList[i].HeadFirstName));
                Assert.That(viewModel.Establishments[i].HeadLastName, Is.EqualTo(estabList[i].HeadLastName));
                Assert.That(viewModel.Establishments[i].HeadTitleName, Is.EqualTo(estabList[i].HeadTitle));
                Assert.That(viewModel.Establishments[i].Id, Is.EqualTo(estabList[i].Id));
                Assert.That(viewModel.Establishments[i].JoinedDate, Is.EqualTo(estabList[i].JoinedDate));
                Assert.That(viewModel.Establishments[i].LAESTAB, Is.EqualTo(estabList[i].LAESTAB));
                Assert.That(viewModel.Establishments[i].LocalAuthorityName, Is.EqualTo(estabList[i].LocalAuthorityName));
                Assert.That(viewModel.Establishments[i].PhaseName, Is.EqualTo(estabList[i].PhaseName));
                Assert.That(viewModel.Establishments[i].StatusName, Is.EqualTo(estabList[i].StatusName));
                Assert.That(viewModel.Establishments[i].TypeName, Is.EqualTo(estabList[i].TypeName));
                Assert.That(viewModel.Establishments[i].Urn, Is.EqualTo(estabList[i].Urn));
            }
        }

        [Test]
        public async Task Group_CreateNewGroup_InvalidType() => Assert.That(async () => await ObjectUnderTest.CreateNewGroup("invalidtype"), Throws.TypeOf<InvalidParameterException>());

        [Test]
        public async Task Group_CreateNewGroup_PermissionDenied()
        {
            GetMock<ICachedLookupService>().Setup(x => x.LocalAuthorityGetAllAsync()).ReturnsAsync(new LookupDto[] { new LookupDto { Name = "[placeholder_la]" } });
            GetMock<ICachedLookupService>().Setup(x => x.GroupTypesGetAllAsync()).ReturnsAsync(new LookupDto[] { new LookupDto { Name = "[placeholder_grouptype]", Id = 1 } });

            GetMock<ISecurityService>().Setup(x => x.GetCreateGroupPermissionAsync(It.IsAny<IPrincipal>())).ReturnsAsync(new CreateGroupPermissionDto { GroupTypes = new eLookupGroupType[0] });
            Assert.That(async () => await ObjectUnderTest.CreateNewGroup("Federation"), Throws.TypeOf<PermissionDeniedException>());
        }

        [Test]
        [TestCase("ChildrensCentre", 899, TestName = "Group_CreateNewGroup_ChildrensCentre_WithLA")]
        [TestCase("ChildrensCentre", null, TestName = "Group_CreateNewGroup_ChildrensCentre_NoLA")]
        [TestCase("Federation", null, TestName = "Group_CreateNewGroup_Federation")]
        [TestCase("Trust", null, TestName = "Group_CreateNewGroup_Trust")]
        [TestCase("Sponsor", null, TestName = "Group_CreateNewGroup_Sponsor")]
        public async Task Group_CreateNewGroup(string type, int? localAuthorityId)
        {
            GetMock<ISecurityService>().Setup(x => x.GetCreateGroupPermissionAsync(It.IsAny<IPrincipal>()))
                .ReturnsAsync(new CreateGroupPermissionDto { GroupTypes = new eLookupGroupType[] { eLookupGroupType.ChildrensCentresCollaboration, eLookupGroupType.Federation, eLookupGroupType.Trust, eLookupGroupType.SchoolSponsor }, CCLocalAuthorityId = localAuthorityId });

            GetMock<ICachedLookupService>().Setup(x => x.LocalAuthorityGetAllAsync()).ReturnsAsync(new LookupDto[] { new LookupDto { Name = "[placeholder_la]", Id = localAuthorityId.GetValueOrDefault() } });
            GetMock<ICachedLookupService>().Setup(x => x.GroupTypesGetAllAsync()).ReturnsAsync(new LookupDto[] { new LookupDto { Name = "[placeholder_grouptype]", Id = 1 } });
            GetMock<ICachedLookupService>().Setup(x => x.GetNameAsync(It.IsAny<Expression<Func<int?>>>(), It.IsAny<string>())).ReturnsAsync("placeholder");

            var result = (ViewResult)await ObjectUnderTest.CreateNewGroup(type);
            var model = (GroupEditorViewModel) result.Model;

            if (type == "ChildrensCentre")
            {
                Assert.That(model.GroupTypeId, Is.EqualTo((int)eLookupGroupType.ChildrensCentresCollaboration));
                Assert.That(model.SaveMode, Is.EqualTo(eSaveMode.DetailsAndLinks));
                Assert.That(result.ViewName, Is.EqualTo("CreateChildrensCentre"));

                if (localAuthorityId.HasValue)
                {
                    Assert.IsFalse(model.IsLocalAuthorityEditable);
                    Assert.That(model.LocalAuthorityId, Is.EqualTo(localAuthorityId));
                    Assert.That(model.LocalAuthorityName, Is.EqualTo("placeholder"));
                }
                else Assert.IsTrue(model.IsLocalAuthorityEditable);
            }
            else
            {
                Assert.That(model.SaveMode, Is.EqualTo(eSaveMode.Details));
                Assert.IsFalse(model.IsLocalAuthorityEditable);
                Assert.That(result.ViewName, Is.EqualTo("Create"));
            }

            switch (type)
            {
                case "Federation":
                    Assert.That(model.GroupTypeId, Is.EqualTo((int)eLookupGroupType.Federation));
                    break;
                case "Trust":
                    Assert.That(model.GroupTypeId, Is.EqualTo((int)eLookupGroupType.Trust));
                    break;
                case "Sponsor":
                    Assert.That(model.GroupTypeId, Is.EqualTo((int)eLookupGroupType.SchoolSponsor));
                    break;
            }
        }

        [Test]
        public async Task Group_Create()
        {
            GetMock<ICachedLookupService>().Setup(x => x.LocalAuthorityGetAllAsync()).ReturnsAsync(new LookupDto[] { new LookupDto { Name = "[placeholder_la]" } });
            GetMock<ICachedLookupService>().Setup(x => x.GroupTypesGetAllAsync()).ReturnsAsync(new LookupDto[] { new LookupDto { Name = "[placeholder_grouptype]", Id = 1 } });
            GetMock<ICachedLookupService>().Setup(x => x.GetNameAsync(It.IsAny<Expression<Func<int?>>>(), It.IsAny<string>())).ReturnsAsync("placeholder");
            GetMock<IGroupsWriteService>().Setup(x => x.ValidateAsync(It.IsAny<SaveGroupDto>(), It.IsAny<IPrincipal>())).ReturnsAsync(new ValidationEnvelopeDto());
            GetMock<IGroupsWriteService>().Setup(x => x.SaveNewAsync(It.IsAny<SaveGroupDto>(), It.IsAny<IPrincipal>())).ReturnsAsync(new ApiResponse<NumericResultDto>(true) { Response = new NumericResultDto { Value = 123 } });
            GetMock<IGroupReadService>().Setup(x => x.ExistsAsync(It.IsAny<IPrincipal>(), It.IsAny<CompaniesHouseNumber?>(), It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<string>(), It.IsAny<int?>())).ReturnsAsync(false);
            GetMock<ISecurityService>().Setup(x => x.CreateAnonymousPrincipal()).Returns(new GenericPrincipal(new GenericIdentity(""), new string[0]));

            var result = (RedirectToRouteResult) await ObjectUnderTest.Create(new GroupEditorViewModel
            {
                GroupName = "test group",
                Action = ActionSave,
                GroupTypeId = (int) eLookupGroupType.Federation,
                OpenDate = new Web.UI.Models.DateTimeViewModel(DateTime.Now)
            });

            Assert.That(result.RouteValues["action"], Is.EqualTo("Details"));
            Assert.That(result.RouteValues["id"], Is.EqualTo(123));
        }

        [Test]
        public async Task Group_Create_WithSimilarNameWarning()
        {
            GetMock<ICachedLookupService>().Setup(x => x.LocalAuthorityGetAllAsync()).ReturnsAsync(new LookupDto[] { new LookupDto { Name = "[placeholder_la]" } });
            GetMock<ICachedLookupService>().Setup(x => x.GroupTypesGetAllAsync()).ReturnsAsync(new LookupDto[] { new LookupDto { Name = "[placeholder_grouptype]", Id = 1 } });
            GetMock<ICachedLookupService>().Setup(x => x.GetNameAsync(It.IsAny<Expression<Func<int?>>>(), It.IsAny<string>())).ReturnsAsync("placeholder");
            GetMock<IGroupsWriteService>().Setup(x => x.ValidateAsync(It.IsAny<SaveGroupDto>(), It.IsAny<IPrincipal>())).ReturnsAsync(new ValidationEnvelopeDto { Warnings = new List<ApiWarning> { new ApiWarning { Code = ApiWarningCodes.GROUP_WITH_SIMILAR_NAME_FOUND, Message = "similar" } } });
            GetMock<IGroupsWriteService>().Setup(x => x.SaveNewAsync(It.IsAny<SaveGroupDto>(), It.IsAny<IPrincipal>())).ReturnsAsync(new ApiResponse<NumericResultDto>(true) { Response = new NumericResultDto { Value = 123 } });
            GetMock<IGroupReadService>().Setup(x => x.ExistsAsync(It.IsAny<IPrincipal>(), It.IsAny<CompaniesHouseNumber?>(), It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<string>(), It.IsAny<int?>())).ReturnsAsync(false);
            GetMock<ISecurityService>().Setup(x => x.CreateAnonymousPrincipal()).Returns(new GenericPrincipal(new GenericIdentity(""), new string[0]));

            var result = (ViewResult) await ObjectUnderTest.Create(new GroupEditorViewModel
            {
                GroupName = "test group",
                Action = ActionSave,
                GroupTypeId = (int)eLookupGroupType.Federation,
                OpenDate = new Web.UI.Models.DateTimeViewModel(DateTime.Now)
            });
            var model = (GroupEditorViewModel) result.Model;
            Assert.That(model.WarningsToProcess.Any(), Is.EqualTo(true));
            
            // Then dismiss the warnings...
            var result2 = (RedirectToRouteResult) await ObjectUnderTest.Create(new GroupEditorViewModel
            {
                GroupName = "test group",
                Action = ActionSave,
                GroupTypeId = (int)eLookupGroupType.Federation,
                OpenDate = new Web.UI.Models.DateTimeViewModel(DateTime.Now),
                ProcessedWarnings = true
            });

            Assert.That(result2.RouteValues["action"], Is.EqualTo("Details"));
            Assert.That(result2.RouteValues["id"], Is.EqualTo(123));
        }


        [Test]
        public async Task Group_Create_WithUnknownWarning_IgnoresIt()
        {
            GetMock<ICachedLookupService>().Setup(x => x.LocalAuthorityGetAllAsync()).ReturnsAsync(new LookupDto[] { new LookupDto { Name = "[placeholder_la]" } });
            GetMock<ICachedLookupService>().Setup(x => x.GroupTypesGetAllAsync()).ReturnsAsync(new LookupDto[] { new LookupDto { Name = "[placeholder_grouptype]", Id = 1 } });
            GetMock<ICachedLookupService>().Setup(x => x.GetNameAsync(It.IsAny<Expression<Func<int?>>>(), It.IsAny<string>())).ReturnsAsync("placeholder");
            GetMock<IGroupsWriteService>().Setup(x => x.ValidateAsync(It.IsAny<SaveGroupDto>(), It.IsAny<IPrincipal>())).ReturnsAsync(new ValidationEnvelopeDto { Warnings = new List<ApiWarning> { new ApiWarning { Code = "unknown", Message = "similar" } } });
            GetMock<IGroupsWriteService>().Setup(x => x.SaveNewAsync(It.IsAny<SaveGroupDto>(), It.IsAny<IPrincipal>())).ReturnsAsync(new ApiResponse<NumericResultDto>(true) { Response = new NumericResultDto { Value = 123 } });
            GetMock<IGroupReadService>().Setup(x => x.ExistsAsync(It.IsAny<IPrincipal>(), It.IsAny<CompaniesHouseNumber?>(), It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<string>(), It.IsAny<int?>())).ReturnsAsync(false);
            GetMock<ISecurityService>().Setup(x => x.CreateAnonymousPrincipal()).Returns(new GenericPrincipal(new GenericIdentity(""), new string[0]));
            
            var result = (RedirectToRouteResult)await ObjectUnderTest.Create(new GroupEditorViewModel
            {
                GroupName = "test group",
                Action = ActionSave,
                GroupTypeId = (int)eLookupGroupType.Federation,
                OpenDate = new Web.UI.Models.DateTimeViewModel(DateTime.Now),
                ProcessedWarnings = true
            });

            Assert.That(result.RouteValues["action"], Is.EqualTo("Details"));
            Assert.That(result.RouteValues["id"], Is.EqualTo(123));
        }


        [SetUp]
        public void SetUpTest() => SetupObjectUnderTest();

        [TearDown]
        public void TearDownTest() => ResetMocks();

        [OneTimeSetUp]
        protected override void InitialiseMocks()
        {
            AddMock<IEstablishmentReadService>();
            AddMock<IGroupReadService>();
            AddMock<IGroupsWriteService>();
            AddMock<ICachedLookupService>();
            AddMock<ICompaniesHouseService>();
            AddMock<ISecurityService>();
            RealObjects.Add(new NomenclatureService());
            base.InitialiseMocks();
        }

    }
}
