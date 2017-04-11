using Edubase.Common.Spatial;
using Edubase.Services.Domain;
using Edubase.Services.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Edubase.Services.Establishments.Models
{
    public class EstablishmentModel : EstablishmentModelBase
    {
        /// <summary>
        /// List of addresses visible to the principal
        /// </summary>
        public List<AdditionalAddressModel> AdditionalAddresses { get; set; }
        
        public ChildrensCentreLocalAuthorityDto CCLAContactDetail { get; set; }

        public LatLon Location { get; set; }

        public override LatLon Coordinate => Location;

        /// <summary>
        /// The number of additional addresses specified, including
        /// any hidden ones.
        /// </summary>
        public int? AdditionalAddressesCount { get; set; }

        public eGovernanceMode? GovernanceMode { get; set; }

        #region IEBT properties
        public string Notes { get; set; }
        public DateTime? DateOfTheLastBridgeVisit { get; set; }
        //public DateTime? DateOfTheLastOfstedVisit { get; set; }//OfstedInspectionDate

        [DisplayName("Date of the last ISI visit")]
        public DateTime? DateOfTheLastISIVisit { get; set; }

        public DateTime? DateOfTheLastWelfareVisit { get; set; }

        [DisplayName("Date of the last FP visit")]
        public DateTime? DateOfTheLastFPVisit { get; set; }

        [DisplayName("Date of the last SIS visit")]
        public DateTime? DateOfTheLastSISVisit { get; set; }
        public DateTime? NextOfstedVisit { get; set; }
        public DateTime? NextGeneralActionRequired { get; set; }

        [DisplayName("Next action required by WEL")]
        public DateTime? NextActionRequiredByWEL { get; set; }

        [DisplayName("Next action required by FP")]
        public DateTime? NextActionRequiredByFP { get; set; }

        //public Lookup Inspectorate { get; set; } //InspectorateId

        public int? IndependentSchoolTypeId { get; set; } // LookupIndependentSchoolType
        public string CharityOrganisation { get; set; }
        public int? CharityRegistrationNumber { get; set; }
        public int? TotalNumberOfFullTimePupils { get; set; }
        public int? TotalNumberOfPartTimePupils { get; set; }
        public int? TotalNumberOfPupilsOfCompulsorySchoolAge { get; set; }

        [Obsolete("Use SENStat/NoStat")]
        [DisplayName("Total number of special pupils under a SEN statement/ECHP")]
        public int? NumberOfSpecialPupilsUnderASENStatementEHCP { get; set; }

        [Obsolete("Use SENStat/NoStat")]
        [DisplayName("Number of special pupils not under a SEN statement/EHCP")]
        public int? NumberOfSpecialPupilsNotUnderASENStatementEHCP { get; set; }


        public int? TotalNumberOfPupilsInPublicCare { get; set; }

        [DisplayName("PT boys (aged 2 and under)")]
        public int? PTBoysAged2AndUnder { get; set; }

        [DisplayName("PT boys (aged 3)")]
        public int? PTBoysAged3 { get; set; }

        [DisplayName("PT boys (aged 4a)")]
        public int? PTBoysAged4A { get; set; }

        [DisplayName("PT boys (aged 4b)")]
        public int? PTBoysAged4B { get; set; }

        [DisplayName("PT boys (aged 4c)")]
        public int? PTBoysAged4C { get; set; }

        public int? TotalNumberOfBoysInBoardingSchools { get; set; }

        [DisplayName("PT girls (aged 2 and under)")]
        public int? PTGirlsAged2AndUnder { get; set; }

        [DisplayName("PT girls (aged 3)")]
        public int? PTGirlsAged3 { get; set; }

        [DisplayName("PT girls (aged 4a)")]
        public int? PTGirlsAged4A { get; set; }

        [DisplayName("PT girls (aged 4b)")]
        public int? PTGirlsAged4B { get; set; }

        [DisplayName("PT girls (aged 4c)")]
        public int? PTGirlsAged4C { get; set; }

        public int? TotalNumberOfGirlsInBoardingSchools { get; set; }
        public int? TotalNumberOfFullTimeStaff { get; set; }
        public int? TotalNumberOfPartTimeStaff { get; set; }
        public int? LowestAnnualRateForDayPupils { get; set; }
        public int? HighestAnnualRateForDayPupils { get; set; }
        public int? LowestAnnualRateForBoardingPupils { get; set; }
        public int? HighestAnnualRateForBoardingPupils { get; set; }
        //public Lookup BoardingEstablishment { get; set; } //ProvisionBoardingId
        //public string ProprietorsName { get; set; } //ProprietorName
        public string ProprietorsStreet { get; set; }
        public string ProprietorsLocality { get; set; }
        public string ProprietorsAddress3 { get; set; }
        public string ProprietorsTown { get; set; }
        public string ProprietorsCounty { get; set; }
        public string ProprietorsPostcode { get; set; }
        public string ProprietorsTelephoneNumber { get; set; }
        public string ProprietorsFaxNumber { get; set; }
        public string ProprietorsEmail { get; set; }
        public string ProprietorsPreferredJobTitle { get; set; }
        public string ChairOfProprietorsBodyName { get; set; }
        public string ChairOfProprietorsBodyStreet { get; set; }
        public string ChairOfProprietorsBodyLocality { get; set; }
        public string ChairOfProprietorsBodyAddress3 { get; set; }
        public string ChairOfProprietorsBodyTown { get; set; }
        public string ChairOfProprietorsBodyCounty { get; set; }
        public string ChairOfProprietorsBodyPostcode { get; set; }
        public string ChairOfProprietorsBodyTelephoneNumber { get; set; }
        public string ChairOfProprietorsBodyFaxNumber { get; set; }
        public string ChairOfProprietorsBodyEmail { get; set; }
        public string ChairOfProprietorsBodyPreferredJobTitle { get; set; }
        public int? AccommodationChangedId { get; set; }

        #endregion

    }
}
