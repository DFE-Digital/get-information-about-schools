using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.WindowsAzure.Storage.Table;

namespace Edubase.Data.Entity
{
    public enum eNewsArticlePartition
    {
        Current,
        Archive
    }

    public enum eNewsArticleEvent
    {
        [Display(Name = "Created")]
        Create,
        [Display(Name = "Updated")]
        Update,
        [Display(Name = "Deleted")]
        Delete
    }

    public enum eNewsArticleStatus
    {
        [Display(Name = "Active")]
        Live,
        [Display(Name = "Future")]
        Future
    }

    public class NewsArticle : TableEntity
    {
        public string Title { get; set; }
       
        private DateTime _articleDate;
        public DateTime ArticleDate
        {
            get => _articleDate.ToLocalTime(); set => _articleDate = value;
        }
        public bool ShowDate { get; set; } = true;
        public string Content { get; set; }

        public int Version { get; set; } = 1;
        public string Tracker { get; set; }
        public string AuditUser { get; set; }
        public string AuditEvent { get; set; }
        public DateTime AuditTimestamp { get; set; }

        public NewsArticle()
        {
            PartitionKey = eNewsArticlePartition.Current.ToString();
            RowKey = Guid.NewGuid().ToString("N").Substring(0, 8);
            Tracker = RowKey;
        }

        [IgnoreProperty]
        public bool Visible => Status == eNewsArticleStatus.Live;

        public eNewsArticleStatus Status =>
            ArticleDate <= DateTime.Now ?
                (eNewsArticleStatus) eNewsArticleStatus.Live :
                (eNewsArticleStatus) eNewsArticleStatus.Future;
    }
}
