using System;

namespace Edubase.Web.UI.Helpers
{
    public class GetEstabSearchFiltersParam
    {
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public string EstablishmentTypeId { get; set; }
        public bool IsSecure16To19User { get; set; }

        public GetEstabSearchFiltersParam(DateTime? from, DateTime? to, string establishmentTypeId, bool isSecure16To19User)
        {
            From = from;
            To = to;
            EstablishmentTypeId = establishmentTypeId;
            IsSecure16To19User = isSecure16To19User;
        }
    }
}
