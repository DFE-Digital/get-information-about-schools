using System;
using FluentValidation;

namespace Edubase.Web.UI.Models.DataQuality.Validators;

public class DataQualityStatusItemValidator : AbstractValidator<DataQualityStatusItem>
{
    public DataQualityStatusItemValidator()
    {
        RuleFor(x => x.LastUpdated)
            .NotEmpty().WithMessage("Please correct the date")
            .Must(x => x.IsValid()).WithMessage("Please correct the date")
            .Must(x => x.ToDateTime().HasValue && x.ToDateTime().Value <= DateTime.Now.Date)
            .WithMessage("Please correct the date");
    }
}
