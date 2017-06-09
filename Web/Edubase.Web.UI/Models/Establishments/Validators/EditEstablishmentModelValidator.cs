using Edubase.Common;
using Edubase.Services.Lookup;
using Edubase.Web.UI.Validation;
using FluentValidation;
using System.Linq;
using Edubase.Services.Enums;
using Edubase.Services.Establishments;
using Edubase.Services;

namespace Edubase.Web.UI.Models.Validators
{
    public class EditEstablishmentModelValidator : EdubaseAbstractValidator<EditEstablishmentModel>
    {
        private ICachedLookupService _lookupService;

        public EditEstablishmentModelValidator(ICachedLookupService lookupService, IEstablishmentReadService establishmentReadService)
        {
            _lookupService = lookupService;

            When(x => x.Action == EditEstablishmentModel.eAction.SaveDetails, () =>
            {
                ConfigureRules();

                RuleFor(x => x.EducationPhaseId)
                    .Cascade(CascadeMode.StopOnFirstFailure)
                    .NotEmpty().WithMessage("Please select a a phase of education")
                    .Must((m, x) => (establishmentReadService.GetEstabType2EducationPhaseMap().AsInts()[m.TypeId.Value]).Contains(x.Value))
                    .WithMessage("Education phase is not valid for the selected type of establishment");
            });

            When(x => x.Action == EditEstablishmentModel.eAction.SaveLocation, () =>
            {
                RuleFor(x => x.LSOACode).MustAsync(async (x, ct) => (await _lookupService.LSOAsGetAllAsync()).FirstOrDefault(l => l.Code == x) != null)
                    .When(x => !x.LSOACode.IsNullOrEmpty()).WithMessage("Area not found, please enter a valid area code").WithSummaryMessage("Area not found for Middle Super Output Area (MSOA)");

                RuleFor(x => x.MSOACode).MustAsync(async (x, ct) => (await _lookupService.MSOAsGetAllAsync()).FirstOrDefault(l => l.Code == x) != null)
                    .When(x => !x.MSOACode.IsNullOrEmpty()).WithMessage("Area not found, please enter a valid area code").WithSummaryMessage("Area not found for Lower Super Output Area (LSOA)");
            });
        }

        private void ConfigureRules()
        {
            RuleFor(x => x.OpenDate).Must(x => x.IsValid()).When(x => x.OpenDate.IsNotEmpty()).WithMessage("Open date is invalid");
            RuleFor(x => x.Name).NotEmpty().WithMessage("Name is invalid");
            RuleFor(x => x.LocalAuthorityId).NotEmpty().WithMessage("Local authority is invalid");
            RuleFor(x => x.TypeId).NotEmpty().WithMessage("Type is invalid");
            RuleFor(x => x.StatusId).NotEmpty().WithMessage("Status is invalid");

            RuleFor(x => x.HeadFirstName)
                .Must(x => !string.IsNullOrWhiteSpace(x))
                .WithMessage("Please enter the headteacher/principal's first name")
                .WithSummaryMessage("You have not entered the headteacher / principal's first name")
                .When(x => EnumSets.AcademiesAndFreeSchools.Contains(x.TypeId.Value) && !string.IsNullOrWhiteSpace(x.OldHeadFirstName));

            RuleFor(x => x.HeadLastName)
                .Must(x => !string.IsNullOrWhiteSpace(x))
                .WithMessage("Please enter the headteacher/principal's last name")
                .WithSummaryMessage("You have not entered the headteacher / principal's last name")
                .When(x => EnumSets.AcademiesAndFreeSchools.Contains(x.TypeId.Value) && !string.IsNullOrWhiteSpace(x.OldHeadLastName));
        }
    }
}