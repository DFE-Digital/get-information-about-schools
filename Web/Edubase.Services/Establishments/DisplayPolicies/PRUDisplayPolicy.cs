#if (!TEXAPI)
using Edubase.Services.Enums;

namespace Edubase.Services.Establishments.DisplayPolicies
{
    public class PRUDisplayPolicy : EstablishmentDisplayPolicy
    {
        protected override bool IsMatchInternal(eLookupEstablishmentType type, eLookupEstablishmentTypeGroup typeGroup)
            => typeGroup == eLookupEstablishmentTypeGroup.LAMaintainedSchools && type == eLookupEstablishmentType.PupilReferralUnit;

        protected override void ConfigureInternal()
        {
            HeadteacherDetails = true;
            AgeRange = true;
            GenderId = true;
            Contact_WebsiteAddress = true;
            CloseDate = IsSchoolClosed;
            ReasonEstablishmentClosedId = IsSchoolClosed;
            Contact_EmailAddress = IsUserLoggedIn;
            ContactAlt_EmailAddress = IsUserLoggedIn;
            LastChangedDate = IsUserLoggedIn;
            FurtherEducationTypeId = true;
            EstablishmentNumber = true;
            OfstedRatingDetails = true;

            // PRU-specific fields
            TeenageMothersProvisionId = true;
            TeenageMothersCapacity = true;
            ChildcareFacilitiesId = true;
            PRUSENId = true;
            PRUEBDId = true;
            PlacesPRU = true;
            PruFulltimeProvisionId = true;
            PruEducatedByOthersId = true;


        }
    }
}
#endif