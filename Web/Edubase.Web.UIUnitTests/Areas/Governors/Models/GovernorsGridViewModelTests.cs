using System;
using System.Collections.Generic;
using System.Linq;
using Edubase.Services.Domain;
using Edubase.Services.Enums;
using Edubase.Services.Governors.DisplayPolicies;
using Edubase.Services.Governors.Models;
using Edubase.Web.UI.Areas.Governors.Models;
using Xunit;

namespace Edubase.Web.UIUnitTests.Areas.Governors.Models
{
    public class GovernorsGridViewModelTests
    {
        [Fact]
        public void CreateGrids_ShouldNotMergeDifferentRoles_ForSamePerson()
        {
            var dob = new DateTime(1980, 1, 1);

            var g1 = Gov("Alex", "J", "Taylor", eLookupGovernorRole.Group_SharedLocalGovernor, dob);
            var g2 = Gov("Alex", "J", "Taylor", eLookupGovernorRole.Establishment_SharedLocalGovernor, dob);

            var dto = BuildDto(g1, g2);
            var vm = Build(dto);

            Assert.Equal(2, vm.Grids.Count);
            Assert.Equal(2, vm.Grids.SelectMany(g => g.Rows).Count());
        }

        [Fact]
        public void CreateGrids_ShouldListAllAppointments_ForSameRole()
        {
            var dob = new DateTime(1990, 1, 1);
            var a1 = Gov("Sam", "", "Morgan", eLookupGovernorRole.LocalGovernor, dob);
            var a2 = Gov("Sam", "", "Morgan", eLookupGovernorRole.LocalGovernor, dob);

            var dto = BuildDto(a1, a2);
            var vm = Build(dto);

            Assert.Single(vm.Grids);
            Assert.Equal(2, vm.Grids.SelectMany(g => g.Rows).Count());
        }

        [Fact]
        public void CreateGrids_ShouldCreateSeparateGrids_ForDistinctRoles()
        {
            var g1 = Gov("Ava", "", "Singh", eLookupGovernorRole.LocalGovernor, null);
            var g2 = Gov("Ava", "", "Singh", eLookupGovernorRole.ChairOfLocalGoverningBody, null);

            var dto = BuildDto(g1, g2);
            var vm = Build(dto);

            Assert.Equal(2, vm.Grids.Count);
            Assert.Equal(2, vm.Grids.SelectMany(g => g.Rows).Count());
        }

        [Fact]
        public void CreateGrids_ShouldUseExactRoleMatching_NoBleedAcrossGrids()
        {
            var dob = new DateTime(1985, 5, 5);
            var shared = Gov("Jamie", "", "Lee", eLookupGovernorRole.Group_SharedLocalGovernor, dob);

            var dto = new GovernorsDetailsDto
            {
                ApplicableRoles = new List<eLookupGovernorRole>
                {
                    eLookupGovernorRole.LocalGovernor,
                    eLookupGovernorRole.Group_SharedLocalGovernor
                },
                CurrentGovernors = new List<GovernorModel> { shared },
                HistoricalGovernors = new List<GovernorModel>(),
                RoleDisplayPolicies = new Dictionary<eLookupGovernorRole, GovernorDisplayPolicy>
                {
                    { eLookupGovernorRole.LocalGovernor, new GovernorDisplayPolicy() },
                    { eLookupGovernorRole.Group_SharedLocalGovernor, new GovernorDisplayPolicy() }
                }
            };

            var vm = Build(dto);

            var localGrid = vm.Grids.Single(g => g.Role == eLookupGovernorRole.LocalGovernor);
            var sharedGrid = vm.Grids.Single(g => g.Role == eLookupGovernorRole.Group_SharedLocalGovernor);

            Assert.Empty(localGrid.Rows);
            Assert.Single(sharedGrid.Rows);
        }

