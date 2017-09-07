using Edubase.Common;
using Microsoft.WindowsAzure.Storage.Table;
using System;

namespace Edubase.Data.Entity
{
    public class ApiRecorderSessionItem : TableEntity
    {
        public string HttpMethod { get; set; }
        public string Path { get; set; }
        public string RequestHeaders { get; set; }
        public string ResponseHeaders { get; set; }
        public string RawRequestBody { get; set; }
        public string RawResponseBody { get; set; }
        public string ElapsedTimeSpan { get; set; }
        /// <summary>
        /// Elapsed milliseconds
        /// </summary>
        public double ElapsedMS { get; set; }

        public ApiRecorderSessionItem(string sessionId, string requestPath)
        {
            PartitionKey = sessionId;
            RowKey = string.Concat(DateTime.UtcNow.ToString("yyyyMMdd-HHmmss"), "~~", requestPath.CleanOfNonChars(false, "-"), "~~", RandomNumber.Next(1, 100));
            Path = requestPath;
        }

        
    }
}
