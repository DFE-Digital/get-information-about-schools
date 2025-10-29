using Edubase.Services.Establishments;
using FluentValidation;

namespace Edubase.Web.UI.Areas.Establishments.Models.Validators;

public class CreateEstablishmentViewModelValidator : AbstractValidator<CreateEstablishmentViewModel>
{
    public CreateEstablishmentViewModelValidator(IEstablishmentReadService establishmentReadService)
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Please enter an establishment name");

        RuleFor(x => x.LocalAuthorityId)
            .NotEmpty()
            .WithMessage("Please select a local authority");

        RuleFor(x => x.EstablishmentTypeId)
            .NotEmpty()
            .WithMessage("Please select an establishment type");

        RuleFor(x => x.GenerateEstabNumber)
            .NotNull()
            .WithMessage("Please select 'Generate number' or 'Enter number'");

        RuleFor(x => x.EstablishmentNumber)
            .NotEmpty()
            .WithMessage("Please enter an establishment number")
            .When(x => x.GenerateEstabNumber.HasValue && !x.GenerateEstabNumber.GetValueOrDefault());
    }
}
