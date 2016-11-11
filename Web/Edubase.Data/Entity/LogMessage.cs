using Edubase.Common;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Text;

namespace Edubase.Data.Entity
{
    public class LogMessage : TableEntity
    {
        public enum eLevel
        {
            Verbose,
            Information,
            Warning,
            Error
        }

        [IgnoreProperty]
        public string Id => string.Concat(RowKey, PartitionKey);
        public string Url { get; set; }
        public string ReferrerUrl { get; set; }
        public string ClientIPAddress { get; set; }
        public string HttpMethod { get; set; }
        public DateTime DateUtc { get; set; } = DateTime.UtcNow;
        public string Exception { get; set; }
        public string Text { get; set; }
        public string UserAgent { get; set; }
        public string Environment { get; set; }
        public eLevel Level { get; set; }
        public string UserName { get; set; }
        public string UserId { get; set; }

        /// <summary>
        /// Initialises with UtcNow as the partition key and DateUtc field, and generates a new Guid for the RowKey
        /// </summary>
        public LogMessage() : this(DateTime.UtcNow, Guid.NewGuid())
        {
        }

        public LogMessage(DateTime when, string text): this(when, Guid.NewGuid())
        {
            Text = text;
        }

        public LogMessage(string text) : this()
        {
            Text = text;
        }

        public LogMessage(Exception exception) : this()
        {
            Text = exception.GetBaseException().Message;
            Exception = exception.ToString();
        }

        public LogMessage(DateTime when, Guid id)
        {
            PartitionKey = CreatePartitionKey(when);
            RowKey = id.ToString("N");
            DateUtc = when;
        }

        public static string CreatePartitionKey(DateTime date) => date.ToString("yyyyMMdd");
        

        public override string ToString()
        {
            var sb = new StringBuilder();
            var props = ReflectionHelper.GetProperties(this);
            foreach (var prop in props)
            {
                var value = ReflectionHelper.GetProperty(this, prop);
                if (!value.IsNullOrEmpty()) sb.AppendLine($"{prop}: {value}");
            }
            return sb.ToString();
        }
    }
}
