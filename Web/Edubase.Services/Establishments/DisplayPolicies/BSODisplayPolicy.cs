using Edubase.Services.Enums;

namespace Edubase.Services.Establishments.DisplayPolicies
{
    public class BSODisplayPolicy : EstablishmentDisplayPolicy
    {
        protected override bool IsMatchInternal(eLookupEstablishmentType type, eLookupEstablishmentTypeGroup typeGroup)
            => typeGroup == eLookupEstablishmentTypeGroup.OtherTypes && type == eLookupEstablishmentType.BritishSchoolsOverseas;
        
        protected override void ConfigureInternal()
        {
            var isUserLoggedIn = Principal.Identity.IsAuthenticated;
            var isSchoolClosed = Establishment.StatusId == (int)eLookupEstablishmentStatus.Closed;

            HeadteacherDetails = true;
            AgeRange = true;
            GenderOfEntry = true;
            LAESTAB = true;
            WebsiteAddress = true;
            CloseDate = true;
            ReasonEstablishmentClosed = true;
            MainEmailAddress = isUserLoggedIn;
            AlternativeEmailAddress = MainEmailAddress;
            LastChangedDate = isUserLoggedIn;
            BSODateOfLastInspectionVisit = BSODateOfNextInspectionVisit = BSOInspectorate = BSOInspectorateReport = true;
            LocationDetails = false;
        }
    }
}