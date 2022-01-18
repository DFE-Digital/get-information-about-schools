using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Routing;
using Edubase.Services.Domain;
using Edubase.Services.Enums;
using Edubase.Services.Establishments;
using Edubase.Services.Establishments.Models;
using Edubase.Services.Exceptions;
using Edubase.Services.Governors;
using Edubase.Services.Governors.DisplayPolicies;
using Edubase.Services.Governors.Models;
using Edubase.Services.Groups;
using Edubase.Services.Groups.Models;
using Edubase.Services.Lookup;
using Edubase.Services.Nomenclature;
using Edubase.Web.UI.Areas.Governors.Controllers;
using Edubase.Web.UI.Areas.Governors.Models;
using Edubase.Web.UI.Exceptions;
using Edubase.Web.UI.Helpers;
using Edubase.Web.UI.Models;
using Moq;
using NUnit.Framework;
using Shouldly;

namespace Edubase.UnitTest.Controllers
{
    [TestFixture]
    public class GovernorControllerTest : UnitTestBase<GovernorController>
    {
        [Test]
        public async Task Gov_Edit_Null_Params()
        {
            await ObjectUnderTest.Edit(null, null, null, null).ShouldThrowAsync<InvalidParameterException>();
        }

        [Test]
        public async Task Gov_Edit_GroupIdSpecified()
        {
            var groupId = 5;
            var governorDetailsDto = new GovernorsDetailsDto
            {
                ApplicableRoles = new List<eLookupGovernorRole> { eLookupGovernorRole.AccountingOfficer, eLookupGovernorRole.Governor },
                RoleDisplayPolicies = new Dictionary<eLookupGovernorRole, GovernorDisplayPolicy>
                {
                    { eLookupGovernorRole.AccountingOfficer, new GovernorDisplayPolicy() },
                    { eLookupGovernorRole.Governor, new GovernorDisplayPolicy() }
                },
                CurrentGovernors = new List<GovernorModel>(),
                HistoricalGovernors = new List<GovernorModel>()
            };

            GetMock<IGovernorsReadService>().Setup(g => g.GetGovernorListAsync(null, groupId, It.IsAny<IPrincipal>())).ReturnsAsync(() => governorDetailsDto);
            GetMock<IGovernorsReadService>().Setup(g => g.GetGovernorPermissions(null, groupId, It.IsAny<IPrincipal>())).ReturnsAsync(() => new GovernorPermissions { Add = true, Update = true, Remove = true });
            GetMock<ILayoutHelper>().Setup(l => l.PopulateLayoutProperties(It.IsAny<GovernorsGridViewModel>(), null, groupId, It.IsAny<IPrincipal>(), It.IsAny<Action<EstablishmentModel>>(), It.IsAny<Action<GroupModel>>())).Returns(Task.CompletedTask);
            SetupCachedLookupService();

            GetMock<ICachedLookupService>().Setup(c => c.GovernorRolesGetAllAsync()).ReturnsAsync(() => new List<LookupDto>
            {
                new LookupDto { Id = (int)eLookupGovernorRole.AccountingOfficer, Name = "Accounting Officer"},
                new LookupDto { Id = (int)eLookupGovernorRole.Governor, Name = "Governor"}
            });

            var result = await ObjectUnderTest.Edit(5, null, null, null);

            var viewResult = result as ViewResult;
            viewResult.ShouldNotBeNull();

            var model = viewResult.Model as GovernorsGridViewModel;
            model.ShouldNotBeNull();

            model.GovernorShared.ShouldBe(false);
            model.RemovalGid.ShouldBeNull();
            model.GroupUId.ShouldBe(groupId);
            model.EstablishmentUrn.ShouldBeNull();

            model.GovernorRoles.Count.ShouldBe(governorDetailsDto.ApplicableRoles.Count);

            viewResult.ViewData.Keys.Contains("DuplicateGovernor").ShouldBe(false);
            viewResult.ViewData.ModelState.IsValid.ShouldBeTrue();
        }

        [Test]
        public async Task Gov_Edit_EstabIdSpecified()
        {
            var establishmentId = 23;
            var governorDetailsDto = new GovernorsDetailsDto
            {
                ApplicableRoles = new List<eLookupGovernorRole> { eLookupGovernorRole.AccountingOfficer, eLookupGovernorRole.Governor },
                RoleDisplayPolicies = new Dictionary<eLookupGovernorRole, GovernorDisplayPolicy>
                {
                    { eLookupGovernorRole.AccountingOfficer, new GovernorDisplayPolicy() },
                    { eLookupGovernorRole.Governor, new GovernorDisplayPolicy() }
                },
                CurrentGovernors = new List<GovernorModel>(),
                HistoricalGovernors = new List<GovernorModel>()
            };

            GetMock<IGovernorsReadService>().Setup(g => g.GetGovernorListAsync(establishmentId, null, It.IsAny<IPrincipal>())).ReturnsAsync(() => governorDetailsDto);
            GetMock<IGovernorsReadService>().Setup(g => g.GetGovernorPermissions(establishmentId, null, It.IsAny<IPrincipal>())).ReturnsAsync(() => new GovernorPermissions { Add = true, Update = true, Remove = true });
            GetMock<ILayoutHelper>().Setup(l => l.PopulateLayoutProperties(It.IsAny<GovernorsGridViewModel>(), establishmentId, null, It.IsAny<IPrincipal>(), It.IsAny<Action<EstablishmentModel>>(), It.IsAny<Action<GroupModel>>())).Returns(Task.CompletedTask);
            SetupCachedLookupService();

            GetMock<ICachedLookupService>().Setup(c => c.GovernorRolesGetAllAsync()).ReturnsAsync(() => new List<LookupDto>
            {
                new LookupDto { Id = (int)eLookupGovernorRole.AccountingOfficer, Name = "Accounting Officer"},
                new LookupDto { Id = (int)eLookupGovernorRole.Governor, Name = "Governor"}
            });

            var result = await ObjectUnderTest.Edit(null, establishmentId, null, null);

            var viewResult = result as ViewResult;
            viewResult.ShouldNotBeNull();

            var model = viewResult.Model as GovernorsGridViewModel;
            model.ShouldNotBeNull();

            model.GovernorShared.ShouldBe(false);
            model.RemovalGid.ShouldBeNull();
            model.GroupUId.ShouldBeNull();
            model.EstablishmentUrn.ShouldBe(establishmentId);

            model.GovernorRoles.Count.ShouldBe(governorDetailsDto.ApplicableRoles.Count);

            viewResult.ViewData.Keys.Contains("DuplicateGovernor").ShouldBe(false);
            viewResult.ViewData.ModelState.IsValid.ShouldBeTrue();
        }

