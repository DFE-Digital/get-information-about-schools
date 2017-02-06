using Edubase.Services.Establishments.Models;

namespace Edubase.Services.Establishments.Downloads
{
    public class EstablishmentDownloadCoreFieldList : EstablishmentFieldList
    {
        public EstablishmentDownloadCoreFieldList()
        {
            Name = true;
            SetAddressFields(true);
            LocalAuthorityId = true;
            HeadteacherDetails = true;
            AgeRange = true;
            EducationPhaseId = true;
            TypeId = true;
            FurtherEducationTypeId = true;
            GroupDetails = true;
            GenderId = true;
            Urn = true;
            EstablishmentNumber = true;
            UKPRN = true;
            StatusId = true;
            AdmissionsPolicyId = true;
            Contact_WebsiteAddress = true;
            Contact_TelephoneNumber = true;
            ReligiousCharacterId = true;
            DioceseId = true;
            ReligiousEthosId = true;
            ProvisionBoardingId = true;
            ProvisionNurseryId = true;
            ProvisionOfficialSixthFormId = true;
            OpenDate = true;
            CloseDate = true;
            HeadPreferredJobTitle = false;
        }
    }
}
