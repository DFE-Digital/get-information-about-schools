using FluentValidation;

namespace Edubase.Web.UI.Models.Validators;

/// <summary>
/// Validates whether the date specified is valid (can be empty).
/// </summary>
public class DateTimeViewModelValidator : AbstractValidator<DateTimeViewModel>
{
    public DateTimeViewModelValidator()
    {
        RuleFor(x => x).Must(x => x.IsEmpty() || x.IsValid()).WithMessage("The date specified is not valid");
    }
}
