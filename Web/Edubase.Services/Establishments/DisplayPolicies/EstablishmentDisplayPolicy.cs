using Edubase.Services.Enums;
using Edubase.Services.Establishments.Models;
using Edubase.Services.Groups.Models;
using System.Security.Principal;

namespace Edubase.Services.Establishments.DisplayPolicies
{
    public abstract class EstablishmentDisplayPolicy : EstablishmentFieldList
    {
        public string HeadteacherLabel { get; set; } = "Headteacher/Principal";
        public string HeadEmailAddressLabel { get; set; } = "Headteacher/Principal email";
        
        public string EstablishmentTypeLabel { get; set; } = "School type";
        public string MainEmailAddressLabel { get; set; } = "Email";
        protected EstablishmentModelBase Establishment { get; private set; }
        protected IPrincipal Principal { get; private set; }
        public bool IsUserLoggedIn { get; set; }
        public bool IsSchoolClosed { get; set; }
        
        public override bool Contact_TelephoneNumber { get; set; } = true;
        public override bool LocalAuthorityId { get; set; } = true;
        public override bool EducationPhaseId { get; set; } = true;
        public override bool TypeId { get; set; } = true;
        public override bool Urn { get; set; } = true;
        public override bool UKPRN { get; set; } = true;
        public override bool StatusId { get; set; } = true;
        public override bool OpenDate { get; set; } = true;
        public override bool ReasonEstablishmentOpenedId { get; set; } = true;
        public override bool Name { get; set; } = true;

        public EstablishmentDisplayPolicy()
        {
            Name = true;
            SetLocationFields(true);
            SetAddressFields(true);
        }

        internal bool IsMatch(EstablishmentModelBase establishment)
            => establishment.TypeId.HasValue 
            && establishment.EstablishmentTypeGroupId.HasValue 
            && IsMatchInternal((eLookupEstablishmentType)establishment.TypeId, (eLookupEstablishmentTypeGroup)establishment.EstablishmentTypeGroupId);

        internal EstablishmentDisplayPolicy Configure(IPrincipal principal, EstablishmentModelBase establishment)
        {
            Establishment = establishment;
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