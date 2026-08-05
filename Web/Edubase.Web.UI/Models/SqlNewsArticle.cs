using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Edubase.Web.UI.Models
{
    [Table("NewsArticles", Schema = "FrontEnd")]
    public class SqlNewsArticle
    {
        public SqlNewsArticle() { }


        [Key, Column(Order = 0)]
        public string PartitionKey { get; set; }
        [Key, Column(Order = 1)]
        public string RowKey { get; set; }

        public string Title { get; set; }
        public DateTime ArticleDate { get; set; }
        public bool ShowDate { get; set; }
        public string Content { get; set; }
        public byte Version { get; set; }
        public string Tracker { get; set; }
        public int AuditUser { get; set; }
        public string AuditEvent { get; set; }
        public DateTime AuditTimeStamp { get; set; }

    }
}
