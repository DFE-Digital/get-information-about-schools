using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edubase.Services.Establishments.DisplayPolicies
{
    public class IEBTDetailDisplayEditPolicy
    {
        [JsonProperty("notes")]
        public bool Notes { get; set; }

        [JsonProperty("associations")]
        public bool Associations { get; set; }

        [JsonProperty("dateOfLastOfstedVisit")]
        public bool DateOfLastOfstedVisit { get; set; }

        [JsonProperty("dateOfTheLastBridgeVisit")]
        public bool DateOfTheLastBridgeVisit { get; set; }

        [JsonProperty("dateOfTheLastISIVisit")]
        public bool DateOfTheLastISIVisit { get; set; }

        [JsonProperty("dateOfTheLastWelfareVisit")]
        public bool DateOfTheLastWelfareVisit { get; set; }

        [JsonProperty("dateOfTheLastFPVisit")]
        public bool DateOfTheLastFPVisit { get; set; }

        [JsonProperty("dateOfTheLastSISVisit")]
        public bool DateOfTheLastSISVisit { get; set; }

        [JsonProperty("nextOfstedVisit")]
        public bool NextOfstedVisit { get; set; }

        [JsonProperty("nextGeneralActionRequired")]
        public bool NextGeneralActionRequired { get; set; }

        [JsonProperty("nextActionRequiredByWEL")]
        public bool NextActionRequiredByWEL { get; set; }

        [JsonProperty("nextActionRequiredByFP")]
        public bool NextActionRequiredByFP { get; set; }

        [JsonProperty("independentSchoolTypeId")]
        public bool IndependentSchoolTypeId { get; set; }

        [JsonProperty("charityOrganisation")]
        public bool CharityOrganisation { get; set; }

        [JsonProperty("charityRegistrationNumber")]
        public bool CharityRegistrationNumber { get; set; }

        [JsonProperty("totalNumberOfFullTimePupils")]
        public bool TotalNumberOfFullTimePupils { get; set; }

        [JsonProperty("totalNumberOfPartTimePupils")]
        public bool TotalNumberOfPartTimePupils { get; set; }

        [JsonProperty("totalNumberOfPupilsOfCompulsorySchoolAge")]
        public bool TotalNumberOfPupilsOfCompulsorySchoolAge { get; set; }

        [JsonProperty("totalNumberOfPupilsInPublicCare")]
        public bool TotalNumberOfPupilsInPublicCare { get; set; }

        [JsonProperty("totalNumberOfBoysInBoardingSchools")]
        public bool TotalNumberOfBoysInBoardingSchools { get; set; }

        [JsonProperty("totalNumberOfGirlsInBoardingSchools")]
        public bool TotalNumberOfGirlsInBoardingSchools { get; set; }

        [JsonProperty("totalNumberOfFullTimeStaff")]
        public bool TotalNumberOfFullTimeStaff { get; set; }

        [JsonProperty("totalNumberOfPartTimeStaff")]
        public bool TotalNumberOfPartTimeStaff { get; set; }

        [JsonProperty("lowestAnnualRateForDayPupils")]
        public bool LowestAnnualRateForDayPupils { get; set; }

        [JsonProperty("highestAnnualRateForDayPupils")]
        public bool HighestAnnualRateForDayPupils { get; set; }

        [JsonProperty("lowestAnnualRateForBoardingPupils")]
        public bool LowestAnnualRateForBoardingPupils { get; set; }

        [JsonProperty("highestAnnualRateForBoardingPupils")]
        public bool HighestAnnualRateForBoardingPupils { get; set; }

        [JsonProperty("proprietorsStreet")]
        public bool ProprietorsStreet { get; set; }

        [JsonProperty("proprietorsLocality")]
        public bool ProprietorsLocality { get; set; }

        [JsonProperty("proprietorsAddress3")]
        public bool ProprietorsAddress3 { get; set; }

        [JsonProperty("proprietorsTown")]
        public bool ProprietorsTown { get; set; }

        [JsonProperty("proprietorsCountyId")]
        public bool ProprietorsCountyId { get; set; }

        [JsonProperty("proprietorsPostcode")]
        public bool ProprietorsPostcode { get; set; }

        [JsonProperty("proprietorsTelephoneNumber")]
        public bool ProprietorsTelephoneNumber { get; set; }

        [JsonProperty("proprietorsFaxNumber")]
        public bool ProprietorsFaxNumber { get; set; }

        [JsonProperty("proprietorsEmail")]
        public bool ProprietorsEmail { get; set; }

        [JsonProperty("proprietorsPreferredJobTitle")]
        public bool ProprietorsPreferredJobTitle { get; set; }

        [JsonProperty("chairOfProprietorsBodyName")]
        public bool ChairOfProprietorsBodyName { get; set; }

        [JsonProperty("chairOfProprietorsBodyStreet")]
        public bool ChairOfProprietorsBodyStreet { get; set; }

        [JsonProperty("chairOfProprietorsBodyLocality")]
        public bool ChairOfProprietorsBodyLocality { get; set; }

        [JsonProperty("chairOfProprietorsBodyAddress3")]
        public bool ChairOfProprietorsBodyAddress3 { get; set; }

        [JsonProperty("chairOfProprietorsBodyTown")]
        public bool ChairOfProprietorsBodyTown { get; set; }

        [JsonProperty("chairOfProprietorsBodyCountyId")]
        public bool ChairOfProprietorsBodyCountyId { get; set; }

        [JsonProperty("chairOfProprietorsBodyPostcode")]
        public bool ChairOfProprietorsBodyPostcode { get; set; }

        [JsonProperty("chairOfProprietorsBodyTelephoneNumber")]
        public bool ChairOfProprietorsBodyTelephoneNumber { get; set; }

        [JsonProperty("chairOfProprietorsBodyFaxNumber")]
        public bool ChairOfProprietorsBodyFaxNumber { get; set; }

        [JsonProperty("chairOfProprietorsBodyEmail")]
        public bool ChairOfProprietorsBodyEmail { get; set; }

        [JsonProperty("chairOfProprietorsBodyPreferredJobTitle")]
        public bool ChairOfProprietorsBodyPreferredJobTitle { get; set; }

        [JsonProperty("accommodationChangedId")]
        public bool AccommodationChangedId { get; set; }

        [JsonProperty("PTBoysAged3")]
        public bool PTBoysAged3 { get; set; }

        [JsonProperty("PTBoysAged4B")]
        public bool PTBoysAged4B { get; set; }

        [JsonProperty("PTGirlsAged4C")]
        public bool PTGirlsAged4C { get; set; }

        [JsonProperty("PTBoysAged4A")]
        public bool PTBoysAged4A { get; set; }

        [JsonProperty("PTGirlsAged4B")]
        public bool PTGirlsAged4B { get; set; }

        [JsonProperty("PTGirlsAged4A")]
        public bool PTGirlsAged4A { get; set; }

        [JsonProperty("PTGirlsAged3")]
        public bool PTGirlsAged3 { get; set; }

        [JsonProperty("PTBoysAged4C")]
        public bool PTBoysAged4C { get; set; }

        [JsonProperty("PTBoysAged2AndUnder")]
        public bool PTBoysAged2AndUnder { get; set; }

        [JsonProperty("PTGirlsAged2AndUnder")]
        public bool PTGirlsAged2AndUnder { get; set; }

        /// <summary>
        /// Returns whether any fields within this display policy are true
        /// </summary>
        /// <returns></returns>
        public bool Any() => Notes || DateOfTheLastBridgeVisit || DateOfTheLastISIVisit || DateOfTheLastWelfareVisit || DateOfTheLastFPVisit || DateOfTheLastSISVisit || NextOfstedVisit || NextGeneralActionRequired 
            || NextActionRequiredByWEL || NextActionRequiredByFP || IndependentSchoolTypeId || CharityOrganisation || CharityRegistrationNumber || TotalNumberOfFullTimePupils || TotalNumberOfPartTimePupils || TotalNumberOfPupilsOfCompulsorySchoolAge 
            || TotalNumberOfPupilsInPublicCare || TotalNumberOfBoysInBoardingSchools || TotalNumberOfGirlsInBoardingSchools || TotalNumberOfFullTimeStaff 
            || TotalNumberOfPartTimeStaff || LowestAnnualRateForDayPupils || HighestAnnualRateForDayPupils || LowestAnnualRateForBoardingPupils || HighestAnnualRateForBoardingPupils || ProprietorsStreet || ProprietorsLocality || ProprietorsAddress3 
            || ProprietorsTown || ProprietorsCountyId || ProprietorsPostcode || ProprietorsTelephoneNumber || ProprietorsFaxNumber || ProprietorsEmail || ProprietorsPreferredJobTitle || ChairOfProprietorsBodyName || ChairOfProprietorsBodyStreet 
            || ChairOfProprietorsBodyLocality || ChairOfProprietorsBodyAddress3 || ChairOfProprietorsBodyTown || ChairOfProprietorsBodyCountyId || ChairOfProprietorsBodyPostcode || ChairOfProprietorsBodyTelephoneNumber || ChairOfProprietorsBodyFaxNumber 
            || ChairOfProprietorsBodyEmail || ChairOfProprietorsBodyPreferredJobTitle || AccommodationChangedId || PTBoysAged3 || PTBoysAged4B || PTGirlsAged4C || PTBoysAged4A || PTGirlsAged4B || PTGirlsAged4A || PTGirlsAged3 || PTBoysAged4C 
            || PTBoysAged2AndUnder || PTGirlsAged2AndUnder;
    }
}
