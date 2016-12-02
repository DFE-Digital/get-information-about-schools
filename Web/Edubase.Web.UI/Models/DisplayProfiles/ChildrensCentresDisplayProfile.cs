using Edubase.Services.Enums;
using Edubase.Web.UI.Helpers;

namespace Edubase.Web.UI.Models.DisplayProfiles
{
    public class ChildrensCentresDisplayProfile : EstablishmentDisplayProfile
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