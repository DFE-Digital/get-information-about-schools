using System;
using FluentValidation;

namespace Edubase.Web.UI.Areas.Establishments.Models.Validators;

public class BulkCreateFreeSchoolsViewModelValidator : AbstractValidator<BulkCreateFreeSchoolsViewModel>
{
    public BulkCreateFreeSchoolsViewModelValidator()
    {
        RuleFor(x => x.BulkFile)
            .NotNull().WithMessage("Please upload a file.")
            .DependentRules(() =>
            {
                RuleFor(x => x.BulkFile.ContentLength)
                    .GreaterThan(0).WithMessage("Please upload a file.");

                RuleFor(x => x.BulkFile.FileName)
                    .Must(name => name.EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase))
                    .WithMessage("Please upload an XLSX file.");
            });
    }
}
