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
                .Must((m, x) => (establishmentReadService.GetEstabType2EducationPhaseMap().AsInts()[m.EstablishmentTypeId.Value]).Contains(x.Value))
                .WithMessage("Education phase is not valid for the selected type of establishment");

            RuleFor(x => x.EstablishmentTypeId).NotEmpty().WithMessage("Please select an establishment type");
            RuleFor(x => x.GenerateEstabNumber).NotNull().WithMessage("Please select to enter a number or have one generated for you");

            RuleFor(x => x.EstablishmentNumber)
                .NotEmpty().WithMessage("Please check the number you've entered")
                .When(x => x.GenerateEstabNumber.HasValue && !x.GenerateEstabNumber.GetValueOrDefault());
        }
    }
}