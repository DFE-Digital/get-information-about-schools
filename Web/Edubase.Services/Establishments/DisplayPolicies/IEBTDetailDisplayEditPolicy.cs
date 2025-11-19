using System.Collections.Generic;
using System.Linq;
using Edubase.Services.Establishments.EditPolicies;

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
            || TotalNumberOfPartTimeStaff || LowestAnnualRateForDayPupils || HighestAnnualRateForDayPupils || LowestAnnualRateForBoardingPupils || HighestAnnualRateForBoardingPupils
            || ProprietorTypeId || AnyProprietors || AnyChairOfProprietors
            || AccommodationChangedId || PTBoysAged3 || PTBoysAged4B || PTGirlsAged4C || PTBoysAged4A || PTGirlsAged4B || PTGirlsAged4A || PTGirlsAged3 || PTBoysAged4C
            || PTBoysAged2AndUnder || PTGirlsAged2AndUnder || BoardingEstablishmentId
            || TotalFTTeachersTutors || TotalPTTeachersTutors || RegistrationSuspended;

        public bool AnyProprietors => Proprietors.Name || Proprietors.Street || Proprietors.Locality || Proprietors.Address3 ||
                                      Proprietors.Town || Proprietors.CountyId || Proprietors.Postcode || Proprietors.TelephoneNumber || Proprietors.Email;

        public bool AnyChairOfProprietors => ChairOfProprietor.Name || ChairOfProprietor.Street || ChairOfProprietor.Locality || ChairOfProprietor.Address3 ||
                                             ChairOfProprietor.Town || ChairOfProprietor.CountyId || ChairOfProprietor.Postcode || ChairOfProprietor.TelephoneNumber || ChairOfProprietor.Email;

        public string[] GetTrueFieldNames()
        {
            var retVal = new List<string>();
            retVal.AddRange(GetType().GetProperties().Where(x => x.PropertyType == typeof(bool) && (bool) x.GetValue(this, null)).Select(x => x.Name));
            return retVal.ToArray();
        }
    }
}
