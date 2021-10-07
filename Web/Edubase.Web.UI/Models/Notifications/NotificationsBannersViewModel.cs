using System.Collections.Generic;
using System.Linq;
using Edubase.Data.Entity;

namespace Edubase.Web.UI.Models.Notifications
{
    public class NotificationsBannersViewModel
    {
        public IEnumerable<NotificationBanner> Banners { get; }

        public NotificationsBannersViewModel(IEnumerable<NotificationBanner> items)
        {
            Banners = items.OrderBy(x => x.Start);
        }
    }
}
