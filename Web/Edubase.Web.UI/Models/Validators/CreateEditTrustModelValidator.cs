using FluentValidation;

namespace Edubase.Web.UI.Models.Validators
{
    public class CreateEditTrustModelValidator : AbstractValidator<CreateEditTrustModel>
    {
        public CreateEditTrustModelValidator()
        {
            RuleFor(x => x.OpenDate).Must(x => x.IsNotEmpty()).WithMessage("Please specify an Open Date");
            RuleFor(x => x.OpenDate).Must(x => x.IsValid()).When(x => x.OpenDate.IsNotEmpty()).WithMessage("Open date is invalid");
            RuleFor(x => x.Name).NotEmpty().WithMessage("Name is invalid");
            RuleFor(x => x.TypeId).NotEmpty().WithMessage("Type is invalid");
        }
        
    }
}