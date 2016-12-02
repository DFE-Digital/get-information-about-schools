using Edubase.Common;
using Edubase.Services.Enums;

namespace Edubase.Services.Establishments.DisplayPolicies
{
    public class ChildrensCentresDisplayPolicy : EstablishmentDisplayPolicy
    {
        protected override bool IsMatchInternal(eLookupEstablishmentType type, eLookupEstablishmentTypeGroup typeGroup)
        {
            return typeGroup.Equals(eLookupEstablishmentTypeGroup.ChildrensCentres)
            &&
            type.OneOfThese(eLookupEstablishmentType.ChildrensCentre, eLookupEstablishmentType.ChildrensCentreLinkedSite);
        }

        protected override void ConfigureInternal()
        {
            // show none of the optional fields.
        }
    }
}