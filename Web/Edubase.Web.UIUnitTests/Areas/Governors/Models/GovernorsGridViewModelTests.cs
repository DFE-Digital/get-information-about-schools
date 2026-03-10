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
            // Arrange
            var dob = new DateTime(1980, 1, 1);
            var g1 = Gov("Alex", "J", "Taylor", eLookupGovernorRole.Group_SharedLocalGovernor, dob);
            var g2 = Gov("Alex", "J", "Taylor", eLookupGovernorRole.Establishment_SharedLocalGovernor, dob);

            var dto = BuildDto(g1, g2);

            // Act
            var vm = Build(dto);

            // Assert
            Assert.Equal(2, vm.Grids.Count);
            var rows = vm.Grids.SelectMany(g => g.Rows).ToList();
            Assert.Equal(2, rows.Count);   
        }

        [Fact]
        public void CreateGrids_ShouldListAllAppointments_WithinSameRole()
        {
            // Arrange
            var dob = new DateTime(1990, 1, 1);
            var a1 = Gov("Sam", "", "Morgan", eLookupGovernorRole.LocalGovernor, dob);
            var a2 = Gov("Sam", "", "Morgan", eLookupGovernorRole.LocalGovernor, dob);

            var dto = BuildDto(a1, a2);

            // Act
            var vm = Build(dto);

            // Assert
            Assert.Single(vm.Grids);
            var rows = vm.Grids.SelectMany(g => g.Rows).ToList();
            Assert.Equal(2, rows.Count);
        }

        [Fact]
        public void CreateGrids_ShouldCreateSeparateGrids_ForDistinctRoles()
        {
            // Arrange
            var g1 = Gov("Ava", "", "Singh", eLookupGovernorRole.LocalGovernor, null);
            var g2 = Gov("Ava", "", "Singh", eLookupGovernorRole.ChairOfLocalGoverningBody, null);

            var dto = BuildDto(g1, g2);

            // Act
            var vm = Build(dto);

            // Assert
            Assert.Equal(2, vm.Grids.Count);
            Assert.Equal(2, vm.Grids.SelectMany(g => g.Rows).Count());
        }

        [Fact]
        public void CreateGrids_ShouldUseExactRoleMatching_NoBleedAcrossGrids()
        {
            // Arrange
            var dob = new DateTime(1985, 5, 5);
            var shared = Gov("Jamie", "", "Lee", eLookupGovernorRole.Group_SharedLocalGovernor, dob);

            // Intentionally include LocalGovernor in ApplicableRoles to ensure a Local grid is created too.
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

            // Act
            var vm = Build(dto);

            // Assert
            Assert.Equal(2, vm.Grids.Count); // Local + Group_Shared grids
            var localGrid = vm.Grids.Single(g => g.Role == eLookupGovernorRole.LocalGovernor);
            var groupGrid = vm.Grids.Single(g => g.Role == eLookupGovernorRole.Group_SharedLocalGovernor);

            Assert.Empty(localGrid.Rows); // exact role matching → no bleed
            Assert.Single(groupGrid.Rows);
        }

        [Fact]
        public void CreateGrids_Historic_ShouldListAllAppointments()
        {
            // Arrange
            var dob = new DateTime(1975, 1, 1);
            var g1 = Gov("Chris", "", "Walker", eLookupGovernorRole.Governor, dob);
            var g2 = Gov("Chris", "", "Walker", eLookupGovernorRole.Governor, dob);

            var dto = BuildHistoric(g1, g2);

            // Act
            var vm = Build(dto);

            // Assert
            Assert.Single(vm.HistoricGrids);
            var rows = vm.HistoricGrids.SelectMany(g => g.Rows).ToList();
            Assert.Equal(2, rows.Count); // NO row dedupe → two historic rows
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
                // Interpretation B: every roleId must have a policy
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
