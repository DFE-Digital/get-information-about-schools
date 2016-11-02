using Edubase.Data.Entity;
using Edubase.Data.Repositories;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MoreLinq;
using System.Configuration;
using Edubase.Common;
using System.Net.Mail;

namespace Edubase.Services
{
    /// <summary>
    /// Logs messages into table storage; these messages should be of low value, as they may be deleted.
    /// Messages are cached in-memory until FlushAsync is called.  
    /// The max number of messages cacheable is [MaximumMessagesCacheableCount].
    /// When the number of messages logged exceeds [MaximumMessagesCacheableCount], the oldest message is ostracized / deleted.
    /// This prevents overloading the message storage medium.
    /// To increase the number of messages stored, called FlushAsync more frequently or change the value of [MaximumMessagesCacheableCount].
    /// </summary>
    public class MessageLoggingService
    {
        public class FlushReport
        {
            public int OstracismsCount { get; set; }
            public int ProcessedCount { get; set; }
        }


        #region Singleton pattern
        private static readonly Lazy<MessageLoggingService> _inst = new Lazy<MessageLoggingService>(() => new MessageLoggingService(), LazyThreadSafetyMode.PublicationOnly);
        public static MessageLoggingService Instance { get { return _inst.Value; } }
        #endregion

        private const int DEFAULT_MAX_ERRORS_IN_CACHE = 500;

        /// <summary>
        /// How many messages were ostracized from the cache since the last flush before the report was saved
        /// </summary>
        private volatile int _ostracizedCount = 0;
        private volatile int _pendingCount = 0;
        private long _totalProcessedCount = 0;
        private long _totalOstrasizedCount = 0;
        private long _totalNumberOfTimesPushCalled = 0;
        private SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
        private readonly object _mutex2 = new object();

        public int OstracizedCount => _ostracizedCount;
        public long TotalOstrasizedCount => _totalOstrasizedCount;
        public long TotalProcessedCount => _totalProcessedCount;
        /// <summary>
        /// Defines how many errors can be cached by this instance
        /// </summary>
        public int MaximumMessagesCacheableCount { get; set; } = DEFAULT_MAX_ERRORS_IN_CACHE;
        public bool DisableStorageReporting { get; set; }
        public bool DisableEmailReporting { get; set; }

        /// <summary>
        /// The concurrent queue of error reports
        /// </summary>
        private ConcurrentQueue<LogMessage> _queue = new ConcurrentQueue<LogMessage>();

        /// <summary>
        /// How many reports are pending being processed
        /// </summary>
        public int PendingCount => _pendingCount;

        /// <summary>
        /// How many times a report has been filed
        /// </summary>
        public long TotalNumberOfTimesFileReportCalled => _totalNumberOfTimesPushCalled;


        /// <summary>
        /// .ctor
        /// </summary>
        private MessageLoggingService() { }

        /// <summary>
        /// Files a tex-based error report for reporting later
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public string Push(string text) => Push(new LogMessage(DateTime.UtcNow, text));

        public string Push(Exception ex) => Push(new LogMessage(ex));

        /// <summary>
        /// Files an error log for reporting later
        /// </summary>
        /// <param name="log"></param>
        /// <returns></returns>
        public string Push(LogMessage log)
        {
            Interlocked.Increment(ref _totalNumberOfTimesPushCalled);
            _queue.Enqueue(log);
            lock (_mutex2) _pendingCount++;
            if (_pendingCount >= MaximumMessagesCacheableCount)
            {
                LogMessage oldMessage;
                if (_queue.TryDequeue(out oldMessage))
                {
                    lock (_mutex2)
                    {
                        _ostracizedCount++;
                        _totalOstrasizedCount++;
                        _pendingCount--;
                    }
                }
            }
            return log.Id;
        }


