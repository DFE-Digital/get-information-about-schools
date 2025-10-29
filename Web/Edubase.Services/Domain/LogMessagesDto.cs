using AzureTableLogger.LogMessages;
using System.Collections.Generic;

namespace Edubase.Services.Domain
{
    public class LogMessagesDto
    {
        public string SkipToken { get; set; }
        public IEnumerable<WebLogMessage> Items { get; set; }

        public LogMessagesDto(IEnumerable<WebLogMessage> items, string skipToken)
        {
            Items = items;
            SkipToken = skipToken;
        }

        public LogMessagesDto()
        {

        }
    }
}