        [Test]
        public async Task Gov_Edit_GroupId_RemovalGid_GidExists()
        {
            var groupId = 5;
            var governorDetailsDto = new GovernorsDetailsDto
            {
                ApplicableRoles = new List<eLookupGovernorRole> { eLookupGovernorRole.AccountingOfficer, eLookupGovernorRole.Governor },
                RoleDisplayPolicies = new Dictionary<eLookupGovernorRole, GovernorDisplayPolicy>
                {
                    { eLookupGovernorRole.AccountingOfficer, new GovernorDisplayPolicy() },
                    { eLookupGovernorRole.Governor, new GovernorDisplayPolicy() }
                },
                CurrentGovernors = new List<GovernorModel>
                {
                    new GovernorModel
                    {
                        Id = 43,
                        RoleId = (int)eLookupGovernorRole.Establishment_SharedLocalGovernor
                    }
                },
                HistoricalGovernors = new List<GovernorModel>()
            };

            GetMock<IGovernorsReadService>().Setup(g => g.GetGovernorListAsync(null, groupId, It.IsAny<IPrincipal>())).ReturnsAsync(() => governorDetailsDto);
            GetMock<IGovernorsReadService>().Setup(g => g.GetGovernorPermissions(null, groupId, It.IsAny<IPrincipal>())).ReturnsAsync(() => new GovernorPermissions { Add = true, Update = true, Remove = true });
            GetMock<ILayoutHelper>().Setup(l => l.PopulateLayoutProperties(It.IsAny<GovernorsGridViewModel>(), null, groupId, It.IsAny<IPrincipal>(), It.IsAny<Action<EstablishmentModel>>(), It.IsAny<Action<GroupModel>>())).Returns(Task.CompletedTask);
            SetupCachedLookupService();

            GetMock<ICachedLookupService>().Setup(c => c.GovernorRolesGetAllAsync()).ReturnsAsync(() => new List<LookupDto>
            {
                new LookupDto { Id = (int)eLookupGovernorRole.AccountingOfficer, Name = "Accounting Officer"},
                new LookupDto { Id = (int)eLookupGovernorRole.Governor, Name = "Governor"}
            });

            var result = await ObjectUnderTest.Edit(5, null, 43, null);

            var viewResult = result as ViewResult;
            viewResult.ShouldNotBeNull();

            var model = viewResult.Model as GovernorsGridViewModel;
            model.ShouldNotBeNull();

            model.GovernorShared.ShouldBe(true);
            model.RemovalGid.ShouldBe(43);
            model.GroupUId.ShouldBe(groupId);

            model.GovernorRoles.Count.ShouldBe(governorDetailsDto.ApplicableRoles.Count);

            viewResult.ViewData.Keys.Contains("DuplicateGovernor").ShouldBe(false);
            viewResult.ViewData.ModelState.IsValid.ShouldBeTrue();
        }

        [Test]
        public async Task Gov_Edit_GroupId_RemovalGid_GidDoesNotExist()
        {
            var groupId = 5;
            var governorDetailsDto = new GovernorsDetailsDto
            {
                ApplicableRoles = new List<eLookupGovernorRole> { eLookupGovernorRole.AccountingOfficer, eLookupGovernorRole.Governor },
                RoleDisplayPolicies = new Dictionary<eLookupGovernorRole, GovernorDisplayPolicy>
                {
                    { eLookupGovernorRole.AccountingOfficer, new GovernorDisplayPolicy() },
                    { eLookupGovernorRole.Governor, new GovernorDisplayPolicy() }
                },
                CurrentGovernors = new List<GovernorModel>(),
                HistoricalGovernors = new List<GovernorModel>()
            };

            GetMock<IGovernorsReadService>().Setup(g => g.GetGovernorListAsync(null, groupId, It.IsAny<IPrincipal>())).ReturnsAsync(() => governorDetailsDto);
            GetMock<IGovernorsReadService>().Setup(g => g.GetGovernorPermissions(null, groupId, It.IsAny<IPrincipal>())).ReturnsAsync(() => new GovernorPermissions { Add = true, Update = true, Remove = true });
            GetMock<ILayoutHelper>().Setup(l => l.PopulateLayoutProperties(It.IsAny<GovernorsGridViewModel>(), null, groupId, It.IsAny<IPrincipal>(), It.IsAny<Action<EstablishmentModel>>(), It.IsAny<Action<GroupModel>>())).Returns(Task.CompletedTask);
            SetupCachedLookupService();

            GetMock<ICachedLookupService>().Setup(c => c.GovernorRolesGetAllAsync()).ReturnsAsync(() => new List<LookupDto>
            {
                new LookupDto { Id = (int)eLookupGovernorRole.AccountingOfficer, Name = "Accounting Officer"},
                new LookupDto { Id = (int)eLookupGovernorRole.Governor, Name = "Governor"}
            });

            var result = await ObjectUnderTest.Edit(5, null, 43, null);

            var viewResult = result as ViewResult;
            viewResult.ShouldNotBeNull();

            var model = viewResult.Model as GovernorsGridViewModel;
            model.ShouldNotBeNull();

            model.GovernorShared.ShouldBe(false);
            model.RemovalGid.ShouldBe(43);
            model.GroupUId.ShouldBe(groupId);

            model.GovernorRoles.Count.ShouldBe(governorDetailsDto.ApplicableRoles.Count);

            viewResult.ViewData.Keys.Contains("DuplicateGovernor").ShouldBe(false);
            viewResult.ViewData.ModelState.IsValid.ShouldBeTrue();
        }

        [Test]
        public async Task Gov_Edit_GroupId_DuplicateGovernorId()
        {
            var groupId = 5;
            var duplicateId = 13;
            var governorDetailsDto = new GovernorsDetailsDto
            {
                ApplicableRoles = new List<eLookupGovernorRole> { eLookupGovernorRole.AccountingOfficer, eLookupGovernorRole.Governor },
                RoleDisplayPolicies = new Dictionary<eLookupGovernorRole, GovernorDisplayPolicy>
                {
                    { eLookupGovernorRole.AccountingOfficer, new GovernorDisplayPolicy() },
                    { eLookupGovernorRole.Governor, new GovernorDisplayPolicy() }
                },
                CurrentGovernors = new List<GovernorModel>(),
                HistoricalGovernors = new List<GovernorModel>()
            };

            var governor = new GovernorModel
            {
                Id = duplicateId
            };

            GetMock<IGovernorsReadService>().Setup(g => g.GetGovernorListAsync(null, groupId, It.IsAny<IPrincipal>())).ReturnsAsync(() => governorDetailsDto);
            GetMock<IGovernorsReadService>().Setup(g => g.GetGovernorPermissions(null, groupId, It.IsAny<IPrincipal>())).ReturnsAsync(() => new GovernorPermissions { Add = true, Update = true, Remove = true });
            GetMock<IGovernorsReadService>().Setup(g => g.GetGovernorAsync(duplicateId, It.IsAny<IPrincipal>())).ReturnsAsync(() => governor);
            GetMock<ILayoutHelper>().Setup(l => l.PopulateLayoutProperties(It.IsAny<GovernorsGridViewModel>(), null, groupId, It.IsAny<IPrincipal>(), It.IsAny<Action<EstablishmentModel>>(), It.IsAny<Action<GroupModel>>())).Returns(Task.CompletedTask);
            SetupCachedLookupService();

            GetMock<ICachedLookupService>().Setup(c => c.GovernorRolesGetAllAsync()).ReturnsAsync(() => new List<LookupDto>
            {
                new LookupDto { Id = (int)eLookupGovernorRole.AccountingOfficer, Name = "Accounting Officer"},
                new LookupDto { Id = (int)eLookupGovernorRole.Governor, Name = "Governor"}
            });

            var result = await ObjectUnderTest.Edit(5, null, null, duplicateId);

            var viewResult = result as ViewResult;
            viewResult.ShouldNotBeNull();

            var model = viewResult.Model as GovernorsGridViewModel;
            model.ShouldNotBeNull();

            model.GovernorShared.ShouldBe(false);
            model.RemovalGid.ShouldBeNull();
            model.GroupUId.ShouldBe(groupId);

            model.GovernorRoles.Count.ShouldBe(governorDetailsDto.ApplicableRoles.Count);

            viewResult.ViewData.Keys.Contains("DuplicateGovernor").ShouldBe(true);
            viewResult.ViewData["DuplicateGovernor"].ShouldBe(governor);
            viewResult.ViewData.ModelState.IsValid.ShouldBeTrue();
        }

