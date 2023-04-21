using System.Collections.Generic;
using System.Linq;
using Edubase.Services.Domain;
using Edubase.Services.Enums;

namespace Edubase.Web.UI
{
    public static class Utility
    {
        public static int[] GetAcademyOpeningsEstablishmentTypeGroupIds(string establishmentCode) =>
            new[]
            {
                establishmentCode == null
                    ? (int) eLookupEstablishmentTypeGroup.Academies
                    : (int) eLookupEstablishmentTypeGroup.Secure16To19Academy
            };

        public static IEnumerable<EstablishmentLookupDto> FilterEstablishmentTypes(
            IEnumerable<EstablishmentLookupDto> establishmentTypes, string establishmentCode = null)
        {
            if (!string.IsNullOrWhiteSpace(establishmentCode) && int.TryParse(establishmentCode, out var estabCode))
                establishmentTypes = establishmentTypes.Where(et => et.Code == estabCode.ToString());

            return establishmentTypes;
        }
    }
}
