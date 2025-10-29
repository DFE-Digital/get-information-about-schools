using System;
using FluentValidation;

namespace Edubase.Web.UI.Areas.Establishments.Models.Validators;

public class BulkAssociateEstabs2GroupsViewModelValidator : AbstractValidator<BulkAssociateEstabs2GroupsViewModel>
{
    public BulkAssociateEstabs2GroupsViewModelValidator()
    {
        RuleFor(x => x.BulkFile)
            .NotNull().WithMessage("Please upload a file.")
            .DependentRules(() =>
            {
                RuleFor(x => x.BulkFile.ContentLength)
                    .GreaterThan(0).WithMessage("Please upload a file.");

                RuleFor(x => x.BulkFile.FileName)
                    .Must(name =>
                        name.EndsWith(".csv", StringComparison.OrdinalIgnoreCase) ||
                        name.EndsWith(".xls", StringComparison.OrdinalIgnoreCase))
                    .WithMessage("Please upload a CSV or XLS file.");
            });
    }
}
