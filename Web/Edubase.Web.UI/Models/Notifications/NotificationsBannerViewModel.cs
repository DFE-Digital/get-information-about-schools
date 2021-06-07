using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Edubase.Common;
using Edubase.Data.Entity;
using Edubase.Web.UI.Helpers;
using Edubase.Web.UI.Models.Validators;
using FluentValidation.Attributes;

namespace Edubase.Web.UI.Models.Notifications
{
    public enum eNotificationBannerAction
    {
        Step1,
        Step2,
        Step3,
        Step4,
        Step5,
        Step6
    }

    public class NotificationsBannerViewModel
    {
        public string Id { get; set; }
        public eNotificationBannerAction Action { get; set; }
        public eNotificationBannerImportance Importance { get; set; }
        public IEnumerable<NotificationTemplate> Templates { get; set; }
        public string TemplateSelected { get; set; }

        [MaxLength(500, ErrorMessage = "The Content field cannot have more than 500 characters"), AllowHtml]
        public string Content { get; set; }
        public DateTimeViewModel Start { get; set; }
        public DateTimeViewModel End { get; set; } 
        public bool Visible { get; set; }

        public NotificationBanner ToBanner(NotificationBanner originalBanner = null)
        {
            var newBanner = originalBanner ?? new NotificationBanner();

            newBanner.Visible = Visible;
            newBanner.Importance = (int) Importance;
            newBanner.Content = Content;
            newBanner.Start = Start.ToDateTime().GetValueOrDefault();
            newBanner.End = End.ToDateTime().GetValueOrDefault();

            return newBanner;
        }

        public int TotalBanners { get; set; }

        public NotificationsBannerViewModel()
        {
        }
        public NotificationsBannerViewModel(NotificationBanner banner)
        {
            Id = banner.RowKey;
            Visible = banner.Visible;
            Importance = (eNotificationBannerImportance)banner.Importance;
            Content = banner.Content;
            Start = new DateTimeViewModel(banner.Start, banner.Start);
            End = new DateTimeViewModel(banner.End, banner.End);
        }
    }
}
