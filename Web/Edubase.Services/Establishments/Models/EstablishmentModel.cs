using Edubase.Common.Spatial;
using Edubase.Services.Domain;
using System;
using System.Collections.Generic;

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
        public int AdditionalAddressesCount { get; set; }

        #region IEBT properties
        public string Notes { get; set; }
        public DateTime? DateOfTheLastBridgeVisit { get; set; }
        //public DateTime? DateOfTheLastOfstedVisit { get; set; }//OfstedInspectionDate
        public DateTime? DateOfTheLastISIVisit { get; set; }
        public DateTime? DateOfTheLastWelfareVisit { get; set; }
        public DateTime? DateOfTheLastFPVisit { get; set; }
        public DateTime? DateOfTheLastSISVisit { get; set; }
        public DateTime? NextOfstedVisit { get; set; }
        public DateTime? NextGeneralActionRequired { get; set; }
        public DateTime? NextActionRequiredByWEL { get; set; }
        public DateTime? NextActionRequiredByFP { get; set; }
        //public Lookup Inspectorate { get; set; } //InspectorateId
        public int? IndependentSchoolTypeId { get; set; } // LookupIndependentSchoolType
        public string CharityOrganisation { get; set; }
        public int? CharityRegistrationNumber { get; set; }
        public int? TotalNumberOfFullTimePupils { get; set; }
        public int? TotalNumberOfPartTimePupils { get; set; }
        public int? TotalNumberOfPupilsOfCompulsorySchoolAge { get; set; }
        public int? NumberOfSpecialPupilsUnderASENStatementEHCP { get; set; }
        public int? NumberOfSpecialPupilsNotUnderASENStatementEHCP { get; set; }
        public int? TotalNumberOfPupilsInPublicCare { get; set; }
        public int? PTBoysAged2AndUnder { get; set; }
        public int? PTBoysAged3 { get; set; }
        public int? PTBoysAged4A { get; set; }
        public int? PTBoysAged4B { get; set; }
        public int? PTBoysAged4C { get; set; }
        public int? TotalNumberOfBoysInBoardingSchools { get; set; }
        public int? PTGirlsAged2AndUnder { get; set; }
        public int? PTGirlsAged3 { get; set; }
        public int? PTGirlsAged4A { get; set; }
        public int? PTGirlsAged4B { get; set; }
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
        public int? AccomodationChangedId { get; set; }

        #endregion

    }
}
