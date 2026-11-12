using Edubase.Data.Entity;
using Edubase.Services.Domain;
using System.Collections.Generic;

namespace Edubase.Web.UI.Models.Admin
{
    public class LogMessagesViewModel
    {
        public IEnumerable<WebLogMessage> Messages { get; set; }
        public string SkipToken { get; set; }
        public string DateFilter { get; set; }

        public LogMessagesViewModel()
        {
        }

        public LogMessagesViewModel(LogMessagesDto dto)
        {
            Messages = dto.Items;
            SkipToken = dto.SkipToken;
        }
    }
}
