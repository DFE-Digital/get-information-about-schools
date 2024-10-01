using Edubase.Common;
using Microsoft.WindowsAzure.Storage.Table;
using System;

namespace Edubase.Data.Entity
{
    public class AZTLoggerMessages : TableEntity
    {
        public DateTime DateUtc { get; set; }

        public string Environment { get; set; }

        public string Exception { get; set; }


        public string Level { get; set; }

        public string Message { get; set; }

        public string Id => this.RowKey + this.PartitionKey;

        public string ClientIpAddress { get; set; }

        public string HttpMethod { get; set; }

        public string ReferrerUrl { get; set; }

        public string RequestJsonBody { get; set; }

        public string Url { get; set; }

        public string UserAgent { get; set; }

        public string UserId { get; set; }

        public string UserName { get; set; }

        public string Severity { get; set; }
    }
}
