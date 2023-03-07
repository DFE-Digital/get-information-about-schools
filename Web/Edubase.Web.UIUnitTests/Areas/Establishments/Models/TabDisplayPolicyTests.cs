using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Web.Security;
using Edubase.Services.Enums;
using Edubase.Services.Establishments.DisplayPolicies;
using Edubase.Services.Establishments.Models;
using Edubase.Web.UI.Areas.Establishments.Models;
using Moq;
using Xunit;

namespace Edubase.Web.UIUnitTests.Areas.Establishments.Models
{
    using ET = eLookupEstablishmentType;

    public class TabDisplayPolicyTests
    {
        /// <summary>
        /// Test Governance tab setting depending on Establishment type
        /// </summary>
        /// <param name="establishmentTypeId"></param>
        /// <param name="expectedGovernanceTab"></param>
        [Theory]
        [MemberData(nameof(EstablishmentTypeGovernanceTestData))]
        public void EstablishmentTypeId_SetsExpectedGovernanceTab(int establishmentTypeId, bool expectedGovernanceTab)
        {
            var establishmentDisplayEditPolicy = new EstablishmentDisplayEditPolicy
            {
                IEBTDetail = new IEBTDetailDisplayEditPolicy() { Proprietors = new ProprietorFieldList<bool>(), ChairOfProprietor = new ProprietorFieldList<bool>() }
            };

            var sutTabDisplayPolicy = new TabDisplayPolicy(new EstablishmentModel { TypeId = establishmentTypeId }, establishmentDisplayEditPolicy, new Mock<IPrincipal>().Object);

            Assert.Equal(expectedGovernanceTab, sutTabDisplayPolicy.Governance);
        }

        /// <summary>
        /// Test Links tab setting depending on Establishment type
        /// </summary>
        [Theory]
        [MemberData(nameof(EstablishmentTypeChangesAndLinksTestData))]
        public void EstablishmentTypeId_SetsExpectedChangesTab(int establishmentTypeId, bool expectedChangesTab, bool expectedLinksTab)
        {
            var establishmentDisplayEditPolicy = new EstablishmentDisplayEditPolicy
            {
                IEBTDetail = new IEBTDetailDisplayEditPolicy() { Proprietors = new ProprietorFieldList<bool>(), ChairOfProprietor = new ProprietorFieldList<bool>() }
            };

            var sutTabDisplayPolicy = new TabDisplayPolicy(new EstablishmentModel { TypeId = establishmentTypeId }, establishmentDisplayEditPolicy, new Mock<IPrincipal>().Object);

            Assert.Equal(expectedChangesTab, sutTabDisplayPolicy.ChangeHistory);
            Assert.Equal(expectedLinksTab, sutTabDisplayPolicy.Links);
        }

        /// <summary>
        /// Test IEBT, Location and HelpDesk tab settings depending on display-policy fields being true
        /// </summary>
        [Theory]
        [MemberData(nameof(DisplayPolicyTestData))]
        public void EstablishmentDisplayEditPolicy_SetsExpectedDependentTabs(EstablishmentDisplayEditPolicy displayPolicy, TabDisplayPolicy expectedTabDisplayPolicy)
        {
            var establishmentModel = new EstablishmentModel { TypeId = It.IsAny<int>() };
            var sutTabDisplayPolicy = new TabDisplayPolicy(establishmentModel, displayPolicy, new Mock<IPrincipal>().Object);

            Assert.Equal(expectedTabDisplayPolicy.IEBT, sutTabDisplayPolicy.IEBT);
            Assert.Equal(expectedTabDisplayPolicy.Helpdesk, sutTabDisplayPolicy.Helpdesk);
        }

