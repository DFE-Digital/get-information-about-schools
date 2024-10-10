using System.Linq;
using Edubase.Services.Enums;
using Edubase.Web.UI.Validation;
using FluentValidation;

namespace Edubase.Web.UI.Areas.Governors.Models.Validators
{
    public class SelectSharedGovernorViewModelValidator : EdubaseAbstractValidator<SelectSharedGovernorViewModel>
    {
        public SelectSharedGovernorViewModelValidator()
        {
            // This section for "singular" governor roles (i.e., roles that can only have one appointee)
            // On the web UI, these are displayed as radio buttons and the `Selected` value is ignored.
            // Instead, the property `SelectedGovernorId` is used to track which governor is selected.
            When(x => EnumSets.eSingularGovernorRoles.Contains(x.Role), () =>
            {
                // Ensure the governor ID is selected and valid
                RuleFor(x => x.SelectedGovernorId)
                    .Must(IsSelectedGovernorIdFound)
                    .WithMessage("Required")
                    .WithSummaryMessage("Select a governor");

                // Validate appointment start date (only if/where the governor is selected)
                RuleFor(x => x.SelectedGovernorId)
                    .Must(IsAppointmentStartDateValid)
                    .When(x => x.SelectedGovernorId != null)
                    .WithMessage("Required")
                    .WithMessage(y =>
                    {
                        int.TryParse(y.SelectedGovernorId, out var governorId);
                        var selectedGovernorRecord = y
                            .Governors
                            .SingleOrDefault(g => g.Id == governorId);

                        return $"Enter a valid appointment end date for {selectedGovernorRecord?.FullName ?? "unknown governor"} ({y.SelectedGovernorId})";
                    });

                // Validate appointment end date (only if/where the governor is selected, and the role is not a `Shared Governance Professional - Establishment`)
                RuleFor(x => x.SelectedGovernorId)
                    .Must(IsAppointmentEndDateValid)
                    .When(x => x.SelectedGovernorId != null && x.Role != eLookupGovernorRole.Establishment_SharedGovernanceProfessional)
                    .WithMessage("Required")
                    .WithMessage(y =>
                    {
                        int.TryParse(y.SelectedGovernorId, out var governorId);
                        var selectedGovernorRecord = y
                            .Governors
                            .SingleOrDefault(g => g.Id == governorId);

                        return $"Enter a valid appointment end date for {(selectedGovernorRecord?.FullName ?? "unknown governor")} ({y.SelectedGovernorId})";
                    });
            });


            // This section for roles that can have multiple appointees.
            // On the web UI, these are displayed as checkboxes and the `Selected` value for each governor
            // is used to track which governors are selected (the `SelectedGovernorId` is ignored).
            // In this case, validations need to be applied to all selected governors (not just a single "selected" governor).
            When(x => !EnumSets.eSingularGovernorRoles.Contains(x.Role), () =>
            {
                // Ensure at least one governor is selected (can be multiple, and at least one needs to be selected)
                RuleFor(x => x.Governors)
                    .Must(x => x.Any(g => g.Selected))
                    .WithMessage("Required")
                    .WithSummaryMessage("Select at least one governor");

                // Validate appointment start date and end date for each selected governor (only if/where the governor is marked as selected)
                RuleForEach(z => z.Governors)
                    .ChildRules(x => x
                        .RuleFor(y => y.AppointmentStartDate)
                        .Must(y => y.IsValid())
                        .When(y => y.Selected)
                        .WithMessage("Required")
                        .WithSummaryMessage(y => $"Enter a valid appointment start date for {y.FullName} ({y.Id})")
                    )
                    .ChildRules(x => x
                        .RuleFor(y => y.AppointmentEndDate)
                        .Must(y => y.IsValid())
                        .When(y => y.Selected)
                        .WithMessage("Required")
                        .WithSummaryMessage(y => $"Enter a valid appointment end date for {y.FullName} ({y.Id})")
                    );
            });
        }

        private static bool IsSelectedGovernorIdFound(SelectSharedGovernorViewModel model, string selectedId)
        {
            if (!int.TryParse(selectedId, out int governorId))
            {
                return false;
            }

            var selectedGovernorRecord = model
                .Governors
                .SingleOrDefault(g => g.Id == governorId);

            return selectedGovernorRecord != null;
        }

        private static bool IsAppointmentStartDateValid(SelectSharedGovernorViewModel model, string selectedId)
        {
            if (!int.TryParse(selectedId, out int governorId))
            {
                return false;
            }

            var selectedGovernorRecord = model
                .Governors
                .SingleOrDefault(g => g.Id == governorId);

            return selectedGovernorRecord?.AppointmentStartDate.IsValid() ?? false;
        }

        private static bool IsAppointmentEndDateValid(SelectSharedGovernorViewModel model, string selectedId)
        {
            if (int.TryParse(selectedId, out int governorId))
            {
                return model.Governors.SingleOrDefault(g => g.Id == governorId)?.AppointmentEndDate.IsValid() ?? false;
            }

            return false;
        }
    }
}
