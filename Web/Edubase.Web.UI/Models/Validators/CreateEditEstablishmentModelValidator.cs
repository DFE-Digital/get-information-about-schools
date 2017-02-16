using Edubase.Common;
using Edubase.Services.Lookup;
using Edubase.Web.UI.Validation;
using FluentValidation;
using System.Linq;

namespace Edubase.Web.UI.Models.Validators
{
    public class CreateEditEstablishmentModelValidator : EdubaseAbstractValidator<EditEstablishmentModel>
    {
        private ICachedLookupService _lookupService;

        public CreateEditEstablishmentModelValidator(ICachedLookupService lookupService)
        {
            _lookupService = lookupService;

            When(x => x.Action == EditEstablishmentModel.eAction.Save, () =>
            {
                ConfigureRules();

                RuleSet("oncreate", () =>
                {
                    ConfigureRules();
                    RuleFor(x => x.OpenDate).Must(x => x.IsNotEmpty()).WithMessage("Please specify an Open Date");
                    RuleFor(x => x.ReasonEstablishmentOpenedId).NotEmpty().WithMessage("Reason opened should be specified");
                    RuleFor(x => x.EducationPhaseId).NotEmpty().WithMessage("Phase should be set");
                });
            });

            When(x => x.Action == EditEstablishmentModel.eAction.AddLinkedSchool, () =>
            {
                RuleFor(x => x.LinkedDateToAdd).Must(x => x.IsValid()).When(x => x.LinkedDateToAdd.IsNotEmpty()).WithMessage("Linked date is invalid");
                RuleFor(x => x.LinkedDateToAdd).Must(x => x.IsNotEmpty()).WithMessage("Please specify a Linked date");
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

            RuleFor(x => x.LSOACode).MustAsync(async (x, ct) => (await _lookupService.LSOAsGetAllAsync()).FirstOrDefault(l => l.Code == x) != null)
                .When(x => !x.LSOACode.IsNullOrEmpty()).WithMessage("Area not found, please enter a valid area code").WithSummaryMessage("Area not found for Middle Super Output Area (MSOA)");

            RuleFor(x => x.MSOACode).MustAsync(async (x, ct) => (await _lookupService.MSOAsGetAllAsync()).FirstOrDefault(l => l.Code == x) != null)
                .When(x => !x.MSOACode.IsNullOrEmpty()).WithMessage("Area not found, please enter a valid area code").WithSummaryMessage("Area not found for Lower Super Output Area (LSOA)");
            
        }
        

    }
}