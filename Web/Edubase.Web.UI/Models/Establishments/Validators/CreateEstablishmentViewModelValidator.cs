using Edubase.Services;
using Edubase.Services.Establishments;
using Edubase.Web.UI.Validation;
using FluentValidation;
using System.Linq;

namespace Edubase.Web.UI.Models.Establishments.Validators
{
    public class CreateEstablishmentViewModelValidator : EdubaseAbstractValidator<CreateEstablishmentViewModel>
    {
        public CreateEstablishmentViewModelValidator(IEstablishmentReadService establishmentReadService)
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("Please enter an establishment name");
            RuleFor(x => x.LocalAuthorityId).NotEmpty().WithMessage("Please select a local authority");

            RuleFor(x => x.EducationPhaseId)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotEmpty().WithMessage("Please select a a phase of education")
                .Must((m, x) => (establishmentReadService.GetEstabType2EducationPhaseMap().AsInts()[m.EstablishmentTypeId]).Contains(x))
                .WithMessage("Education phase is not valid for the selected type of establishment");

            RuleFor(x => x.EstablishmentTypeId).NotEmpty().WithMessage("Please select an establishment type");
            RuleFor(x => x.GenerateEstabNumber).NotNull().WithMessage("Please select to enter a number or have one generated for you");

            RuleFor(x => x.EstablishmentNumber)
                .NotEmpty().WithMessage("Please check the number you've entered")
                .When(x => x.GenerateEstabNumber.HasValue && !x.GenerateEstabNumber.GetValueOrDefault());
        }
    }

    public class CreateChildrensCentreViewModelValidator : EdubaseAbstractValidator<CreateChildrensCentreViewModel>
    {
        public CreateChildrensCentreViewModelValidator(IEstablishmentReadService establishmentReadService)
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("Please enter an establishment name");
            RuleFor(x => x.LocalAuthorityId).NotEmpty().WithMessage("Please select a local authority");

            RuleFor(x => x.EducationPhaseId)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotEmpty().WithMessage("Please select a a phase of education")
                .Must((m, x) => (establishmentReadService.GetEstabType2EducationPhaseMap().AsInts()[m.EstablishmentTypeId]).Contains(x))
                .WithMessage("Education phase is not valid for the selected type of establishment")
                .When(x => x.EstablishmentTypeId != 41);

            RuleFor(x => x.EstablishmentTypeId)
                .NotEmpty()
                .WithMessage("Please select an establishment type");

            RuleFor(x => x.GenerateEstabNumber)
                .NotNull()
                .WithMessage("Please select to enter a number or have one generated for you")
                .When(x => x.EstablishmentTypeId != 41);

            RuleFor(x => x.EstablishmentNumber)
                .NotEmpty().WithMessage("Please check the number you've entered")
                .When(x => x.GenerateEstabNumber.HasValue && !x.GenerateEstabNumber.GetValueOrDefault());

            When(x => x.EstablishmentTypeId == 41, () =>
            {
                RuleFor(x => x.OpenDate)
                    .Must(x => x.IsValid() && x.IsNotEmpty())
                    .WithMessage("Please enter the date of opening");

                RuleFor(x => x.Address.Line1)
                    .Must(x => !string.IsNullOrWhiteSpace(x))
                    .WithMessage("Please enter the street");

                RuleFor(x => x.Address.CityOrTown)
                    .Must(x => !string.IsNullOrWhiteSpace(x))
                    .WithMessage("Please enter the town");

                RuleFor(x => x.Address.County)
                    .Must(x => x != null)
                    .WithMessage("Please select the country");

                RuleFor(x => x.Address.PostCode)
                    .Must(x => !string.IsNullOrWhiteSpace(x))
                    .WithMessage("Please enter the postcode");

                RuleFor(x => x.ManagerFirstName)
                    .Must(x => !string.IsNullOrWhiteSpace(x))
                    .WithMessage("Please enter the manager first name");

                RuleFor(x => x.ManagerLastName)
                    .Must(x => !string.IsNullOrWhiteSpace(x))
                    .WithMessage("Please enter the manager last name");

                RuleFor(x => x.CentreEmail)
                    .Must(x => !string.IsNullOrWhiteSpace(x))
                    .WithMessage("Please enter the manager email");

                RuleFor(x => x.Telephone)
                    .Must(x => !string.IsNullOrWhiteSpace(x))
                    .WithMessage("Please enter the telephone");

                RuleFor(x => x.OperationalHoursId)
                    .Must(x => x != null)
                    .WithMessage("Please select the operational hours");

                RuleFor(x => x.NumberOfUnderFives)
                    .Must(x => x != null)
                    .WithMessage("Please enter the number of under 5s");

                RuleFor(x => x.GovernanceId)
                    .Must(x => x != null)
                    .WithMessage("Please select the governance type");

                RuleFor(x => x.DisadvantagedAreaId)
                    .Must(x => x != null)
                    .WithMessage("Please select a disadvantaged area option");

                RuleFor(x => x.DirectProvisionOfEarlyYears)
                    .Must(x => x != null)
                    .WithMessage("Please select a direct provision of early years option");

                RuleFor(x => x.GovernanceDetail)
                    .Must(x => !string.IsNullOrWhiteSpace(x))
                    .WithMessage("Please enter governance detail");
            });

        }
    }
}