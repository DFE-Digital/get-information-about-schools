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
        [DisplayName("IEBT Notes")]
        public string Notes { get; set; }

        [DisplayName("Associations")]
        public string Associations { get; set; }

        [DisplayName("Date of the last Ofsted visit")]
        public DateTime? DateOfLastOfstedVisit { get; set; }

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

        public int? ProprietorTypeId { get; set; }
        public List<ProprietorModel> Proprietors { get; set; } = new List<ProprietorModel>();
        public ProprietorModel ChairOfProprietor { get; set; }

        public int? AccommodationChangedId { get; set; }
        public int? BoardingEstablishmentId { get; set; }
        public int? TotalFTTeachersTutors { get; set; }
        public int? TotalPTTeachersTutors { get; set; }
        public string RegistrationSuspended { get; set; }
    }
}
