using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System;
using Edubase.Data.Entity.Lookups;
using Edubase.Data.Entity.ComplexTypes;
using Edubase.Common;
using Edubase.Data.Entity.Permissions;
using Edubase.Data.Identity;
using System.Data.Entity.Spatial;
using Newtonsoft.Json;

namespace Edubase.Data.Entity
{
    [Serializable]
    public class Establishment : EdubaseEntity
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Urn { get; set; }
        
        public LocalAuthority LocalAuthority { get; set; }

        
        public int? LocalAuthorityId { get; set; }

        
        public int? EstablishmentNumber { get; set; }

        
        public string Name { get; set; }

        [Column("Type")]
        public LookupEstablishmentType EstablishmentType { get; set; }
        
        public LookupEstablishmentStatus Status { get; set; }

        
        public int? StatusId { get; set; }

        public LookupReasonEstablishmentOpened ReasonEstablishmentOpened { get; set; }

        public LookupReasonEstablishmentClosed ReasonEstablishmentClosed { get; set; }

        public int? ReasonEstablishmentOpenedId { get; set; }

        public int? ReasonEstablishmentClosedId { get; set; }

        
        public DateTime? OpenDate { get; set; }

        
        public DateTime? CloseDate { get; set; }

        public LookupEducationPhase EducationPhase { get; set; }

        
        public int? EducationPhaseId { get; set; }

        public int? StatutoryLowAge { get; set; }

        public int? StatutoryHighAge { get; set; }

        public LookupProvisionBoarding ProvisionBoarding { get; set; }

        public int? ProvisionBoardingId { get; set; }

        public LookupProvisionNursery ProvisionNursery { get; set; }

        public int? ProvisionNurseryId { get; set; }

        public LookupProvisionOfficialSixthForm ProvisionOfficialSixthForm { get; set; }

        public int? ProvisionOfficialSixthFormId { get; set; }

        public LookupGender Gender { get; set; }

        
        public int? GenderId { get; set; }

        public LookupReligiousCharacter ReligiousCharacter { get; set; }

        public int? ReligiousCharacterId { get; set; }

        public LookupReligiousEthos ReligiousEthos { get; set; }

        public int? ReligiousEthosId { get; set; }

        public LookupDiocese Diocese { get; set; }
        
        public int? DioceseId { get; set; }

        public LookupAdmissionsPolicy AdmissionsPolicy { get; set; }

        
        public int? AdmissionsPolicyId { get; set; }

        public int? Capacity { get; set; }

        public LookupProvisionSpecialClasses ProvisionSpecialClasses { get; set; }

        public int? ProvisionSpecialClassesId { get; set; }

        public int? UKPRN { get; set; }

        public DateTime? LastChangedDate { get; set; }

        public Address Address { get; set; }

        
        public string HeadFirstName { get; set; }

        
        public string HeadLastName { get; set; }

        public LookupHeadTitle HeadTitle { get; set; }

        [NotMapped]
        public string HeadteacherFullName => StringUtil.ConcatNonEmpties(" ", HeadTitle?.ToString()?.RemoveSubstring("Unknown"), HeadFirstName, HeadLastName);

        
        public int? HeadTitleId { get; set; }

        public string HeadEmailAddress { get; set; }

        public ContactDetail Contact { get; set; }

        public ContactDetail ContactAlt { get; set; }

        [ForeignKey("EstablishmentType")]
        public int? TypeId { get; set; }


        [NotMapped]
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

        public LookupEstablishmentTypeGroup EstablishmentTypeGroup { get; set; }
        public int? EstablishmentTypeGroupId { get; set; }

        public byte? OfstedRating { get; set; }
        public DateTime? OfstedInspectionDate { get; set; }

        public LookupInspectorate Inspectorate { get; set; }
        public int? InspectorateId { get; set; }

        public LookupSection41Approved Section41Approved { get; set; }
        public int? Section41ApprovedId { get; set; }



        public string ProprietorName { get; set; }


        public int? SENStat { get; set; }
        public int? SENNoStat { get; set; }


        public LookupSpecialEducationNeeds SEN1 { get; set; }
        public LookupSpecialEducationNeeds SEN2 { get; set; }
        public LookupSpecialEducationNeeds SEN3 { get; set; }
        public LookupSpecialEducationNeeds SEN4 { get; set; }
        public int? SEN1Id { get; set; }
        public int? SEN2Id { get; set; }
        public int? SEN3Id { get; set; }
        public int? SEN4Id { get; set; }

