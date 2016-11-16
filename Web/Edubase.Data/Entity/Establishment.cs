using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System;
using Edubase.Data.Entity.Lookups;
using Edubase.Data.Entity.ComplexTypes;
using Edubase.Common;
using Edubase.Data.Entity.Permissions;
using Edubase.Data.Identity;
using System.Data.Entity.Spatial;

namespace Edubase.Data.Entity
{
    public class Establishment : EdubaseEntity
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Urn { get; set; }
        
        public LocalAuthority LocalAuthority { get; set; }

        [RestrictPermission]
        public int? LocalAuthorityId { get; set; }

        [RestrictPermission]
        public int? EstablishmentNumber { get; set; }

        [RequiresApproval]
        public string Name { get; set; }

        [Column("Type"), RestrictPermission]
        public LookupEstablishmentType EstablishmentType { get; set; }
        
        public LookupEstablishmentStatus Status { get; set; }

        [RestrictPermission]
        public int? StatusId { get; set; }

        public LookupReasonEstablishmentOpened ReasonEstablishmentOpened { get; set; }

        public LookupReasonEstablishmentClosed ReasonEstablishmentClosed { get; set; }

        public int? ReasonEstablishmentOpenedId { get; set; }

        public int? ReasonEstablishmentClosedId { get; set; }

        [RestrictPermission]
        public DateTime? OpenDate { get; set; }

        [RestrictPermission]
        public DateTime? CloseDate { get; set; }

        public LookupEducationPhase EducationPhase { get; set; }

        [RestrictPermission]
        public int? EducationPhaseId { get; set; }

        [RestrictAction(Roles.Academy, ActionType.Approve)]
        public int? StatutoryLowAge { get; set; }

        [RestrictAction(Roles.Academy, ActionType.Approve)]
        public int? StatutoryHighAge { get; set; }

        public LookupProvisionBoarding ProvisionBoarding { get; set; }

        public int? ProvisionBoardingId { get; set; }

        public LookupProvisionNursery ProvisionNursery { get; set; }

        public int? ProvisionNurseryId { get; set; }

        public LookupProvisionOfficialSixthForm ProvisionOfficialSixthForm { get; set; }

        public int? ProvisionOfficialSixthFormId { get; set; }

        public LookupGender Gender { get; set; }

        [RestrictPermission]
        public int? GenderId { get; set; }

        public LookupReligiousCharacter ReligiousCharacter { get; set; }

        public int? ReligiousCharacterId { get; set; }

        public LookupReligiousEthos ReligiousEthos { get; set; }

        public int? ReligiousEthosId { get; set; }

        public LookupDiocese Diocese { get; set; }
        
        public int? DioceseId { get; set; }

        public LookupAdmissionsPolicy AdmissionsPolicy { get; set; }

        [RestrictPermission]
        public int? AdmissionsPolicyId { get; set; }

        [RestrictAction(Roles.Academy, ActionType.Approve)]
        public int? Capacity { get; set; }

        public LookupProvisionSpecialClasses ProvisionSpecialClasses { get; set; }

        public int? ProvisionSpecialClassesId { get; set; }

        public int? UKPRN { get; set; }

        public DateTime? LastChangedDate { get; set; }

        public Address Address { get; set; }

        [RequiresApproval]
        public string HeadFirstName { get; set; }

        [RequiresApproval]
        public string HeadLastName { get; set; }

        public LookupHeadTitle HeadTitle { get; set; }

        [NotMapped]
        public string HeadteacherFullName => StringUtil.ConcatNonEmpties(" ", HeadTitle?.ToString(), HeadFirstName, HeadLastName);

        [RequiresApproval]
        public int? HeadTitleId { get; set; }

        public ContactDetail Contact { get; set; }

        public ContactDetail ContactAlt { get; set; }

        [ForeignKey("EstablishmentType")]
        public int? TypeId { get; set; }


        [NotMapped, RestrictPermission]
        public int? LAESTAB
        {
            get { return string.Concat(LocalAuthorityId, EstablishmentNumber).ToInteger(); }
            set
            {
                if (value.HasValue)
                {
                    var temp = value.ToString();
                    if (temp.Length == 7)
                    {
                        EstablishmentNumber = temp.Substring(3, 4).ToInteger();
                    }
                }
            }
        }

        public int? Easting { get; set; }

        public int? Northing { get; set; }
        
        public DbGeography Location { get; set; }

        private string _fullAddress = null;

        [NotMapped]
        public string FullAddress
        {
            get { return _fullAddress ?? Address?.ToString(); }
            set { _fullAddress = value; }
        }

        public Establishment()
        {
            Contact = new ContactDetail();
            ContactAlt = new ContactDetail();
            Address = new Address();
        }

        public override string ToString() => base.ToString() + $"({Name})";

    }
}
