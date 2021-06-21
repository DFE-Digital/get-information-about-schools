using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Edubase.Data.Entity;

namespace Edubase.Web.UI.Models.Notifications
{
    public class NotificationsTemplatesViewModel
    {
        public IEnumerable<NotificationTemplate> Templates { get; }

        public NotificationsTemplatesViewModel(IEnumerable<NotificationTemplate> items)
        {
            Templates = items.OrderBy(x => x.Content);
        }
    }
}
