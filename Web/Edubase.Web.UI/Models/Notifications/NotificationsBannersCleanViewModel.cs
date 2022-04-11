using System.Collections.Generic;
using System.Linq;
using Edubase.Data.Entity;

namespace Edubase.Web.UI.Models.Notifications
{
    public class NotificationsBannersArchiveExpiredViewModel
    {
        public List<NotificationBanner> GroupedBanners { get; }

        public NotificationsBannersArchiveExpiredViewModel(IEnumerable<NotificationBanner> items)
        {
            GroupedBanners = items.ToList();
        }
    }
}
