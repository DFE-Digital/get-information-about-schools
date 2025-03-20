using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using System.Web.Mvc;
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

        [MaxLength(500, ErrorMessage = "The Content field cannot have more than 500 characters"), AllowHtml]
        public string Content { get; set; }
        public DateTimeViewModel Start { get; set; }
        public DateTime? StartOriginal { get; set; }
        public DateTimeViewModel End { get; set; }
        public bool GoBack { get; set; }

        [Display(Name = "First link")]
        public string LinkUrl1 { get; set; }
        [Display(Name = "Textbox for link 1")]
        public string LinkText1 { get; set; }
        [Display(Name = "Second link")]
        public string LinkUrl2 { get; set; }
        [Display(Name = "Textbox for link 2")]
        public string LinkText2 { get; set; }

 public NotificationBanner ToBanner(NotificationBanner originalBanner = null)
 {
     var newBanner = originalBanner ?? new NotificationBanner();

     newBanner.Importance = (int)Importance;

     newBanner.Start = DateTime.SpecifyKind(Start.ToDateTime().GetValueOrDefault(), DateTimeKind.Local);
     newBanner.End = DateTime.SpecifyKind(End.ToDateTime().GetValueOrDefault(), DateTimeKind.Local);

     var sanitizedMessage = Regex.Replace(Content ?? "", "<a .*?</a>", "").Trim();

     newBanner.Content = sanitizedContentWithLinks(sanitizedMessage, LinkUrl1, LinkText1, LinkUrl2, LinkText2);

     return newBanner;
 }

 private string sanitizedUrl(string url)
 {
     return Uri.TryCreate(url, UriKind.Absolute, out var validUrl) ? validUrl.ToString() : "";
 }

 private string sanitizedText(string text)
 {
     return string.IsNullOrWhiteSpace(text) ? "" : text.Replace("<", "&lt;").Replace(">", "&gt;");
 }

 private string sanitizedContentWithLinks(string content, string url1, string text1, string url2, string text2)
 {
     content = sanitizedText(content);

     List<string> formatted = new List<string> { content };

     if (!string.IsNullOrWhiteSpace(url1) && !string.IsNullOrWhiteSpace(text1))
     {
         formatted.Add($"<a href=\"{sanitizedUrl(url1)}\" target=\"_blank\" rel=\"noopener noreferrer\">{sanitizedText(text1)}</a>");
     }
     if (!string.IsNullOrWhiteSpace(url2) && !string.IsNullOrWhiteSpace(text2))
     {
         formatted.Add($"<a href=\"{sanitizedUrl(url2)}\" target=\"_blank\" rel=\"noopener noreferrer\">{sanitizedText(text2)}</a>");
     }
     return string.Join(" ", formatted);
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
