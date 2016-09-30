using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System;
using Edubase.Data.Entity.Lookups;
using Edubase.Data.Entity.ComplexTypes;

namespace Edubase.Data.Entity
{
    public class Establishment : EdubaseEntity
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Urn { get; set; }
        public LocalAuthority LocalAuthority { get; set; }
        public int? LocalAuthorityId { get; set; }
        public int? EstablishmentNumber { get; set; }
        public string Name { get; set; }
        [Column("Type")]
        public EstablishmentType EstablishmentType { get; set; }
        public int? TypeId { get; set; }
        public EstablishmentStatus Status { get; set; }
        public int? StatusId { get; set; }
        public ReasonEstablishmentOpened ReasonEstablishmentOpened { get; set; }
        public ReasonEstablishmentClosed ReasonEstablishmentClosed { get; set; }
        public int? ReasonEstablishmentOpenedId { get; set; }
        public int? ReasonEstablishmentClosedId { get; set; }
        public DateTime? OpenDate { get; set; }
        public DateTime? CloseDate { get; set; }
        public EducationPhase EducationPhase { get; set; }
        public int? EducationPhaseId { get; set; }
        public int? StatutoryLowAge { get; set; }
        public int? StatutoryHighAge { get; set; }
        public ProvisionBoarding ProvisionBoarding { get; set; }
        public int? ProvisionBoardingId { get; set; }
        public ProvisionNursery ProvisionNursery { get; set; }
        public int? ProvisionNurseryId { get; set; }
        public ProvisionOfficialSixthForm ProvisionOfficialSixthForm { get; set; }
        public int? ProvisionOfficialSixthFormId { get; set; }
        public Gender Gender { get; set; }
        public int? GenderId { get; set; }
        public ReligiousCharacter ReligiousCharacter { get; set; }
        public int? ReligiousCharacterId { get; set; }
        public ReligiousEthos ReligiousEthos { get; set; }
        public int? ReligiousEthosId { get; set; }
        public Diocese Diocese { get; set; }
        public string DioceseId { get; set; }
        public AdmissionsPolicy AdmissionsPolicy { get; set; }
        public int? AdmissionsPolicyId { get; set; }
        public int? Capacity { get; set; }
        public ProvisionSpecialClasses ProvisionSpecialClasses { get; set; }
        public int? ProvisionSpecialClassesId { get; set; }
        public int? UKPRN { get; set; }

        [Obsolete("This is potentially obsolete due to base.LastUpdatedUtc")]
        public DateTime? LastChangedDate { get; set; }
        public Address Address { get; set; }

        public string HeadFirstName { get; set; }
        public string HeadLastName { get; set; }
        public HeadTitle HeadTitle { get; set; }
        public int? HeadTitleId { get; set; }

        public ContactDetail Contact { get; set; }
        public ContactDetail ContactAlt { get; set; }

        [NotMapped]
        public string LAESTAB => string.Concat(LocalAuthorityId, EstablishmentNumber);

        public Establishment()
        {
            Contact = new ContactDetail();
            ContactAlt = new ContactDetail();
            Address = new Address();
        }

    }
}