        [Test]
        public async Task Gov_Edit_RoleExists()
        {
            var establishmentId = 23;
            var governorDetailsDto = new GovernorsDetailsDto
            {
                ApplicableRoles = new List<eLookupGovernorRole> { eLookupGovernorRole.AccountingOfficer, eLookupGovernorRole.Governor },
                RoleDisplayPolicies = new Dictionary<eLookupGovernorRole, GovernorDisplayPolicy>
                {
                    { eLookupGovernorRole.AccountingOfficer, new GovernorDisplayPolicy() },
                    { eLookupGovernorRole.Governor, new GovernorDisplayPolicy() }
                },
                CurrentGovernors = new List<GovernorModel>(),
                HistoricalGovernors = new List<GovernorModel>()
            };

            GetMock<IGovernorsReadService>().Setup(g => g.GetGovernorListAsync(establishmentId, null, It.IsAny<IPrincipal>())).ReturnsAsync(() => governorDetailsDto);
            GetMock<IGovernorsReadService>().Setup(g => g.GetGovernorPermissions(establishmentId, null, It.IsAny<IPrincipal>())).ReturnsAsync(() => new GovernorPermissions { Add = true, Update = true, Remove = true });
            GetMock<ILayoutHelper>().Setup(l => l.PopulateLayoutProperties(It.IsAny<GovernorsGridViewModel>(), establishmentId, null, It.IsAny<IPrincipal>(), It.IsAny<Action<EstablishmentModel>>(), It.IsAny<Action<GroupModel>>())).Returns(Task.CompletedTask);
            SetupCachedLookupService();

            GetMock<ICachedLookupService>().Setup(c => c.GovernorRolesGetAllAsync()).ReturnsAsync(() => new List<LookupDto>
            {
                new LookupDto { Id = (int)eLookupGovernorRole.AccountingOfficer, Name = "Accounting Officer"},
                new LookupDto { Id = (int)eLookupGovernorRole.Governor, Name = "Governor"}
            });

            var result = await ObjectUnderTest.Edit(null, establishmentId, null, null, true);

            var viewResult = result as ViewResult;
            viewResult.ShouldNotBeNull();

            var model = viewResult.Model as GovernorsGridViewModel;
            model.ShouldNotBeNull();

            model.GovernorShared.ShouldBe(false);
            model.RemovalGid.ShouldBeNull();
            model.GroupUId.ShouldBeNull();
            model.EstablishmentUrn.ShouldBe(establishmentId);

            model.GovernorRoles.Count.ShouldBe(governorDetailsDto.ApplicableRoles.Count);

            viewResult.ViewData.Keys.Contains("DuplicateGovernor").ShouldBe(false);
            viewResult.ViewData.ModelState.IsValid.ShouldBeFalse();
            viewResult.ViewData.ModelState["role"].Errors.Count.ShouldBe(1);
        }

        [Test]
        public void Gov_View_ModelSpecified()
        {
            var model = new GovernorsGridViewModel();
            var result = ObjectUnderTest.View(null, null, model);

            var viewResult = result as ViewResult;
            viewResult.ShouldNotBeNull();
            viewResult.ViewName.ShouldBe("~/Areas/Governors/Views/Governor/ViewEdit.cshtml");

            var modelResult = viewResult.Model as GovernorsGridViewModel;
            modelResult.ShouldNotBeNull();
            modelResult.ShouldBe(model);
        }

        [Test]
        public void Gov_View_groupUIdSpecified()
        {
            var groupUId = 10;
            var governorDetailsDto = new GovernorsDetailsDto
            {
                ApplicableRoles = new List<eLookupGovernorRole> { eLookupGovernorRole.AccountingOfficer, eLookupGovernorRole.Governor },
                RoleDisplayPolicies = new Dictionary<eLookupGovernorRole, GovernorDisplayPolicy>
                {
                    { eLookupGovernorRole.AccountingOfficer, new GovernorDisplayPolicy() },
                    { eLookupGovernorRole.Governor, new GovernorDisplayPolicy() }
                },
                CurrentGovernors = new List<GovernorModel>(),
                HistoricalGovernors = new List<GovernorModel>()
            };

            var groupModel = new GroupModel { DelegationInformation = "delegation info" };

            GetMock<IGovernorsReadService>().Setup(g => g.GetGovernorListAsync(null, groupUId, It.IsAny<IPrincipal>())).ReturnsAsync(() => governorDetailsDto);
            GetMock<IGovernorsReadService>().Setup(g => g.GetGovernorPermissions(null, groupUId, It.IsAny<IPrincipal>())).ReturnsAsync(() => new GovernorPermissions { Add = true, Update = true, Remove = true });
            GetMock<IGroupReadService>().Setup(g => g.GetAsync(groupUId, It.IsAny<IPrincipal>())).ReturnsAsync(() => new ServiceResultDto<GroupModel>(groupModel));
            SetupCachedLookupService();

            var result = ObjectUnderTest.View(groupUId, null, null);

            var viewResult = result as ViewResult;
            viewResult.ShouldNotBeNull();
            viewResult.ViewName.ShouldBe("~/Areas/Governors/Views/Governor/ViewEdit.cshtml");

            var modelResult = viewResult.Model as GovernorsGridViewModel;
            modelResult.ShouldNotBeNull();
            modelResult.ShowDelegationAndCorpContactInformation.ShouldBeFalse();
            modelResult.DelegationInformation.ShouldBe(groupModel.DelegationInformation);
            modelResult.GroupUId.ShouldBe(groupUId);
            modelResult.EstablishmentUrn.ShouldBeNull();
        }

        [Test]
        public void Gov_View_establishmentUrnSpecified()
        {
            var establishmentUrn = 26;
            var governorDetailsDto = new GovernorsDetailsDto
            {
                ApplicableRoles = new List<eLookupGovernorRole> { eLookupGovernorRole.AccountingOfficer, eLookupGovernorRole.Governor },
                RoleDisplayPolicies = new Dictionary<eLookupGovernorRole, GovernorDisplayPolicy>
                {
                    { eLookupGovernorRole.AccountingOfficer, new GovernorDisplayPolicy() },
                    { eLookupGovernorRole.Governor, new GovernorDisplayPolicy() }
                },
                CurrentGovernors = new List<GovernorModel>(),
                HistoricalGovernors = new List<GovernorModel>()
            };

            var establishment = new EstablishmentModel();

            GetMock<IGovernorsReadService>().Setup(g => g.GetGovernorListAsync(establishmentUrn, null, It.IsAny<IPrincipal>())).ReturnsAsync(() => governorDetailsDto);
            GetMock<IGovernorsReadService>().Setup(g => g.GetGovernorPermissions(establishmentUrn, null, It.IsAny<IPrincipal>())).ReturnsAsync(() => new GovernorPermissions { Add = true, Update = true, Remove = true });
            GetMock<IEstablishmentReadService>().Setup(e => e.GetAsync(establishmentUrn, It.IsAny<IPrincipal>())).ReturnsAsync(() => new ServiceResultDto<EstablishmentModel>(establishment));
            GetMock<IEstablishmentReadService>().Setup(e => e.GetPermissibleLocalGovernorsAsync(establishmentUrn, It.IsAny<IPrincipal>())).ReturnsAsync(() => new List<LookupDto>());
            SetupCachedLookupService();

            var result = ObjectUnderTest.View(null, establishmentUrn, null);

            var viewResult = result as ViewResult;
            viewResult.ShouldNotBeNull();
            viewResult.ViewName.ShouldBe("~/Areas/Governors/Views/Governor/ViewEdit.cshtml");

            var modelResult = viewResult.Model as GovernorsGridViewModel;
            modelResult.ShouldNotBeNull();
            modelResult.GovernanceMode.ShouldBeNull();
            modelResult.GroupUId.ShouldBeNull();
            modelResult.EstablishmentUrn.ShouldBe(establishmentUrn);
        }

        [Test]
        public async Task Gov_AddEditOrReplace_NullParams()
        {
            GetMock<ControllerContext>().SetupGet(c => c.RouteData).Returns(new RouteData(new Route("", new PageRouteHandler("~/")), new PageRouteHandler("~/")));
            await ObjectUnderTest.AddEditOrReplace(null, null, null, null).ShouldThrowAsync<EdubaseException>();
        }

