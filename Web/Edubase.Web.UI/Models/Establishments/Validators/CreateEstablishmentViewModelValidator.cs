using Edubase.Web.UI.Validation;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Edubase.Web.UI.Models.Establishments.Validators
{
    public class CreateEstablishmentViewModelValidator : EdubaseAbstractValidator<CreateEstablishmentViewModel>
    {
        public CreateEstablishmentViewModelValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("Please enter an establishment name");
            RuleFor(x => x.LocalAuthorityId).NotEmpty().WithMessage("Please select a local authority");
            RuleFor(x => x.EducationPhaseId).NotEmpty().WithMessage("Please select a a phase of education");
            RuleFor(x => x.EstablishmentTypeId).NotEmpty().WithMessage("Please select an establishment type");
            RuleFor(x => x.GenerateEstabNumber).NotNull().WithMessage("Please select to enter a number or have one generated for you");

            RuleFor(x => x.EstablishmentNumber)
                .NotEmpty().WithMessage("Please check the number you've entered")
                .When(x => x.GenerateEstabNumber.HasValue && !x.GenerateEstabNumber.GetValueOrDefault());
        }
    }
}