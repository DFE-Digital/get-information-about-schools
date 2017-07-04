using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edubase.Services.Establishments.Models
{
    public class IEBTModel
    {
        public string Notes { get; set; }
        public DateTime? DateOfTheLastBridgeVisit { get; set; }

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

        public int? IndependentSchoolTypeId { get; set; }
        public string CharityOrganisation { get; set; }
        public int? CharityRegistrationNumber { get; set; }
        public int? TotalNumberOfFullTimePupils { get; set; }
        public int? TotalNumberOfPartTimePupils { get; set; }
        public int? TotalNumberOfPupilsOfCompulsorySchoolAge { get; set; }

        public int? TotalNumberOfPupilsInPublicCare { get; set; }

        [DisplayName("PT boys (aged 2 and under)"), JsonProperty("PTBoysAged2AndUnder")]
        public int? PTBoysAged2AndUnder { get; set; }

        [DisplayName("PT boys (aged 3)"), JsonProperty("PTBoysAged3")]
        public int? PTBoysAged3 { get; set; }

        [DisplayName("PT boys (aged 4a)"), JsonProperty("PTBoysAged4A")]
        public int? PTBoysAged4A { get; set; }

        [DisplayName("PT boys (aged 4b)"), JsonProperty("PTBoysAged4B")]
        public int? PTBoysAged4B { get; set; }

        [DisplayName("PT boys (aged 4c)"), JsonProperty("PTBoysAged4C")]
        public int? PTBoysAged4C { get; set; }

        public int? TotalNumberOfBoysInBoardingSchools { get; set; }

        [DisplayName("PT girls (aged 2 and under)"), JsonProperty("PTGirlsAged2AndUnder")]
        public int? PTGirlsAged2AndUnder { get; set; }

        [DisplayName("PT girls (aged 3)"), JsonProperty("PTGirlsAged3")]
        public int? PTGirlsAged3 { get; set; }

        [DisplayName("PT girls (aged 4a)"), JsonProperty("PTGirlsAged4A")]
        public int? PTGirlsAged4A { get; set; }

        [DisplayName("PT girls (aged 4b)"), JsonProperty("PTGirlsAged4B")]
        public int? PTGirlsAged4B { get; set; }

        [DisplayName("PT girls (aged 4c)"), JsonProperty("PTGirlsAged4C")]
        public int? PTGirlsAged4C { get; set; }

        public int? TotalNumberOfGirlsInBoardingSchools { get; set; }
        public int? TotalNumberOfFullTimeStaff { get; set; }
        public int? TotalNumberOfPartTimeStaff { get; set; }
        public int? LowestAnnualRateForDayPupils { get; set; }
        public int? HighestAnnualRateForDayPupils { get; set; }
        public int? LowestAnnualRateForBoardingPupils { get; set; }
        public int? HighestAnnualRateForBoardingPupils { get; set; }

        public string ProprietorsStreet { get; set; }
        public string ProprietorsLocality { get; set; }
        public string ProprietorsAddress3 { get; set; }
        public string ProprietorsTown { get; set; }
        public int? ProprietorsCountyId { get; set; }
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
        public int? ChairOfProprietorsBodyCountyId { get; set; }
        public string ChairOfProprietorsBodyPostcode { get; set; }
        public string ChairOfProprietorsBodyTelephoneNumber { get; set; }
        public string ChairOfProprietorsBodyFaxNumber { get; set; }
        public string ChairOfProprietorsBodyEmail { get; set; }
        public string ChairOfProprietorsBodyPreferredJobTitle { get; set; }
        public int? AccommodationChangedId { get; set; }
    }
}
