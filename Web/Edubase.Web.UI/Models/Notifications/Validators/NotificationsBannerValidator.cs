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
                .Must(x => x.IsValid()).WithMessage("Set a valid date")
                .When(x => x.Action == eNotificationBannerAction.Schedule)
                .Must(x => x.ToDateTime().GetValueOrDefault() > DateTime.Now.ToLocalTime()).WithMessage("Start date and time must be in the future")
                .When(x => x.Action == eNotificationBannerAction.Schedule && (x.Id == null || x.StartOriginal.GetValueOrDefault() != x.Start.ToDateTime()));

            RuleFor(x => x.End)
                .Must(x => x.IsValid()).WithMessage("Set a valid date")
                .Must((v, dt) => v.End.ToDateTime().GetValueOrDefault() > v.Start.ToDateTime().GetValueOrDefault()).WithMessage("End date must be after the start date")
                .When(x => x.Action == eNotificationBannerAction.Schedule);
        }
    }
}
