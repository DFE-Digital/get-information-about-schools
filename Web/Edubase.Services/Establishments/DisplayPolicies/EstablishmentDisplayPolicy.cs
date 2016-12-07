using Edubase.Services.Enums;
using Edubase.Services.Establishments.Models;
using Edubase.Services.Groups.Models;
using System.Security.Principal;

namespace Edubase.Services.Establishments.DisplayPolicies
{
    public abstract class EstablishmentDisplayPolicy
    {
        public bool HeadteacherDetails { get; set; }

        public string HeadteacherLabel { get; set; } = "Headteacher/Principal";

        public bool HeadEmailAddress { get; set; }

        public string HeadEmailAddressLabel { get; set; } = "Headteacher/Principal email";

        public bool AgeRange { get; set; }

        public bool GenderOfEntry { get; set; }
        public bool GroupDetails { get; set; }

        public bool LAESTAB { get; set; }

        public bool AdmissionsPolicy { get; set; }

        public bool EstablishmentType { get; set; } = true;

        public string EstablishmentTypeLabel { get; set; } = "School type";

        public bool FurtherEducationType { get; set; }

        public bool TelephoneNumber { get; set; } = true;
        public bool WebsiteAddress { get; set; }

        public bool OfstedRatingDetails { get; set; }

        public bool Inspectorate { get; set; }
        public bool Proprietor { get; set; }

        public bool ReligiousCharacter { get; set; }
        public bool Diocese { get; set; }
        public bool ReligiousEhtos { get; set; }
        public bool BoardingProvision { get; set; }
        public bool NurseryProvision { get; set; }
        public bool OfficialSixthFormProvision { get; set; }
        public bool Capacity { get; set; }
        public bool Section41Approved { get; set; }
        public bool CloseDate { get; set; }
        public bool ReasonEstablishmentClosed { get; set; }
        public bool SpecialClasses { get; set; }
        public bool SENStat { get; set; }
        public bool SENNoStat { get; set; }
        public bool MainEmailAddress { get; set; }
        public string MainEmailAddressLabel { get; set; } = "Email";
        public bool AlternativeEmailAddress { get; set; }
        public bool HeadPreferredJobTitle { get; set; }
        public bool LastChangedDate { get; set; }
        public bool SENProvisions { get; set; }
        public bool TeenageMothers { get; set; }
        public bool TeenageMothersCapacity { get; set; }
        public bool ChildcareFacilities { get; set; }
        public bool SENFacilities { get; set; }
        public bool PupilsWithEBD { get; set; }
        public bool PRUNumberOfPlaces { get; set; }
        public bool PRUFullTimeProvision { get; set; }
        public bool PRUPupilsEducatedByOthers { get; set; }
        public bool TypeOfResourcedProvision { get; set; }
        public bool ResourcedProvisionOnRoll { get; set; }
        public bool ResourcedProvisionCapacity { get; set; }
        public bool SenUnitOnRoll { get; set; }
        public bool SenUnitCapacity { get; set; }


        

        public bool BSOInspectorate { get; set; }
        public bool BSOInspectorateReport { get; set; }
        public bool BSODateOfLastInspectionVisit { get; set; }
        public bool BSODateOfNextInspectionVisit { get; set; }
        public bool LocationDetails { get; set; } = true;


        //public bool RSCRegion { get; set; }
        //public bool GovernmentOfficeRegion { get; set; }
        //public bool AdministrativeDistrict { get; set; }
        //public bool AdministrativeWard { get; set; }
        //public bool ParliamentaryConstituency { get; set; }
        //public bool UrbanRural { get; set; }
        //public bool GSSLA { get; set; }
        //public bool CASWard { get; set; }
        //public bool MSOA { get; set; }
        //public bool LSOA { get; set; }



        public bool CCOperationalHours { get; set; }
        public bool CCGovernance { get; set; }
        public bool CCUnder5YearsOfAgeCount { get; set; }
        public bool CCPhaseType { get; set; }

        public bool CCGovernanceDetail { get; set; }
        public bool CCLAContactDetail { get; set; }

        public bool DeliveryModel { get; set; }

        public bool GroupCollaborationName { get; set; }
        public bool GroupLeadCentre { get; set; }

        public bool CCDisadvantagedArea { get; set; }

        public bool CCDirectProvisionOfEarlyYears { get; set; }

        protected EstablishmentModel Establishment { get; private set; }

    
        protected GroupModel Group { get; private set; }
        protected IPrincipal Principal { get; private set; }

        public bool IsUserLoggedIn { get; set; }
        public bool IsSchoolClosed { get; set; }


        internal bool IsMatch(EstablishmentModel establishment)
            => establishment.TypeId.HasValue 
            && establishment.EstablishmentTypeGroupId.HasValue 
            && IsMatchInternal((eLookupEstablishmentType)establishment.TypeId, (eLookupEstablishmentTypeGroup)establishment.EstablishmentTypeGroupId);

        internal EstablishmentDisplayPolicy Configure(IPrincipal principal, EstablishmentModel establishment, GroupModel group)
        {
            Establishment = establishment;
            Group = group;
            Principal = principal;

            IsUserLoggedIn = Principal.Identity.IsAuthenticated;
            IsSchoolClosed = Establishment.StatusId == (int)eLookupEstablishmentStatus.Closed;

            ConfigureInternal();
            return this;
        }

        protected abstract bool IsMatchInternal(eLookupEstablishmentType type, eLookupEstablishmentTypeGroup typeGroup);
        protected abstract void ConfigureInternal();
    }
}