using Edubase.Services.Enums;

namespace Edubase.Services.Establishments.DisplayPolicies
{
    public class BSODisplayPolicy : EstablishmentDisplayPolicy
    {
        protected override bool IsMatchInternal(eLookupEstablishmentType type, eLookupEstablishmentTypeGroup typeGroup)
            => typeGroup == eLookupEstablishmentTypeGroup.OtherTypes && type == eLookupEstablishmentType.BritishSchoolsOverseas;
        
        protected override void ConfigureInternal()
        {
            HeadteacherDetails = true;
            AgeRange = true;
            GenderOfEntry = true;
            LAESTAB = true;
            WebsiteAddress = true;
            CloseDate = true;
            ReasonEstablishmentClosed = true;
            MainEmailAddress = IsUserLoggedIn;
            AlternativeEmailAddress = MainEmailAddress;
            LastChangedDate = IsUserLoggedIn;
            BSODateOfLastInspectionVisit = BSODateOfNextInspectionVisit = BSOInspectorate = BSOInspectorateReport = true;
            LocationDetails = false;
        }
    }
}