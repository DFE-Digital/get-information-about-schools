namespace Edubase.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialiseDB : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AdmissionsPolicy",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        CreatedUtc = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        LastUpdatedUtc = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ProvisionBoarding",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        CreatedUtc = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        LastUpdatedUtc = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Company",
                c => new
                    {
                        GroupUID = c.Int(nullable: false, identity: true),
                        GroupId = c.String(),
                        Name = c.String(),
                        CompaniesHouseNumber = c.String(),
                        GroupTypeId = c.Int(),
                        ClosedDate = c.DateTime(precision: 7, storeType: "datetime2"),
                        GroupStatus = c.String(),
                        GroupStatusCode = c.String(),
                    })
                .PrimaryKey(t => t.GroupUID)
                .ForeignKey("dbo.GroupType", t => t.GroupTypeId)
                .Index(t => t.GroupTypeId);
            
            CreateTable(
                "dbo.GroupType",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        CreatedUtc = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        LastUpdatedUtc = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Diocese",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false),
                        CreatedUtc = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        LastUpdatedUtc = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.EducationPhase",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        CreatedUtc = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        LastUpdatedUtc = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ReasonEstablishmentClosed",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        CreatedUtc = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        LastUpdatedUtc = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ReasonEstablishmentOpened",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        CreatedUtc = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        LastUpdatedUtc = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Establishment",
                c => new
                    {
                        Urn = c.Int(nullable: false, identity: true),
                        LocalAuthorityId = c.Int(),
                        EstablishmentNumber = c.Int(),
                        Name = c.String(),
                        TypeId = c.Int(),
                        StatusId = c.Int(),
                        ReasonEstablishmentOpenedId = c.Int(),
                        ReasonEstablishmentClosedId = c.Int(),
                        OpenDate = c.DateTime(precision: 7, storeType: "datetime2"),
                        CloseDate = c.DateTime(precision: 7, storeType: "datetime2"),
                        EducationPhaseId = c.Int(),
                        StatutoryLowAge = c.Int(),
                        StatutoryHighAge = c.Int(),
                        ProvisionBoardingId = c.Int(),
                        ProvisionNurseryId = c.Int(),
                        ProvisionOfficialSixthFormId = c.Int(),
                        GenderId = c.Int(),
                        ReligiousCharacterId = c.Int(),
                        ReligiousEthosId = c.Int(),
                        DioceseId = c.String(maxLength: 128),
                        AdmissionsPolicyId = c.Int(),
                        Capacity = c.Int(),
                        ProvisionSpecialClassesId = c.Int(),
                        UKPRN = c.Int(),
                        LastChangedDate = c.DateTime(precision: 7, storeType: "datetime2"),
                        Address_Line1 = c.String(),
                        Address_Line2 = c.String(),
                        Address_Line3 = c.String(),
                        Address_CityOrTown = c.String(),
                        Address_County = c.String(),
                        Address_Country = c.String(),
                        Address_Locality = c.String(),
                        Address_PostCode = c.String(),
                        Address_Easting = c.String(),
                        Address_Northing = c.String(),
                        Address_Latitude = c.Double(nullable: false),
                        Address_Longitude = c.Double(nullable: false),
                        HeadFirstName = c.String(),
                        HeadLastName = c.String(),
                        HeadTitleId = c.Int(),
                        Contact_TelephoneNumber = c.String(),
                        Contact_EmailAddress = c.String(),
                        Contact_WebsiteAddress = c.String(),
                        ContactAlt_TelephoneNumber = c.String(),
                        ContactAlt_EmailAddress = c.String(),
                        ContactAlt_WebsiteAddress = c.String(),
                        CreatedUtc = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        LastUpdatedUtc = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        IsDeleted = c.Boolean(nullable: false),
                        EstablishmentType_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Urn)
                .ForeignKey("dbo.AdmissionsPolicy", t => t.AdmissionsPolicyId)
                .ForeignKey("dbo.Diocese", t => t.DioceseId)
                .ForeignKey("dbo.EducationPhase", t => t.EducationPhaseId)
                .ForeignKey("dbo.EstablishmentType", t => t.EstablishmentType_Id)
                .ForeignKey("dbo.Gender", t => t.GenderId)
                .ForeignKey("dbo.HeadTitle", t => t.HeadTitleId)
                .ForeignKey("dbo.LocalAuthority", t => t.LocalAuthorityId)
                .ForeignKey("dbo.ProvisionBoarding", t => t.ProvisionBoardingId)
                .ForeignKey("dbo.ProvisionNursery", t => t.ProvisionNurseryId)
                .ForeignKey("dbo.ProvisionOfficialSixthForm", t => t.ProvisionOfficialSixthFormId)
                .ForeignKey("dbo.ProvisionSpecialClasses", t => t.ProvisionSpecialClassesId)
                .ForeignKey("dbo.ReasonEstablishmentClosed", t => t.ReasonEstablishmentClosedId)
                .ForeignKey("dbo.ReasonEstablishmentOpened", t => t.ReasonEstablishmentOpenedId)
                .ForeignKey("dbo.ReligiousCharacter", t => t.ReligiousCharacterId)
                .ForeignKey("dbo.ReligiousEthos", t => t.ReligiousEthosId)
                .ForeignKey("dbo.EstablishmentStatus", t => t.StatusId)
                .Index(t => t.LocalAuthorityId)
                .Index(t => t.StatusId)
                .Index(t => t.ReasonEstablishmentOpenedId)
                .Index(t => t.ReasonEstablishmentClosedId)
                .Index(t => t.EducationPhaseId)
                .Index(t => t.ProvisionBoardingId)
                .Index(t => t.ProvisionNurseryId)
                .Index(t => t.ProvisionOfficialSixthFormId)
                .Index(t => t.GenderId)
                .Index(t => t.ReligiousCharacterId)
                .Index(t => t.ReligiousEthosId)
                .Index(t => t.DioceseId)
                .Index(t => t.AdmissionsPolicyId)
                .Index(t => t.ProvisionSpecialClassesId)
                .Index(t => t.HeadTitleId)
                .Index(t => t.EstablishmentType_Id);
            
            CreateTable(
                "dbo.EstablishmentType",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        CreatedUtc = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        LastUpdatedUtc = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Gender",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        CreatedUtc = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        LastUpdatedUtc = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.HeadTitle",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        CreatedUtc = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        LastUpdatedUtc = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.LocalAuthority",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Group = c.String(),
                        Order = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ProvisionNursery",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        CreatedUtc = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        LastUpdatedUtc = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ProvisionOfficialSixthForm",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        CreatedUtc = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        LastUpdatedUtc = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ProvisionSpecialClasses",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        CreatedUtc = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        LastUpdatedUtc = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ReligiousCharacter",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        CreatedUtc = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        LastUpdatedUtc = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ReligiousEthos",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        CreatedUtc = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        LastUpdatedUtc = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.EstablishmentStatus",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        CreatedUtc = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        LastUpdatedUtc = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.MAT",
                c => new
                    {
                        GroupUID = c.Short(nullable: false, identity: true),
                        GroupID = c.String(),
                        GroupName = c.String(),
                        CompaniesHouseNumber = c.String(),
                        GroupTypeCode = c.String(),
                        GroupType = c.String(),
                        ClosedDate = c.String(),
                        GroupStatusCode = c.String(),
                        GroupStatus = c.String(),
                    })
                .PrimaryKey(t => t.GroupUID);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.SchoolMAT",
                c => new
                    {
                        URN = c.Int(nullable: false, identity: true),
                        LinkedUID = c.Short(nullable: false),
                        JoinedDate = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                    })
                .PrimaryKey(t => t.URN)
                .ForeignKey("dbo.MAT", t => t.LinkedUID, cascadeDelete: true)
                .Index(t => t.LinkedUID);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(precision: 7, storeType: "datetime2"),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.SchoolMAT", "LinkedUID", "dbo.MAT");
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.Establishment", "StatusId", "dbo.EstablishmentStatus");
            DropForeignKey("dbo.Establishment", "ReligiousEthosId", "dbo.ReligiousEthos");
            DropForeignKey("dbo.Establishment", "ReligiousCharacterId", "dbo.ReligiousCharacter");
            DropForeignKey("dbo.Establishment", "ReasonEstablishmentOpenedId", "dbo.ReasonEstablishmentOpened");
            DropForeignKey("dbo.Establishment", "ReasonEstablishmentClosedId", "dbo.ReasonEstablishmentClosed");
            DropForeignKey("dbo.Establishment", "ProvisionSpecialClassesId", "dbo.ProvisionSpecialClasses");
            DropForeignKey("dbo.Establishment", "ProvisionOfficialSixthFormId", "dbo.ProvisionOfficialSixthForm");
            DropForeignKey("dbo.Establishment", "ProvisionNurseryId", "dbo.ProvisionNursery");
            DropForeignKey("dbo.Establishment", "ProvisionBoardingId", "dbo.ProvisionBoarding");
            DropForeignKey("dbo.Establishment", "LocalAuthorityId", "dbo.LocalAuthority");
            DropForeignKey("dbo.Establishment", "HeadTitleId", "dbo.HeadTitle");
            DropForeignKey("dbo.Establishment", "GenderId", "dbo.Gender");
            DropForeignKey("dbo.Establishment", "EstablishmentType_Id", "dbo.EstablishmentType");
            DropForeignKey("dbo.Establishment", "EducationPhaseId", "dbo.EducationPhase");
            DropForeignKey("dbo.Establishment", "DioceseId", "dbo.Diocese");
            DropForeignKey("dbo.Establishment", "AdmissionsPolicyId", "dbo.AdmissionsPolicy");
            DropForeignKey("dbo.Company", "GroupTypeId", "dbo.GroupType");
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.SchoolMAT", new[] { "LinkedUID" });
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.Establishment", new[] { "EstablishmentType_Id" });
            DropIndex("dbo.Establishment", new[] { "HeadTitleId" });
            DropIndex("dbo.Establishment", new[] { "ProvisionSpecialClassesId" });
            DropIndex("dbo.Establishment", new[] { "AdmissionsPolicyId" });
            DropIndex("dbo.Establishment", new[] { "DioceseId" });
            DropIndex("dbo.Establishment", new[] { "ReligiousEthosId" });
            DropIndex("dbo.Establishment", new[] { "ReligiousCharacterId" });
            DropIndex("dbo.Establishment", new[] { "GenderId" });
            DropIndex("dbo.Establishment", new[] { "ProvisionOfficialSixthFormId" });
            DropIndex("dbo.Establishment", new[] { "ProvisionNurseryId" });
            DropIndex("dbo.Establishment", new[] { "ProvisionBoardingId" });
            DropIndex("dbo.Establishment", new[] { "EducationPhaseId" });
            DropIndex("dbo.Establishment", new[] { "ReasonEstablishmentClosedId" });
            DropIndex("dbo.Establishment", new[] { "ReasonEstablishmentOpenedId" });
            DropIndex("dbo.Establishment", new[] { "StatusId" });
            DropIndex("dbo.Establishment", new[] { "LocalAuthorityId" });
            DropIndex("dbo.Company", new[] { "GroupTypeId" });
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.SchoolMAT");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.MAT");
            DropTable("dbo.EstablishmentStatus");
            DropTable("dbo.ReligiousEthos");
            DropTable("dbo.ReligiousCharacter");
            DropTable("dbo.ProvisionSpecialClasses");
            DropTable("dbo.ProvisionOfficialSixthForm");
            DropTable("dbo.ProvisionNursery");
            DropTable("dbo.LocalAuthority");
            DropTable("dbo.HeadTitle");
            DropTable("dbo.Gender");
            DropTable("dbo.EstablishmentType");
            DropTable("dbo.Establishment");
            DropTable("dbo.ReasonEstablishmentOpened");
            DropTable("dbo.ReasonEstablishmentClosed");
            DropTable("dbo.EducationPhase");
            DropTable("dbo.Diocese");
            DropTable("dbo.GroupType");
            DropTable("dbo.Company");
            DropTable("dbo.ProvisionBoarding");
            DropTable("dbo.AdmissionsPolicy");
        }
    }
}