        [Test]
        public async Task Gov_AddEditOrReplace_RoleSpecified_Single_NotShared_AlreadyExists()
        {
            var estabUrn = 4;
            GetMock<IGovernorsReadService>().Setup(g => g.GetGovernorListAsync(estabUrn, null, It.IsAny<IPrincipal>())).ReturnsAsync(() => new GovernorsDetailsDto
            {
                CurrentGovernors = new List<GovernorModel> { new GovernorModel { RoleId = (int) eLookupGovernorRole.ChairOfGovernors } }
            });
            GetMock<ControllerContext>().SetupGet(c => c.RouteData).Returns(new RouteData(new Route("", new PageRouteHandler("~/")), new PageRouteHandler("~/")));

            var result = await ObjectUnderTest.AddEditOrReplace(null, estabUrn, eLookupGovernorRole.ChairOfGovernors, null);

            var redirectResult = result as RedirectToRouteResult;
            redirectResult.ShouldNotBeNull();
            redirectResult.RouteName.ShouldBe("EstabEditGovernance");
        }

        [Test]
        public async Task Gov_AddEditOrReplace_RoleSpecified_Single_NotShared_DoesntExist()
        {
            var estabUrn = 4;
            GetMock<IGovernorsReadService>().Setup(g => g.GetGovernorListAsync(estabUrn, null, It.IsAny<IPrincipal>())).ReturnsAsync(() => new GovernorsDetailsDto
            {
                CurrentGovernors = new List<GovernorModel>()
            });
            GetMock<ControllerContext>().SetupGet(c => c.RouteData).Returns(new RouteData(new Route("", new PageRouteHandler("~/")), new PageRouteHandler("~/")));
            GetMock<ILayoutHelper>().Setup(l => l.PopulateLayoutProperties(It.IsAny<CreateEditGovernorViewModel>(), estabUrn, null, It.IsAny<IPrincipal>(), It.IsAny<Action<EstablishmentModel>>(), It.IsAny<Action<GroupModel>>())).Returns(Task.CompletedTask);
            GetMock<IGovernorsReadService>().Setup(g => g.GetEditorDisplayPolicyAsync(eLookupGovernorRole.ChairOfGovernors, false, It.IsAny<IPrincipal>())).ReturnsAsync(() => new GovernorDisplayPolicy());
            SetupCachedLookupService();

            var result = await ObjectUnderTest.AddEditOrReplace(null, estabUrn, eLookupGovernorRole.ChairOfGovernors, null);

            var viewResult = result as ViewResult;
            viewResult.ShouldNotBeNull();

            var model = viewResult.Model as CreateEditGovernorViewModel;
            model.ShouldNotBeNull();
            model.GovernorRole.ShouldBe(eLookupGovernorRole.ChairOfGovernors);
            model.EstablishmentUrn.ShouldBe(estabUrn);
            model.GroupUId.ShouldBeNull();
        }

        [Test]
        public async Task Gov_AddEditOrReplace_GIDSpecified()
        {
            var estabUrn = 4;
            var governorId = 1032;

            var governor = new GovernorModel
            {
                Id = governorId,
                RoleId = (int) eLookupGovernorRole.Governor
            };

            GetMock<ControllerContext>().SetupGet(c => c.RouteData).Returns(new RouteData(new Route("", new PageRouteHandler("~/")), new PageRouteHandler("~/")));
            GetMock<IGovernorsReadService>().Setup(g => g.GetGovernorAsync(governorId, It.IsAny<IPrincipal>())).ReturnsAsync(() => governor);
            GetMock<ILayoutHelper>().Setup(l => l.PopulateLayoutProperties(It.IsAny<CreateEditGovernorViewModel>(), estabUrn, null, It.IsAny<IPrincipal>(), It.IsAny<Action<EstablishmentModel>>(), It.IsAny<Action<GroupModel>>())).Returns(Task.CompletedTask);
            GetMock<IGovernorsReadService>().Setup(g => g.GetEditorDisplayPolicyAsync(eLookupGovernorRole.Governor, false, It.IsAny<IPrincipal>())).ReturnsAsync(() => new GovernorDisplayPolicy());
            SetupCachedLookupService();

            var result = await ObjectUnderTest.AddEditOrReplace(null, estabUrn, null, 1032);

            var viewResult = result as ViewResult;
            viewResult.ShouldNotBeNull();

            var model = viewResult.Model as CreateEditGovernorViewModel;
            model.ShouldNotBeNull();
            model.Mode.ShouldBe(CreateEditGovernorViewModel.EditMode.Edit);
        }

        [Test]
        public async Task Gov_AddEditOrReplace_RoleSpecified_Shared()
        {
            var estabUrn = 4;
            GetMock<IGovernorsReadService>().Setup(g => g.GetGovernorListAsync(estabUrn, null, It.IsAny<IPrincipal>())).ReturnsAsync(() => new GovernorsDetailsDto
            {
                CurrentGovernors = new List<GovernorModel>()
            });
            GetMock<ControllerContext>().SetupGet(c => c.RouteData).Returns(new RouteData(new Route("", new PageRouteHandler("~/")), new PageRouteHandler("~/")));

            var result = await ObjectUnderTest.AddEditOrReplace(null, estabUrn, eLookupGovernorRole.Establishment_SharedChairOfLocalGoverningBody, null);

            var redirectResult = result as RedirectToRouteResult;
            redirectResult.ShouldNotBeNull();
            redirectResult.RouteName.ShouldBe("SelectSharedGovernor");
        }

        [Test]
        public async Task Gov_DeleteOrRetireGovernor_NoAction()
        {
            await ObjectUnderTest.DeleteOrRetireGovernor(new GovernorsGridViewModel()).ShouldThrowAsync<InvalidParameterException>();
        }

        //[Test]
        //public async Task Gov_DeleteOrRetireGovernor_InvalidModel()
        //{
        //    var groupId = 2436;

        //    var governorDetailsDto = new GovernorsDetailsDto
        //    {
        //        ApplicableRoles = new List<eLookupGovernorRole> { eLookupGovernorRole.AccountingOfficer, eLookupGovernorRole.Governor },
        //        RoleDisplayPolicies = new Dictionary<eLookupGovernorRole, GovernorDisplayPolicy>
        //        {
        //            { eLookupGovernorRole.AccountingOfficer, new GovernorDisplayPolicy() },
        //            { eLookupGovernorRole.Governor, new GovernorDisplayPolicy() }
        //        },
        //        CurrentGovernors = new List<GovernorModel>(),
        //        HistoricalGovernors = new List<GovernorModel>()
        //    };

        //    GetMock<IGovernorsReadService>().Setup(g => g.GetGovernorListAsync(null, groupId, It.IsAny<IPrincipal>())).ReturnsAsync(() => governorDetailsDto);
        //    SetupCachedLookupService();

        //    GetMock<ICachedLookupService>().Setup(c => c.GovernorRolesGetAllAsync()).ReturnsAsync(() => new List<LookupDto>
        //    {
        //        new LookupDto { Id = (int)eLookupGovernorRole.AccountingOfficer, Name = "Accounting Officer"},
        //        new LookupDto { Id = (int)eLookupGovernorRole.Governor, Name = "Governor"}
        //    });

        //    GetMock<ILayoutHelper>().Setup(l => l.PopulateLayoutProperties(It.IsAny<CreateEditGovernorViewModel>(), null, groupId, It.IsAny<IPrincipal>(), It.IsAny<Action<EstablishmentModel>>(), It.IsAny<Action<GroupModel>>())).Returns(Task.CompletedTask);

        //    var result = await ObjectUnderTest.DeleteOrRetireGovernor(new GovernorsGridViewModel { GroupUId = groupId, Action = "Save", RemovalAppointmentEndDate = new DateTimeViewModel()});
        //}

