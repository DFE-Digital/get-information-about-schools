using System.Collections.Generic;
using System.Linq;
using Edubase.Services.Domain;
using Edubase.Services.Enums;
using ET = Edubase.Services.Enums.eLookupEstablishmentType;

namespace Edubase.Services.Establishments.EditPolicies
{
    public static class LookupOptionsFilter
    {
        private static readonly List<int> secureAcademies16to19StatusIds = new List<int> {
            (int) eLookupEstablishmentStatus.PendingApproval,
            (int) eLookupEstablishmentStatus.Open,
            (int) eLookupEstablishmentStatus.ProposedToOpen,
            (int) eLookupEstablishmentStatus.RejectedOpening,
            (int) eLookupEstablishmentStatus.OpenButProposedToClose,
            (int) eLookupEstablishmentStatus.CreatedInError,
            (int) eLookupEstablishmentStatus.Closed
        };

        public static IEnumerable<LookupDto> FilterForEstablishmentType(this IEnumerable<LookupDto> lookups, int? typeId)
        {
            return typeId == (int) ET.AcademySecure16to19 ? lookups.Where(x => secureAcademies16to19StatusIds.Contains(x.Id)) : lookups;
        }
    }
}
