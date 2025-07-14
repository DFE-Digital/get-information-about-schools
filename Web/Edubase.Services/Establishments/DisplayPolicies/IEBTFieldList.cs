using Newtonsoft.Json;

namespace Edubase.Services.Establishments.DisplayPolicies
{
    public class IEBTFieldList<T>
    {
        #region Fields

        [JsonProperty("notes")]
        public T Notes { get; set; }

        [JsonProperty("associations")]
        public T Associations { get; set; }

        [JsonProperty("dateOfLastOfstedVisit")]
        public T DateOfLastOfstedVisit { get; set; }

        [JsonProperty("boardingEstablishmentId")]
        public T BoardingEstablishmentId { get; set; }

        [JsonProperty("dateOfTheLastBridgeVisit")]
        public T DateOfTheLastBridgeVisit { get; set; }

        [JsonProperty("dateOfTheLastISIVisit")]
        public T DateOfTheLastISIVisit { get; set; }

        [JsonProperty("dateOfTheLastWelfareVisit")]
        public T DateOfTheLastWelfareVisit { get; set; }

        [JsonProperty("dateOfTheLastFPVisit")]
        public T DateOfTheLastFPVisit { get; set; }

        [JsonProperty("dateOfTheLastSISVisit")]
        public T DateOfTheLastSISVisit { get; set; }

        [JsonProperty("nextOfstedVisit")]
        public T NextOfstedVisit { get; set; }

        [JsonProperty("nextGeneralActionRequired")]
        public T NextGeneralActionRequired { get; set; }

        [JsonProperty("nextActionRequiredByWEL")]
        public T NextActionRequiredByWEL { get; set; }

        [JsonProperty("nextActionRequiredByFP")]
        public T NextActionRequiredByFP { get; set; }

        [JsonProperty("independentSchoolTypeId")]
        public T IndependentSchoolTypeId { get; set; }

        [JsonProperty("charityOrganisation")]
        public T CharityOrganisation { get; set; }

        [JsonProperty("charityRegistrationNumber")]
        public T CharityRegistrationNumber { get; set; }

        [JsonProperty("totalNumberOfFullTimePupils")]
        public T TotalNumberOfFullTimePupils { get; set; }

        [JsonProperty("totalNumberOfPartTimePupils")]
        public T TotalNumberOfPartTimePupils { get; set; }

        [JsonProperty("totalNumberOfPupilsOfCompulsorySchoolAge")]
        public T TotalNumberOfPupilsOfCompulsorySchoolAge { get; set; }

        [JsonProperty("totalNumberOfPupilsInPublicCare")]
        public T TotalNumberOfPupilsInPublicCare { get; set; }

        [JsonProperty("totalNumberOfBoysInBoardingSchools")]
        public T TotalNumberOfBoysInBoardingSchools { get; set; }

        [JsonProperty("totalNumberOfGirlsInBoardingSchools")]
        public T TotalNumberOfGirlsInBoardingSchools { get; set; }

        [JsonProperty("totalNumberOfFullTimeStaff")]
        public T TotalNumberOfFullTimeStaff { get; set; }

        [JsonProperty("totalNumberOfPartTimeStaff")]
        public T TotalNumberOfPartTimeStaff { get; set; }

        [JsonProperty("lowestAnnualRateForDayPupils")]
        public T LowestAnnualRateForDayPupils { get; set; }

        [JsonProperty("highestAnnualRateForDayPupils")]
        public T HighestAnnualRateForDayPupils { get; set; }

        [JsonProperty("lowestAnnualRateForBoardingPupils")]
        public T LowestAnnualRateForBoardingPupils { get; set; }

        [JsonProperty("highestAnnualRateForBoardingPupils")]
        public T HighestAnnualRateForBoardingPupils { get; set; }

        [JsonProperty("proprietorTypeId")]
        public T ProprietorTypeId { get; set; }

        [JsonProperty("proprietors")]
        public ProprietorFieldList<T> Proprietors { get; set; } = new ProprietorFieldList<T>();

        [JsonProperty("chairOfProprietor")]
        public ProprietorFieldList<T> ChairOfProprietor { get; set; } = new ProprietorFieldList<T>();

        [JsonProperty("accommodationChangedId")]
        public T AccommodationChangedId { get; set; }

        [JsonProperty("PTBoysAged3")]
        public T PTBoysAged3 { get; set; }

        [JsonProperty("PTBoysAged4B")]
        public T PTBoysAged4B { get; set; }

        [JsonProperty("PTGirlsAged4C")]
        public T PTGirlsAged4C { get; set; }

        [JsonProperty("PTBoysAged4A")]
        public T PTBoysAged4A { get; set; }

        [JsonProperty("PTGirlsAged4B")]
        public T PTGirlsAged4B { get; set; }

        [JsonProperty("PTGirlsAged4A")]
        public T PTGirlsAged4A { get; set; }

        [JsonProperty("PTGirlsAged3")]
        public T PTGirlsAged3 { get; set; }

        [JsonProperty("PTBoysAged4C")]
        public T PTBoysAged4C { get; set; }

        [JsonProperty("PTBoysAged2AndUnder")]
        public T PTBoysAged2AndUnder { get; set; }

        [JsonProperty("PTGirlsAged2AndUnder")]
        public T PTGirlsAged2AndUnder { get; set; }

        [JsonProperty("totalFTTeachersTutors")]
        public T TotalFTTeachersTutors { get; set; }

        [JsonProperty("totalPTTeachersTutors")]
        public T TotalPTTeachersTutors { get; set; }

        [JsonProperty("registrationSuspendedId")]
        public T RegistrationSuspended { get; set; }

        #endregion
    }
}