        // test data for EstablishmentDisplayEditPolicy and expected TabDisplayPolicy
        public static IEnumerable<object[]> DisplayPolicyTestData()
        {
            return new List<object[]> {
                new object[] {
                    new EstablishmentDisplayEditPolicy() {
                        HelpdeskNotes = false,
                        // location fields
                        RSCRegionId = false,
                        GovernmentOfficeRegionId = false,
                        AdministrativeDistrictId = false,
                        AdministrativeWardId = false,
                        ParliamentaryConstituencyId = false,
                        UrbanRuralId = false,
                        GSSLAId = false,
                        Easting = false,
                        Northing = false,
                        MSOAId = false,
                        LSOAId = false,
                        // IEBT details
                        IEBTDetail = NoIEBTDetail },
                    new TabDisplayPolicy() {
                        Helpdesk = false,
                        Location = false,
                        IEBT = false }
                    },
                new object[] {
                    new EstablishmentDisplayEditPolicy() {
                        HelpdeskNotes = true,
                        // location fields
                        RSCRegionId = true,
                        GovernmentOfficeRegionId = false,
                        AdministrativeDistrictId = false,
                        AdministrativeWardId = false,
                        ParliamentaryConstituencyId = false,
                        UrbanRuralId = false,
                        GSSLAId = false,
                        Easting = false,
                        Northing = false,
                        MSOAId = false,
                        LSOAId = false,
                        // IEBT details
                        IEBTDetail = IEBTDetail },
                    new TabDisplayPolicy() {
                        Helpdesk = true,
                        Location = true,
                        IEBT = true }
                }
            };
        }

        // data for establishmentType and expected GovernanceTab setting
        public static IEnumerable<object[]> EstablishmentTypeGovernanceTestData()
        {
            return new List<object[]>
            {
                new object[] { ET.Academy1619Converter, true},
                new object[] { ET.Academy1619SponsorLed, true},
                new object[] { ET.AcademyAlternativeProvisionConverter, true},
                new object[] { ET.AcademyAlternativeProvisionSponsorLed, true},
                new object[] { ET.AcademyConverter, true},
                new object[] { ET.AcademySpecialConverter, true},
                new object[] { ET.AcademySpecialSponsorLed, true},
                new object[] { ET.AcademySponsorLed, true },
                new object[] { ET.BritishSchoolsOverseas, false},
                new object[] { ET.ChildrensCentre, false},
                new object[] { ET.ChildrensCentreLinkedSite, false},
                new object[] { ET.CityTechnologyCollege, true},
                new object[] { ET.CommunitySchool, true},
                new object[] { ET.CommunitySpecialSchool, true},
                new object[] { ET.EuropeanSchools, false},
                new object[] { ET.EYSetting, false},
                new object[] { ET.FoundationSchool, true },
                new object[] { ET.FoundationSpecialSchool, true},
                new object[] { ET.FreeSchools, true},
                new object[] { ET.FreeSchools1619, true},
                new object[] { ET.FreeSchoolsAlternativeProvision, true},
                new object[] { ET.FreeSchoolsSpecial, true},
                new object[] { ET.FurtherEducation, false},
                new object[] { ET.HigherEducationInstitutions, false},
                new object[] { ET.IndependentSchoolApprovedForSENPupils, false},
                new object[] { ET.InstitutionFundedByOtherGovernmentDepartment, false},
                new object[] { ET.LANurserySchool, true},
                new object[] { ET.NonmaintainedSpecialSchool, false},
                new object[] { ET.OffshoreSchools, false},
                new object[] { ET.OnlineProvider, false},
                new object[] { ET.OtherIndependentSchool, false},
                new object[] { ET.OtherIndependentSpecialSchool, false},
                new object[] { ET.PlayingForSuccessCentres, false},
                new object[] { ET.PupilReferralUnit, true},
                new object[] { ET.SecureAcademies16to19, false},
                new object[] { ET.SecureUnits, false },
                new object[] { ET.ServiceChildrensEducation, false},
                new object[] { ET.SixthFormCentres, false},
                new object[] { ET.SpecialPost16Institution, false},
                new object[] { ET.StudioSchools, true},
                new object[] { ET.UniversityTechnicalCollege, true},
                new object[] { ET.VoluntaryAidedSchool, true},
                new object[] { ET.VoluntaryControlledSchool, true},
                new object[] { ET.WelshEstablishment, false}
            };
        }

