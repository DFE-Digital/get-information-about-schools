using System;
using System.Collections.Generic;
using System.Linq;
using Edubase.Services.Domain;
using Edubase.Services.Enums;
using Edubase.Services.Governors.DisplayPolicies;
using Edubase.Services.Governors.Models;
using Edubase.Services.Lookup;
using Edubase.Web.UI.Areas.Governors.Models;
using Xunit;

namespace Edubase.Web.UIUnitTests.Areas.Governors.Models
{
    public class GovernorsGridViewModelTests
    {
        private GovernorModel Gov(string first, string last, eLookupGovernorRole role, DateTime dob)
        {
            return new GovernorModel
            {
                Id = new Random().Next(1000, 9999),
                Person_FirstName = first,
                Person_LastName = last,
                RoleId = (int) role,
                DOB = dob,
            };
        }

        private GovernorsDetailsDto DtoWith(params GovernorModel[] govs)
        {
            return new GovernorsDetailsDto
            {
                ApplicableRoles = govs.Select(g => (eLookupGovernorRole) g.RoleId).Distinct().ToList(),
                CurrentGovernors = govs.ToList(),
                HistoricalGovernors = new List<GovernorModel>(),
                RoleDisplayPolicies = govs
                    .Select(g => (eLookupGovernorRole) g.RoleId)
                    .Distinct()
                    .ToDictionary(r => r, r => new GovernorDisplayPolicy())
            };
        }

        private GovernorsDetailsDto DtoHistoric(params GovernorModel[] govs)
        {
            return new GovernorsDetailsDto
            {
                ApplicableRoles = govs.Select(g => (eLookupGovernorRole) g.RoleId).Distinct().ToList(),
                HistoricalGovernors = govs.ToList(),
                CurrentGovernors = new List<GovernorModel>(),
                RoleDisplayPolicies = govs
                    .Select(g => (eLookupGovernorRole) g.RoleId)
                    .Distinct()
                    .ToDictionary(r => r, r => new GovernorDisplayPolicy())
            };
        }

        private GovernorsGridViewModel Build(GovernorsDetailsDto dto)
        {
            return new GovernorsGridViewModel(
                dto,
                editMode: false,
                groupUId: null,
                establishmentUrn: 1234,
                nationalities: Array.Empty<LookupDto>(),
                appointingBodies: Array.Empty<LookupDto>(),
                titles: Array.Empty<LookupDto>(),
                governorPermissions: new GovernorPermissions()
            );
        }

        [Fact]
        public void CreateGrids_ShouldDeduplicateRows_WhenLocalAndSharedRepresentSamePerson()
        {
            // Arrange
            var dob = new DateTime(1980, 1, 1);
            var g1 = Gov("Alex", "Taylor", eLookupGovernorRole.LocalGovernor, dob);
            var g2 = Gov("Alex", "Taylor", eLookupGovernorRole.Group_SharedLocalGovernor, dob);
            var dto = DtoWith(g1, g2);

            // Act
            var vm = Build(dto);
            var rows = vm.Grids.SelectMany(g => g.Rows).ToList();

            // Assert
            Assert.Single(rows);
        }

        [Fact]
        public void CreateGrids_ShouldCollapseEquivalentRoles_IntoSingleGrid()
        {
            // Arrange
            var dob = new DateTime(1980, 1, 1);
            var g1 = Gov("Alex", "Taylor", eLookupGovernorRole.LocalGovernor, dob);
            var g2 = Gov("Alex", "Taylor", eLookupGovernorRole.Group_SharedLocalGovernor, dob);
            var dto = DtoWith(g1, g2);

            // Act
            var vm = Build(dto);

            // Assert
            Assert.Single(vm.Grids);  // Only one grid for the role family
        }

        [Fact]
        public void CreateGrids_ShouldChooseRepresentativeRole_ThatHasDisplayPolicy()
        {
            // Arrange
            var dob = new DateTime(1980, 1, 1);
            var local = Gov("Alex", "Taylor", eLookupGovernorRole.LocalGovernor, dob);
            var shared = Gov("Alex", "Taylor", eLookupGovernorRole.Group_SharedLocalGovernor, dob);

            var dto = DtoWith(local, shared);

            // Remove the LocalGovernor's policy to force shared-role selection
            dto.RoleDisplayPolicies.Remove(eLookupGovernorRole.LocalGovernor);

            // Act
            var vm = Build(dto);

            // Assert: the remaining policy is for Group_SharedLocalGovernor
            Assert.Single(vm.Grids);
            Assert.Equal(eLookupGovernorRole.Group_SharedLocalGovernor, vm.Grids[0].Role);
        }

        [Fact]
        public void CreateGrids_ShouldDeduplicateHistoricGovernors()
        {
            // Arrange
            var dob = new DateTime(1980, 1, 1);
            var g1 = Gov("Alex", "Taylor", eLookupGovernorRole.LocalGovernor, dob);
            var g2 = Gov("Alex", "Taylor", eLookupGovernorRole.Group_SharedLocalGovernor, dob);
            var dto = DtoHistoric(g1, g2);

            // Act
            var vm = Build(dto);
            var rows = vm.HistoricGrids.SelectMany(g => g.Rows).ToList();

            // Assert
            Assert.Single(rows);
        }
    }
}
