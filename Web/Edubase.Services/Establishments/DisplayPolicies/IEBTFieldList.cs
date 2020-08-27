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

        [JsonProperty("proprietorsStreet")]
        public T ProprietorsStreet { get; set; }

        [JsonProperty("proprietorsLocality")]
        public T ProprietorsLocality { get; set; }

        [JsonProperty("proprietorsAddress3")]
        public T ProprietorsAddress3 { get; set; }

        [JsonProperty("proprietorsTown")]
        public T ProprietorsTown { get; set; }

        [JsonProperty("proprietorsCountyId")]
        public T ProprietorsCountyId { get; set; }

        [JsonProperty("proprietorsPostcode")]
        public T ProprietorsPostcode { get; set; }

        [JsonProperty("proprietorsTelephoneNumber")]
        public T ProprietorsTelephoneNumber { get; set; }

        [JsonProperty("proprietorsFaxNumber")]
        public T ProprietorsFaxNumber { get; set; }

        [JsonProperty("proprietorsEmail")]
        public T ProprietorsEmail { get; set; }

        [JsonProperty("proprietorsPreferredJobTitle")]
        public T ProprietorsPreferredJobTitle { get; set; }

        [JsonProperty("chairOfProprietorsBodyName")]
        public T ChairOfProprietorsBodyName { get; set; }

        [JsonProperty("chairOfProprietorsBodyStreet")]
        public T ChairOfProprietorsBodyStreet { get; set; }

        [JsonProperty("chairOfProprietorsBodyLocality")]
        public T ChairOfProprietorsBodyLocality { get; set; }

        [JsonProperty("chairOfProprietorsBodyAddress3")]
        public T ChairOfProprietorsBodyAddress3 { get; set; }

        [JsonProperty("chairOfProprietorsBodyTown")]
        public T ChairOfProprietorsBodyTown { get; set; }

        [JsonProperty("chairOfProprietorsBodyCountyId")]
        public T ChairOfProprietorsBodyCountyId { get; set; }

        [JsonProperty("chairOfProprietorsBodyPostcode")]
        public T ChairOfProprietorsBodyPostcode { get; set; }

        [JsonProperty("chairOfProprietorsBodyTelephoneNumber")]
        public T ChairOfProprietorsBodyTelephoneNumber { get; set; }

        [JsonProperty("chairOfProprietorsBodyFaxNumber")]
        public T ChairOfProprietorsBodyFaxNumber { get; set; }

        [JsonProperty("chairOfProprietorsBodyEmail")]
        public T ChairOfProprietorsBodyEmail { get; set; }

        [JsonProperty("chairOfProprietorsBodyPreferredJobTitle")]
        public T ChairOfProprietorsBodyPreferredJobTitle { get; set; }

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

        #endregion
    }
}
