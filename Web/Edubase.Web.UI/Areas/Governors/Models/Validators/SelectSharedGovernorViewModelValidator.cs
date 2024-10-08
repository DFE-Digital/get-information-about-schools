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
            When(x => EnumSets.eSingularGovernorRoles.Contains(x.Role), () =>
            {
                // Ensure the governor ID is selected and valid
                RuleFor(x => x.SelectedGovernorId)
                    .Must(IsSelectedGovernorIdFound)
                    .WithMessage("Required")
                    .WithSummaryMessage("Select a governor");

                // Validate appointment start date
                RuleFor(x => x.SelectedGovernorId)
                    .Must(IsAppointmentStartDateValid)
                    .When(x => x.SelectedGovernorId != null)
                    .WithMessage("Required")
                    .WithMessage(y =>
                    {

                        int.TryParse(y.SelectedGovernorId, out int governorId);

                        var selectedGovernorRecord = y
                            .Governors
                            .SingleOrDefault(g => g.Id == governorId);

                        return "Enter a valid appointment end date for " + (selectedGovernorRecord?.FullName ?? "unknown governor") + " (" + y.SelectedGovernorId + ")";
                    });

                // Validate appointment end date
                RuleFor(x => x.SelectedGovernorId)
                    .Must(IsAppointmentEndDateValid)
                    .When(x => x.SelectedGovernorId != null && x.Role != eLookupGovernorRole.Establishment_SharedGovernanceProfessional)
                    .WithMessage("Required")
                    .WithMessage(y =>
                    {

                        int.TryParse(y.SelectedGovernorId, out int governorId);

                        var selectedGovernorRecord = y
                            .Governors
                            .SingleOrDefault(g => g.Id == governorId);

                        return "Enter a valid appointment end date for " + (selectedGovernorRecord?.FullName ?? "unknown governor") + " (" + y.SelectedGovernorId + ")";
                    });
            });


            // When the role may have multiple appointees (i.e., is not "singular"),
            // validations need to be performed on the list of governors
            When(x => !EnumSets.eSingularGovernorRoles.Contains(x.Role), () =>
            {
                // Ensure at least one governor is selected (can be multiple, and at least one needs to be selected)
                RuleFor(x => x.Governors)
                    .Must(x => x.Any(g => g.Selected))
                    .WithMessage("Required")
                    .WithSummaryMessage("Select at least one governor");

                RuleForEach(z => z.Governors)
                    .ChildRules(x => x
                        .RuleFor(y => y.AppointmentStartDate)
                        .Must(y => y.IsValid())
                        .When(y => y.Selected)
                        .WithMessage("Required")
                        .WithSummaryMessage(y => "Enter a valid appointment start date for " + y.FullName + " (" + y.Id + ")")

                    )
                    .ChildRules(x => x
                        .RuleFor(y => y.AppointmentEndDate)
                        .Must(y => y.IsValid())
                        .When(y => y.Selected)
                        .WithMessage("Required")
                        .WithSummaryMessage(y => "Enter a valid appointment end date for " + y.FullName + " (" + y.Id + ")")
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
