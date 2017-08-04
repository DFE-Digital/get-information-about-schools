using System;

namespace Edubase.Services.Texuna.Glimpse
{
    public class ApiTraceData
    {
        public DateTime StartTime { get; set; }
        public string Method { get; set; }
        public string Url { get; set; }
        public int ResponseCode { get; set; }
        public string Request { get; set; }
        public string Response { get; set; }
        public int DurationMillis { get; set; }
        public string ClientIpAddress { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
    }
}