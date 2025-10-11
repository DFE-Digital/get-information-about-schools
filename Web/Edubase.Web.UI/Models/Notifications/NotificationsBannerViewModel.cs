using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Edubase.Common;
using Edubase.Data.Entity;

namespace Edubase.Web.UI.Models.Notifications
{
    public enum eNotificationBannerAction
    {
        Start,
        TypeChoice,
        Message,
        Schedule,
        Review
    }

    public class NotificationsBannerViewModel
    {
        public string Id { get; set; }
        public int Counter { get; set; }
        public eNotificationBannerAction Action { get; set; }
        public eNotificationBannerImportance Importance { get; set; }
        public IEnumerable<NotificationTemplate> Templates { get; set; }
        public string TemplateSelected { get; set; }

        [MaxLength(500, ErrorMessage = "The Content field cannot have more than 500 characters")]
        public string Content { get; set; }
        public DateTimeViewModel Start { get; set; }
        public DateTime? StartOriginal { get; set; }
        public DateTimeViewModel End { get; set; }
        public bool GoBack { get; set; }

        public NotificationBanner ToBanner(NotificationBanner originalBanner = null)
        {
            var newBanner = originalBanner ?? new NotificationBanner();

            newBanner.Importance = (int) Importance;
            newBanner.Content = Content;
            newBanner.Start = DateTime.SpecifyKind(Start.ToDateTime().GetValueOrDefault(), DateTimeKind.Local);
            newBanner.End = DateTime.SpecifyKind(End.ToDateTime().GetValueOrDefault(), DateTimeKind.Local);

            return newBanner;
        }

        public int TotalBanners { get; set; }
        public int TotalLiveBanners { get; set; }

        public NotificationsBannerViewModel()
        {
        }
        public NotificationsBannerViewModel Set(NotificationBanner banner)
        {
            Id = banner.RowKey;
            Importance = (eNotificationBannerImportance)banner.Importance;
            Content = banner.Content;
            Start = new DateTimeViewModel(banner.Start, banner.Start);
            End = new DateTimeViewModel(banner.End, banner.End);

            return this;
        }
    }
}
