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
        public void SinglePersonRole_SelectedGovernorId_WhenNull_ThenError()
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
        public void SinglePersonRole_SelectedGovernorId_WhenNoGovernorSelected_ThenError()
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
                .WithCustomState("Select a governor");
        }

        [Fact]
        public void MultiPersonRole_Governors_WhenNoGovernorSelected_ThenError()
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
                Role = ArbitraryMultiPersonRole,
            };

            // Act
            var result = _validator.TestValidate(model);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Governors)
                .WithErrorMessage("Required")
                .WithCustomState("Select at least one governor");
        }

        [Fact]
        public void SinglePersonRole_SelectedGovernorId_WhenValid_NoError()
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
        public void MultiPersonRole_Governors_WhenValid_NoError()
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

        [Fact]
        public void MultiPersonRole_Governors_WhenInvalid_ThenError()
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
                        AppointmentStartDate = _arbitraryInvalidStartDate // Invalid entry
                    }
                },
                Role = ArbitraryMultiPersonRole,
            };

            // Act
            var result = _validator.TestValidate(model);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Governors)
                .WithErrorMessage("Required")
                .WithCustomState("Select at least one governor");
        }

        [Fact]
        public void SinglePersonRole_SelectedGovernorId_WhenMultipleGovernorsButNoneSelected_ThenError()
        {
            // Arrange
            var model = new SelectSharedGovernorViewModel
            {
                SelectedGovernorId = null,
                Governors = new List<SharedGovernorViewModel>
                {
                    new SharedGovernorViewModel { Id = 1, Selected = false },
                    new SharedGovernorViewModel { Id = 2, Selected = false },
                },
                Role = ArbitrarySinglePersonRole,
            };

            // Act
            var result = _validator.TestValidate(model);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.SelectedGovernorId)
                .WithErrorMessage("Required")
                .WithCustomState("Select a governor");
        }

        [Fact]
        public void MultiPersonRole_Governors_WhenMultipleGovernorsButNoneSelected_ThenError()
        {
            // Arrange
            var model = new SelectSharedGovernorViewModel
            {
                SelectedGovernorId = null,
                Governors = new List<SharedGovernorViewModel>
                {
                    new SharedGovernorViewModel { Id = 1, Selected = false },
                    new SharedGovernorViewModel { Id = 2, Selected = false },
                },
                Role = ArbitraryMultiPersonRole,
            };

            // Act
            var result = _validator.TestValidate(model);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Governors)
                .WithErrorMessage("Required")
                .WithCustomState("Select at least one governor");
        }

        [Fact]
        public void SinglePersonRole_SelectedGovernorId_WhenMultipleGovernorsAndOneSelected_NoError()
        {
            // Arrange
            var model = new SelectSharedGovernorViewModel
            {
                SelectedGovernorId = "2",
                Governors = new List<SharedGovernorViewModel>
                {
                    new SharedGovernorViewModel { Id = 1, Selected = false, AppointmentStartDate = _arbitraryValidStartDate, AppointmentEndDate = _arbitraryValidEndDate },
                    new SharedGovernorViewModel { Id = 2, Selected = true, AppointmentStartDate = _arbitraryValidStartDate, AppointmentEndDate = _arbitraryValidEndDate },
                },
                Role = ArbitrarySinglePersonRole,
            };

            // Act
            var result = _validator.TestValidate(model);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.SelectedGovernorId);
        }

        [Fact]
        public void MultiPersonRole_Governors_WhenMultipleGovernorsAndOneSelected_NoError()
        {
            // Arrange
            var model = new SelectSharedGovernorViewModel
            {
                SelectedGovernorId = null,
                Governors = new List<SharedGovernorViewModel>
                {
                    new SharedGovernorViewModel { Id = 1, Selected = true, AppointmentStartDate = _arbitraryValidStartDate, AppointmentEndDate = _arbitraryValidEndDate },
                    new SharedGovernorViewModel { Id = 2, Selected = false, AppointmentStartDate = _arbitraryValidStartDate, AppointmentEndDate = _arbitraryValidEndDate },
                },
                Role = ArbitraryMultiPersonRole,
            };

            // Act
            var result = _validator.TestValidate(model);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.Governors);
        }

        [Fact]
        public void MultiPersonRole_Governors_WhenMultipleGovernorsAndMultipleSelected_NoError()
        {
            // Arrange
            var model = new SelectSharedGovernorViewModel
            {
                SelectedGovernorId = null,
                Governors = new List<SharedGovernorViewModel>
                {
                    new SharedGovernorViewModel { Id = 1, Selected = true, AppointmentStartDate = _arbitraryValidStartDate, AppointmentEndDate = _arbitraryValidEndDate },
                    new SharedGovernorViewModel { Id = 2, Selected = true, AppointmentStartDate = _arbitraryValidStartDate, AppointmentEndDate = _arbitraryValidEndDate },
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
