using System;

namespace Edubase.Data.Repositories.EF
{
    public class SqlNotificationBanner
    {
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public byte Importance { get; set; }
        public string Content { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public byte Version { get; set; }
        public string Tracker { get; set; }
        public int AuditUser { get; set; }
        public string AuditEvent { get; set; }
        public DateTime AuditTimestamp { get; set; }
    }
}