        [Fact]
        public void CreateGrids_Historic_ShouldListAllAppointments()
        {
            var dob = new DateTime(1975, 1, 1);
            var g1 = Gov("Chris", "", "Walker", eLookupGovernorRole.Governor, dob);
            var g2 = Gov("Chris", "", "Walker", eLookupGovernorRole.Governor, dob);

            var dto = BuildHistoric(g1, g2);
            var vm = Build(dto);

            Assert.Single(vm.HistoricGrids);
            Assert.Equal(2, vm.HistoricGrids.SelectMany(g => g.Rows).Count());
        }

        [Fact]
        public void CreateGrids_ShouldThrow_WhenDisplayPolicyMissing()
        {
            var g = Gov("Alex", "", "Test", eLookupGovernorRole.LocalGovernor, null);

            var dto = new GovernorsDetailsDto
            {
                ApplicableRoles = new List<eLookupGovernorRole> { eLookupGovernorRole.LocalGovernor },
                CurrentGovernors = new List<GovernorModel> { g },
                RoleDisplayPolicies = new Dictionary<eLookupGovernorRole, GovernorDisplayPolicy>() // missing!
            };

            Assert.Throws<Exception>(() => Build(dto));
        }

        [Fact]
        public void CreateGrids_ShouldNotThrow_WhenDisplayPolicyPresent()
        {
            var g = Gov("Alex", "", "Test", eLookupGovernorRole.LocalGovernor, null);

            var dto = BuildDto(g);

            var vm = Build(dto);

            Assert.Single(vm.Grids);
        }

        [Fact]
        public void CreateGrids_ShouldSortRowsByName_WhenSortValueNull()
        {
            var g1 = Gov("Charlie", "", "Zed", eLookupGovernorRole.LocalGovernor, null);
            var g2 = Gov("Adam", "", "Baker", eLookupGovernorRole.LocalGovernor, null);
            var g3 = Gov("Ben", "", "Alpha", eLookupGovernorRole.LocalGovernor, null);

            var dto = BuildDto(g1, g2, g3);
            var vm = Build(dto);

            var rows = vm.Grids.Single().Rows.Select(r => r.Model.GetFullName()).ToList();

            Assert.Equal(new[] { "Adam Baker", "Ben Alpha", "Charlie Zed" }, rows);
        }

        [Fact]
        public void CreateGrids_ShouldThrow_WhenMultipleAppointmentsForSameUrn()
        {
            var g = Gov("Alex", "", "Test", eLookupGovernorRole.LocalGovernor, null);

            g.Appointments = new[]
            {
                new GovernorAppointment { EstablishmentUrn = 12345 },
                new GovernorAppointment { EstablishmentUrn = 12345 } // duplicate URN → should fail
            };

            var dto = BuildDto(g);

            Assert.Throws<InvalidOperationException>(() => Build(dto));
        }

        [Fact]
        public void CreateGrids_ShouldSelectCorrectAppointment_ForSharedRoles()
        {
            var g = Gov("Jordan", "", "Scott", eLookupGovernorRole.Group_SharedLocalGovernor, null);

            g.Appointments = new[]
            {
                new GovernorAppointment { EstablishmentUrn = 99999, AppointmentStartDate = new DateTime(2020,1,1) },
                new GovernorAppointment { EstablishmentUrn = 12345, AppointmentStartDate = new DateTime(2021,1,1) }
            };

            var dto = new GovernorsDetailsDto
            {
                ApplicableRoles = new List<eLookupGovernorRole>
            {
                eLookupGovernorRole.Group_SharedLocalGovernor
            },
                CurrentGovernors = new List<GovernorModel> { g },
                HistoricalGovernors = new List<GovernorModel>(),
                RoleDisplayPolicies = new Dictionary<eLookupGovernorRole, GovernorDisplayPolicy>
            {
                {
                    eLookupGovernorRole.Group_SharedLocalGovernor,
                    new GovernorDisplayPolicy
                    {
                        FullName = true,
                        AppointmentStartDate = true
                    }
                }
            }
            };

            var vm = Build(dto);

            var row = vm.Grids.Single().Rows.Single();

            var cellTexts = row.Cells.Select(c => c.Text).ToList();
            Assert.Contains("1 January 2021", cellTexts);
        }

