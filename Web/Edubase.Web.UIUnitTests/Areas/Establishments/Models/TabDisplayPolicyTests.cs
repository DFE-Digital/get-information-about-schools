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
        [MemberData(nameof(EstablishmentTypeTestData))]
        public void EstablishmentTypeId_SetsExpectedGovernanceTab(int establishmentTypeId, bool expectedGovernanceTab)
        {
            var establishmentDisplayEditPolicy = new EstablishmentDisplayEditPolicy
            {
                IEBTDetail = new IEBTDetailDisplayEditPolicy() { Proprietors = new ProprietorFieldList<bool>(), ChairOfProprietor = new ProprietorFieldList<bool>() }
            };

            var sutTabDisplayPolicy = new TabDisplayPolicy(establishmentTypeId, establishmentDisplayEditPolicy, new Mock<IPrincipal>().Object);

            Assert.Equal(expectedGovernanceTab, sutTabDisplayPolicy.Governance);
        }

        /// <summary>
        /// Test IEBT, Location and HelpDesk tab settings depending on display-policy fields being true
        /// </summary>
        /// <param name="displayPolicy"></param>
        /// <param name="expectedTabDisplayPolicy"></param>
        [Theory]
        [MemberData(nameof(DisplayPolicyTestData))]
        public void EstablishmentDisplayEditPolicy_SetsExpectedDependentTabs(EstablishmentDisplayEditPolicy displayPolicy, TabDisplayPolicy expectedTabDisplayPolicy)
        {
            var sutTabDisplayPolicy = new TabDisplayPolicy(It.IsAny<int?>(), displayPolicy, new Mock<IPrincipal>().Object);

            Assert.Equal(expectedTabDisplayPolicy.IEBT, sutTabDisplayPolicy.IEBT);
            Assert.Equal(expectedTabDisplayPolicy.Location, sutTabDisplayPolicy.Location);
            Assert.Equal(expectedTabDisplayPolicy.Helpdesk, sutTabDisplayPolicy.Helpdesk);
        }
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

        public static IEnumerable<object[]> EstablishmentTypeTestData()
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
