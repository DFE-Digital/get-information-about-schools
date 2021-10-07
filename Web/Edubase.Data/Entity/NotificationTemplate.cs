using System;
using Microsoft.WindowsAzure.Storage.Table;

namespace Edubase.Data.Entity
{
    public class NotificationTemplate : TableEntity
    {
        public string Content { get; set; }

        public NotificationTemplate()
        {
            PartitionKey = string.Empty;
            RowKey = Guid.NewGuid().ToString("N").Substring(0, 8);
        }
    }
}
