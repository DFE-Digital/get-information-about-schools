#if (!TEXAPI)
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
            GenderId = true;
            EstablishmentNumber = true;
            Contact_WebsiteAddress = true;
            CloseDate = true;
            ReasonEstablishmentClosedId = true;
            Contact_EmailAddress = ContactAlt_EmailAddress = IsUserLoggedIn;
            LastChangedDate = IsUserLoggedIn;
            BSODateOfLastInspectionVisit = BSODateOfNextInspectionVisit = BSOInspectorateId = BSOInspectorateReportUrl = true;
            SetLocationFields(false);
        }
    }
}
#endif