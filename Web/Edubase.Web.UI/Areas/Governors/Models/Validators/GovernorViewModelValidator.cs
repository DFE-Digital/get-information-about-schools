using System;
using Edubase.Common;
using Edubase.Services.Enums;
using FluentValidation;

namespace Edubase.Web.UI.Areas.Governors.Models.Validators;

public class GovernorViewModelValidator<T> : AbstractValidator<T> where T : GovernorViewModel
{
    public GovernorViewModelValidator()
    {
        RuleFor(x => x.AppointmentStartDate)
            .Must(x => x.IsValid())
            .WithMessage("This date is invalid")
            .WithSummaryMessage("Date of appointment is invalid")
            .When(x => x.AppointmentStartDate.IsNotEmpty())

            .Must(x => x.IsNotEmpty())
            .WithMessage("Required")
            .WithSummaryMessage("Date of appointment is required")

            .Must(x => x.ToDateTime().HasValue && x.ToDateTime().Value <= DateTime.Today)
            .WithMessage("This cannot be a future date")
            .WithSummaryMessage("Date of appointment cannot be a future date")
            .When(x => x.AppointmentStartDate.IsNotEmpty());

        RuleFor(x => x.AppointmentEndDate)
            .Must(x => x.IsValid())
            .WithMessage("This date is invalid")
            .WithSummaryMessage(x => x.GovernorRole == eLookupGovernorRole.Member
                ? "Date stepped down is invalid"
                : "Date term ends is invalid")
            .When(x => x.AppointmentEndDate.IsNotEmpty())

            .Must(x => x.IsNotEmpty())
            .WithMessage("Required")
            .WithSummaryMessage(x => x.GovernorRole == eLookupGovernorRole.Member
                ? "Date stepped down is required"
                : "Date term ends is required")
            .When(x =>
                x.GovernorRole.OneOfThese(
                    eLookupGovernorRole.ChairOfGovernors,
                    eLookupGovernorRole.ChairOfLocalGoverningBody,
                    eLookupGovernorRole.ChairOfTrustees,
                    eLookupGovernorRole.Governor,
                    eLookupGovernorRole.Trustee,
                    eLookupGovernorRole.LocalGovernor)
                && !x.AppointingBodyId.OneOfThese(
                    eLookupGovernorAppointingBody.ExofficioByVirtueOfOfficeAsHeadteacherprincipal,
                    eLookupGovernorAppointingBody.ExofficioFoundationGovernorAppointedByFoundationByVirtueOfTheOfficeTheyHold))

            .Must((model, endDate) =>
                endDate.ToDateTime().HasValue &&
                model.AppointmentStartDate.ToDateTime().HasValue &&
                endDate.ToDateTime().Value > model.AppointmentStartDate.ToDateTime().Value)
            .WithMessage("Date must be after the date of appointment")
            .WithSummaryMessage(x => x.GovernorRole == eLookupGovernorRole.Member
                ? "Date stepped down must be after the date of appointment"
                : "Date term ends must be after the date of appointment")
            .When(x => x.AppointmentStartDate.IsValid() && x.AppointmentEndDate.IsValid());

        RuleFor(x => x.DOB)
            .Must(x => x.IsValid())
            .WithMessage("This date is invalid")
            .WithSummaryMessage("Date of birth")
            .When(x => x.DOB.IsNotEmpty());

        RuleFor(x => x.FirstName)
            .NotEmpty()
            .WithMessage("Required")
            .WithSummaryMessage("First name is required");

        RuleFor(x => x.LastName)
            .NotEmpty()
            .WithMessage("Required")
            .WithSummaryMessage("Last name is required");

        RuleFor(x => x.EmailAddress)
            .NotEmpty()
            .WithMessage("Required")
            .WithSummaryMessage("Email address is required")
            .When(x => !x.GovernorRole.OneOfThese(
                eLookupGovernorRole.Governor,
                eLookupGovernorRole.Trustee,
                eLookupGovernorRole.LocalGovernor,
                eLookupGovernorRole.Member));

        RuleFor(x => x.TelephoneNumber)
            .NotEmpty()
            .WithMessage("Required")
            .WithSummaryMessage("Telephone number is required")
            .When(x => x.GovernorRole.OneOfThese(
                eLookupGovernorRole.ChairOfGovernors,
                eLookupGovernorRole.ChairOfLocalGoverningBody));

        RuleFor(x => x.AppointingBodyId)
            .NotNull()
            .WithMessage("Required")
            .WithSummaryMessage("Appointing body is required")
            .When(x => !x.GovernorRole.OneOfThese(
                eLookupGovernorRole.AccountingOfficer,
                eLookupGovernorRole.ChiefFinancialOfficer));
    }
}
