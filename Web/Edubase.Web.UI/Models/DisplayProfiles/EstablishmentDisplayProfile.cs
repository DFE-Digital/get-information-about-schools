using Edubase.Data.Entity;
using Edubase.Services.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;

namespace Edubase.Web.UI.Models.DisplayProfiles
{
    public abstract class EstablishmentDisplayProfile
    {
        public bool HeadteacherDetails { get; set; }
        public bool AgeRange { get; set; }

        public bool GenderOfEntry { get; set; }
        public bool GroupDetails { get; set; }

        public bool LAESTAB { get; set; }

        public bool AdmissionsPolicy { get; set; }
        public bool FurtherEducationType { get; set; }

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

        protected Establishment Establishment { get; private set; }

        protected GroupCollection Group { get; private set; }
        protected IPrincipal Principal { get; private set; }
        

        internal bool IsMatch(Establishment establishment)
            => IsMatchInternal((eLookupEstablishmentType)establishment.TypeId, (eLookupEstablishmentTypeGroup)establishment.EstablishmentTypeGroupId);

        internal EstablishmentDisplayProfile Configure(IPrincipal principal, Establishment establishment, GroupCollection group)
        {
            Establishment = establishment;
            Group = group;
            Principal = principal;
            ConfigureInternal();
            return this;
        }

        protected abstract bool IsMatchInternal(eLookupEstablishmentType type, eLookupEstablishmentTypeGroup typeGroup);
        protected abstract void ConfigureInternal();
    }
}