using FluentValidation;

namespace Edubase.Web.UI.Models.Validators
{
    public class CreateEditGroupModelValidator : AbstractValidator<CreateEditGroupModel>
    {
        public CreateEditGroupModelValidator()
        {
            When(x => x.Action.Equals("save", System.StringComparison.OrdinalIgnoreCase), () =>
            {
                ConfigureRules();
                RuleSet("oncreate", () =>
                {
                    RuleFor(x => x.OpenDate).Must(x => x.IsNotEmpty()).WithMessage("Please specify an Open Date");
                    ConfigureRules();
                });
            });
        }

        private void ConfigureRules()
        {
            RuleFor(x => x.OpenDate).Must(x => x.IsValid()).When(x => x.OpenDate.IsNotEmpty()).WithMessage("Open date is invalid");
            RuleFor(x => x.Name).NotEmpty().WithMessage("Name is invalid");
            RuleFor(x => x.TypeId).NotEmpty().WithMessage("Type is invalid");
            RuleFor(x => x.CompaniesHouseNumber).NotEmpty().WithMessage("Companies house number is invalid");
        }
    }
}