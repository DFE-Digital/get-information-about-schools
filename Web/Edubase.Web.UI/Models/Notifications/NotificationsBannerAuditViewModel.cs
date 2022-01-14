using System.Collections.Generic;
using System.Linq;
using Edubase.Data.Entity;
using Edubase.Web.UI.Helpers;

namespace Edubase.Web.UI.Models.Notifications
{
    public class NotificationsBannerAuditViewModel
    {
        public List<NotificationBanner> Banners { get; }

        public NotificationsBannerAuditViewModel(IEnumerable<NotificationBanner> items, string sortBy)
        {
            var sortOrder = "desc";
            if (!string.IsNullOrEmpty(sortBy))
            {
                var split = string.Concat(sortBy, "-").Split('-');
                sortBy = split[0];
                sortOrder = split[1];
            }

            if (sortOrder == "desc")
            {
                switch (sortBy)
                {
                    case nameof(NotificationBanner.AuditTimestamp):
                        items = items.OrderByDescending(x => x.AuditTimestamp);
                        break;

                    case nameof(NotificationBanner.Status):
                        items = items.OrderByDescending(x => x.Status.EnumDisplayNameFor());
                        break;

                    case nameof(NotificationBanner.Version):
                        items = items.OrderByDescending(x => x.Version);
                        break;

                    case nameof(NotificationBanner.AuditUser):
                        items = items.OrderByDescending(x => x.AuditUser);
                        break;

                    case nameof(NotificationBanner.Start):
                        items = items.OrderByDescending(x => x.Start);
                        break;

                    case nameof(NotificationBanner.End):
                        items = items.OrderByDescending(x => x.End);
                        break;

                    case nameof(NotificationBanner.Importance):
                        items = items.OrderByDescending(x => x.Importance);
                        break;

                    case nameof(NotificationBanner.Content):
                        items = items.OrderByDescending(x => x.Content);
                        break;

                    default:
                        items = items.OrderByDescending(x => x.Version);
                        break;
                }
            }
            else
            {
                switch (sortBy)
                {
                    case nameof(NotificationBanner.AuditTimestamp):
                        items = items.OrderBy(x => x.AuditTimestamp);
                        break;

                    case nameof(NotificationBanner.Status):
                        items = items.OrderBy(x => x.Status.EnumDisplayNameFor());
                        break;

                    case nameof(NotificationBanner.Version):
                        items = items.OrderBy(x => x.Version);
                        break;

                    case nameof(NotificationBanner.AuditUser):
                        items = items.OrderBy(x => x.AuditUser);
                        break;

                    case nameof(NotificationBanner.Start):
                        items = items.OrderBy(x => x.Start);
                        break;

                    case nameof(NotificationBanner.End):
                        items = items.OrderBy(x => x.End);
                        break;

                    case nameof(NotificationBanner.Importance):
                        items = items.OrderBy(x => x.Importance);
                        break;

                    case nameof(NotificationBanner.Content):
                        items = items.OrderBy(x => x.Content);
                        break;

                    default:
                        items = items.OrderBy(x => x.Version);
                        break;
                }
            }


            Banners = items.ToList();
        }
    }
}