        public LookupTeenageMothersProvision TeenageMothersProvision { get; set; }
        public int? TeenageMothersProvisionId { get; set; }
        public int? TeenageMothersCapacity { get; set; }

        public LookupChildcareFacilities ChildcareFacilities { get; set; }
        public int? ChildcareFacilitiesId { get; set; }

        public LookupPRUSEN PRUSEN { get; set; }
        public int? PRUSENId { get; set; }

        public LookupPRUEBD PRUEBD { get; set; }
        public int? PRUEBDId { get; set; }

        public int? PlacesPRU { get; set; }

        public LookupPruFulltimeProvision PruFulltimeProvision { get; set; }
        public int? PruFulltimeProvisionId { get; set; }

        public LookupPruEducatedByOthers PruEducatedByOthers { get; set; }
        public int? PruEducatedByOthersId { get; set; }

        public LookupTypeOfResourcedProvision TypeOfResourcedProvision { get; set; }
        public int? TypeOfResourcedProvisionId { get; set; }

        public int? ResourcedProvisionOnRoll { get; set; }
        public int? ResourcedProvisionCapacity { get; set; }

        public LookupGovernmentOfficeRegion GovernmentOfficeRegion { get; set; }
        public int? GovernmentOfficeRegionId { get; set; }

        public LookupDistrictAdministrative AdministrativeDistrict { get; set; }
        public int? AdministrativeDistrictId { get; set; }

        public LookupAdministrativeWard AdministrativeWard { get; set; }
        public int? AdministrativeWardId { get; set; }

        public LookupParliamentaryConstituency ParliamentaryConstituency { get; set; }
        public int? ParliamentaryConstituencyId { get; set; }

        public LookupUrbanRural UrbanRural { get; set; }
        public int? UrbanRuralId { get; set; }

        public LookupGSSLA GSSLA { get; set; }
        public int? GSSLAId { get; set; }


        public LookupCASWard CASWard { get; set; }
        public int? CASWardId { get; set; }


        public LookupMSOA MSOA { get; set; }
        public int? MSOAId { get; set; }
        public LookupLSOA LSOA { get; set; }
        public int? LSOAId { get; set; }

        public LookupFurtherEducationType FurtherEducationType { get; set; }
        public int? FurtherEducationTypeId { get; set; }

        public LookupCCGovernance CCGovernance { get; set; }
        public int? CCGovernanceId { get; set; }

        public string CCGovernanceDetail { get; set; }

        public LookupCCOperationalHours CCOperationalHours { get; set; }
        public int? CCOperationalHoursId { get; set; }

        public LookupCCPhaseType CCPhaseType { get; set; }
        public int? CCPhaseTypeId { get; set; }

        public LookupCCGroupLead CCGroupLead { get; set; }
        public int? CCGroupLeadId { get; set; }

        public int? CCDisadvantagedAreaId { get; set; }
        public LookupCCDisadvantagedArea CCDisadvantagedArea { get; set; }

        public LookupDirectProvisionOfEarlyYears CCDirectProvisionOfEarlyYears { get; set; }
        public int? CCDirectProvisionOfEarlyYearsId { get; set; }

        public LookupDeliveryModel CCDeliveryModel { get; set; }
        public int? CCDeliveryModelId { get; set; }

        public int? CCUnder5YearsOfAgeCount { get; set; }

        public int? SenUnitOnRoll { get; set; }
        public int? SenUnitCapacity { get; set; }

        /// <summary>
        /// Regional School Comissioner
        /// </summary>
        public LocalAuthority RSCRegion { get; set; }
        public int? RSCRegionId { get; set; }

        public LookupInspectorateName BSOInspectorate { get; set; }
        public int? BSOInspectorateId { get; set; }
        public string BSOInspectorateReportUrl { get; set; }
        public DateTime? BSODateOfLastInspectionVisit { get; set; }
        public DateTime? BSODateOfNextInspectionVisit { get; set; }

        private string _fullAddress = null;

        [NotMapped]
        public string FullAddress
        {
            get { return _fullAddress ?? Address?.ToString(); }
            set { _fullAddress = value; }
        }

        /// <summary>
        /// Addtional addresses data (JSON encoded object)
        /// - No need for querying or indexing of this data, hence JSON is fine.
        /// </summary>
        public string AdditionalAddresses { get; set; }

        public Establishment()
        {
            Contact = new ContactDetail();
            ContactAlt = new ContactDetail();
            Address = new Address();
        }

        public override string ToString() => base.ToString() + $"({Name})";

    }
}
