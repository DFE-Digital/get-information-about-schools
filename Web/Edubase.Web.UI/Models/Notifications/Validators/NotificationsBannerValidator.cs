using System;
using FluentValidation;

namespace Edubase.Web.UI.Models.Notifications.Validators
{
    public class NotificationsBannerValidator : AbstractValidator<NotificationsBannerViewModel>
{
    public NotificationsBannerValidator()
    {
        RuleFor(x => x.Content)
            .NotNull().WithMessage("The Content field cannot be empty")
            .When(x => x.Action == eNotificationBannerAction.Message);

        RuleFor(x => x.Start)
            .Must(s => s.IsValid()).WithMessage("Set a valid start date")
            .Must(s => s.Hour.HasValue && s.Minute.HasValue).WithMessage("Enter a start time")
            .When(x => x.Action == eNotificationBannerAction.Schedule);

        RuleFor(x => x.Start)
            .Must(s => s.ToDateTime().HasValue && s.ToDateTime().Value > DateTime.Now)
            .WithMessage("Start date and start time must be in the future")
            .When(x => x.Action == eNotificationBannerAction.Schedule &&
                       x.Start != null &&
                       x.Start.IsValid() &&
                       x.Start.Hour.HasValue &&
                       x.Start.Minute.HasValue);

        RuleFor(x => x.End)
            .Must(e => e.IsValid()).WithMessage("Set a valid end date")
            .Must(e => e.Hour.HasValue && e.Minute.HasValue).WithMessage("Enter an end time")
            .When(x => x.Action == eNotificationBannerAction.Schedule);

        RuleFor(x => x)
            .Must(x =>
            {
                var start = x.Start?.ToDateTime();
                var end = x.End?.ToDateTime();
                return start.HasValue && end.HasValue && end > start;
            })
            .WithMessage("End date must be after the start date")
            .When(x => x.Action == eNotificationBannerAction.Schedule &&
                       x.Start != null && x.End != null &&
                       x.Start.IsValid() && x.End.IsValid() &&
                       x.Start.Hour.HasValue && x.Start.Minute.HasValue &&
                       x.End.Hour.HasValue && x.End.Minute.HasValue);
    }
}
}
