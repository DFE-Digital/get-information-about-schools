using System;
using System.Linq;
using System.Web.Mvc;
using Edubase.Services.Enums;
using Edubase.Web.UI.Areas.Governors.Models;
using Xunit;

namespace Edubase.Web.UIUnitTests.Areas.Governors.Models
{
    public class GovernorViewModelTests
    {
        [Fact]
        public void EmptyViewModelHasEmptyFullName()
        {
            var governorViewModel = new GovernorViewModel();
            Assert.Equal(string.Empty, governorViewModel.FullName);
        }

        [Fact]
        public void FullNameContainsTitle()
        {
            var arbitraryInt = 1;
            var arbitraryTitle = "Arbitrary Title";
            var governorViewModel = new GovernorViewModel()
            {
                GovernorTitleId = arbitraryInt,
                Titles = new[]
                {
                    new SelectListItem() { Value = arbitraryInt.ToString(), Text = arbitraryTitle},
                }
            };
            Assert.Equal(arbitraryTitle, governorViewModel.FullName);
        }

        [Fact]
        public void FullNameContainsTitleExceptWhenNotApplicable()
        {
            var arbitraryInt = 1;
            var governorViewModel = new GovernorViewModel()
            {
                GovernorTitleId = arbitraryInt,
                Titles = new[]
                {
                    new SelectListItem() { Value = arbitraryInt.ToString(), Text = "Not-applicable"},
                }
            };
            Assert.Equal(string.Empty, governorViewModel.FullName);
        }

        [Fact]
        public void FullNameContainsTitleExceptWhenNotRecorded()
        {
            var arbitraryInt = 1;
            var governorViewModel = new GovernorViewModel()
            {
                GovernorTitleId = arbitraryInt,
                Titles = new[]
                {
                    new SelectListItem() { Value = arbitraryInt.ToString(), Text = "Not recorded"},
                }
            };
            Assert.Equal(string.Empty, governorViewModel.FullName);
        }

        [Fact]
        public void FullNameContainsFirstName()
        {
            var vm = new GovernorViewModel { FirstName = "ABC" };
            Assert.Equal("ABC", vm.FullName);
        }

        [Fact]
        public void FullNameContainsMiddleName()
        {
            var vm = new GovernorViewModel { MiddleName = "ABC" };
            Assert.Equal("ABC", vm.FullName);
        }

        [Fact]
        public void FullNameContainsLastName()
        {
            var vm = new GovernorViewModel { LastName = "ABC" };
            Assert.Equal("ABC", vm.FullName);
        }

        [Fact]
        public void FullNameContainsFirstAndMiddle()
        {
            var vm = new GovernorViewModel
            {
                FirstName = "A",
                LastName = "B"
            };
            Assert.Equal("A B", vm.FullName);
        }

        [Fact]
        public void FullNameContainsFirstAndLast()
        {
            var vm = new GovernorViewModel
            {
                FirstName = "A",
                LastName = "C"
            };
            Assert.Equal("A C", vm.FullName);
        }

        [Fact]
        public void FullNameContainsFirstMiddleLast()
        {
            var vm = new GovernorViewModel
            {
                FirstName = "A",
                MiddleName = "B",
                LastName = "C"
            };
            Assert.Equal("A B C", vm.FullName);
        }

        [Fact]
        public void FullNameContainsTitleAndFirst()
        {
            var vm = new GovernorViewModel
            {
                GovernorTitleId = 1,
                Titles = new[] { new SelectListItem { Value = "1", Text = "Dr" } },
                FirstName = "Tom"
            };
            Assert.Equal("Dr Tom", vm.FullName);
        }

        [Fact]
        public void FullNameContainsTitleFirstMiddleLast()
        {
            var vm = new GovernorViewModel
            {
                GovernorTitleId = 1,
                Titles = new[] { new SelectListItem { Value = "1", Text = "Prof" } },
                FirstName = "A",
                MiddleName = "B",
                LastName = "C"
            };
            Assert.Equal("Prof A B C", vm.FullName);
        }

        [Fact]
        public void FullName_TrimsAllValues()
        {
            var vm = new GovernorViewModel
            {
                GovernorTitleId = 1,
                Titles = new[] { new SelectListItem { Value = "1", Text = " Dr " } },
                FirstName = " A ",
                MiddleName = " B ",
                LastName = " C "
            };
            Assert.Equal("Dr A B C", vm.FullName);
        }

