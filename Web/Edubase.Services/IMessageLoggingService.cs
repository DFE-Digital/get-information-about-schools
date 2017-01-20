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

        /// <summary>
        /// Pushes a message into message storage
        /// </summary>
        /// <param name="ex"></param>
        /// <returns>Message Id</returns>
        string Push(LogMessage log);
        
        /// <summary>
        /// Pushes an exception into message storage
        /// </summary>
        /// <param name="ex"></param>
        /// <returns>Message Id</returns>
        string Push(Exception ex);
    }
}