        [Test]
        public async Task Gov_DeleteOrRetireGovernor_Save_ApiError()
        {
            var groupId = 2436;
            var governorId = 6224;
            var errorKey = "Test";
            var errorMessage = "Test Error";

            var governorDetailsDto = new GovernorsDetailsDto
            {
                ApplicableRoles = new List<eLookupGovernorRole> { eLookupGovernorRole.AccountingOfficer, eLookupGovernorRole.Governor },
                RoleDisplayPolicies = new Dictionary<eLookupGovernorRole, GovernorDisplayPolicy>
                {
                    { eLookupGovernorRole.AccountingOfficer, new GovernorDisplayPolicy() },
                    { eLookupGovernorRole.Governor, new GovernorDisplayPolicy() }
                },
                CurrentGovernors = new List<GovernorModel>(),
                HistoricalGovernors = new List<GovernorModel>()
            };

            GetMock<IGovernorsReadService>().Setup(g => g.GetGovernorListAsync(null, groupId, It.IsAny<IPrincipal>())).ReturnsAsync(() => governorDetailsDto);
            GetMock<IGovernorsReadService>().Setup(g => g.GetGovernorPermissions(null, groupId, It.IsAny<IPrincipal>())).ReturnsAsync(() => new GovernorPermissions { Add = true, Update = true, Remove = true });
            SetupCachedLookupService();

            GetMock<ICachedLookupService>().Setup(c => c.GovernorRolesGetAllAsync()).ReturnsAsync(() => new List<LookupDto>
            {
                new LookupDto { Id = (int)eLookupGovernorRole.AccountingOfficer, Name = "Accounting Officer"},
                new LookupDto { Id = (int)eLookupGovernorRole.Governor, Name = "Governor"}
            });

            GetMock<ILayoutHelper>().Setup(l => l.PopulateLayoutProperties(It.IsAny<GovernorsGridViewModel>(), null, groupId, It.IsAny<IPrincipal>(), It.IsAny<Action<EstablishmentModel>>(), It.IsAny<Action<GroupModel>>())).Returns(Task.CompletedTask);
            GetMock<IGovernorsWriteService>()
                .Setup(g => g.UpdateDatesAsync(governorId, It.IsAny<DateTime>(), It.IsAny<IPrincipal>()))
                .ReturnsAsync(() => new ApiResponse(false)
                {
                    Errors = new[]
                    {
                        new ApiError {Code = "Test", Fields = errorKey, Message = errorMessage}
                    }
                });

            var result = await ObjectUnderTest.DeleteOrRetireGovernor(new GovernorsGridViewModel { GroupUId = groupId, Action = "Save", RemovalAppointmentEndDate = new DateTimeViewModel(DateTime.Now), RemovalGid = governorId });

            var viewResult = result as ViewResult;
            viewResult.ShouldNotBeNull();

            viewResult.ViewData.ModelState.IsValid.ShouldBeFalse();
            viewResult.ViewData.ModelState.Count.ShouldBe(1);
            viewResult.ViewData.ModelState.ContainsKey(errorKey).ShouldBeTrue();
            viewResult.ViewData.ModelState[errorKey].Errors.Count.ShouldBe(1);
            viewResult.ViewData.ModelState[errorKey].Errors[0].ErrorMessage.ShouldBe(errorMessage);
        }

        [Test]
        public async Task Gov_DeleteOrRetireGovernor_Save_UnknownApiError()
        {
            var groupId = 2436;
            var governorId = 6224;

            var governorDetailsDto = new GovernorsDetailsDto
            {
                ApplicableRoles = new List<eLookupGovernorRole> { eLookupGovernorRole.AccountingOfficer, eLookupGovernorRole.Governor },
                RoleDisplayPolicies = new Dictionary<eLookupGovernorRole, GovernorDisplayPolicy>
                {
                    { eLookupGovernorRole.AccountingOfficer, new GovernorDisplayPolicy() },
                    { eLookupGovernorRole.Governor, new GovernorDisplayPolicy() }
                },
                CurrentGovernors = new List<GovernorModel>(),
                HistoricalGovernors = new List<GovernorModel>()
            };

            GetMock<IGovernorsReadService>().Setup(g => g.GetGovernorListAsync(null, groupId, It.IsAny<IPrincipal>())).ReturnsAsync(() => governorDetailsDto);
            SetupCachedLookupService();

            GetMock<ICachedLookupService>().Setup(c => c.GovernorRolesGetAllAsync()).ReturnsAsync(() => new List<LookupDto>
            {
                new LookupDto { Id = (int)eLookupGovernorRole.AccountingOfficer, Name = "Accounting Officer"},
                new LookupDto { Id = (int)eLookupGovernorRole.Governor, Name = "Governor"}
            });

            GetMock<ILayoutHelper>().Setup(l => l.PopulateLayoutProperties(It.IsAny<GovernorsGridViewModel>(), null, groupId, It.IsAny<IPrincipal>(), It.IsAny<Action<EstablishmentModel>>(), It.IsAny<Action<GroupModel>>())).Returns(Task.CompletedTask);
            GetMock<IGovernorsWriteService>()
                .Setup(g => g.UpdateDatesAsync(governorId, It.IsAny<DateTime>(), It.IsAny<IPrincipal>()))
                .ReturnsAsync(() => new ApiResponse(false)
                {
                    Errors = new ApiError[0]
                });

            await ObjectUnderTest.DeleteOrRetireGovernor(new GovernorsGridViewModel { GroupUId = groupId, Action = "Save", RemovalAppointmentEndDate = new DateTimeViewModel(DateTime.Now), RemovalGid = governorId }).ShouldThrowAsync<TexunaApiSystemException>();
        }

        [TestCase(16513, null)]
        [TestCase(null, 81681)]
        public async Task Gov_DeleteOrRetireGovernor_Group_Save_OK(int? estabId, int? groupId)
        {
            var governorId = 6224;

            var governorDetailsDto = new GovernorsDetailsDto
            {
                ApplicableRoles = new List<eLookupGovernorRole> { eLookupGovernorRole.AccountingOfficer, eLookupGovernorRole.Governor },
                RoleDisplayPolicies = new Dictionary<eLookupGovernorRole, GovernorDisplayPolicy>
                {
                    { eLookupGovernorRole.AccountingOfficer, new GovernorDisplayPolicy() },
                    { eLookupGovernorRole.Governor, new GovernorDisplayPolicy() }
                },
                CurrentGovernors = new List<GovernorModel>(),
                HistoricalGovernors = new List<GovernorModel>()
            };

            GetMock<IGovernorsReadService>().Setup(g => g.GetGovernorListAsync(estabId, groupId, It.IsAny<IPrincipal>())).ReturnsAsync(() => governorDetailsDto);
            SetupCachedLookupService();

            GetMock<ICachedLookupService>().Setup(c => c.GovernorRolesGetAllAsync()).ReturnsAsync(() => new List<LookupDto>
            {
                new LookupDto { Id = (int)eLookupGovernorRole.AccountingOfficer, Name = "Accounting Officer"},
                new LookupDto { Id = (int)eLookupGovernorRole.Governor, Name = "Governor"}
            });

            GetMock<ILayoutHelper>().Setup(l => l.PopulateLayoutProperties(It.IsAny<GovernorsGridViewModel>(), estabId, groupId, It.IsAny<IPrincipal>(), It.IsAny<Action<EstablishmentModel>>(), It.IsAny<Action<GroupModel>>())).Returns(Task.CompletedTask);
            GetMock<IGovernorsWriteService>()
                .Setup(g => g.UpdateDatesAsync(governorId, It.IsAny<DateTime>(), It.IsAny<IPrincipal>()))
                .ReturnsAsync(() => new ApiResponse(true));

            var result = await ObjectUnderTest.DeleteOrRetireGovernor(new GovernorsGridViewModel { GroupUId = groupId, EstablishmentUrn = estabId, Action = "Save", RemovalAppointmentEndDate = new DateTimeViewModel(DateTime.Now), RemovalGid = governorId });

            var viewResult = result as RedirectResult;
            viewResult.ShouldNotBeNull();

            viewResult.Url.ShouldContain(estabId.HasValue ? "#school-governance" : "#governance");
            GetMock<IGovernorsWriteService>().Verify(g => g.UpdateDatesAsync(governorId, It.IsAny<DateTime>(), It.IsAny<IPrincipal>()), Times.Once);
        }

