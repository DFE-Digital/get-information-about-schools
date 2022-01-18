using Edubase.Common;
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
using Edubase.Services.ExternalLookup;
using Moq;
using NUnit.Framework;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.UI;
using Edubase.Services.Governors;
using Edubase.Services.Governors.Models;
using static Edubase.Web.UI.Areas.Groups.Models.CreateEdit.GroupEditorViewModel;
using static Edubase.Web.UI.Areas.Groups.Models.CreateEdit.GroupEditorViewModelBase;

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
            var govrs = GetMock<IGovernorsReadService>();
            var id = GetMock<IIdentity>();
            var estabList = CreateEstabList();

            id.Setup(x => x.IsAuthenticated).Returns(isUserLoggedOn);
            grs.Setup(x => x.GetAsync(It.IsAny<int>(), It.IsAny<IPrincipal>())).ReturnsAsync(new ServiceResultDto<GroupModel>(new GroupModel { GroupUId = 1, Name = "grp" }));
            grs.Setup(x => x.GetLinksAsync(It.IsAny<int>(), It.IsAny<IPrincipal>())).ReturnsAsync(Enumerable.Empty<LinkedGroupModel>());
            grs.Setup(x => x.CanEditAsync(It.IsAny<int>(), It.IsAny<IPrincipal>())).ReturnsAsync(canUserEdit);
            grs.Setup(x => x.CanEditGovernanceAsync(It.IsAny<int>(), It.IsAny<IPrincipal>())).ReturnsAsync(canUserEdit);
            grs.Setup(x => x.GetChangeHistoryAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<IPrincipal>())).ReturnsAsync(new PaginatedResult<GroupChangeDto>());
            grs.Setup(x => x.GetEstablishmentGroupsAsync(It.IsAny<int>(), It.IsAny<IPrincipal>(), true)).ReturnsAsync(estabList);
            govrs.Setup(x => x.GetGovernorPermissions(null, It.IsAny<int>(), It.IsAny<IPrincipal>())).ReturnsAsync(() => new GovernorPermissions { Add = true, Update = true, Remove = true });
            var response = (ViewResult)await ObjectUnderTest.Details(1);

            if (!isUserLoggedOn)
            {
                grs.Verify(x => x.GetChangeHistoryAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<IPrincipal>()), Times.Never());
            }
            else
            {
                grs.Verify(x => x.GetChangeHistoryAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<IPrincipal>()), Times.Once());
            }

            var viewModel = (GroupDetailViewModel)response.Model;
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
        public void Group_CreateNewGroup_InvalidType() => Assert.That(async () => await ObjectUnderTest.CreateNewGroup("invalidtype"), Throws.TypeOf<InvalidParameterException>());

        [Test]
        public void Group_CreateNewGroup_PermissionDenied()
        {
            GetMock<IPrincipal>().Setup(x => x.IsInRole(It.IsAny<string>())).Returns(true);
            InjectBasicLAsAndGroupTypes();
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

            InjectBasicLAsAndGroupTypes();

            GetMock<IPrincipal>().Setup(x => x.IsInRole(It.IsAny<string>())).Returns(false);

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
            InjectBasicLAsAndGroupTypes();
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
            }, ActionSave);

            Assert.That(result.RouteValues["action"], Is.EqualTo("Details"));
            Assert.That(result.RouteValues["id"], Is.EqualTo(123));
        }

        [Test]
        public async Task Group_Create_WithSimilarNameWarning()
        {
            InjectBasicLAsAndGroupTypes();
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
            }, ActionSave);
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
            }, ActionSave);

            Assert.That(result2.RouteValues["action"], Is.EqualTo("Details"));
            Assert.That(result2.RouteValues["id"], Is.EqualTo(123));
        }


        [Test]
        public async Task Group_Create_WithUnknownWarning_IgnoresIt()
        {
            InjectBasicLAsAndGroupTypes();
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
            }, ActionSave);

            Assert.That(result.RouteValues["action"], Is.EqualTo("Details"));
            Assert.That(result.RouteValues["id"], Is.EqualTo(123));
        }


        [Test]
        public async Task Group_EditDetails()
        {
            var grs = GetMock<IGroupReadService>();
            var estabList = CreateEstabList();

            GetMock<IPrincipal>().Setup(x => x.IsInRole(It.IsAny<string>())).Returns(true);

            InjectBasicLAsAndGroupTypes();

            var domainModel = new GroupModel
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
                GroupTypeId = (int)eLookupGroupType.Federation,
                GroupUId = 123,
                HeadFirstName = Faker.Name.First(),
                HeadLastName = Faker.Name.Last(),
                HeadTitleId = 1,
                LocalAuthorityId = 1,
                ManagerEmailAddress = Faker.Internet.Email(),
                Name = "I am Federation",
                OpenDate = DateTime.Now.Subtract(TimeSpan.FromDays(10)),
                StatusId = (int)eLookupGroupStatus.Open
            };

            grs.Setup(x => x.GetAsync(It.IsAny<int>(), It.IsAny<IPrincipal>())).ReturnsAsync(new ServiceResultDto<GroupModel>(domainModel));
            grs.Setup(x => x.GetEstablishmentGroupsAsync(It.IsAny<int>(), It.IsAny<IPrincipal>(), true)).ReturnsAsync(estabList);

            var response = (ViewResult)await ObjectUnderTest.EditDetails(123);
            var vm = (GroupEditorViewModel) response.Model;
            Assert.That(response.ViewName, Is.EqualTo("EditDetails"));
            Assert.That(vm.GroupName, Is.EqualTo("I am Federation"));
            Assert.That(vm.GroupId, Is.EqualTo("123"));
            Assert.That(vm.GroupType, Is.EqualTo(eLookupGroupType.Federation));
            Assert.That(vm.GroupTypeId, Is.EqualTo((int)eLookupGroupType.Federation));
            Assert.That(vm.GroupTypeMode, Is.EqualTo(eGroupTypeMode.Federation));
            Assert.That(vm.GroupTypeName, Is.EqualTo("placeholder"));
            Assert.That(vm.GroupUId, Is.EqualTo(123));
            Assert.That(vm.InEditMode, Is.True);
            Assert.That(vm.IsLocalAuthorityEditable, Is.False);
            Assert.That(vm.LinkedEstablishments.Establishments.Count, Is.EqualTo(10));
            Assert.That(vm.ListOfEstablishmentsPluralName, Is.EqualTo("Schools"));
            Assert.That(vm.LocalAuthorityId, Is.EqualTo(1));
            Assert.That(vm.LocalAuthorityName, Is.EqualTo("placeholder"));
            Assert.That(vm.ManagerEmailAddress, Is.EqualTo(domainModel.ManagerEmailAddress));
            Assert.That(vm.OpenDate.ToDateTime().GetValueOrDefault().Date, Is.EqualTo(domainModel.OpenDate.GetValueOrDefault().Date));
            Assert.That(vm.ClosedDate.ToDateTime().GetValueOrDefault().Date, Is.EqualTo(domainModel.ClosedDate.GetValueOrDefault().Date));
            Assert.That(vm.OpenDateLabel, Is.EqualTo("Open date"));
            Assert.That(vm.PageTitle, Is.EqualTo("Edit federation"));
            Assert.That(vm.StatusId, Is.EqualTo((int)eLookupGroupStatus.Open));
            Assert.That(vm.Address, Is.EqualTo(domainModel.Address.ToString()));
            Assert.That(vm.CanUserCloseAndMarkAsCreatedInError, Is.False);
            Assert.That(vm.CCLeadCentreUrn, Is.EqualTo(estabList.Single(x => x.CCIsLeadCentre == true).Urn));
        }

        [Test]
        [TestCase(eLookupGroupType.SingleacademyTrust, "Single-academy trust name", "Close this single-academy trust and mark as created in error", TestName = "Group_EditDetails_DynamicLabels_SingleacademyTrust")]
        [TestCase(eLookupGroupType.MultiacademyTrust, "Multi-academy trust name", "Close this multi-academy trust and mark as created in error", TestName = "Group_EditDetails_DynamicLabels_MultiacademyTrust")]
        [TestCase(eLookupGroupType.ChildrensCentresCollaboration, "Children's centres collaboration name", "", TestName = "Group_EditDetails_DynamicLabels_ChildrensCentresCollaboration")]
        [TestCase(eLookupGroupType.ChildrensCentresGroup, "Children's centres group name", "", TestName = "Group_EditDetails_DynamicLabels_ChildrensCentresGroup")]
        [TestCase(eLookupGroupType.Federation, "Federation name", "", TestName = "Group_EditDetails_DynamicLabels_Federation")]
        [TestCase(eLookupGroupType.Trust, "Foundation trust name", "", TestName = "Group_EditDetails_DynamicLabels_Trust")]
        [TestCase(eLookupGroupType.SchoolSponsor, "Academy sponsor name", "Close this academy sponsor and mark as created in error", TestName = "Group_EditDetails_DynamicLabels_SchoolSponsor")]
        public async Task Group_EditDetails_DynamicLabels(eLookupGroupType groupType, string groupNameLabelText, string closeAndMarkAsCreatedInErrorLabelText)
        {
            var grs = GetMock<IGroupReadService>();
            var estabList = CreateEstabList();

            GetMock<IPrincipal>().Setup(x => x.IsInRole(It.IsAny<string>())).Returns(true);

            InjectBasicLAsAndGroupTypes();

            var domainModel = new GroupModel
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
                Name = "I am a Group",
                OpenDate = DateTime.Now.Subtract(TimeSpan.FromDays(10)),
                StatusId = (int) eLookupGroupStatus.Open
            };

            grs.Setup(x => x.GetAsync(It.IsAny<int>(), It.IsAny<IPrincipal>())).ReturnsAsync(new ServiceResultDto<GroupModel>(domainModel));
            grs.Setup(x => x.GetEstablishmentGroupsAsync(It.IsAny<int>(), It.IsAny<IPrincipal>(), true)).ReturnsAsync(estabList);

            var response = (ViewResult) await ObjectUnderTest.EditDetails(123);
            var vm = (GroupEditorViewModel) response.Model;

            Assert.That(vm.GroupNameLabel, Is.EqualTo(groupNameLabelText));
            if (vm.CanUserCloseAndMarkAsCreatedInError)
            {
                Assert.That(vm.CloseAndMarkAsCreatedInErrorLabel, Is.EqualTo(closeAndMarkAsCreatedInErrorLabelText));
            }
            else
            {
                Assert.That(vm.CloseAndMarkAsCreatedInErrorLabel, Is.Null);
            }
        }


        [Test]
        public async Task Group_EditLinks()
        {
            var grs = GetMock<IGroupReadService>();
            var estabList = CreateEstabList();

            InjectBasicLAsAndGroupTypes();

            var domainModel = new GroupModel
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
                GroupTypeId = (int)eLookupGroupType.Federation,
                GroupUId = 123,
                HeadFirstName = Faker.Name.First(),
                HeadLastName = Faker.Name.Last(),
                HeadTitleId = 1,
                LocalAuthorityId = 1,
                ManagerEmailAddress = Faker.Internet.Email(),
                Name = "I am Federation",
                OpenDate = DateTime.Now.Subtract(TimeSpan.FromDays(10)),
                StatusId = (int)eLookupGroupStatus.Open
            };

            grs.Setup(x => x.GetAsync(It.IsAny<int>(), It.IsAny<IPrincipal>())).ReturnsAsync(new ServiceResultDto<GroupModel>(domainModel));
            grs.Setup(x => x.GetEstablishmentGroupsAsync(It.IsAny<int>(), It.IsAny<IPrincipal>(), true)).ReturnsAsync(estabList);

            var response = (ViewResult)await ObjectUnderTest.EditLinks(123);
            var vm = (GroupEditorViewModel)response.Model;
            Assert.That(response.ViewName, Is.EqualTo(string.Empty));
            Assert.That(vm.GroupName, Is.EqualTo("I am Federation"));
            Assert.That(vm.GroupUId, Is.EqualTo(123));
            Assert.That(vm.GroupType, Is.EqualTo(eLookupGroupType.Federation));
            Assert.That(vm.GroupTypeId, Is.EqualTo((int)eLookupGroupType.Federation));
            Assert.That(vm.GroupTypeMode, Is.EqualTo(eGroupTypeMode.Federation));
            Assert.That(vm.GroupTypeName, Is.EqualTo("placeholder"));
            Assert.That(vm.CCLeadCentreUrn, Is.EqualTo(estabList.Single(x => x.CCIsLeadCentre == true).Urn));
            Assert.That(vm.SaveMode, Is.EqualTo(eSaveMode.Links));
            Assert.That(vm.InEditMode, Is.True);
            Assert.That(vm.LinkedEstablishments.Establishments.Count, Is.EqualTo(10));
            Assert.That(vm.ListOfEstablishmentsPluralName, Is.EqualTo("Schools"));
            Assert.That(vm.SelectedTabName, Is.EqualTo("links"));
            Assert.That(vm.OpenDateLabel, Is.EqualTo("Open date"));
            Assert.That(vm.PageTitle, Is.EqualTo("Edit federation"));
            Assert.That(vm.CanUserCloseAndMarkAsCreatedInError, Is.False);
        }

        [Test]
        public void Group_Convert()
        {
            var response = (ViewResult) ObjectUnderTest.Convert();
            var vm = (ConvertSATViewModel)response.Model;
            Assert.That(response.ViewName, Is.EqualTo(string.Empty));
            Assert.That(response.Model, Is.TypeOf<ConvertSATViewModel>());
        }

        [Test]
        public async Task Group_Convert_FindNonExistentSAT()
        {
            var grs = GetMock<IGroupReadService>();
            grs.Setup(x => x.SearchByIdsAsync(It.IsAny<string>(), It.Is<int?>(i => i == 1000), It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<IPrincipal>())).ReturnsAsync(() => new ApiPagedResult<SearchGroupDocument>(0, new List<SearchGroupDocument>()));
            
            var response = (ViewResult) await ObjectUnderTest.Convert(new ConvertSATViewModel
            {
                ActionName = "find",
                Text = "1000"
            });

            Assert.That(ObjectUnderTest.ModelState["Text"].Errors[0].ErrorMessage, Is.EqualTo("We were unable to find a single-academy trust matching those details"));
            Assert.That(response.ViewName, Is.EqualTo(string.Empty));
        }

        [Test]
        public async Task Group_Convert_FindInvalidGroupType()
        {
            var grs = GetMock<IGroupReadService>();
            grs.Setup(x => x.SearchByIdsAsync(It.IsAny<string>(), It.Is<int?>(i => i == 1000), It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<IPrincipal>())).ReturnsAsync(() => new ApiPagedResult<SearchGroupDocument>(1, new List<SearchGroupDocument>(){
                new SearchGroupDocument{ Name="Group 1000", GroupUId = 1000, GroupTypeId=(int)eLookupGroupType.MultiacademyTrust }
            }));

            var response = (ViewResult)await ObjectUnderTest.Convert(new ConvertSATViewModel
            {
                ActionName = "find",
                Text = "1000"
            });

            Assert.That(ObjectUnderTest.ModelState["Text"].Errors[0].ErrorMessage, Is.EqualTo("That's an invalid group because it's of the wrong type."));
            Assert.That(response.ViewName, Is.EqualTo(string.Empty));
        }

        [Test]
        public async Task Group_Convert_FindValidGroup()
        {
            GetMock<ICachedLookupService>().Setup(x => x.GetNameAsync(It.IsAny<Expression<Func<int?>>>(), It.IsAny<string>())).ReturnsAsync("placeholder");
            GetMock<IGroupReadService>().Setup(x => x.SearchByIdsAsync(It.IsAny<string>(), It.Is<int?>(i => i == 1000), It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<IPrincipal>())).ReturnsAsync(() => new ApiPagedResult<SearchGroupDocument>(1, new List<SearchGroupDocument>(){
                new SearchGroupDocument{ Name="Group 1000", GroupUId = 1000, GroupTypeId=(int)eLookupGroupType.SingleacademyTrust }
            }));

            var vm = new ConvertSATViewModel
            {
                ActionName = "find",
                Text = "1000"
            };
            var response = (ViewResult)await ObjectUnderTest.Convert(vm);
            vm.Details.Name.ShouldBe("Group 1000");
            vm.Details.GroupUId.ShouldBe(1000);
            vm.Details.GroupTypeId.ShouldBe((int)eLookupGroupType.SingleacademyTrust);
            vm.CountryName.ShouldBe("placeholder");
            vm.CountyName.ShouldBe("placeholder");
            vm.Token.ShouldNotBe(null);
            response.ViewName.ShouldBe("");
        }

        [Test]
        public async Task Group_Convert_Confirm_Success()
        {
            var domainModel = new SearchGroupDocument { Name = "Group 1000", GroupUId = 1000, GroupTypeId = (int)eLookupGroupType.SingleacademyTrust };
            GetMock<IGroupsWriteService>().Setup(x => x.ConvertSAT2MAT(It.Is<int>(i => i == 1000), It.Is<bool>(b => b == true), It.IsAny<IPrincipal>())).ReturnsAsync(new ApiResponse<NumericResultDto>(true) { Response = new NumericResultDto { Value = 200 } });

            var vm = new ConvertSATViewModel
            {
                Token = UriHelper.SerializeToUrlToken(domainModel),
                ActionName = "confirm",
                CopyGovernanceInfo = true
            };
            var response = (RedirectToRouteResult) await ObjectUnderTest.Convert(vm);
            response.RouteName.ShouldBe("GroupDetails");
            response.RouteValues["id"].ShouldBe(200);
            GetMock<IGroupsWriteService>().Verify(x => x.ConvertSAT2MAT(It.Is<int>(i => i == 1000), It.Is<bool>(b => b == true), It.IsAny<IPrincipal>()), Times.Once);
        }

        [Test]
        public async Task Group_Convert_Confirm_WithValidationError()
        {
            var domainModel = new SearchGroupDocument { Name = "Group 1000", GroupUId = 1000, GroupTypeId = (int)eLookupGroupType.SingleacademyTrust };
            GetMock<IGroupsWriteService>().Setup(x => x.ConvertSAT2MAT(It.Is<int>(i => i == 1000), It.Is<bool>(b => b == true), It.IsAny<IPrincipal>())).ReturnsAsync(new ApiResponse<NumericResultDto>(false) { Errors = new [] { new ApiError { Code="test", Message="msg" } } });

            var vm = new ConvertSATViewModel
            {
                Token = UriHelper.SerializeToUrlToken(domainModel),
                ActionName = "confirm",
                CopyGovernanceInfo = true
            };
            var response = (ViewResult)await ObjectUnderTest.Convert(vm);
            response.ViewName.ShouldBe("");
            Assert.That(ObjectUnderTest.ModelState[""].Errors[0].ErrorMessage, Is.EqualTo("msg"));
            GetMock<IGroupsWriteService>().Verify(x => x.ConvertSAT2MAT(It.Is<int>(i => i == 1000), It.Is<bool>(b => b == true), It.IsAny<IPrincipal>()), Times.Once);
        }

        [Test]
        public async Task Group_EditDetails_Post_Success()
        {
            GetMock<IGroupsWriteService>().Setup(x => x.ValidateAsync(It.IsAny<SaveGroupDto>(), It.IsAny<IPrincipal>())).ReturnsAsync(new ValidationEnvelopeDto());
            GetMock<IGroupReadService>().Setup(x => x.ExistsAsync(It.IsAny<IPrincipal>(), It.IsAny<CompaniesHouseNumber?>(), It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<string>(), It.IsAny<int?>())).ReturnsAsync(false);
            GetMock<IGroupReadService>().Setup(x => x.GetModelChangesAsync(It.IsAny<GroupModel>(), It.IsAny<IPrincipal>())).ReturnsAsync(new List<ChangeDescriptorDto>());
            GetMock<ISecurityService>().Setup(x => x.CreateAnonymousPrincipal()).Returns(new GenericPrincipal(new GenericIdentity(""), new string[0]));
            GetMock<IGroupsWriteService>().Setup(x => x.SaveAsync(It.IsAny<SaveGroupDto>(), It.IsAny<IPrincipal>())).ReturnsAsync(new ApiResponse(true));
            GetMock<IGroupReadService>().Setup(x =>
                x.GetEstablishmentGroupsAsync(It.IsAny<int>(), It.IsAny<IPrincipal>(), It.IsAny<bool>())).ReturnsAsync(new List<EstablishmentGroupModel>());
            InjectBasicLAsAndGroupTypes();

            var viewModel = new GroupEditorViewModel
            {
                GroupUId = 123,
                GroupName = "This is a test",
                Action = ActionSave,
                GroupTypeId = (int)eLookupGroupType.Federation
            };
            var result = (RedirectToRouteResult) await ObjectUnderTest.EditDetails(viewModel);
            Assert.That(result.RouteValues["action"], Is.EqualTo("Details"));
            Assert.That(result.RouteValues["id"], Is.EqualTo(123));

            GetMock<IGroupsWriteService>().Verify(x => x.SaveAsync(It.Is<SaveGroupDto>(v => v.LinkedEstablishments == null && v.GroupUId == 123 && v.Group != null), It.IsAny<IPrincipal>()), Times.Once);
        }

        [Test]
        public async Task Group_EditLinks_Post_Success()
        {
            var domainModel = CreateGroupModel();
            var gws = GetMock<IGroupsWriteService>();
            var grs = GetMock<IGroupReadService>();
            grs.Setup(x => x.GetAsync(It.IsAny<int>(), It.IsAny<IPrincipal>())).ReturnsAsync(new ServiceResultDto<GroupModel>(domainModel));
            gws.Setup(x => x.ValidateAsync(It.IsAny<SaveGroupDto>(), It.IsAny<IPrincipal>())).ReturnsAsync(new ValidationEnvelopeDto());
            grs.Setup(x => x.ExistsAsync(It.IsAny<IPrincipal>(), It.IsAny<CompaniesHouseNumber?>(), It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<string>(), It.IsAny<int?>())).ReturnsAsync(false);
            GetMock<ISecurityService>().Setup(x => x.CreateAnonymousPrincipal()).Returns(new GenericPrincipal(new GenericIdentity(""), new string[0]));
            gws.Setup(x => x.SaveAsync(It.IsAny<SaveGroupDto>(), It.IsAny<IPrincipal>())).ReturnsAsync(new ApiResponse(true));
            InjectBasicLAsAndGroupTypes();

            var viewModel = new GroupEditorViewModel
            {
                GroupUId = 123,
                GroupName = "This is a test",
                Action = ActionSave,
                SaveMode = eSaveMode.Links,
                GroupTypeId = (int)eLookupGroupType.Federation,
                LinkedEstablishments = new GroupLinkedEstablishmentsViewModel
                {
                    Establishments = CreateEstablishmentGroupViewModelList(1)
                }
            };
            var result = (RedirectToRouteResult)await ObjectUnderTest.EditLinks(viewModel);
            Assert.That(result.RouteValues["action"], Is.EqualTo("Details"));
            Assert.That(result.RouteValues["id"], Is.EqualTo(123));

            GetMock<IGroupsWriteService>().Verify(x => x.SaveAsync(It.Is<SaveGroupDto>(v => v.LinkedEstablishments != null && v.GroupUId == 123 && v.Group == null), It.IsAny<IPrincipal>()), Times.Once);
        }


        [Test]
        public async Task Group_EditLinks_RemoveEstablishment_Success()
        {
            var domainModel = CreateGroupModel();

            var grs = GetMock<IGroupReadService>();
            grs.Setup(x => x.GetAsync(It.IsAny<int>(), It.IsAny<IPrincipal>())).ReturnsAsync(new ServiceResultDto<GroupModel>(domainModel));
            grs.Setup(x => x.ExistsAsync(It.IsAny<IPrincipal>(), It.IsAny<CompaniesHouseNumber?>(), It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<string>(), It.IsAny<int?>())).ReturnsAsync(false);
            GetMock<IGroupsWriteService>().Setup(x => x.ValidateAsync(It.IsAny<SaveGroupDto>(), It.IsAny<IPrincipal>())).ReturnsAsync(new ValidationEnvelopeDto());
            GetMock<ISecurityService>().Setup(x => x.CreateAnonymousPrincipal()).Returns(new GenericPrincipal(new GenericIdentity(""), new string[0]));
            GetMock<IGroupsWriteService>().Setup(x => x.SaveAsync(It.IsAny<SaveGroupDto>(), It.IsAny<IPrincipal>())).ReturnsAsync(new ApiResponse(true));
            InjectBasicLAsAndGroupTypes();

            var estabs = CreateEstablishmentGroupViewModelList(3);
            var viewModel = new GroupEditorViewModel
            {
                GroupUId = 123,
                GroupName = "This is a test",
                Action = ActionLinkedEstablishmentRemove + estabs[0].Urn,
                SaveMode = eSaveMode.Links,
                GroupTypeId = (int)eLookupGroupType.Federation,
                LinkedEstablishments = new GroupLinkedEstablishmentsViewModel
                {
                    Establishments = estabs
                }
            };
            var result = (ViewResult)await ObjectUnderTest.EditLinks(viewModel);
            viewModel.LinkedEstablishments.Establishments.Count.ShouldBe(2);
        }

        [Test]
        public async Task Group_EditLinks_CancelEditMode()
        {
            var domainModel = CreateGroupModel();

            var grs = GetMock<IGroupReadService>();
            grs.Setup(x => x.GetAsync(It.IsAny<int>(), It.IsAny<IPrincipal>())).ReturnsAsync(new ServiceResultDto<GroupModel>(domainModel));
            GetMock<IGroupsWriteService>().Setup(x => x.ValidateAsync(It.IsAny<SaveGroupDto>(), It.IsAny<IPrincipal>())).ReturnsAsync(new ValidationEnvelopeDto());
            GetMock<IGroupReadService>().Setup(x => x.ExistsAsync(It.IsAny<IPrincipal>(), It.IsAny<CompaniesHouseNumber?>(), It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<string>(), It.IsAny<int?>())).ReturnsAsync(false);
            GetMock<ISecurityService>().Setup(x => x.CreateAnonymousPrincipal()).Returns(new GenericPrincipal(new GenericIdentity(""), new string[0]));
            GetMock<IGroupsWriteService>().Setup(x => x.SaveAsync(It.IsAny<SaveGroupDto>(), It.IsAny<IPrincipal>())).ReturnsAsync(new ApiResponse(true));
            InjectBasicLAsAndGroupTypes();

            var viewModel = new GroupEditorViewModel
            {
                GroupUId = 123,
                GroupName = "This is a test",
                Action = ActionLinkedEstablishmentCancelEdit,
                SaveMode = eSaveMode.Links,
                GroupTypeId = (int)eLookupGroupType.Federation,
                LinkedEstablishments = new GroupLinkedEstablishmentsViewModel
                {
                    Establishments = CreateEstablishmentGroupViewModelList(3)
                }
            };
            viewModel.LinkedEstablishments.Establishments[0].EditMode = true;
            var result = (ViewResult)await ObjectUnderTest.EditLinks(viewModel);
            viewModel.LinkedEstablishments.Establishments[0].EditMode.ShouldBe(false);
        }


        [Test]
        public async Task Group_EditLinks_SetEditMode_Success()
        {
            var domainModel = CreateGroupModel();

            var grs = GetMock<IGroupReadService>();
            grs.Setup(x => x.GetAsync(It.IsAny<int>(), It.IsAny<IPrincipal>())).ReturnsAsync(new ServiceResultDto<GroupModel>(domainModel));
            grs.Setup(x => x.ExistsAsync(It.IsAny<IPrincipal>(), It.IsAny<CompaniesHouseNumber?>(), It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<string>(), It.IsAny<int?>())).ReturnsAsync(false);
            GetMock<IGroupsWriteService>().Setup(x => x.ValidateAsync(It.IsAny<SaveGroupDto>(), It.IsAny<IPrincipal>())).ReturnsAsync(new ValidationEnvelopeDto());
            GetMock<ISecurityService>().Setup(x => x.CreateAnonymousPrincipal()).Returns(new GenericPrincipal(new GenericIdentity(""), new string[0]));
            GetMock<IGroupsWriteService>().Setup(x => x.SaveAsync(It.IsAny<SaveGroupDto>(), It.IsAny<IPrincipal>())).ReturnsAsync(new ApiResponse(true));
            InjectBasicLAsAndGroupTypes();

            var estabs = CreateEstablishmentGroupViewModelList(3);
            var viewModel = new GroupEditorViewModel
            {
                GroupUId = 123,
                GroupName = "This is a test",
                Action = ActionLinkedEstablishmentEdit + estabs[0].Urn,
                SaveMode = eSaveMode.Links,
                GroupTypeId = (int)eLookupGroupType.Federation,
                LinkedEstablishments = new GroupLinkedEstablishmentsViewModel
                {
                    Establishments = estabs
                }
            };
            var result = (ViewResult)await ObjectUnderTest.EditLinks(viewModel);
            viewModel.LinkedEstablishments.Establishments[0].EditMode.ShouldBe(true);
            viewModel.LinkedEstablishments.Establishments[0].JoinedDateEditable.ToDateTime().Value.Date.ShouldBe(viewModel.LinkedEstablishments.Establishments[0].JoinedDate.Value.Date);
        }

        [Test]
        public async Task Group_EditLinks_SaveJoinedDate_Success()
        {
            var domainModel = CreateGroupModel();

            var grs = GetMock<IGroupReadService>();
            grs.Setup(x => x.GetAsync(It.IsAny<int>(), It.IsAny<IPrincipal>())).ReturnsAsync(new ServiceResultDto<GroupModel>(domainModel));
            grs.Setup(x => x.ExistsAsync(It.IsAny<IPrincipal>(), It.IsAny<CompaniesHouseNumber?>(), It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<string>(), It.IsAny<int?>())).ReturnsAsync(false);
            GetMock<IGroupsWriteService>().Setup(x => x.ValidateAsync(It.IsAny<SaveGroupDto>(), It.IsAny<IPrincipal>())).ReturnsAsync(new ValidationEnvelopeDto());
            GetMock<ISecurityService>().Setup(x => x.CreateAnonymousPrincipal()).Returns(new GenericPrincipal(new GenericIdentity(""), new string[0]));
            GetMock<IGroupsWriteService>().Setup(x => x.SaveAsync(It.IsAny<SaveGroupDto>(), It.IsAny<IPrincipal>())).ReturnsAsync(new ApiResponse(true));
            InjectBasicLAsAndGroupTypes();

            var estabs = CreateEstablishmentGroupViewModelList(3);
            var viewModel = new GroupEditorViewModel
            {
                GroupUId = 123,
                GroupName = "This is a test",
                Action = ActionLinkedEstablishmentSave,
                SaveMode = eSaveMode.Links,
                GroupTypeId = (int)eLookupGroupType.Federation,
                LinkedEstablishments = new GroupLinkedEstablishmentsViewModel
                {
                    Establishments = estabs
                }
            };
            estabs[0].JoinedDateEditable = new Web.UI.Models.DateTimeViewModel(DateTime.Now.AddDays(1));
            estabs[0].EditMode = true;
            var result = (ViewResult)await ObjectUnderTest.EditLinks(viewModel);
            viewModel.LinkedEstablishments.Establishments[0].EditMode.ShouldBe(false);
            viewModel.LinkedEstablishments.Establishments[0].JoinedDate.Value.Date.ShouldBe(viewModel.LinkedEstablishments.Establishments[0].JoinedDateEditable.ToDateTime().Value.Date);
        }

        [Test]
        public async Task Group_SaveNewAcademyTrust()
        {
            var gws = GetMock<IGroupsWriteService>();
            gws.Setup(x => x.ValidateAsync(It.IsAny<SaveGroupDto>(), It.IsAny<IPrincipal>())).ReturnsAsync(new ValidationEnvelopeDto());
            gws.Setup(x => x.SaveNewAsync(It.IsAny<SaveGroupDto>(), It.IsAny<IPrincipal>())).ReturnsAsync(new ApiResponse<NumericResultDto>(true) { Response = new NumericResultDto { Value = 123 } });

            GetMock<ISecurityService>().Setup(x => x.GetCreateGroupPermissionAsync(It.IsAny<IPrincipal>()))
                .ReturnsAsync(new CreateGroupPermissionDto { GroupTypes = new eLookupGroupType[] { eLookupGroupType.MultiacademyTrust }});


            var vm = new CreateAcademyTrustViewModel
            {
                CompaniesHouseAddressToken = UriHelper.SerializeToUrlToken(new AddressDto { Line1 = "line1", CityOrTown = "Bobville" }),
                CompaniesHouseNumber = "67362546543",
                TypeId = (int)eLookupGroupType.MultiacademyTrust,
                Name = "Multi acad",
                OpenDate = DateTime.Now,
                GroupId = "54243"
            };
            var result = (RedirectToRouteResult)await ObjectUnderTest.SaveNewAcademyTrust(vm);
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
            AddMock<IGovernorsReadService>();
            AddMock<ICachedLookupService>();
            AddMock<ICompaniesHouseService>();
            AddMock<ISecurityService>();
            AddMock<IExternalLookupService>();
            RealObjects.Add(new NomenclatureService());
            base.InitialiseMocks();
        }

        /// <summary>
        /// Sets up LocalAuthorityGetAllAsync and GroupTypesGetAllAsync
        /// </summary>
        private void InjectBasicLAsAndGroupTypes()
        {
            var cls = GetMock<ICachedLookupService>();
            cls.Setup(x => x.LocalAuthorityGetAllAsync()).ReturnsAsync(new LookupDto[] { new LookupDto { Name = "[placeholder_la]" } });
            cls.Setup(x => x.GroupTypesGetAllAsync()).ReturnsAsync(new LookupDto[] { new LookupDto { Name = "[placeholder_grouptype]", Id = 1 } });
            cls.Setup(x => x.GetNameAsync(It.IsAny<Expression<Func<int?>>>(), It.IsAny<string>())).ReturnsAsync("placeholder");
        }
        private GroupModel CreateGroupModel()
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
                GroupTypeId = (int)eLookupGroupType.Federation,
                GroupUId = 123,
                HeadFirstName = Faker.Name.First(),
                HeadLastName = Faker.Name.Last(),
                HeadTitleId = 1,
                LocalAuthorityId = 1,
                ManagerEmailAddress = Faker.Internet.Email(),
                Name = "I am Federation",
                OpenDate = DateTime.Now.Subtract(TimeSpan.FromDays(10)),
                StatusId = (int)eLookupGroupStatus.Open
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
                CCIsLeadCentre = (x == y),
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
                CCIsLeadCentre = (x == y),
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

    }
}