        /// <summary>
        /// Flushes the messages by recording each one in the storage medium
        /// <returns>How many items were processed successfully</returns>
        /// </summary>
        public async Task<FlushReport> FlushAsync()
        {
            var retVal = new FlushReport();
            if (_queue.Count == 0) return retVal;
            
            try
            {
                if (!await _semaphore.WaitAsync(1000)) return retVal;

                if (_queue.Count > 0)
                {
                    var logs = DequeuePendingMessages();

                    try
                    {
                        await SendToStorage(logs);

                        _totalProcessedCount += (retVal.ProcessedCount = logs.Count);

                        lock (_mutex2)
                        {
                            retVal.OstracismsCount = _ostracizedCount;
                            _ostracizedCount = 0;
                            _pendingCount -= retVal.ProcessedCount;
                        }

                        if (retVal.OstracismsCount > 0)
                        {
                            var msg = $"EX_CACHE_OVERFLOW: There were {retVal.OstracismsCount} exception(s) ostracized from the cache in this period.";
                            if (!DisableStorageReporting) await new LogMessageRepository().CreateAsync(new LogMessage(msg));
                        }


                        await SendEmailSummary(logs, retVal.OstracismsCount);

                    }
                    catch (Exception ex)
                    {
                        logs.ForEach(x => _queue.Enqueue(x));
                        Push(ex);
                    }
                }
            }
            catch  { } // do nothing
            finally { _semaphore.Release(); }
            
            return retVal;
        }

        private async Task SendToStorage(List<LogMessage> logs)
        {
            if (!DisableStorageReporting)
            {
                await new LogMessageRepository().CreateAsync(logs);
            }
        }

        private List<LogMessage> DequeuePendingMessages()
        {
            var logs = new List<LogMessage>();
            for (int i = 0; i < MaximumMessagesCacheableCount; i++)
            {
                LogMessage log;
                if (_queue.TryDequeue(out log))
                {
                    logs.Add(log);
                }
                else break;
            }

            return logs;
        }

        /// <summary>
        /// Returns a snapshot of the current queue
        /// </summary>
        /// <returns></returns>
        public LogMessage[] GetPending() => _queue.ToArray();


        private async Task SendEmailSummary(List<LogMessage> messages, int ostracismsCount)
        {
            if (DisableEmailReporting) return;
            var recipientEmailAddress = ConfigurationManager.AppSettings["ErrorReportingEmailAddress"];

            if (!recipientEmailAddress.IsValidEmail())
                return;

            try
            {
                var sb = new StringBuilder();
                const int reportableCount = 20;
                var sep = new string('-', 50);
                if (messages.Count == 1) sb.AppendLine($"There was just 1 log message in this period");
                else sb.AppendLine($"There were {messages.Count} log messages in this period");
                
                if (messages.Count > reportableCount)
                {
                    sb.AppendLine($"...but 20 are reported here; the other {messages.Count - reportableCount} are in Table Storage.");
                    messages = messages.Take(20).ToList();
                }

                sb.AppendLine();
                if (ostracismsCount == 1) sb.AppendLine($"There was 1 _ostracised_ log message in this period.");
                else if(ostracismsCount > 1) sb.AppendLine($"There were {ostracismsCount} _ostracised_ log messages in this period.");
                
                sb.AppendLine();
                sb.AppendLine(sep);
                sb.AppendLine();

                messages.ForEach(x =>
                {
                    sb.Append(x);
                    sb.AppendLine();
                    sb.AppendLine(sep);
                    sb.AppendLine();
                });

                sb.AppendLine();
                sb.AppendLine();
                sb.AppendLine();
                sb.AppendLine("[END]");


                var msg = new MailMessage
                {
                    IsBodyHtml = false,
                    Body = sb.ToString(),
                    From = new MailAddress("edubase@contentsupport.co.uk"),
                    Subject = "Edubase message report"
                };

                msg.To.Add(recipientEmailAddress);

                await new EmailService().SendAsync(msg);
            }
            catch (Exception ex)
            {
                Push(ex);
            }
        }
    }
}
