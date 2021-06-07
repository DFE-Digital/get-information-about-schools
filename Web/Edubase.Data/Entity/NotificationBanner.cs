using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Edubase.Common;
using Microsoft.WindowsAzure.Storage.Table;

namespace Edubase.Data.Entity
{
    public enum eNotificationBannerPartition
    {
        Current,
        Archive
    }
    public class NotificationBanner : TableEntity
    {
        public int Importance { get; set; }
        public string Content { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public bool Visible { get; set; }
        public int Version { get; set; } = 1;
        public string Tracker { get; set; }

        public NotificationBanner()
        {
            PartitionKey = eNotificationBannerPartition.Current.ToString();
            RowKey = Guid.NewGuid().ToString("N").Substring(0, 8);
            Tracker = RowKey;
        }
    }
}
