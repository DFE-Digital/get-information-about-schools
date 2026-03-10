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

        private GovernorsDetailsDto BuildDtoWith(params GovernorModel[] govs)
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

        // Create a GovernorDetailsDto with historical governors only.
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

        // Build a fully constructed GovernorGridViewModel with the minimum required inputs.
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

        [Fact]
        public void CreateGrids_ShouldNotDeduplicate_WhenTwoDifferentPeopleShareSameNameAndDob_DifferentMiddleName()
        {
            // Arrange
            var dob = new DateTime(1980, 1, 1);

            var p1 = Gov("Alex", "Taylor", eLookupGovernorRole.LocalGovernor, dob);
            p1.Person_MiddleName = "James";
            p1.PostCode = "AB1 2CD";

            var p2 = Gov("Alex", "Taylor", eLookupGovernorRole.LocalGovernor, dob);
            p1.Person_MiddleName = "Marie";
            p2.PostCode = "XY9 9ZZ";    // Different person

            var dto = BuildDtoWith(p1, p2);

            // Act
            var vm = Build(dto);
            var rows = vm.Grids.SelectMany(g => g.Rows).ToList();

            // Assert
            Assert.Equal(2, rows.Count);
        }


        [Fact]
        public void LocalGovernorFamily_ShouldCollapseToSingleGrid_AndSingleRow()
        {
            var dob = new DateTime(1982, 3, 3);

            var local = Gov("Jamie", "Lee", eLookupGovernorRole.LocalGovernor, dob);
            var shared = Gov("Jamie", "Lee", eLookupGovernorRole.Group_SharedLocalGovernor, dob);

            var dto = BuildDtoWith(local, shared);
            var vm = Build(dto);

            Assert.Single(vm.Grids); // role-level dedupe
            Assert.Single(vm.Grids.SelectMany(g => g.Rows));
        }

        [Fact]
        public void ChairFamily_ShouldPickSharedRole_WhenLocalHasNoPolicy()
        {
            var dob = new DateTime(1979, 4, 4);

            var chairLocal = Gov("Ava", "Singh", eLookupGovernorRole.ChairOfLocalGoverningBody, dob);
            var chairShared = Gov("Ava", "Singh", eLookupGovernorRole.Group_SharedChairOfLocalGoverningBody, dob);

            var dto = BuildDtoWith(chairLocal, chairShared);

            // Only shared has policy
            dto.RoleDisplayPolicies = new Dictionary<eLookupGovernorRole, GovernorDisplayPolicy>
            {
                { eLookupGovernorRole.Group_SharedChairOfLocalGoverningBody, new GovernorDisplayPolicy() }
            };

            var vm = Build(dto);

            Assert.Single(vm.Grids);
            Assert.Equal(eLookupGovernorRole.Group_SharedChairOfLocalGoverningBody, vm.Grids[0].Role);
            Assert.Single(vm.Grids.SelectMany(g => g.Rows));
        }

        [Fact]
        public void GovernanceProfessionalFamily_ShouldCollapseToSingleGrid_AndSingleRow()
        {
            var dob = new DateTime(1990, 6, 6);

            var gpLocal = Gov("Sam", "Morgan", eLookupGovernorRole.GovernanceProfessionalToAnIndividualAcademyOrFreeSchool, dob);
            var gpShared = Gov("Sam", "Morgan", eLookupGovernorRole.Group_SharedGovernanceProfessional, dob);

            var dto = BuildDtoWith(gpLocal, gpShared);

            // Only shared has guaranteed policy
            dto.RoleDisplayPolicies = new Dictionary<eLookupGovernorRole, GovernorDisplayPolicy>
            {
                { eLookupGovernorRole.Group_SharedGovernanceProfessional, new GovernorDisplayPolicy() }
            };

            var vm = Build(dto);

            Assert.Single(vm.Grids);  // role family merges
            Assert.Equal(eLookupGovernorRole.Group_SharedGovernanceProfessional, vm.Grids[0].Role);
            Assert.Single(vm.Grids.SelectMany(g => g.Rows));
        }

        [Fact]
        public void MultipleFamilies_ShouldRenderOneGridPerFamily_AndSingleRowPerFamily()
        {
            var dob1 = new DateTime(1985, 2, 2);
            var dob2 = new DateTime(1975, 3, 3);
            var dob3 = new DateTime(1991, 7, 7);

            // Family 1: Local Gov
            var gLocal = Gov("Alex", "Taylor", eLookupGovernorRole.LocalGovernor, dob1);
            var gShared = Gov("Alex", "Taylor", eLookupGovernorRole.Group_SharedLocalGovernor, dob1);

            // Family 2: Chairs
            var cLocal = Gov("Ava", "Singh", eLookupGovernorRole.ChairOfLocalGoverningBody, dob2);
            var cShared = Gov("Ava", "Singh", eLookupGovernorRole.Group_SharedChairOfLocalGoverningBody, dob2);

            // Family 3: Governance Professional
            var gpLocal = Gov("Sam", "Morgan", eLookupGovernorRole.GovernanceProfessionalToAnIndividualAcademyOrFreeSchool, dob3);
            var gpShare = Gov("Sam", "Morgan", eLookupGovernorRole.Group_SharedGovernanceProfessional, dob3);

            var dto = BuildDtoWith(gLocal, gShared, cLocal, cShared, gpLocal, gpShare);

            dto.RoleDisplayPolicies = new Dictionary<eLookupGovernorRole, GovernorDisplayPolicy>
            {
                { eLookupGovernorRole.LocalGovernor, new GovernorDisplayPolicy() },
                { eLookupGovernorRole.Group_SharedChairOfLocalGoverningBody, new GovernorDisplayPolicy() },
                { eLookupGovernorRole.Group_SharedGovernanceProfessional, new GovernorDisplayPolicy() }
            };

            var vm = Build(dto);

            // Expect one grid per family → 3 families
            Assert.Equal(3, vm.Grids.Count);

            // Expect one deduped row per family → 3 rows
            Assert.Equal(3, vm.Grids.SelectMany(g => g.Rows).Count());
        }

    }
}
