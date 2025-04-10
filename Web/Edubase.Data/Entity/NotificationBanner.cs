using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.WindowsAzure.Storage.Table;

namespace Edubase.Data.Entity
{
    public enum eNotificationBannerPartition
    {
        Current,
        Archive
    }

    public enum eNotificationBannerEvent
    {
        [Display(Name = "Created")]
        Create,
        [Display(Name = "Updated")]
        Update,
        [Display(Name = "Deleted")]
        Delete
    }

    public enum eNotificationBannerStatus
    {
        [Display(Name = "Active")]
        Live,
        [Display(Name = "Future")]
        Future,
        [Display(Name = "Expired")]
        Expired
    }

    public class NotificationBanner : TableEntity
    {
        public int Importance { get; set; }
        public string Content { get; set; }

        private DateTime _start;
        public DateTime Start
        {
            get => _start.ToLocalTime(); set => _start = value;
        }

        private DateTime _end;
        public DateTime End
        {
            get => _end.ToLocalTime(); set => _end = value;
        }

        public int Version { get; set; } = 1;
        public string Tracker { get; set; }
        public string AuditUser { get; set; }
        public string AuditEvent { get; set; }
        public DateTime AuditTimestamp { get; set; }

        public NotificationBanner()
        {
            PartitionKey = eNotificationBannerPartition.Current.ToString();
            RowKey = Guid.NewGuid().ToString("N").Substring(0, 8);
            Tracker = RowKey;
        }

        [IgnoreProperty]
        public bool Visible => Status == eNotificationBannerStatus.Live;

        public eNotificationBannerStatus Status
        {
            get
            {
                if (Start <= DateTime.Now && End > DateTime.Now)
                {
                    return eNotificationBannerStatus.Live;
                }
                else if (Start > DateTime.Now)
                {
                    return eNotificationBannerStatus.Future;
                }

                return eNotificationBannerStatus.Expired;
            }
        }

        [Display(Name = "First link")]
        public string LinkUrl1 { get; set; }
        [Display(Name = "Textbox for link 1")]
        public string LinkText1 { get; set; }
        [Display(Name = "Second link")]
        public string LinkUrl2 { get; set; }
        [Display(Name = "Textbox for link 2")]
        public string LinkText2 { get; set; }
    }
}
