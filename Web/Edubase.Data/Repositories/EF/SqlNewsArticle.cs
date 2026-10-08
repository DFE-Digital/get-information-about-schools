using System;

namespace Edubase.Data.Repositories.EF
{
    public class SqlNewsArticle
    {
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public string Title { get; set; }
        public DateTime ArticleDate { get; set; }
        public bool ShowDate { get; set; }
        public string Content { get; set; }
        public byte Version { get; set; }
        public string Tracker { get; set; }
        public int AuditUser { get; set; }
        public string AuditEvent { get; set; }
        public DateTime AuditTimestamp { get; set; }
    }
}
