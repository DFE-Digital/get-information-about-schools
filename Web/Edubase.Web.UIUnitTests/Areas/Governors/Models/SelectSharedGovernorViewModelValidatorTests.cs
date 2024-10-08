using System.Collections.Generic;
using Edubase.Services.Enums;
using Edubase.Web.UI.Areas.Governors.Models;
using Edubase.Web.UI.Areas.Governors.Models.Validators;
using Edubase.Web.UI.Models;
using FluentValidation.TestHelper;
using Xunit;

namespace Edubase.Web.UIUnitTests.Areas.Governors.Models
{
    public class SelectSharedGovernorViewModelValidatorTests
    {
        private readonly SelectSharedGovernorViewModelValidator _validator = new SelectSharedGovernorViewModelValidator();

        private const eLookupGovernorRole ArbitrarySinglePersonRole = eLookupGovernorRole.ChairOfGovernors;
        private const eLookupGovernorRole ArbitraryMultiPersonRole = eLookupGovernorRole.Governor;

        private readonly DateTimeViewModel _arbitraryValidStartDate = new DateTimeViewModel { Day = 1, Month = 1, Year = 2020 };
        private readonly DateTimeViewModel _arbitraryValidEndDate = new DateTimeViewModel { Day = 1, Month = 1, Year = 2055 };

        private readonly DateTimeViewModel _arbitraryInvalidStartDate = new DateTimeViewModel { Day = 50, Month = 20, Year = null };
        private readonly DateTimeViewModel _arbitraryInvalidEndDate = new DateTimeViewModel { Day = 50, Month = 20, Year = null };



        [Fact]
        public void SinglePersonRole__SelectedGovernorId__WhenNull__And__GovernorsList_Empty__Then__GovernorId_ValidationError()
        {
            // Arrange
            var model = new SelectSharedGovernorViewModel
            {
                SelectedGovernorId = null,
                Governors = new List<SharedGovernorViewModel>(), // No governors
                Role = ArbitrarySinglePersonRole,
            };

            // Act
            var result = _validator.TestValidate(model);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.SelectedGovernorId)
                .WithErrorMessage("Required");
        }

        [Fact]
        public void SinglePersonRole__SelectedGovernorId__WhenNull__And__GovernorsList_NoneSelected__Then__GovernorId_ValidationError()
        {
            // Arrange
            var model = new SelectSharedGovernorViewModel
            {
                SelectedGovernorId = null, // Invalid scenario
                Governors = new List<SharedGovernorViewModel>
                {
                    new SharedGovernorViewModel
                    {
                        Id = 1,
                        Selected = false,
                        AppointmentStartDate = new DateTimeViewModel(),
                        AppointmentEndDate = new DateTimeViewModel()
                    }
                },
                Role = ArbitrarySinglePersonRole,
            };

            // Act
            var result = _validator.TestValidate(model);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.SelectedGovernorId)
                .WithErrorMessage("Required")
                .WithCustomState("You must select a governor");
        }

        [Fact]
        public void ShouldHaveValidationErrorWhenNoSelectedGovernorForMultiRole()
        {
            // Arrange
            var model = new SelectSharedGovernorViewModel
            {
                SelectedGovernorId = null, // Not applicable for multi roles
                Governors = new List<SharedGovernorViewModel>
                {
                    new SharedGovernorViewModel
                    {
                        Id = 1,
                        Selected = false,
                        AppointmentStartDate = new DateTimeViewModel(),
                        AppointmentEndDate = new DateTimeViewModel()
                    }
                },
                Role = ArbitrarySinglePersonRole,
            };

            // Act
            var result = _validator.TestValidate(model);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.SelectedGovernorId)
                .WithErrorMessage("Required")
                .WithCustomState("You must select a governor");
        }

        [Fact]
        public void ShouldNotHaveValidationErrorWhenRequiredFieldsAreValidForSingularRole()
        {
            // Arrange
            var model = new SelectSharedGovernorViewModel
            {
                SelectedGovernorId = "1",
                Governors = new List<SharedGovernorViewModel>
                {
                    new SharedGovernorViewModel
                    {
                        Id = 1,
                        Selected = true,
                        AppointmentStartDate = new DateTimeViewModel { Day = 1, Month = 1, Year = 2020 },
                        AppointmentEndDate = new DateTimeViewModel { Day = 1, Month = 1, Year = 2025 }
                    }
                },
                Role = ArbitrarySinglePersonRole,
            };

            // Act
            var result = _validator.TestValidate(model);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.SelectedGovernorId);
        }

        [Fact]
        public void ShouldNotHaveValidationErrorWhenRequiredFieldsAreValidForMultiRole()
        {
            // Arrange
            var model = new SelectSharedGovernorViewModel
            {
                SelectedGovernorId = null, // Not applicable for multi roles
                Governors = new List<SharedGovernorViewModel>
                {
                    new SharedGovernorViewModel
                    {
                        Id = 1,
                        Selected = true,
                        AppointmentStartDate = new DateTimeViewModel { Day = 1, Month = 1, Year = 2020 },
                        AppointmentEndDate = new DateTimeViewModel { Day = 1, Month = 1, Year = 2025 }
                    }
                },
                Role = ArbitraryMultiPersonRole,
            };

            // Act
            var result = _validator.TestValidate(model);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.Governors);
        }
    }
}