        [Fact]
        public void CreateGrids_ShouldCreateSeparateGrids_ForGovernanceProfessionalRoles()
        {
            var g1 = Gov("GP1", "", "One", eLookupGovernorRole.GovernanceProfessionalToAMat, null);
            var g2 = Gov("GP2", "", "Two", eLookupGovernorRole.Group_SharedGovernanceProfessional, null);

            var dto = BuildDto(g1, g2);
            var vm = Build(dto);

            Assert.Equal(2, vm.Grids.Count);
        }

        [Fact]
        public void CreateGrids_ShouldIncludeSharedWithColumn_ForSharedGP()
        {
            var g = Gov("Sam", "", "GP", eLookupGovernorRole.Group_SharedGovernanceProfessional, null);

            var dto = BuildDto(g);
            var vm = Build(dto);

            var headers = vm.Grids.Single().HeaderCells.Select(h => h.Text).ToList();

            Assert.Contains("Shared with", headers);
        }

        [Fact]
        public void CreateGrids_ShouldNotIncludeSharedWithColumn_ForNonSharedGP()
        {
            var g = Gov("Sam", "", "GP", eLookupGovernorRole.GovernanceProfessionalToAMat, null);

            var dto = BuildDto(g);
            var vm = Build(dto);

            var headers = vm.Grids.Single().HeaderCells.Select(h => h.Text).ToList();

            Assert.DoesNotContain("Shared with", headers);
        }

        [Fact]
        public void CreateGrids_ShouldCreateOneGridPerRole_NoBleed()
        {
            var g1 = Gov("A", "", "Test", eLookupGovernorRole.LocalGovernor, null);
            var g2 = Gov("B", "", "Test", eLookupGovernorRole.Governor, null);
            var g3 = Gov("C", "", "Test", eLookupGovernorRole.ChairOfLocalGoverningBody, null);

            var dto = BuildDto(g1, g2, g3);
            var vm = Build(dto);

            Assert.Equal(3, vm.Grids.Count);
            Assert.Single(vm.Grids.Single(g => g.Role == eLookupGovernorRole.LocalGovernor).Rows);
            Assert.Single(vm.Grids.Single(g => g.Role == eLookupGovernorRole.Governor).Rows);
            Assert.Single(vm.Grids.Single(g => g.Role == eLookupGovernorRole.ChairOfLocalGoverningBody).Rows);
        }

        private GovernorModel Gov(string first, string middle, string last, eLookupGovernorRole role, DateTime? dob)
        {
            return new GovernorModel
            {
                Id = new Random().Next(1000, 999999),
                Person_FirstName = first,
                Person_MiddleName = middle,
                Person_LastName = last,
                RoleId = (int) role,
                DOB = dob
            };
        }

        private GovernorsDetailsDto BuildDto(params GovernorModel[] govs)
        {
            var roles = govs.Select(g => (eLookupGovernorRole) g.RoleId).Distinct().ToList();

            return new GovernorsDetailsDto
            {
                ApplicableRoles = roles,
                CurrentGovernors = govs.ToList(),
                HistoricalGovernors = new List<GovernorModel>(),
                RoleDisplayPolicies = roles.ToDictionary(r => r, _ => new GovernorDisplayPolicy())
            };
        }

        private GovernorsDetailsDto BuildHistoric(params GovernorModel[] govs)
        {
            var roles = govs.Select(g => (eLookupGovernorRole) g.RoleId).Distinct().ToList();

            return new GovernorsDetailsDto
            {
                ApplicableRoles = roles,
                CurrentGovernors = new List<GovernorModel>(),
                HistoricalGovernors = govs.ToList(),
                RoleDisplayPolicies = roles.ToDictionary(r => r, _ => new GovernorDisplayPolicy())
            };
        }

        private GovernorsGridViewModel Build(GovernorsDetailsDto dto)
        {
            return new GovernorsGridViewModel(
                dto,
                editMode: false,
                groupUId: null,
                establishmentUrn: 12345,
                nationalities: Array.Empty<LookupDto>(),
                appointingBodies: Array.Empty<LookupDto>(),
                titles: Array.Empty<LookupDto>(),
                governorPermissions: new GovernorPermissions());
        }
    }
}
