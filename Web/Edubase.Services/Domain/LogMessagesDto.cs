using Edubase.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AzureTableLogger.LogMessages;

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
