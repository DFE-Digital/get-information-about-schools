using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Edubase.Web.UI.Models
{
    [Table("NotificationBanners", Schema = "FrontEnd")]
    public class SqlNotificationBanner
    {
        public SqlNotificationBanner() { }


        [Key, Column(Order = 0)]
        public string PartitionKey { get; set; }
        [Key, Column(Order = 1)]
        public string RowKey { get; set; }
        public byte Importance { get; set; }
        public string Content { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public byte Version { get; set; }
        public string Tracker { get; set; }
        public int AuditUser { get; set; }
        public string AuditEvent { get; set; }
        public DateTime AuditTimeStamp { get; set; }
    }
}
