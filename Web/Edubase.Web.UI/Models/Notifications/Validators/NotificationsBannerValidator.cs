using System;
using System.Collections.Generic;
using System.Linq;
using FluentValidation;

namespace Edubase.Web.UI.Models.Notifications.Validators
{
    public class NotificationsBannerValidator : AbstractValidator<NotificationsBannerViewModel>
    {
        public NotificationsBannerValidator()
        {
            RuleFor(x => x.Content)
                .NotNull().WithMessage("The Content field cannot be empty")
                .When(x => x.Action == eNotificationBannerAction.Step4);

            RuleFor(x => x.Start)
                .Must(x => x.IsValid()).WithMessage("Set a valid date")
                .When(x => x.Action == eNotificationBannerAction.Step5);

            RuleFor(x => x.End)
                .Must(x => x.IsValid()).WithMessage("Set a valid date")
                .When(x => x.Action == eNotificationBannerAction.Step5);
        }
    }
}
