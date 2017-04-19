using System;
using Edubase.Common;
using Edubase.Services.Enums;
using Edubase.Web.UI.Validation;
using FluentValidation;

namespace Edubase.Web.UI.Areas.Governors.Models.Validators
{
    public class GovernorViewModelValidator<T> : EdubaseAbstractValidator<T> where T : GovernorViewModel
    {
        public GovernorViewModelValidator()
        {
            RuleFor(x => x.AppointmentStartDate)
                .Must(x => x.IsValid())
                .WithMessage("This date is invalid")
                .WithSummaryMessage("Date of appointment is invalid")
                .When(x => x.AppointmentStartDate.IsNotEmpty(), ApplyConditionTo.CurrentValidator)

                .Must(x => x.IsNotEmpty())
                .WithSummaryMessage("Date of appointment is required")
                .WithMessage("Required")
                .When(x => x.AppointmentStartDate.IsEmpty()
                           && !x.GovernorRole.OneOfThese(
                               eLookupGovernorRole.SharedChairOfLocalGoverningBody,
                               eLookupGovernorRole.SharedLocalGovernor), ApplyConditionTo.CurrentValidator)

                .Must(x => x.ToDateTime().Value <= DateTime.Today)
                .WithMessage("This cannot be a future date")
                .WithSummaryMessage("Date of appointment cannot be a future date")
                .When(x => x.AppointmentStartDate.IsNotEmpty(), ApplyConditionTo.CurrentValidator);

            RuleFor(x => x.AppointmentEndDate)
                .Must(x => x.IsValid())
                .WithMessage("This date is invalid")
                .WithSummaryMessage(x => x.GovernorRole == eLookupGovernorRole.Member ? "Date stepped down is invalid" : "Date term ends is invalid")
                .When(x => x.AppointmentEndDate.IsNotEmpty(), ApplyConditionTo.CurrentValidator)
                .Must(x => x.IsNotEmpty())
                .WithSummaryMessage(x => x.GovernorRole == eLookupGovernorRole.Member ? "Date stepped down is required" : "Date term ends is required")
                .WithMessage("Required")
                .When(x => x.GovernorRole.OneOfThese(
                               eLookupGovernorRole.ChairOfGovernors,
                               eLookupGovernorRole.ChairOfLocalGoverningBody,
                               eLookupGovernorRole.ChairOfTrustees,
                               eLookupGovernorRole.Governor,
                               eLookupGovernorRole.Trustee,
                               eLookupGovernorRole.LocalGovernor)
                           && !x.AppointingBodyId.OneOfThese(
                               eLookupGovernorAppointingBody.ExofficioByVirtueOfOfficeAsHeadteacherprincipal,
                               eLookupGovernorAppointingBody.ExofficioFoundationGovernorAppointedByFoundationByVirtueOfTheOfficeTheyHold),
                    ApplyConditionTo.CurrentValidator)
                .Must((model, x) => x.ToDateTime().Value > model.AppointmentStartDate.ToDateTime().Value)
                .WithMessage("Date must be after the date of appointment")
                .WithSummaryMessage(x => x.GovernorRole == eLookupGovernorRole.Member ? "Date stepped down must be after the date of appointment" : "Date term ends must be after the date of appointment")
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
                    eLookupGovernorRole.ChairOfGovernors,
                    eLookupGovernorRole.ChairOfLocalGoverningBody,
                    eLookupGovernorRole.ChairOfTrustees,
                    eLookupGovernorRole.AccountingOfficer,
                    eLookupGovernorRole.ChiefFinancialOfficer));

            RuleFor(x => x.TelephoneNumber)
                .NotEmpty()
                .WithSummaryMessage("Telephone number is required")
                .WithMessage("Required")
                .When(x => x.GovernorRole.OneOfThese(eLookupGovernorRole.ChairOfGovernors, eLookupGovernorRole.ChairOfLocalGoverningBody));

            RuleFor(x => x.AppointingBodyId)
                .NotNull()
                .WithSummaryMessage("Appointing body is required")
                .WithMessage("Required")
                .When(x => !x.GovernorRole.OneOfThese(eLookupGovernorRole.AccountingOfficer, eLookupGovernorRole.ChiefFinancialOfficer));
        }
    }
}