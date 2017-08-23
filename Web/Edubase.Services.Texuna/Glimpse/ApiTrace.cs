using System;

namespace Edubase.Services.Texuna.Glimpse
{
    public static class ApiTrace
    {
        private static readonly Lazy<ApiTraceList> LazyData = new Lazy<ApiTraceList>();

        public static ApiTraceList Data => LazyData.Value;
    }
}