        // data for establishmentType and expected GovernanceTab setting
        public static IEnumerable<object[]> EstablishmentTypeChangesAndLinksTestData()
        {
            return new List<object[]>
            {
                new object[] { ET.Academy1619Converter,true, true},
                new object[] { ET.Academy1619SponsorLed,true, true},
                new object[] { ET.AcademyAlternativeProvisionConverter,true, true},
                new object[] { ET.AcademyAlternativeProvisionSponsorLed,true, true},
                new object[] { ET.AcademyConverter,true, true},
                new object[] { ET.AcademySpecialConverter,true, true},
                new object[] { ET.AcademySpecialSponsorLed,true, true},
                new object[] { ET.AcademySponsorLed, true, true},
                new object[] { ET.BritishSchoolsOverseas, true, true},
                new object[] { ET.ChildrensCentre, true, true},
                new object[] { ET.ChildrensCentreLinkedSite, true, true},
                new object[] { ET.CityTechnologyCollege,true, true},
                new object[] { ET.CommunitySchool,true, true},
                new object[] { ET.CommunitySpecialSchool,true, true},
                new object[] { ET.EuropeanSchools, true, true},
                new object[] { ET.EYSetting, true, true},
                new object[] { ET.FoundationSchool, true, true},
                new object[] { ET.FoundationSpecialSchool,true, true},
                new object[] { ET.FreeSchools,true, true},
                new object[] { ET.FreeSchools1619,true, true},
                new object[] { ET.FreeSchoolsAlternativeProvision,true, true},
                new object[] { ET.FreeSchoolsSpecial,true, true},
                new object[] { ET.FurtherEducation, true, true},
                new object[] { ET.HigherEducationInstitutions, true, true},
                new object[] { ET.IndependentSchoolApprovedForSENPupils, true, true},
                new object[] { ET.InstitutionFundedByOtherGovernmentDepartment, true, true},
                new object[] { ET.LANurserySchool,true, true},
                new object[] { ET.NonmaintainedSpecialSchool, true, true},
                new object[] { ET.OffshoreSchools, true, true},
                new object[] { ET.OnlineProvider, true, true},
                new object[] { ET.OtherIndependentSchool, true, true},
                new object[] { ET.OtherIndependentSpecialSchool, true, true},
                new object[] { ET.PlayingForSuccessCentres, true, true},
                new object[] { ET.PupilReferralUnit,true, true},
                new object[] { ET.SecureAcademies16to19, true, false},
                new object[] { ET.SecureUnits, true, true},
                new object[] { ET.ServiceChildrensEducation, true, true},
                new object[] { ET.SixthFormCentres, true, true},
                new object[] { ET.SpecialPost16Institution, true, true},
                new object[] { ET.StudioSchools,true, true},
                new object[] { ET.UniversityTechnicalCollege,true, true},
                new object[] { ET.VoluntaryAidedSchool,true, true},
                new object[] { ET.VoluntaryControlledSchool,true, true},
                new object[] { ET.WelshEstablishment,true, true}
            };
        }

        public static IEBTDetailDisplayEditPolicy IEBTDetail => new IEBTDetailDisplayEditPolicy()
        {
            Notes = true
        };

        public static IEBTDetailDisplayEditPolicy NoIEBTDetail => new IEBTDetailDisplayEditPolicy
        {
            Notes = false,
            DateOfTheLastBridgeVisit = false,
            DateOfTheLastISIVisit = false,
            DateOfTheLastWelfareVisit = false,
            DateOfTheLastFPVisit = false,
            DateOfTheLastSISVisit = false,
            NextOfstedVisit = false,
            NextGeneralActionRequired = false,
            NextActionRequiredByWEL = false,
            NextActionRequiredByFP = false,
            IndependentSchoolTypeId = false,
            CharityOrganisation = false,
            CharityRegistrationNumber = false,
            TotalNumberOfFullTimePupils = false,
            TotalNumberOfPartTimePupils = false,
            TotalNumberOfPupilsOfCompulsorySchoolAge = false,
            TotalNumberOfPupilsInPublicCare = false,
            TotalNumberOfBoysInBoardingSchools = false,
            TotalNumberOfGirlsInBoardingSchools = false,
            TotalNumberOfFullTimeStaff = false,
            TotalNumberOfPartTimeStaff = false,
            LowestAnnualRateForDayPupils = false,
            HighestAnnualRateForDayPupils = false,
            LowestAnnualRateForBoardingPupils = false,
            HighestAnnualRateForBoardingPupils = false,
            ProprietorTypeId = false,
            Proprietors = new ProprietorFieldList<bool>(),
            ChairOfProprietor = new ProprietorFieldList<bool>(),
            AccommodationChangedId = false,
            PTBoysAged3 = false,
            PTBoysAged4B = false,
            PTGirlsAged4C = false,
            PTBoysAged4A = false,
            PTGirlsAged4B = false,
            PTGirlsAged4A = false,
            PTGirlsAged3 = false,
            PTBoysAged4C = false,
            PTBoysAged2AndUnder = false,
            PTGirlsAged2AndUnder = false,
            BoardingEstablishmentId = false,
            TotalFTTeachersTutors = false,
            TotalPTTeachersTutors = false
        };
    }
}
