using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Azure;
using Azure.Data.Tables;

namespace Edubase.Data.Entity;

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

public class NewsArticle : ITableEntity
{
    public string PartitionKey { get; set; } = eNewsArticlePartition.Current.ToString();
    public string RowKey { get; set; } = Guid.NewGuid().ToString("N")[..8];
    public DateTimeOffset? Timestamp { get; set; }
    public ETag ETag { get; set; }

    public string Title { get; set; }

    private DateTime _articleDate;
    public DateTime ArticleDate
    {
        get => _articleDate.ToLocalTime();
        set => _articleDate = value;
    }

    public bool ShowDate { get; set; } = true;
    public string Content { get; set; }

    public int Version { get; set; } = 1;
    public string Tracker { get; set; } = Guid.NewGuid().ToString("N")[..8];
    public string AuditUser { get; set; }
    public string AuditEvent { get; set; }
    public DateTime AuditTimestamp { get; set; }

    [IgnoreDataMember]
    public bool Visible => Status == eNewsArticleStatus.Live;

    [IgnoreDataMember]
    public eNewsArticleStatus Status =>
        ArticleDate <= DateTime.Now
            ? eNewsArticleStatus.Live
            : eNewsArticleStatus.Future;
}
