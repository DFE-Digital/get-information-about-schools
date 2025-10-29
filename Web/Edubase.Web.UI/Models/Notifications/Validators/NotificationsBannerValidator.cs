using System;
using FluentValidation;

namespace Edubase.Web.UI.Models.Notifications.Validators;

public class NotificationsBannerValidator : AbstractValidator<NotificationsBannerViewModel>
{
    public NotificationsBannerValidator()
    {
        When(x => x.Action == eNotificationBannerAction.Message, () =>
        {
            RuleFor(x => x.Content)
                .NotNull()
                .WithMessage("The Content field cannot be empty");
        });

        When(x => x.Action == eNotificationBannerAction.Schedule, () =>
        {
            RuleFor(x => x.Start)
                .Must(s => s != null && s.IsValid())
                .WithMessage("Set a valid start date");

            RuleFor(x => x.Start)
                .Must(s => s.Hour.HasValue && s.Minute.HasValue)
                .WithMessage("Enter a start time");

            RuleFor(x => x.Start)
                .Must((model, start) =>
                    start.ToDateTime().HasValue &&
                    start.ToDateTime().Value > DateTime.Now &&
                    (!model.StartOriginal.HasValue || start.ToDateTime().Value != model.StartOriginal.Value))
                .WithMessage("Start date and start time must be in the future")
                .When(x => x.Start != null &&
                           x.Start.IsValid() &&
                           x.Start.Hour.HasValue &&
                           x.Start.Minute.HasValue);

            RuleFor(x => x.End)
                .Must(e => e != null && e.IsValid())
                .WithMessage("Set a valid end date");

            RuleFor(x => x.End)
                .Must(e => e.Hour.HasValue && e.Minute.HasValue)
                .WithMessage("Enter an end time");

            RuleFor(x => x)
                .Must(x =>
                {
                    var start = x.Start?.ToDateTime();
                    var end = x.End?.ToDateTime();
                    return start.HasValue && end.HasValue && end > start;
                })
                .WithMessage("End date must be after the start date")
                .When(x => x.Start != null && x.End != null &&
                           x.Start.IsValid() && x.End.IsValid() &&
                           x.Start.Hour.HasValue && x.Start.Minute.HasValue &&
                           x.End.Hour.HasValue && x.End.Minute.HasValue);
        });
    }
}
