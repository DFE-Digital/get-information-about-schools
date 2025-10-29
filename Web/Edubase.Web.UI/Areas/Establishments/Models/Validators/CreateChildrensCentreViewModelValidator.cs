using Edubase.Common;
using Edubase.Services.Enums;
using Edubase.Services.Establishments;
using Edubase.Web.UI.Areas.Establishments.Models.Validators.Extensions;
using FluentValidation;
using CreateSteps = Edubase.Web.UI.Areas.Establishments.Models.CreateEstablishmentViewModel.eEstabCreateSteps;

namespace Edubase.Web.UI.Areas.Establishments.Models.Validators;

public class CreateChildrensCentreViewModelValidator : AbstractValidator<CreateChildrensCentreViewModel>
{
    public CreateChildrensCentreViewModelValidator(IEstablishmentReadService establishmentReadService)
    {
        When(x => x.ActionStep == CreateSteps.PhaseOfEducation, () =>
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Please enter an establishment name");

            RuleFor(x => x.LocalAuthorityId)
                .NotEmpty().WithMessage("Please select a local authority");

            RuleFor(x => x.EstablishmentTypeId)
                .NotEmpty().WithMessage("Please select an establishment type");

            RuleFor(x => x.LocalAuthorityId)
                .Must(x => x == 153)
                .WithMessage("Please select 'Does not apply'")
                .When(x => x.EstablishmentTypeId.OneOfThese(
                    eLookupEstablishmentType.BritishSchoolsOverseas,
                    eLookupEstablishmentType.OnlineProvider));
        });

        When(x => x.ActionStep == CreateSteps.EstabNumber && x.EstablishmentTypeId != 41, () =>
        {
            RuleFor(x => x.EducationPhaseId)
                .Cascade(CascadeMode.Stop)
                .NotNull().WithMessage("Please select a phase of education")
                .NotEmpty().WithMessage("Please select a phase of education")
                .Must((model, phaseId) =>
                {
                    var map = establishmentReadService.GetEstabType2EducationPhaseMap().AsInts();
                    return model.EstablishmentTypeId.HasValue &&
                           map.ContainsKey(model.EstablishmentTypeId.Value) &&
                           map[model.EstablishmentTypeId.Value].Contains(phaseId.Value);
                })
                .WithMessage("Education phase is not valid for the selected type of establishment");

            RuleFor(x => x.GenerateEstabNumber)
                .NotNull().WithMessage("Please select 'Generate number' or 'Enter number'");
        });

        When(x => x.ActionStep == CreateSteps.Completed && x.EstablishmentTypeId != 41, () =>
        {
            RuleFor(x => x.EstablishmentNumber)
                .NotEmpty().WithMessage("Please enter an establishment number")
                .When(x => x.GenerateEstabNumber.HasValue && !x.GenerateEstabNumber.GetValueOrDefault());
        });

        When(x => x.ActionStep == CreateSteps.Completed && x.EstablishmentTypeId == 41, () =>
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
                .NotNull().WithMessage("Please select the county");

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
                .NotNull().WithMessage("Please select the operational hours");

            RuleFor(x => x.NumberOfUnderFives)
                .NotNull().WithMessage("Please enter the number of under 5s");

            RuleFor(x => x.GovernanceId)
                .NotNull().WithMessage("Please select the governance type");

            RuleFor(x => x.DisadvantagedAreaId)
                .NotNull().WithMessage("Please select a disadvantaged area option");

            RuleFor(x => x.DirectProvisionOfEarlyYears)
                .NotNull().WithMessage("Please select a direct provision of early years option");

            RuleFor(x => x.GovernanceDetail)
                .Must(x => !string.IsNullOrWhiteSpace(x))
                .WithMessage("Please enter governance detail");

            RuleFor(x => x.EstablishmentStatusId)
                .NotNull().WithMessage("Please enter establishment status");
        });
    }
}
