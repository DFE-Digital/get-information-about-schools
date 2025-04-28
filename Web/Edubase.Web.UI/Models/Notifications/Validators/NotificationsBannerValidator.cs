using System;
using FluentValidation;

namespace Edubase.Web.UI.Models.Notifications.Validators
{
    public class NotificationsBannerValidator : AbstractValidator<NotificationsBannerViewModel>
    {
        public NotificationsBannerValidator()
        {
            CascadeMode = CascadeMode.StopOnFirstFailure;

            RuleFor(x => x.Content)
                .NotNull().WithMessage("The Content field cannot be empty")
                .When(x => x.Action == eNotificationBannerAction.Message);

            When(x => x.Action == eNotificationBannerAction.Schedule, () =>
            {
                RuleFor(x => x.Start)
                    .Must(start =>
                    {
                        if (!start.Day.HasValue || !start.Month.HasValue || !start.Year.HasValue ||
                            !start.Hour.HasValue || !start.Minute.HasValue)
                        {
                            return true;
                        }
                        return start.ToDateTime() > DateTime.Now.ToLocalTime();
                    }).WithMessage("Start date and time must be in the future.");
            });

            When(x => x.Action == eNotificationBannerAction.Schedule, () =>
            {
                RuleFor(x => x.End)
                    .Must((model, end) =>
                    {
                        if (!model.Start.Day.HasValue || !model.Start.Month.HasValue || !model.Start.Year.HasValue ||
                            !model.Start.Hour.HasValue || !model.Start.Minute.HasValue ||
                            !end.Day.HasValue || !end.Month.HasValue || !end.Year.HasValue ||
                            !end.Hour.HasValue || !end.Minute.HasValue)
                        {
                            return true;
                        }

                        var startDate = new DateTime(model.Start.Year.Value, model.Start.Month.Value, model.Start.Day.Value,
                            model.Start.Hour.Value, model.Start.Minute.Value, 0);
                        var endDate = new DateTime(end.Year.Value, end.Month.Value, end.Day.Value,
                            end.Hour.Value, end.Minute.Value, 0);

                        return endDate > startDate;
                    })
                    .WithMessage("End date must be after the start date.")
                    .When(x => x.Action == eNotificationBannerAction.Schedule);
            });
        }
    }
}
