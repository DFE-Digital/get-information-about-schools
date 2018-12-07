using System.Collections.Generic;
using System.Linq;

namespace Edubase.Services.Establishments.DisplayPolicies
{
    public class IEBTDetailDisplayEditPolicy : IEBTFieldList<bool>
    {
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

        public string[] GetTrueFieldNames()
        {
            var retVal = new List<string>();
            retVal.AddRange(GetType().GetProperties().Where(x => x.PropertyType == typeof(bool) && (bool) x.GetValue(this, null)).Select(x => x.Name));
            return retVal.ToArray();
        }
    }
}
