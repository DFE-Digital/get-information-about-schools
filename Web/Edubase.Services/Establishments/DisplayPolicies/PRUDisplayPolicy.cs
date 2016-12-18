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
            GenderOfEntry = true;
            WebsiteAddress = true;
            CloseDate = IsSchoolClosed;
            ReasonEstablishmentClosed = IsSchoolClosed;
            MainEmailAddress = IsUserLoggedIn;
            AlternativeEmailAddress = MainEmailAddress;
            LastChangedDate = IsUserLoggedIn;
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