        [Fact]
        public void NewGovernorViewModel_Defaults_OriginalFlags_ToNull()
        {
            var vm = new GovernorViewModel();

            Assert.Null(vm.IsOriginalChairOfTrustees);
            Assert.Null(vm.IsOriginalSignatoryMember);
        }

        [Fact]
        public void YesNoSelect_Defaults_No_Selected()
        {
            var vm = new GovernorViewModel();
            var items = vm.YesNoSelect.ToList();

            var yes = Assert.Single(items.Where(x => x.Value == "true"));
            var no = Assert.Single(items.Where(x => x.Value == "false"));

            Assert.False(yes.Selected);
            Assert.True(no.Selected);
        }
    }


    public class CreateEditGovernorViewModelTests
    {
        [Fact]
        public void ExistingGovernors_Defaults_ToEmpty()
        {
            var vm = new CreateEditGovernorViewModel();
            Assert.Empty(vm.ExistingGovernors);
        }

        [Fact]
        public void ReplaceGovernorViewModel_NotNull()
        {
            var vm = new CreateEditGovernorViewModel();
            Assert.NotNull(vm.ReplaceGovernorViewModel);
        }

        [Theory]
        [InlineData(null, null, null, "GroupAddGovernor")]
        [InlineData(1, null, null, "EstabAddGovernor")]
        [InlineData(null, 5, null, "GroupEditGovernor")]
        [InlineData(1, 5, null, "EstabEditGovernor")]
        [InlineData(null, null, 22, "GroupReplaceGovernor")]
        [InlineData(1, null, 22, "EstabReplaceGovernor")]
        public void FormPostRouteName_ComputedCorrectly(
            int? establishmentUrn,
            int? gid,
            int? replaceGid,
            string expected)
        {
            var vm = new CreateEditGovernorViewModel
            {
                EstablishmentUrn = establishmentUrn,
                GID = gid,
                ReplaceGovernorViewModel = new ReplaceGovernorViewModel { GID = replaceGid }
            };

            Assert.Equal(expected, vm.FormPostRouteName);
        }

        #region AllowReinstateAsGovernor Tests

        [Fact]
        public void AllowReinstate_True_ForEligibleChair_WithEstabUrn()
        {
            var vm = new CreateEditGovernorViewModel
            {
                EstablishmentUrn = 7,
                GovernorRole = eLookupGovernorRole.ChairOfGovernors
            };

            Assert.True(vm.AllowReinstateAsGovernor);
        }

        [Fact]
        public void AllowReinstate_False_WhenNoEstablishmentUrn()
        {
            var vm = new CreateEditGovernorViewModel
            {
                EstablishmentUrn = null,
                GovernorRole = eLookupGovernorRole.ChairOfGovernors
            };

            Assert.False(vm.AllowReinstateAsGovernor);
        }

        [Fact]
        public void AllowReinstate_False_WhenNotChair()
        {
            var vm = new CreateEditGovernorViewModel
            {
                EstablishmentUrn = 123,
                GovernorRole = eLookupGovernorRole.Governor
            };

            Assert.False(vm.AllowReinstateAsGovernor);
        }

        #endregion

        #region Label Tests

        [Fact]
        public void Reinstate_Label_UsesTrustee_ForChairOfTrustees()
        {
            var vm = new CreateEditGovernorViewModel
            {
                GovernorRole = eLookupGovernorRole.ChairOfTrustees
            };

            Assert.Equal("Re-instate as trustee", vm.ReinstateAsGovernorCheckboxLabel);
        }

        [Fact]
        public void Reinstate_Label_UsesGovernor_ForOtherChairRoles()
        {
            var vm = new CreateEditGovernorViewModel
            {
                GovernorRole = eLookupGovernorRole.ChairOfGovernors
            };

            Assert.Equal("Re-instate as governor", vm.ReinstateAsGovernorCheckboxLabel);
        }

        [Fact]
        public void SelectPrevious_Label_UsesTrustee_ForChairOfTrustees()
        {
            var vm = new CreateEditGovernorViewModel
            {
                GovernorRole = eLookupGovernorRole.ChairOfTrustees
            };

            Assert.Equal("Choose trustee", vm.SelectPreviousGovernorLabel);
        }

        [Fact]
        public void SelectPrevious_Label_UsesGovernor_ForOtherChairRoles()
        {
            var vm = new CreateEditGovernorViewModel
            {
                GovernorRole = eLookupGovernorRole.ChairOfGovernors
            };

            Assert.Equal("Choose governor", vm.SelectPreviousGovernorLabel);
        }

        #endregion

    }
}
