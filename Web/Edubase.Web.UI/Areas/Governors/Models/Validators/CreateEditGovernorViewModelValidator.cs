using Edubase.Common;
using Edubase.Web.UI.Validation;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GAB = Edubase.Services.Enums.eLookupGovernorAppointingBody;
using GR = Edubase.Services.Enums.eLookupGovernorRole;

namespace Edubase.Web.UI.Areas.Governors.Models.Validators
{
    public class CreateEditGovernorViewModelValidator : EdubaseAbstractValidator<CreateEditGovernorViewModel>
    {
        public CreateEditGovernorViewModelValidator()
        {
            RuleFor(x => x.AppointmentStartDate)
                .Must(x => x.IsValid())
                .WithMessage("This date is invalid")
                .WithSummaryMessage("Date of appointment is invalid")
                .When(x => x.AppointmentStartDate.IsNotEmpty(), ApplyConditionTo.CurrentValidator)
                
                .Must(x => x.IsNotEmpty())
                .WithSummaryMessage("Date of appointment is required")
                .WithMessage("Required")
                .When(x => x.AppointmentStartDate.IsEmpty(), ApplyConditionTo.CurrentValidator)
                
                .Must(x => x.ToDateTime().Value <= DateTime.Today)
                .WithMessage("This cannot be a future date")
                .WithSummaryMessage("Date of appointment cannot be a future date")
                .When(x => x.AppointmentStartDate.IsNotEmpty(), ApplyConditionTo.CurrentValidator);

            RuleFor(x => x.AppointmentEndDate)
                .Must(x => x.IsValid())
                .WithMessage("This date is invalid")
                .WithSummaryMessage(x => x.GovernorRole == GR.Member ? "Date stepped down is invalid" : "Date term ends is invalid")
                .When(x => x.AppointmentEndDate.IsNotEmpty(), ApplyConditionTo.CurrentValidator)
                .Must(x => x.IsNotEmpty())
                .WithSummaryMessage(x => x.GovernorRole == GR.Member ? "Date stepped down is required" : "Date term ends is required")
                .WithMessage("Required")
                .When(x => x.GovernorRole.OneOfThese(
                    GR.ChairOfGovernors,
                    GR.ChairOfLocalGoverningBody,
                    GR.ChairOfTrustees,
                    GR.Governor,
                    GR.Trustee,
                    GR.LocalGovernor)
                    && !x.AppointingBodyId.OneOfThese(
                    GAB.ExofficioByVirtueOfOfficeAsHeadteacherprincipal,
                    GAB.ExofficioFoundationGovernorAppointedByFoundationByVirtueOfTheOfficeTheyHold),
                    ApplyConditionTo.CurrentValidator)
                .Must((model, x) => x.ToDateTime().Value > model.AppointmentStartDate.ToDateTime().Value)
                .WithMessage("Date must be after the date of appointment")
                .WithSummaryMessage(x => x.GovernorRole == GR.Member ? "Date stepped down must be after the date of appointment" : "Date term ends must be after the date of appointment")
                .When(x => x.AppointmentStartDate.IsValid() && x.AppointmentEndDate.IsValid());
            
            RuleFor(x => x.DOB)
                .Must(x => x.IsValid())
                .WithMessage("This date is invalid")
                .WithSummaryMessage("Date of birth")
                .When(x => x.DOB.IsNotEmpty(), ApplyConditionTo.CurrentValidator);
            
            RuleFor(x => x.FirstName).NotEmpty().WithSummaryMessage("First name is required").WithMessage("Required");
            RuleFor(x => x.LastName).NotEmpty().WithSummaryMessage("Last name is required").WithMessage("Required");

            RuleFor(x => x.EmailAddress)
                .NotEmpty()
                .WithSummaryMessage("Email address is required")
                .WithMessage("Required")
                .When(x => x.GovernorRole.OneOfThese(
                    GR.ChairOfGovernors,
                    GR.ChairOfLocalGoverningBody,
                    GR.ChairOfTrustees,
                    GR.AccountingOfficer,
                    GR.ChiefFinancialOfficer));

            RuleFor(x => x.TelephoneNumber)
                .NotEmpty()
                .WithSummaryMessage("Telephone number is required")
                .WithMessage("Required")
                .When(x => x.GovernorRole.OneOfThese(GR.ChairOfGovernors, GR.ChairOfLocalGoverningBody));

            RuleFor(x => x.AppointingBodyId)
                .NotNull()
                .WithSummaryMessage("Appointing body is required")
                .WithMessage("Required")
                .When(x => !x.GovernorRole.OneOfThese(GR.AccountingOfficer, GR.ChiefFinancialOfficer));

            When(x => x.ReplaceGovernorViewModel.GID.HasValue, () =>
            {
                RuleFor(x => x.ReplaceGovernorViewModel.AppointmentEndDate)
                    .Must(x => x.IsValid())
                    .WithMessage("Required")
                    .WithSummaryMessage("Appointment end date is required for the governor you're replacing")
                    .When(x => !x.ReplaceGovernorViewModel.AppointmentEndDate.IsValid(), ApplyConditionTo.CurrentValidator)
                    
                    .Must(x => x.IsValid() && x.ToDateTime().Value < DateTime.Today)
                    .WithMessage("Date must be before today")
                    .WithSummaryMessage("Appointment end date must be before today")
                    .When(x => x.ReplaceGovernorViewModel.AppointmentEndDate.IsValid() && x.ReplaceGovernorViewModel.AppointmentEndDate.IsNotEmpty(), ApplyConditionTo.CurrentValidator)

                    .Must((model, x) => x.ToDateTime().Value < model.AppointmentStartDate.ToDateTime().Value)
                    .WithMessage("Date must be before Appointment start date")
                    .WithSummaryMessage("Appointment end date must be before Appointment start date")
                    .When(x => x.ReplaceGovernorViewModel.AppointmentEndDate.IsNotEmpty() && x.AppointmentStartDate.IsNotEmpty(), ApplyConditionTo.CurrentValidator);
            });
            
        }
    }
}