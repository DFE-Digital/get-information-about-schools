using Edubase.Services.Enums;
using Edubase.Web.UI.Helpers;

namespace Edubase.Web.UI.Models.DisplayProfiles
{
    public class PRUDisplayProfile : EstablishmentDisplayProfile
    {
        protected override bool IsMatchInternal(eLookupEstablishmentType type, eLookupEstablishmentTypeGroup typeGroup)
            => typeGroup == eLookupEstablishmentTypeGroup.LAMaintainedSchools && type == eLookupEstablishmentType.PupilReferralUnit;

        protected override void ConfigureInternal()
        {
            var isUserLoggedIn = Principal.Identity.IsAuthenticated;
            var isSchoolClosed = Establishment.StatusId == (int)eLookupEstablishmentStatus.Closed;

            HeadteacherDetails = true;
            AgeRange = true;
            GenderOfEntry = true;
            WebsiteAddress = true;
            CloseDate = isSchoolClosed;
            ReasonEstablishmentClosed = isSchoolClosed;
            MainEmailAddress = isUserLoggedIn;
            AlternativeEmailAddress = MainEmailAddress;
            LastChangedDate = isUserLoggedIn;
            FurtherEducationType = true;
            LAESTAB = true;
            OfstedRatingDetails = true;

            // PRU-specific fields
            TeenageMothers = true;
            TeenageMothersCapacity = true;
            ChildcareFacilities = true;
            SENFacilities = true;
            PupilsWithEBD = true;
            PRUNumberOfPlaces = true;
            PRUFullTimeProvision = true;
            PRUPupilsEducatedByOthers = true;


        }
    }
}