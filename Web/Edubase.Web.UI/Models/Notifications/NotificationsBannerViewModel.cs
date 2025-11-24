using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using Edubase.Common;
using Edubase.Data.Entity;
using Edubase.Web.UI.Helpers;
using Edubase.Web.UI.Helpers.ValueProviders;

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
        [ValidateUrl(ErrorMessage = "The URL entered is invalid")]
        [RequireUrlNotification("LinkText1", ErrorMessage = "The URL link must be entered when the message is populated.")]
        public string LinkUrl1 { get; set; }
        [Display(Name = "Textbox for link 1")]
        public string LinkText1 { get; set; }
        [Display(Name = "Second link")]
        [ValidateUrl(ErrorMessage = "The URL entered is invalid")]
        [RequireUrlNotification("LinkText2", ErrorMessage = "The URL link must be entered when the message is populated.")]
        public string LinkUrl2 { get; set; }
        [Display(Name = "Textbox for link 2")]
        public string LinkText2 { get; set; }

 public NotificationBanner ToBanner(NotificationBanner originalBanner = null)
 {
     var newBanner = originalBanner ?? new NotificationBanner();

     newBanner.Importance = (int)Importance;

     newBanner.Start = DateTime.SpecifyKind(Start.ToDateTime().GetValueOrDefault(), DateTimeKind.Local);
     newBanner.End = DateTime.SpecifyKind(End.ToDateTime().GetValueOrDefault(), DateTimeKind.Local);

     var cleaned = Regex.Replace(Content ?? "", "<a .*?</a>", "", RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
     cleaned = Regex.Replace(cleaned, @"<br\s*/?>", "", RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));

     newBanner.Content = sanitizedContentWithLinks(cleaned.Trim(), LinkUrl1, LinkText1, LinkUrl2, LinkText2);

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

 public string sanitizedContentWithLinks(string content, string url1, string text1, string url2, string text2)
 {
     content = sanitizedText(content);

     List<string> formatted = new List<string> { content };

     if (!string.IsNullOrWhiteSpace(url1))
     {
         if (!string.IsNullOrWhiteSpace(text1))
         {
             formatted.Add($"<br /><a href=\"{sanitizedUrl(url1)}\" target=\"_blank\" rel=\"noopener noreferrer\">{sanitizedText(text1)}</a>");
         }
         else
         {
             formatted.Add($"<br />{sanitizedUrl(url1)}");
         }

     }
     if (!string.IsNullOrWhiteSpace(url2))
     {
         if (!string.IsNullOrWhiteSpace(text2))
         {
             formatted.Add($"<br /><a href=\"{sanitizedUrl(url2)}\" target=\"_blank\" rel=\"noopener noreferrer\">{sanitizedText(text2)}</a>");
         }
         else
         {
             formatted.Add($"<br />{sanitizedUrl(url2)}");
         }

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
            Start = new DateTimeViewModel(banner.Start, banner.Start);
            End = new DateTimeViewModel(banner.End, banner.End);

            var rawContent = banner.Content ?? "";

            var matches = Regex.Matches(rawContent, @"<a\s+href=""(.*?)"".*?>(.*?)</a>", RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));

            if (matches.Count >= 1)
            {
                LinkUrl1 = matches[0].Groups[1].Value;
                LinkText1 = matches[0].Groups[2].Value;
            }

            if (matches.Count >= 2)
            {
                LinkUrl2 = matches[1].Groups[1].Value;
                LinkText2 = matches[1].Groups[2].Value;
            }

            var contentWithoutLinks = Regex.Replace(rawContent, @"<a\s+href=""[^""]*""[^>]*>.*?</a>", "", RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
            var contentWithoutBreaks = Regex.Replace(contentWithoutLinks, @"<br\s*/?>", "", RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));

            Content = System.Net.WebUtility.HtmlDecode(contentWithoutBreaks.Trim());

            return this;
        }
    }
}
