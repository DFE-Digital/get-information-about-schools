using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System;
using Edubase.Data.Entity.Lookups;
using Edubase.Data.Entity.ComplexTypes;
using Edubase.Common;
using Edubase.Data.Entity.Permissions;
using Edubase.Data.Identity;

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
        public EstablishmentType EstablishmentType { get; set; }

        public EstablishmentStatus Status { get; set; }

        [RestrictPermission]
        public int? StatusId { get; set; }
        public ReasonEstablishmentOpened ReasonEstablishmentOpened { get; set; }
        public ReasonEstablishmentClosed ReasonEstablishmentClosed { get; set; }
        public int? ReasonEstablishmentOpenedId { get; set; }
        public int? ReasonEstablishmentClosedId { get; set; }
        [RestrictPermission]
        public DateTime? OpenDate { get; set; }

        [RestrictPermission]
        public DateTime? CloseDate { get; set; }

        public EducationPhase EducationPhase { get; set; }

        [RestrictPermission]
        public int? EducationPhaseId { get; set; }

        [RestrictAction(Roles.Academy, ActionType.Approve)]
        public int? StatutoryLowAge { get; set; }

        [RestrictAction(Roles.Academy, ActionType.Approve)]
        public int? StatutoryHighAge { get; set; }
        public ProvisionBoarding ProvisionBoarding { get; set; }
        public int? ProvisionBoardingId { get; set; }
        public ProvisionNursery ProvisionNursery { get; set; }
        public int? ProvisionNurseryId { get; set; }
        public ProvisionOfficialSixthForm ProvisionOfficialSixthForm { get; set; }
        public int? ProvisionOfficialSixthFormId { get; set; }
        public Gender Gender { get; set; }

        [RestrictPermission]
        public int? GenderId { get; set; }
        public ReligiousCharacter ReligiousCharacter { get; set; }
        public int? ReligiousCharacterId { get; set; }
        public ReligiousEthos ReligiousEthos { get; set; }
        public int? ReligiousEthosId { get; set; }
        public Diocese Diocese { get; set; }
        public string DioceseId { get; set; }
        public AdmissionsPolicy AdmissionsPolicy { get; set; }

        [RestrictPermission]
        public int? AdmissionsPolicyId { get; set; }

        [RestrictAction(Roles.Academy, ActionType.Approve)]
        public int? Capacity { get; set; }
        public ProvisionSpecialClasses ProvisionSpecialClasses { get; set; }
        public int? ProvisionSpecialClassesId { get; set; }
        public int? UKPRN { get; set; }
        [Obsolete("This is potentially obsolete due to base.LastUpdatedUtc")]
        public DateTime? LastChangedDate { get; set; }
        public Address Address { get; set; }

        [RequiresApproval]
        public string HeadFirstName { get; set; }

        [RequiresApproval]
        public string HeadLastName { get; set; }

        public HeadTitle HeadTitle { get; set; }

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

        public Establishment()
        {
            Contact = new ContactDetail();
            ContactAlt = new ContactDetail();
            Address = new Address();
        }

    }
}
