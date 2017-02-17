namespace Edubase.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EstabIEBTfields : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Establishment", "Notes", c => c.String());
            AddColumn("dbo.Establishment", "DateOfTheLastBridgeVisit", c => c.DateTime(precision: 7, storeType: "datetime2"));
            AddColumn("dbo.Establishment", "DateOfTheLastISIVisit", c => c.DateTime(precision: 7, storeType: "datetime2"));
            AddColumn("dbo.Establishment", "DateOfTheLastWelfareVisit", c => c.DateTime(precision: 7, storeType: "datetime2"));
            AddColumn("dbo.Establishment", "DateOfTheLastFPVisit", c => c.DateTime(precision: 7, storeType: "datetime2"));
            AddColumn("dbo.Establishment", "DateOfTheLastSISVisit", c => c.DateTime(precision: 7, storeType: "datetime2"));
            AddColumn("dbo.Establishment", "NextOfstedVisit", c => c.DateTime(precision: 7, storeType: "datetime2"));
            AddColumn("dbo.Establishment", "NextGeneralActionRequired", c => c.DateTime(precision: 7, storeType: "datetime2"));
            AddColumn("dbo.Establishment", "NextActionRequiredByWEL", c => c.DateTime(precision: 7, storeType: "datetime2"));
            AddColumn("dbo.Establishment", "NextActionRequiredByFP", c => c.DateTime(precision: 7, storeType: "datetime2"));
            AddColumn("dbo.Establishment", "IndependentSchoolTypeId", c => c.Int());
            AddColumn("dbo.Establishment", "CharityOrganisation", c => c.String());
            AddColumn("dbo.Establishment", "CharityRegistrationNumber", c => c.Int());
            AddColumn("dbo.Establishment", "TotalNumberOfFullTimePupils", c => c.Int());
            AddColumn("dbo.Establishment", "TotalNumberOfPartTimePupils", c => c.Int());
            AddColumn("dbo.Establishment", "TotalNumberOfPupilsOfCompulsorySchoolAge", c => c.Int());
            AddColumn("dbo.Establishment", "NumberOfSpecialPupilsUnderASENStatementEHCP", c => c.Int());
            AddColumn("dbo.Establishment", "NumberOfSpecialPupilsNotUnderASENStatementEHCP", c => c.Int());
            AddColumn("dbo.Establishment", "TotalNumberOfPupilsInPublicCare", c => c.Int());
            AddColumn("dbo.Establishment", "PTBoysAged2AndUnder", c => c.Int());
            AddColumn("dbo.Establishment", "PTBoysAged3", c => c.Int());
            AddColumn("dbo.Establishment", "PTBoysAged4A", c => c.Int());
            AddColumn("dbo.Establishment", "PTBoysAged4B", c => c.Int());
            AddColumn("dbo.Establishment", "PTBoysAged4C", c => c.Int());
            AddColumn("dbo.Establishment", "TotalNumberOfBoysInBoardingSchools", c => c.Int());
            AddColumn("dbo.Establishment", "PTGirlsAged2AndUnder", c => c.Int());
            AddColumn("dbo.Establishment", "PTGirlsAged3", c => c.Int());
            AddColumn("dbo.Establishment", "PTGirlsAged4A", c => c.Int());
            AddColumn("dbo.Establishment", "PTGirlsAged4B", c => c.Int());
            AddColumn("dbo.Establishment", "PTGirlsAged4C", c => c.Int());
            AddColumn("dbo.Establishment", "TotalNumberOfGirlsInBoardingSchools", c => c.Int());
            AddColumn("dbo.Establishment", "TotalNumberOfFullTimeStaff", c => c.Int());
            AddColumn("dbo.Establishment", "TotalNumberOfPartTimeStaff", c => c.Int());
            AddColumn("dbo.Establishment", "LowestAnnualRateForDayPupils", c => c.Int());
            AddColumn("dbo.Establishment", "HighestAnnualRateForDayPupils", c => c.Int());
            AddColumn("dbo.Establishment", "LowestAnnualRateForBoardingPupils", c => c.Int());
            AddColumn("dbo.Establishment", "HighestAnnualRateForBoardingPupils", c => c.Int());
            AddColumn("dbo.Establishment", "ProprietorsStreet", c => c.String());
            AddColumn("dbo.Establishment", "ProprietorsLocality", c => c.String());
            AddColumn("dbo.Establishment", "ProprietorsAddress3", c => c.String());
            AddColumn("dbo.Establishment", "ProprietorsTown", c => c.String());
            AddColumn("dbo.Establishment", "ProprietorsCounty", c => c.String());
            AddColumn("dbo.Establishment", "ProprietorsPostcode", c => c.String());
            AddColumn("dbo.Establishment", "ProprietorsTelephoneNumber", c => c.String());
            AddColumn("dbo.Establishment", "ProprietorsFaxNumber", c => c.String());
            AddColumn("dbo.Establishment", "ProprietorsEmail", c => c.String());
            AddColumn("dbo.Establishment", "ProprietorsPreferredJobTitle", c => c.String());
            AddColumn("dbo.Establishment", "ChairOfProprietorsBodyName", c => c.String());
            AddColumn("dbo.Establishment", "ChairOfProprietorsBodyStreet", c => c.String());
            AddColumn("dbo.Establishment", "ChairOfProprietorsBodyLocality", c => c.String());
            AddColumn("dbo.Establishment", "ChairOfProprietorsBodyAddress3", c => c.String());
            AddColumn("dbo.Establishment", "ChairOfProprietorsBodyTown", c => c.String());
            AddColumn("dbo.Establishment", "ChairOfProprietorsBodyCounty", c => c.String());
            AddColumn("dbo.Establishment", "ChairOfProprietorsBodyPostcode", c => c.String());
            AddColumn("dbo.Establishment", "ChairOfProprietorsBodyTelephoneNumber", c => c.String());
            AddColumn("dbo.Establishment", "ChairOfProprietorsBodyFaxNumber", c => c.String());
            AddColumn("dbo.Establishment", "ChairOfProprietorsBodyEmail", c => c.String());
            AddColumn("dbo.Establishment", "ChairOfProprietorsBodyPreferredJobTitle", c => c.String());
            AddColumn("dbo.Establishment", "AccommodationChangedId", c => c.Int());
            CreateIndex("dbo.Establishment", "IndependentSchoolTypeId");
            CreateIndex("dbo.Establishment", "AccommodationChangedId");
            AddForeignKey("dbo.Establishment", "AccommodationChangedId", "dbo.LookupAccommodationChanged", "Id");
            AddForeignKey("dbo.Establishment", "IndependentSchoolTypeId", "dbo.LookupIndependentSchoolType", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Establishment", "IndependentSchoolTypeId", "dbo.LookupIndependentSchoolType");
            DropForeignKey("dbo.Establishment", "AccommodationChangedId", "dbo.LookupAccommodationChanged");
            DropIndex("dbo.Establishment", new[] { "AccommodationChangedId" });
            DropIndex("dbo.Establishment", new[] { "IndependentSchoolTypeId" });
            DropColumn("dbo.Establishment", "AccommodationChangedId");
            DropColumn("dbo.Establishment", "ChairOfProprietorsBodyPreferredJobTitle");
            DropColumn("dbo.Establishment", "ChairOfProprietorsBodyEmail");
            DropColumn("dbo.Establishment", "ChairOfProprietorsBodyFaxNumber");
            DropColumn("dbo.Establishment", "ChairOfProprietorsBodyTelephoneNumber");
            DropColumn("dbo.Establishment", "ChairOfProprietorsBodyPostcode");
            DropColumn("dbo.Establishment", "ChairOfProprietorsBodyCounty");
            DropColumn("dbo.Establishment", "ChairOfProprietorsBodyTown");
            DropColumn("dbo.Establishment", "ChairOfProprietorsBodyAddress3");
            DropColumn("dbo.Establishment", "ChairOfProprietorsBodyLocality");
            DropColumn("dbo.Establishment", "ChairOfProprietorsBodyStreet");
            DropColumn("dbo.Establishment", "ChairOfProprietorsBodyName");
            DropColumn("dbo.Establishment", "ProprietorsPreferredJobTitle");
            DropColumn("dbo.Establishment", "ProprietorsEmail");
            DropColumn("dbo.Establishment", "ProprietorsFaxNumber");
            DropColumn("dbo.Establishment", "ProprietorsTelephoneNumber");
            DropColumn("dbo.Establishment", "ProprietorsPostcode");
            DropColumn("dbo.Establishment", "ProprietorsCounty");
            DropColumn("dbo.Establishment", "ProprietorsTown");
            DropColumn("dbo.Establishment", "ProprietorsAddress3");
            DropColumn("dbo.Establishment", "ProprietorsLocality");
            DropColumn("dbo.Establishment", "ProprietorsStreet");
            DropColumn("dbo.Establishment", "HighestAnnualRateForBoardingPupils");
            DropColumn("dbo.Establishment", "LowestAnnualRateForBoardingPupils");
            DropColumn("dbo.Establishment", "HighestAnnualRateForDayPupils");
            DropColumn("dbo.Establishment", "LowestAnnualRateForDayPupils");
            DropColumn("dbo.Establishment", "TotalNumberOfPartTimeStaff");
            DropColumn("dbo.Establishment", "TotalNumberOfFullTimeStaff");
            DropColumn("dbo.Establishment", "TotalNumberOfGirlsInBoardingSchools");
            DropColumn("dbo.Establishment", "PTGirlsAged4C");
            DropColumn("dbo.Establishment", "PTGirlsAged4B");
            DropColumn("dbo.Establishment", "PTGirlsAged4A");
            DropColumn("dbo.Establishment", "PTGirlsAged3");
            DropColumn("dbo.Establishment", "PTGirlsAged2AndUnder");
            DropColumn("dbo.Establishment", "TotalNumberOfBoysInBoardingSchools");
            DropColumn("dbo.Establishment", "PTBoysAged4C");
            DropColumn("dbo.Establishment", "PTBoysAged4B");
            DropColumn("dbo.Establishment", "PTBoysAged4A");
            DropColumn("dbo.Establishment", "PTBoysAged3");
            DropColumn("dbo.Establishment", "PTBoysAged2AndUnder");
            DropColumn("dbo.Establishment", "TotalNumberOfPupilsInPublicCare");
            DropColumn("dbo.Establishment", "NumberOfSpecialPupilsNotUnderASENStatementEHCP");
            DropColumn("dbo.Establishment", "NumberOfSpecialPupilsUnderASENStatementEHCP");
            DropColumn("dbo.Establishment", "TotalNumberOfPupilsOfCompulsorySchoolAge");
            DropColumn("dbo.Establishment", "TotalNumberOfPartTimePupils");
            DropColumn("dbo.Establishment", "TotalNumberOfFullTimePupils");
            DropColumn("dbo.Establishment", "CharityRegistrationNumber");
            DropColumn("dbo.Establishment", "CharityOrganisation");
            DropColumn("dbo.Establishment", "IndependentSchoolTypeId");
            DropColumn("dbo.Establishment", "NextActionRequiredByFP");
            DropColumn("dbo.Establishment", "NextActionRequiredByWEL");
            DropColumn("dbo.Establishment", "NextGeneralActionRequired");
            DropColumn("dbo.Establishment", "NextOfstedVisit");
            DropColumn("dbo.Establishment", "DateOfTheLastSISVisit");
            DropColumn("dbo.Establishment", "DateOfTheLastFPVisit");
            DropColumn("dbo.Establishment", "DateOfTheLastWelfareVisit");
            DropColumn("dbo.Establishment", "DateOfTheLastISIVisit");
            DropColumn("dbo.Establishment", "DateOfTheLastBridgeVisit");
            DropColumn("dbo.Establishment", "Notes");
        }
    }
}
