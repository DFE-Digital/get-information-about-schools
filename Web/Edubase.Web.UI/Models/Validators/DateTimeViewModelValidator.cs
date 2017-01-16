using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Edubase.Web.UI.Models.Validators
{
    public class DateTimeViewModelValidator : AbstractValidator<DateTimeViewModel>
    {
        public DateTimeViewModelValidator()
        {
            RuleFor(x => x).Must(x => x.IsValid()).WithMessage("The date specified is not valid");
        }
    }
}