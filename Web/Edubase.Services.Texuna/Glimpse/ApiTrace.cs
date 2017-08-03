namespace Edubase.Services.Texuna.Glimpse
{
    public static class ApiTrace
    {
        static ApiTrace()
        {
            Data = new ApiTraceList();
        }

        public static ApiTraceList Data { get; set; }
    }
}