using Edubase.Common;
using Edubase.Services.Enums;

namespace Edubase.Services.Establishments.DisplayPolicies
{
    public class ChildrensCentreLinkedSitesDisplayPolicy : EstablishmentDisplayPolicy
    {
        protected override bool IsMatchInternal(eLookupEstablishmentType type, eLookupEstablishmentTypeGroup typeGroup)
        {
            return typeGroup.Equals(eLookupEstablishmentTypeGroup.ChildrensCentres)
            && type.Equals(eLookupEstablishmentType.ChildrensCentreLinkedSite);
        }

        protected override void ConfigureInternal()
        {
            TypeId = false;
            Contact_TelephoneNumber = false;
            CloseDate = IsSchoolClosed;
            ReasonEstablishmentClosedId = IsSchoolClosed;
        }
    }
}