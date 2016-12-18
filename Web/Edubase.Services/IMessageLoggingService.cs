using System;
using System.Threading.Tasks;
using Edubase.Data.Entity;

namespace Edubase.Services
{
    public interface IMessageLoggingService
    {
        string InstanceId { get; }
        bool DisableEmailReporting { get; set; }
        bool DisableStorageReporting { get; set; }
        int MaximumMessagesCacheableCount { get; set; }
        int OstracizedCount { get; }
        int PendingCount { get; }
        long TotalNumberOfTimesFileReportCalled { get; }
        long TotalOstrasizedCount { get; }
        long TotalProcessedCount { get; }

        Task<MessageLoggingService.FlushReport> FlushAsync();
        LogMessage[] GetPending();
        string Push(string text);
        string Push(LogMessage log);
        string Push(Exception ex);
    }
}