        [Test]
        public async Task Gov_DeleteOrRetireGovernor_SharedGov_Save_ApiError()
        {
            var governorId = 2436;
            var estabId = 6151;
            var errorKey = "Test";
            var errorMessage = "Test Error";

            var governorDetailsDto = new GovernorsDetailsDto
            {
                ApplicableRoles = new List<eLookupGovernorRole> { eLookupGovernorRole.AccountingOfficer, eLookupGovernorRole.Governor },
                RoleDisplayPolicies = new Dictionary<eLookupGovernorRole, GovernorDisplayPolicy>
                {
                    { eLookupGovernorRole.AccountingOfficer, new GovernorDisplayPolicy() },
                    { eLookupGovernorRole.Governor, new GovernorDisplayPolicy() }
                },
                CurrentGovernors = new List<GovernorModel>(),
                HistoricalGovernors = new List<GovernorModel>()
            };

            var governor = new GovernorModel
            {
                Id = governorId,
                Appointments = new[]
                {
                    new GovernorAppointment
                    {
                        EstablishmentUrn = estabId,
                        AppointmentStartDate = DateTime.Now.AddDays(-30)
                    }
                }
            };

            var error = new ApiResponse(false)
            {
                Errors = new[]
                {
                    new ApiError {Code = "Test", Fields = errorKey, Message = errorMessage}
                }
            };

            GetMock<ILayoutHelper>().Setup(l => l.PopulateLayoutProperties(It.IsAny<GovernorsGridViewModel>(), estabId, null, It.IsAny<IPrincipal>(), It.IsAny<Action<EstablishmentModel>>(), It.IsAny<Action<GroupModel>>())).Returns(Task.CompletedTask);
            GetMock<IGovernorsReadService>().Setup(g => g.GetGovernorListAsync(estabId, null, It.IsAny<IPrincipal>())).ReturnsAsync(() => governorDetailsDto);
            GetMock<IGovernorsReadService>().Setup(g => g.GetGovernorPermissions(estabId, null, It.IsAny<IPrincipal>())).ReturnsAsync(() => new GovernorPermissions { Add = true, Update = true, Remove = true });
            GetMock<IGovernorsReadService>().Setup(g => g.GetGovernorAsync(governorId, It.IsAny<IPrincipal>())).ReturnsAsync(() => governor);
            GetMock<IGovernorsWriteService>().Setup(g => g.UpdateSharedGovernorAppointmentAsync(governorId, estabId, It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<IPrincipal>())).ReturnsAsync(() => error);
            SetupCachedLookupService();

            var result = await ObjectUnderTest.DeleteOrRetireGovernor(new GovernorsGridViewModel
            {
                EstablishmentUrn = estabId,
                Action = "Save",
                RemovalAppointmentEndDate = new DateTimeViewModel(DateTime.Now),
                RemovalGid = governorId,
                GovernorShared = true
            });

            var viewResult = result as ViewResult;
            viewResult.ShouldNotBeNull();
            viewResult.ViewData.ModelState.IsValid.ShouldBeFalse();
            viewResult.ViewData.ModelState.Count.ShouldBe(1);
            viewResult.ViewData.ModelState.ContainsKey(errorKey).ShouldBeTrue();
            viewResult.ViewData.ModelState[errorKey].Errors.Count.ShouldBe(1);
            viewResult.ViewData.ModelState[errorKey].Errors[0].ErrorMessage.ShouldBe(errorMessage);
        }

        [Test]
        public async Task Gov_DeleteOrRetireGovernor_SharedGov_Save_Estab_OK()
        {
            var governorId = 2436;
            var estabId = 6151;

            var governorDetailsDto = new GovernorsDetailsDto
            {
                ApplicableRoles = new List<eLookupGovernorRole> { eLookupGovernorRole.AccountingOfficer, eLookupGovernorRole.Governor },
                RoleDisplayPolicies = new Dictionary<eLookupGovernorRole, GovernorDisplayPolicy>
                {
                    { eLookupGovernorRole.AccountingOfficer, new GovernorDisplayPolicy() },
                    { eLookupGovernorRole.Governor, new GovernorDisplayPolicy() }
                },
                CurrentGovernors = new List<GovernorModel>(),
                HistoricalGovernors = new List<GovernorModel>()
            };

            var governor = new GovernorModel
            {
                Id = governorId,
                Appointments = new[]
                {
                    new GovernorAppointment
                    {
                        EstablishmentUrn = estabId,
                        AppointmentStartDate = DateTime.Now.AddDays(-30)
                    }
                }
            };

            var error = new ApiResponse(true);

            GetMock<ILayoutHelper>().Setup(l => l.PopulateLayoutProperties(It.IsAny<GovernorsGridViewModel>(), estabId, null, It.IsAny<IPrincipal>(), It.IsAny<Action<EstablishmentModel>>(), It.IsAny<Action<GroupModel>>())).Returns(Task.CompletedTask);
            GetMock<IGovernorsReadService>().Setup(g => g.GetGovernorListAsync(estabId, null, It.IsAny<IPrincipal>())).ReturnsAsync(() => governorDetailsDto);
            GetMock<IGovernorsReadService>().Setup(g => g.GetGovernorAsync(governorId, It.IsAny<IPrincipal>())).ReturnsAsync(() => governor);
            GetMock<IGovernorsWriteService>().Setup(g => g.UpdateSharedGovernorAppointmentAsync(governorId, estabId, It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<IPrincipal>())).ReturnsAsync(() => error);
            SetupCachedLookupService();

            var result = await ObjectUnderTest.DeleteOrRetireGovernor(new GovernorsGridViewModel
            {
                EstablishmentUrn = estabId,
                Action = "Save",
                RemovalAppointmentEndDate = new DateTimeViewModel(DateTime.Now),
                RemovalGid = governorId,
                GovernorShared = true
            });

            var viewResult = result as RedirectResult;
            viewResult.ShouldNotBeNull();

            viewResult.Url.ShouldContain("#school-governance");
            GetMock<IGovernorsWriteService>().Verify(g => g.UpdateSharedGovernorAppointmentAsync(governorId, estabId, It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<IPrincipal>()), Times.Once);
        }

        [Test]
        public async Task Gov_DeleteOrRetireGovernor_Estab_Remove_Shared_OK()
        {
            var estabId = 7845;
            var governorId = 6224;

            var governorDetailsDto = new GovernorsDetailsDto
            {
                ApplicableRoles = new List<eLookupGovernorRole> { eLookupGovernorRole.AccountingOfficer, eLookupGovernorRole.Governor },
                RoleDisplayPolicies = new Dictionary<eLookupGovernorRole, GovernorDisplayPolicy>
                {
                    { eLookupGovernorRole.AccountingOfficer, new GovernorDisplayPolicy() },
                    { eLookupGovernorRole.Governor, new GovernorDisplayPolicy() }
                },
                CurrentGovernors = new List<GovernorModel>(),
                HistoricalGovernors = new List<GovernorModel>()
            };

            GetMock<IGovernorsReadService>().Setup(g => g.GetGovernorListAsync(estabId, null, It.IsAny<IPrincipal>())).ReturnsAsync(() => governorDetailsDto);
            GetMock<IGovernorsWriteService>().Setup(g => g.UpdateDatesAsync(governorId, It.IsAny<DateTime>(), It.IsAny<IPrincipal>())).ReturnsAsync(() => new ApiResponse(true));
            GetMock<ILayoutHelper>().Setup(l => l.PopulateLayoutProperties(It.IsAny<GovernorsGridViewModel>(), estabId, null, It.IsAny<IPrincipal>(), It.IsAny<Action<EstablishmentModel>>(), It.IsAny<Action<GroupModel>>())).Returns(Task.CompletedTask);
            GetMock<IGovernorsWriteService>().Setup(g => g.DeleteSharedGovernorAppointmentAsync(governorId, estabId, It.IsAny<IPrincipal>())).Returns(Task.CompletedTask);
            SetupCachedLookupService();

            GetMock<ICachedLookupService>().Setup(c => c.GovernorRolesGetAllAsync()).ReturnsAsync(() => new List<LookupDto>
            {
                new LookupDto { Id = (int)eLookupGovernorRole.AccountingOfficer, Name = "Accounting Officer"},
                new LookupDto { Id = (int)eLookupGovernorRole.Governor, Name = "Governor"}
            });

            var result = await ObjectUnderTest.DeleteOrRetireGovernor(new GovernorsGridViewModel
            {
                EstablishmentUrn = estabId,
                Action = "Remove",
                RemovalAppointmentEndDate = new DateTimeViewModel(DateTime.Now),
                RemovalGid = governorId,
                GovernorShared = true
            });

            var viewResult = result as RedirectResult;
            viewResult.ShouldNotBeNull();

            viewResult.Url.ShouldContain("#school-governance");
            GetMock<IGovernorsWriteService>().Verify(g => g.DeleteSharedGovernorAppointmentAsync(governorId, estabId, It.IsAny<IPrincipal>()), Times.Once);
        }

