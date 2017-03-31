#if (!TEXAPI)
using Edubase.Common;
using Edubase.Services.Enums;

namespace Edubase.Services.Establishments.DisplayPolicies
{
    public class OtherDisplayPolicy : EstablishmentDisplayPolicy
    {
        protected override bool IsMatchInternal(eLookupEstablishmentType type, eLookupEstablishmentTypeGroup typeGroup)
        {
            return typeGroup.OneOfThese(eLookupEstablishmentTypeGroup.OtherTypes, eLookupEstablishmentTypeGroup.Colleges)
            &&
            type.OneOfThese(eLookupEstablishmentType.InstitutionFundedByOtherGovernmentDepartment,
                            eLookupEstablishmentType.OffshoreSchools,
                            eLookupEstablishmentType.SecureUnits,
                            eLookupEstablishmentType.ServiceChildrensEducation,
                            eLookupEstablishmentType.SixthFormCentres,
                            eLookupEstablishmentType.Miscellaneous);
        }

        protected override void ConfigureInternal()
        {
            CloseDate = true;
            ReasonEstablishmentClosedId = true;
            EstablishmentNumber = true;
            HeadteacherDetails = true;
            GenderId = true;
            Contact_EmailAddress = IsUserLoggedIn;
            ContactAlt_EmailAddress = IsUserLoggedIn;
            LastChangedDate = IsUserLoggedIn;
        }
    }
}
#endif