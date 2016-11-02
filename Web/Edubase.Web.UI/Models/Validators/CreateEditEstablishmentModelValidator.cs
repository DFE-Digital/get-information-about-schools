using Edubase.Services;
using FluentValidation;

namespace Edubase.Web.UI.Models.Validators
{
    public class CreateEditEstablishmentModelValidator : AbstractValidator<CreateEditEstablishmentModel>
    {
        public CreateEditEstablishmentModelValidator()
        {
            When(x => x.Action == CreateEditEstablishmentModel.eAction.Save, () =>
            {
                ConfigureRules();
                RuleSet("oncreate", () =>
                {
                    ConfigureRules();
                    RuleFor(x => x.OpenDate).Must(x => x.IsNotEmpty()).WithMessage("Please specify an Open Date");
                    RuleFor(x => x.ReasonEstablishmentOpenedId).NotEmpty().WithMessage("Reason opened should be specified");
                    RuleFor(x => x.EducationPhaseId).NotEmpty().WithMessage("Phase should be set");
                    ConfigureLAEstabRule();
                });
            });

            When(x => x.Action == CreateEditEstablishmentModel.eAction.AddLinkedSchool, () =>
            {
                RuleFor(x => x.LinkedDateToAdd).Must(x => x.IsValid()).When(x => x.LinkedDateToAdd.IsNotEmpty()).WithMessage("Linked date is invalid");
                RuleFor(x => x.LinkedDateToAdd).Must(x => x.IsNotEmpty()).WithMessage("Please specify a Linked date ");
                RuleFor(x => x.LinkTypeToAdd).NotNull().WithMessage("Please specify a Link Type");
            });
        }

        private void ConfigureRules()
        {
            RuleFor(x => x.OpenDate).Must(x => x.IsValid()).When(x => x.OpenDate.IsNotEmpty()).WithMessage("Open date is invalid");
            RuleFor(x => x.Name).NotEmpty().WithMessage("Name is invalid");
            RuleFor(x => x.LocalAuthorityId).NotEmpty().WithMessage("Local authority is invalid");
            RuleFor(x => x.TypeId).NotEmpty().WithMessage("Type is invalid");
            RuleFor(x => x.StatusId).NotEmpty().WithMessage("Status is invalid");
        }

        private void ConfigureLAEstabRule()
        {
            When(x => x.TypeId.HasValue && x.EducationPhaseId.HasValue, () =>
            {
                When(x => new EstablishmentService().GetEstabNumberEntryPolicy(x.TypeId.Value, x.EducationPhaseId.Value) == EstablishmentService.EstabNumberEntryPolicy.UserDefined, () =>
                {
                    RuleFor(x => x.LAESTAB)
                    .Cascade(CascadeMode.StopOnFirstFailure)
                    .NotEmpty()
                    .WithMessage("LAESTAB is invalid")
                    .Must((m, v) => v.ToString().StartsWith(m.LocalAuthorityId.ToString()))
                    .WithMessage("LAESTAB should be a numeric value of LA Code + Establishment Number")
                    .Must(x => x.ToString().Length == 7)
                    .WithMessage("The LAESTAB should be 7 characters long");
                });

                When(x => new EstablishmentService().GetEstabNumberEntryPolicy(x.TypeId.Value, x.EducationPhaseId.Value) == EstablishmentService.EstabNumberEntryPolicy.NonePermitted, () =>
                {
                    RuleFor(x => x.LAESTAB).Empty().WithMessage("The LAESTAB should be empty for this combination of phase and establishment type.");
                });
            });
        }
    }
}