        [Test]
        public async Task Gov_DeleteOrRetireGovernor_Estab_Remove_NonShared_OK()
        {
            var estabId = 7845;
            var governorId = 6224;

            var governorDetailsDto = new GovernorsDetailsDto
            {
                ApplicableRoles = new List<eLookupGovernorRole> { eLookupGovernorRole.AccountingOfficer, eLookupGovernorRole.Governor },
                RoleDisplayPolicies = new Dictionary<eLookupGovernorRole, GovernorDisplayPolicy>
                {
                    { eLookupGovernorRole.AccountingOfficer, new GovernorDisplayPolicy() },
                    { eLookupGovernorRole.Governor, new GovernorDisplayPolicy() }
                },
                CurrentGovernors = new List<GovernorModel>(),
                HistoricalGovernors = new List<GovernorModel>()
            };

            GetMock<IGovernorsReadService>().Setup(g => g.GetGovernorListAsync(estabId, null, It.IsAny<IPrincipal>())).ReturnsAsync(() => governorDetailsDto);
            GetMock<IGovernorsWriteService>().Setup(g => g.UpdateDatesAsync(governorId, It.IsAny<DateTime>(), It.IsAny<IPrincipal>())).ReturnsAsync(() => new ApiResponse(true));
            GetMock<ILayoutHelper>().Setup(l => l.PopulateLayoutProperties(It.IsAny<GovernorsGridViewModel>(), estabId, null, It.IsAny<IPrincipal>(), It.IsAny<Action<EstablishmentModel>>(), It.IsAny<Action<GroupModel>>())).Returns(Task.CompletedTask);
            GetMock<IGovernorsWriteService>().Setup(g => g.DeleteAsync(governorId, It.IsAny<IPrincipal>())).Returns(Task.CompletedTask);
            SetupCachedLookupService();

            GetMock<ICachedLookupService>().Setup(c => c.GovernorRolesGetAllAsync()).ReturnsAsync(() => new List<LookupDto>
            {
                new LookupDto { Id = (int)eLookupGovernorRole.AccountingOfficer, Name = "Accounting Officer"},
                new LookupDto { Id = (int)eLookupGovernorRole.Governor, Name = "Governor"}
            });

            var result = await ObjectUnderTest.DeleteOrRetireGovernor(new GovernorsGridViewModel
            {
                EstablishmentUrn = estabId,
                Action = "Remove",
                RemovalAppointmentEndDate = new DateTimeViewModel(DateTime.Now),
                RemovalGid = governorId
            });

            var viewResult = result as RedirectResult;
            viewResult.ShouldNotBeNull();

            viewResult.Url.ShouldContain("#school-governance");
            GetMock<IGovernorsWriteService>().Verify(g => g.DeleteAsync(governorId, It.IsAny<IPrincipal>()), Times.Once);
        }

        [TestCase(null, 153513)]
        [TestCase(16851, null)]
        public async Task Gov_Post_AddEditOrReplace_ApiError(int? estabId, int? groupId)
        {
            var errorKey = "test";
            var errorMessage = "test message";

            SetupCachedLookupService();
            GetMock<IGovernorsReadService>().Setup(g => g.GetEditorDisplayPolicyAsync(It.IsAny<eLookupGovernorRole>(), It.IsAny<bool>(), It.IsAny<IPrincipal>())).ReturnsAsync(() => new GovernorDisplayPolicy());
            GetMock<ILayoutHelper>().Setup(l => l.PopulateLayoutProperties(It.IsAny<CreateEditGovernorViewModel>(), It.IsAny<int?>(), It.IsAny<int?>(), It.IsAny<IPrincipal>(), It.IsAny<Action<EstablishmentModel>>(), It.IsAny<Action<GroupModel>>())).Returns(Task.CompletedTask);
            GetMock<IGovernorsWriteService>()
                .Setup(g => g.ValidateAsync(It.IsAny<GovernorModel>(), It.IsAny<IPrincipal>()))
                .ReturnsAsync(() => new ValidationEnvelopeDto()
                {
                    Errors =
                    {
                        new ApiError {Code = "Test", Fields = errorKey, Message = errorMessage}
                    }
                });

            var result = await ObjectUnderTest.AddEditOrReplace(new CreateEditGovernorViewModel { EstablishmentUrn = estabId, GroupUId = groupId });

            var viewResult = result as ViewResult;
            viewResult.ShouldNotBeNull();
            var model = viewResult.Model as CreateEditGovernorViewModel;
            model.ShouldNotBeNull();
            viewResult.ViewData.ModelState.IsValid.ShouldBeFalse();
            viewResult.ViewData.ModelState.ContainsKey(errorKey).ShouldBeTrue();
            viewResult.ViewData.ModelState[errorKey].Errors.Count.ShouldBe(1);
            viewResult.ViewData.ModelState[errorKey].Errors[0].ErrorMessage.ShouldBe(errorMessage);
        }

        [Test]
        public async Task Gov_Get_ReplaceChair()
        {
            var estabId = 364631;
            var gid = 135454;

            var governor = new GovernorModel
            {
                Id = gid,
                RoleId = (int) eLookupGovernorRole.Establishment_SharedChairOfLocalGoverningBody
            };

            var governorDetailsDto = new GovernorsDetailsDto
            {
                ApplicableRoles = new List<eLookupGovernorRole> { eLookupGovernorRole.AccountingOfficer, eLookupGovernorRole.Governor },
                RoleDisplayPolicies = new Dictionary<eLookupGovernorRole, GovernorDisplayPolicy>
                {
                    { eLookupGovernorRole.AccountingOfficer, new GovernorDisplayPolicy() },
                    { eLookupGovernorRole.Governor, new GovernorDisplayPolicy() }
                },
                CurrentGovernors = new List<GovernorModel>(),
                HistoricalGovernors = new List<GovernorModel>()
            };

            GetMock<IGovernorsReadService>().Setup(g => g.GetGovernorListAsync(It.IsAny<int?>(), It.IsAny<int?>(), It.IsAny<IPrincipal>())).ReturnsAsync(() => governorDetailsDto);
            GetMock<IGovernorsReadService>().Setup(g => g.GetGovernorAsync(gid, It.IsAny<IPrincipal>())).ReturnsAsync(() => governor);
            GetMock<IGovernorsReadService>().Setup(g => g.GetSharedGovernorsAsync(estabId, It.IsAny<IPrincipal>())).ReturnsAsync(() => new List<GovernorModel>());
            GetMock<IGovernorsReadService>().Setup(g => g.GetEditorDisplayPolicyAsync(It.IsAny<eLookupGovernorRole>(), false, It.IsAny<IPrincipal>())).ReturnsAsync(() => new GovernorDisplayPolicy());
            GetMock<ILayoutHelper>().Setup(l => l.PopulateLayoutProperties(It.IsAny<ReplaceChairViewModel>(), estabId, null, It.IsAny<IPrincipal>(), It.IsAny<Action<EstablishmentModel>>(), It.IsAny<Action<GroupModel>>())).Returns(Task.CompletedTask);
            SetupCachedLookupService();

            var result = await ObjectUnderTest.ReplaceChair(estabId, gid);
            var viewResult = result as ViewResult;
            viewResult.ShouldNotBeNull();

            var model = viewResult.Model as ReplaceChairViewModel;
            model.ShouldNotBeNull();
            model.ExistingGovernorId.ShouldBe(gid);
            model.Role.ShouldBe((eLookupGovernorRole) governor.RoleId);
        }

