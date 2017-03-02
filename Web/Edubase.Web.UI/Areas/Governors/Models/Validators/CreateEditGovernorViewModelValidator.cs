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
                .When(x => x.AppointmentStartDate.IsEmpty(), ApplyConditionTo.CurrentValidator);

            RuleFor(x => x.AppointmentEndDate)
                .Must(x => x.IsValid())
                .WithMessage("This date is invalid")
                .WithSummaryMessage("Date term ends")
                .When(x => x.AppointmentEndDate.IsNotEmpty(), ApplyConditionTo.CurrentValidator)
                .Must(x => x.IsNotEmpty())
                .WithSummaryMessage("Date of appointment is required")
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
                    ApplyConditionTo.CurrentValidator);
            

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
            
        }
    }
}