        [Test]
        public async Task Gov_Put_ReplaceChair_NewChairShared()
        {
            var govId = 465134;
            var newGovId = 68543;
            var estabUrn = 16802;

            var model = new ReplaceChairViewModel
            {
                ExistingGovernorId = govId,
                Urn = estabUrn,
                NewChairType = ReplaceChairViewModel.ChairType.SharedChair,
                SharedGovernors = new List<SharedGovernorViewModel>
                {
                    new SharedGovernorViewModel
                    {
                        Id = newGovId,
                        AppointmentEndDate = new DateTimeViewModel(DateTime.Now.AddYears(1))
                    }
                },
                SelectedGovernorId = newGovId,
                DateTermEnds = new DateTimeViewModel(DateTime.Today),
            };

            GetMock<IGovernorsWriteService>().Setup(g => g.AddSharedGovernorAppointmentAsync(newGovId, estabUrn, It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<IPrincipal>())).ReturnsAsync(() => new ApiResponse(true));

            var result = await ObjectUnderTest.ReplaceChair(model);
            var redirectResult = result as RedirectResult;
            redirectResult.ShouldNotBeNull();
            redirectResult.Url.ShouldContain("#school-governance");

            GetMock<IGovernorsWriteService>().Verify(g => g.AddSharedGovernorAppointmentAsync(newGovId, estabUrn, It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<IPrincipal>()), Times.Once);
        }

        [Test]
        public async Task Gov_Put_ReplaceChair_NewChairNotShared_NoValidationErrors()
        {
            var govId = 465134;
            var estabUrn = 16802;

            var model = new ReplaceChairViewModel
            {
                ExistingGovernorId = govId,
                Urn = estabUrn,
                NewChairType = ReplaceChairViewModel.ChairType.LocalChair,
                NewLocalGovernor = new CreateEditGovernorViewModel
                {
                    AppointmentEndDate = new DateTimeViewModel(DateTime.Today.AddYears(1)),
                    DOB = new DateTimeViewModel(),
                },
                DateTermEnds = new DateTimeViewModel(DateTime.Today),
            };

            GetMock<IGovernorsWriteService>().Setup(g => g.ValidateAsync(It.IsAny<GovernorModel>(), It.IsAny<IPrincipal>())).ReturnsAsync(() => new ValidationEnvelopeDto { Errors = new List<ApiError>() });
            GetMock<IGovernorsWriteService>().Setup(g => g.SaveAsync(It.IsAny<GovernorModel>(), It.IsAny<IPrincipal>())).ReturnsAsync(() => new ApiResponse<int>(true));

            var result = await ObjectUnderTest.ReplaceChair(model);
            var redirectResult = result as RedirectResult;
            redirectResult.ShouldNotBeNull();
            redirectResult.Url.ShouldContain("#school-governance");
        }

        [Test]
        public async Task Gov_Put_ReplaceChair_NewChairNotShared_ValidationErrors()
        {
            var govId = 465134;
            var estabUrn = 16802;
            var errorKey = "Test";
            var errorText = "Test Message";

            var existingGov = new GovernorModel
            {
                RoleId = (int) eLookupGovernorRole.ChairOfLocalGoverningBody
            };

            var model = new ReplaceChairViewModel
            {
                ExistingGovernorId = govId,
                Urn = estabUrn,
                NewChairType = ReplaceChairViewModel.ChairType.LocalChair,
                NewLocalGovernor = new CreateEditGovernorViewModel
                {
                    AppointmentEndDate = new DateTimeViewModel(DateTime.Today.AddYears(1)),
                    DOB = new DateTimeViewModel(),
                },
                DateTermEnds = new DateTimeViewModel(DateTime.Today),
            };

            GetMock<IGovernorsWriteService>()
                .Setup(g => g.ValidateAsync(It.IsAny<GovernorModel>(), It.IsAny<IPrincipal>()))
                .ReturnsAsync(() => new ValidationEnvelopeDto
                {
                    Errors = new List<ApiError>
                    {
                        new ApiError
                        {
                            Fields = errorKey,
                            Message = errorText
                        }
                    }
                });

            var governorDetailsDto = new GovernorsDetailsDto
            {
                ApplicableRoles = new List<eLookupGovernorRole> { eLookupGovernorRole.AccountingOfficer, eLookupGovernorRole.Governor },
                RoleDisplayPolicies = new Dictionary<eLookupGovernorRole, GovernorDisplayPolicy>
                {
                    { eLookupGovernorRole.AccountingOfficer, new GovernorDisplayPolicy() },
                    { eLookupGovernorRole.Governor, new GovernorDisplayPolicy() }
                },
                CurrentGovernors = new List<GovernorModel>(),
                HistoricalGovernors = new List<GovernorModel>()
            };

            GetMock<IGovernorsReadService>().Setup(g => g.GetGovernorListAsync(It.IsAny<int?>(), It.IsAny<int?>(), It.IsAny<IPrincipal>())).ReturnsAsync(() => governorDetailsDto);


            GetMock<IGovernorsReadService>().Setup(g => g.GetGovernorAsync(model.ExistingGovernorId, It.IsAny<IPrincipal>())).ReturnsAsync(() => existingGov);
            GetMock<IGovernorsReadService>().Setup(g => g.GetSharedGovernorsAsync(estabUrn, It.IsAny<IPrincipal>())).ReturnsAsync(() => new List<GovernorModel>());
            GetMock<IGovernorsReadService>().Setup(g => g.GetEditorDisplayPolicyAsync(It.IsAny<eLookupGovernorRole>(), false, It.IsAny<IPrincipal>())).ReturnsAsync(() => new GovernorDisplayPolicy());
            GetMock<ILayoutHelper>().Setup(l => l.PopulateLayoutProperties(It.IsAny<ReplaceChairViewModel>(), estabUrn, null, It.IsAny<IPrincipal>(), It.IsAny<Action<EstablishmentModel>>(), It.IsAny<Action<GroupModel>>())).Returns(Task.CompletedTask);
            SetupCachedLookupService();

            var result = await ObjectUnderTest.ReplaceChair(model);
            var viewResult = result as ViewResult;
            viewResult.ShouldNotBeNull();
            var modelResult = viewResult.Model as ReplaceChairViewModel;
            modelResult.ShouldNotBeNull();
            modelResult.ShouldBe(model);
        }

        [SetUp]
        public void SetUpTest() => SetupObjectUnderTest();

        [TearDown]
        public void TearDownTest() => ResetMocks();

        [OneTimeSetUp]
        protected override void InitialiseMocks()
        {
            AddMock<IGovernorsReadService>();
            AddMock<NomenclatureService>();
            AddMock<ICachedLookupService>();
            AddMock<IGovernorsWriteService>();
            AddMock<IGroupReadService>();
            AddMock<IEstablishmentReadService>();
            AddMock<ILayoutHelper>();
            base.InitialiseMocks();
        }
    }
}

;
