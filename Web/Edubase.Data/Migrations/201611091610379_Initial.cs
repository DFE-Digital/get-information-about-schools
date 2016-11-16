namespace Edubase.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.EstablishmentApprovalQueue",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Urn = c.Int(nullable: false),
                        Name = c.String(),
                        NewValue = c.String(),
                        IsApproved = c.Boolean(nullable: false),
                        IsRejected = c.Boolean(nullable: false),
                        RejectionReason = c.String(),
                        OriginatorUserId = c.String(maxLength: 128),
                        ProcessorUserId = c.String(maxLength: 128),
                        OldValue = c.String(),
                        ProcessedDateUtc = c.DateTime(precision: 7, storeType: "datetime2"),
                        CreatedUtc = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        LastUpdatedUtc = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        IsDeleted = c.Boolean(nullable: false),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Establishment", t => t.Urn, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.OriginatorUserId)
                .ForeignKey("dbo.AspNetUsers", t => t.ProcessorUserId)
                .Index(t => t.Urn)
                .Index(t => t.OriginatorUserId)
                .Index(t => t.ProcessorUserId);
            
            CreateTable(
                "dbo.Establishment",
                c => new
                    {
                        Urn = c.Int(nullable: false, identity: true),
                        LocalAuthorityId = c.Int(),
                        EstablishmentNumber = c.Int(),
                        Name = c.String(),
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
                        DioceseId = c.Int(),
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
                        HeadFirstName = c.String(),
                        HeadLastName = c.String(),
                        HeadTitleId = c.Int(),
                        Contact_TelephoneNumber = c.String(),
                        Contact_EmailAddress = c.String(),
                        Contact_WebsiteAddress = c.String(),
                        ContactAlt_TelephoneNumber = c.String(),
                        ContactAlt_EmailAddress = c.String(),
                        ContactAlt_WebsiteAddress = c.String(),
                        TypeId = c.Int(),
                        Easting = c.Int(),
                        Northing = c.Int(),
                        Location = c.Geography(),
                        CreatedUtc = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        LastUpdatedUtc = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        IsDeleted = c.Boolean(nullable: false),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Urn)
                .ForeignKey("dbo.LookupAdmissionsPolicy", t => t.AdmissionsPolicyId)
                .ForeignKey("dbo.LookupDiocese", t => t.DioceseId)
                .ForeignKey("dbo.LookupEducationPhase", t => t.EducationPhaseId)
                .ForeignKey("dbo.LookupEstablishmentType", t => t.TypeId)
                .ForeignKey("dbo.LookupGender", t => t.GenderId)
                .ForeignKey("dbo.LookupHeadTitle", t => t.HeadTitleId)
                .ForeignKey("dbo.LocalAuthority", t => t.LocalAuthorityId)
                .ForeignKey("dbo.LookupProvisionBoarding", t => t.ProvisionBoardingId)
                .ForeignKey("dbo.LookupProvisionNursery", t => t.ProvisionNurseryId)
                .ForeignKey("dbo.LookupProvisionOfficialSixthForm", t => t.ProvisionOfficialSixthFormId)
                .ForeignKey("dbo.LookupProvisionSpecialClasses", t => t.ProvisionSpecialClassesId)
                .ForeignKey("dbo.LookupReasonEstablishmentClosed", t => t.ReasonEstablishmentClosedId)
                .ForeignKey("dbo.LookupReasonEstablishmentOpened", t => t.ReasonEstablishmentOpenedId)
                .ForeignKey("dbo.LookupReligiousCharacter", t => t.ReligiousCharacterId)
                .ForeignKey("dbo.LookupReligiousEthos", t => t.ReligiousEthosId)
                .ForeignKey("dbo.LookupEstablishmentStatus", t => t.StatusId)
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
                .Index(t => t.TypeId);
            
            CreateTable(
                "dbo.LookupAdmissionsPolicy",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        DisplayOrder = c.Short(),
                        Code = c.String(),
                        CreatedUtc = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        LastUpdatedUtc = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        IsDeleted = c.Boolean(nullable: false),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.LookupDiocese",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        DisplayOrder = c.Short(),
                        Code = c.String(),
                        CreatedUtc = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        LastUpdatedUtc = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        IsDeleted = c.Boolean(nullable: false),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.LookupEducationPhase",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        DisplayOrder = c.Short(),
                        Code = c.String(),
                        CreatedUtc = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        LastUpdatedUtc = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        IsDeleted = c.Boolean(nullable: false),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.LookupEstablishmentType",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        DisplayOrder = c.Short(),
                        Code = c.String(),
                        CreatedUtc = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        LastUpdatedUtc = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        IsDeleted = c.Boolean(nullable: false),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.LookupGender",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        DisplayOrder = c.Short(),
                        Code = c.String(),
                        CreatedUtc = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        LastUpdatedUtc = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        IsDeleted = c.Boolean(nullable: false),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.LookupHeadTitle",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        DisplayOrder = c.Short(),
                        Code = c.String(),
                        CreatedUtc = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        LastUpdatedUtc = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        IsDeleted = c.Boolean(nullable: false),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
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
                        CreatedUtc = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        LastUpdatedUtc = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        IsDeleted = c.Boolean(nullable: false),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.LookupProvisionBoarding",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        DisplayOrder = c.Short(),
                        Code = c.String(),
                        CreatedUtc = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        LastUpdatedUtc = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        IsDeleted = c.Boolean(nullable: false),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.LookupProvisionNursery",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        DisplayOrder = c.Short(),
                        Code = c.String(),
                        CreatedUtc = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        LastUpdatedUtc = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        IsDeleted = c.Boolean(nullable: false),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.LookupProvisionOfficialSixthForm",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        DisplayOrder = c.Short(),
                        Code = c.String(),
                        CreatedUtc = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        LastUpdatedUtc = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        IsDeleted = c.Boolean(nullable: false),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.LookupProvisionSpecialClasses",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        DisplayOrder = c.Short(),
                        Code = c.String(),
                        CreatedUtc = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        LastUpdatedUtc = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        IsDeleted = c.Boolean(nullable: false),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.LookupReasonEstablishmentClosed",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        DisplayOrder = c.Short(),
                        Code = c.String(),
                        CreatedUtc = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        LastUpdatedUtc = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        IsDeleted = c.Boolean(nullable: false),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.LookupReasonEstablishmentOpened",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        DisplayOrder = c.Short(),
                        Code = c.String(),
                        CreatedUtc = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        LastUpdatedUtc = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        IsDeleted = c.Boolean(nullable: false),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.LookupReligiousCharacter",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        DisplayOrder = c.Short(),
                        Code = c.String(),
                        CreatedUtc = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        LastUpdatedUtc = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        IsDeleted = c.Boolean(nullable: false),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.LookupReligiousEthos",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        DisplayOrder = c.Short(),
                        Code = c.String(),
                        CreatedUtc = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        LastUpdatedUtc = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        IsDeleted = c.Boolean(nullable: false),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.LookupEstablishmentStatus",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        DisplayOrder = c.Short(),
                        Code = c.String(),
                        CreatedUtc = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        LastUpdatedUtc = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        IsDeleted = c.Boolean(nullable: false),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id);
            
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
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.EstablishmentChangeHistory",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Urn = c.Int(nullable: false),
                        Name = c.String(),
                        NewValue = c.String(),
                        OriginatorUserId = c.String(maxLength: 128),
                        ApproverUserId = c.String(maxLength: 128),
                        OldValue = c.String(),
                        EffectiveDateUtc = c.DateTime(precision: 7, storeType: "datetime2"),
                        RequestedDateUtc = c.DateTime(precision: 7, storeType: "datetime2"),
                        CreatedUtc = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        LastUpdatedUtc = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        IsDeleted = c.Boolean(nullable: false),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.ApproverUserId)
                .ForeignKey("dbo.Establishment", t => t.Urn, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.OriginatorUserId)
                .Index(t => t.Urn)
                .Index(t => t.OriginatorUserId)
                .Index(t => t.ApproverUserId);
            
            CreateTable(
                "dbo.EstablishmentLink",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        EstablishmentUrn = c.Int(),
                        LinkedEstablishmentUrn = c.Int(),
                        LinkName = c.String(),
                        LinkTypeId = c.Int(),
                        LinkEstablishedDate = c.DateTime(precision: 7, storeType: "datetime2"),
                        CreatedUtc = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        LastUpdatedUtc = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        IsDeleted = c.Boolean(nullable: false),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Establishment", t => t.EstablishmentUrn)
                .ForeignKey("dbo.Establishment", t => t.LinkedEstablishmentUrn)
                .ForeignKey("dbo.LookupEstablishmentLinkType", t => t.LinkTypeId)
                .Index(t => t.EstablishmentUrn)
                .Index(t => t.LinkedEstablishmentUrn)
                .Index(t => t.LinkTypeId);
            
            CreateTable(
                "dbo.LookupEstablishmentLinkType",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        DisplayOrder = c.Short(),
                        Code = c.String(),
                        CreatedUtc = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        LastUpdatedUtc = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        IsDeleted = c.Boolean(nullable: false),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.EstablishmentTrust",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TrustGroupUID = c.Int(nullable: false),
                        EstablishmentUrn = c.Int(nullable: false),
                        JoinedDate = c.DateTime(precision: 7, storeType: "datetime2"),
                        CreatedUtc = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        LastUpdatedUtc = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        IsDeleted = c.Boolean(nullable: false),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Establishment", t => t.EstablishmentUrn, cascadeDelete: true)
                .ForeignKey("dbo.Trust", t => t.TrustGroupUID, cascadeDelete: true)
                .Index(t => t.TrustGroupUID)
                .Index(t => t.EstablishmentUrn);
            
            CreateTable(
                "dbo.Trust",
                c => new
                    {
                        GroupUID = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        CompaniesHouseNumber = c.String(),
                        GroupTypeId = c.Int(),
                        ClosedDate = c.DateTime(precision: 7, storeType: "datetime2"),
                        StatusId = c.Int(),
                        OpenDate = c.DateTime(precision: 7, storeType: "datetime2"),
                        Head_Title = c.String(),
                        Head_FirstName = c.String(),
                        Head_MiddleName = c.String(),
                        Head_LastName = c.String(),
                        Address = c.String(),
                        CreatedUtc = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        LastUpdatedUtc = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        IsDeleted = c.Boolean(nullable: false),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.GroupUID)
                .ForeignKey("dbo.LookupGroupType", t => t.GroupTypeId)
                .ForeignKey("dbo.LookupEstablishmentStatus", t => t.StatusId)
                .Index(t => t.GroupTypeId)
                .Index(t => t.StatusId);
            
            CreateTable(
                "dbo.LookupGroupType",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        DisplayOrder = c.Short(),
                        Code = c.String(),
                        CreatedUtc = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        LastUpdatedUtc = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        IsDeleted = c.Boolean(nullable: false),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Governor",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        EstablishmentUrn = c.Int(nullable: false),
                        Person_Title = c.String(),
                        Person_FirstName = c.String(),
                        Person_MiddleName = c.String(),
                        Person_LastName = c.String(),
                        PreviousPerson_Title = c.String(),
                        PreviousPerson_FirstName = c.String(),
                        PreviousPerson_MiddleName = c.String(),
                        PreviousPerson_LastName = c.String(),
                        AppointmentStartDate = c.DateTime(precision: 7, storeType: "datetime2"),
                        AppointmentEndDate = c.DateTime(precision: 7, storeType: "datetime2"),
                        RoleId = c.Int(),
                        AppointingBodyId = c.Int(),
                        EmailAddress = c.String(),
                        DOB = c.DateTime(precision: 7, storeType: "datetime2"),
                        Nationality = c.String(),
                        PostCode = c.String(),
                        UID = c.Int(),
                        CreatedUtc = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        LastUpdatedUtc = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        IsDeleted = c.Boolean(nullable: false),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.LookupGovernorAppointingBody", t => t.AppointingBodyId)
                .ForeignKey("dbo.Establishment", t => t.EstablishmentUrn, cascadeDelete: true)
                .ForeignKey("dbo.LookupGovernorRole", t => t.RoleId)
                .Index(t => t.EstablishmentUrn)
                .Index(t => t.RoleId)
                .Index(t => t.AppointingBodyId);
            
            CreateTable(
                "dbo.LookupGovernorAppointingBody",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        DisplayOrder = c.Short(),
                        Code = c.String(),
                        CreatedUtc = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        LastUpdatedUtc = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        IsDeleted = c.Boolean(nullable: false),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.LookupGovernorRole",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        DisplayOrder = c.Short(),
                        Code = c.String(),
                        CreatedUtc = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        LastUpdatedUtc = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        IsDeleted = c.Boolean(nullable: false),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.LookupAccommodationChanged",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        DisplayOrder = c.Short(),
                        Code = c.String(),
                        CreatedUtc = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        LastUpdatedUtc = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        IsDeleted = c.Boolean(nullable: false),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.LookupBoardingEstablishment",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        DisplayOrder = c.Short(),
                        Code = c.String(),
                        CreatedUtc = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        LastUpdatedUtc = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        IsDeleted = c.Boolean(nullable: false),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.LookupCCGovernance",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        DisplayOrder = c.Short(),
                        Code = c.String(),
                        CreatedUtc = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        LastUpdatedUtc = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        IsDeleted = c.Boolean(nullable: false),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.LookupCCOperationalHours",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        DisplayOrder = c.Short(),
                        Code = c.String(),
                        CreatedUtc = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        LastUpdatedUtc = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        IsDeleted = c.Boolean(nullable: false),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.LookupCCPhaseType",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        DisplayOrder = c.Short(),
                        Code = c.String(),
                        CreatedUtc = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        LastUpdatedUtc = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        IsDeleted = c.Boolean(nullable: false),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.LookupChildcareFacilities",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        DisplayOrder = c.Short(),
                        Code = c.String(),
                        CreatedUtc = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        LastUpdatedUtc = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        IsDeleted = c.Boolean(nullable: false),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.LookupDirectProvisionOfEarlyYears",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        DisplayOrder = c.Short(),
                        Code = c.String(),
                        CreatedUtc = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        LastUpdatedUtc = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        IsDeleted = c.Boolean(nullable: false),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.LookupFurtherEducationType",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        DisplayOrder = c.Short(),
                        Code = c.String(),
                        CreatedUtc = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        LastUpdatedUtc = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        IsDeleted = c.Boolean(nullable: false),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.LookupIndependentSchoolType",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        DisplayOrder = c.Short(),
                        Code = c.String(),
                        CreatedUtc = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        LastUpdatedUtc = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        IsDeleted = c.Boolean(nullable: false),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.LookupInspectorateName",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        DisplayOrder = c.Short(),
                        Code = c.String(),
                        CreatedUtc = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        LastUpdatedUtc = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        IsDeleted = c.Boolean(nullable: false),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.LookupInspectorate",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        DisplayOrder = c.Short(),
                        Code = c.String(),
                        CreatedUtc = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        LastUpdatedUtc = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        IsDeleted = c.Boolean(nullable: false),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.LookupLocalGovernors",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        DisplayOrder = c.Short(),
                        Code = c.String(),
                        CreatedUtc = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        LastUpdatedUtc = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        IsDeleted = c.Boolean(nullable: false),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.LookupNationality",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        DisplayOrder = c.Short(),
                        Code = c.String(),
                        CreatedUtc = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        LastUpdatedUtc = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        IsDeleted = c.Boolean(nullable: false),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.LookupPRUEBD",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        DisplayOrder = c.Short(),
                        Code = c.String(),
                        CreatedUtc = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        LastUpdatedUtc = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        IsDeleted = c.Boolean(nullable: false),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.LookupPruEducatedByOthers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        DisplayOrder = c.Short(),
                        Code = c.String(),
                        CreatedUtc = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        LastUpdatedUtc = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        IsDeleted = c.Boolean(nullable: false),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.LookupPruFulltimeProvision",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        DisplayOrder = c.Short(),
                        Code = c.String(),
                        CreatedUtc = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        LastUpdatedUtc = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        IsDeleted = c.Boolean(nullable: false),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.LookupPRUSEN",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        DisplayOrder = c.Short(),
                        Code = c.String(),
                        CreatedUtc = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        LastUpdatedUtc = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        IsDeleted = c.Boolean(nullable: false),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.LookupResourcedProvision",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        DisplayOrder = c.Short(),
                        Code = c.String(),
                        CreatedUtc = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        LastUpdatedUtc = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        IsDeleted = c.Boolean(nullable: false),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.LookupSection41Approved",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        DisplayOrder = c.Short(),
                        Code = c.String(),
                        CreatedUtc = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        LastUpdatedUtc = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        IsDeleted = c.Boolean(nullable: false),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.LookupSpecialEducationNeeds",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        DisplayOrder = c.Short(),
                        Code = c.String(),
                        CreatedUtc = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        LastUpdatedUtc = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        IsDeleted = c.Boolean(nullable: false),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.LookupTeenageMothersProvision",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        DisplayOrder = c.Short(),
                        Code = c.String(),
                        CreatedUtc = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        LastUpdatedUtc = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        IsDeleted = c.Boolean(nullable: false),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.LookupTypeOfResourcedProvision",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        DisplayOrder = c.Short(),
                        Code = c.String(),
                        CreatedUtc = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        LastUpdatedUtc = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        IsDeleted = c.Boolean(nullable: false),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.EstablishmentPermission",
                c => new
                    {
                        PropertyName = c.String(nullable: false, maxLength: 128),
                        RoleName = c.String(nullable: false, maxLength: 128),
                        AllowUpdate = c.Boolean(nullable: false),
                        AllowApproval = c.Boolean(nullable: false),
                        CreatedUtc = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        LastUpdatedUtc = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        IsDeleted = c.Boolean(nullable: false),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => new { t.PropertyName, t.RoleName });
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.Governor", "RoleId", "dbo.LookupGovernorRole");
            DropForeignKey("dbo.Governor", "EstablishmentUrn", "dbo.Establishment");
            DropForeignKey("dbo.Governor", "AppointingBodyId", "dbo.LookupGovernorAppointingBody");
            DropForeignKey("dbo.EstablishmentTrust", "TrustGroupUID", "dbo.Trust");
            DropForeignKey("dbo.Trust", "StatusId", "dbo.LookupEstablishmentStatus");
            DropForeignKey("dbo.Trust", "GroupTypeId", "dbo.LookupGroupType");
            DropForeignKey("dbo.EstablishmentTrust", "EstablishmentUrn", "dbo.Establishment");
            DropForeignKey("dbo.EstablishmentLink", "LinkTypeId", "dbo.LookupEstablishmentLinkType");
            DropForeignKey("dbo.EstablishmentLink", "LinkedEstablishmentUrn", "dbo.Establishment");
            DropForeignKey("dbo.EstablishmentLink", "EstablishmentUrn", "dbo.Establishment");
            DropForeignKey("dbo.EstablishmentChangeHistory", "OriginatorUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.EstablishmentChangeHistory", "Urn", "dbo.Establishment");
            DropForeignKey("dbo.EstablishmentChangeHistory", "ApproverUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.EstablishmentApprovalQueue", "ProcessorUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.EstablishmentApprovalQueue", "OriginatorUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.EstablishmentApprovalQueue", "Urn", "dbo.Establishment");
            DropForeignKey("dbo.Establishment", "StatusId", "dbo.LookupEstablishmentStatus");
            DropForeignKey("dbo.Establishment", "ReligiousEthosId", "dbo.LookupReligiousEthos");
            DropForeignKey("dbo.Establishment", "ReligiousCharacterId", "dbo.LookupReligiousCharacter");
            DropForeignKey("dbo.Establishment", "ReasonEstablishmentOpenedId", "dbo.LookupReasonEstablishmentOpened");
            DropForeignKey("dbo.Establishment", "ReasonEstablishmentClosedId", "dbo.LookupReasonEstablishmentClosed");
            DropForeignKey("dbo.Establishment", "ProvisionSpecialClassesId", "dbo.LookupProvisionSpecialClasses");
            DropForeignKey("dbo.Establishment", "ProvisionOfficialSixthFormId", "dbo.LookupProvisionOfficialSixthForm");
            DropForeignKey("dbo.Establishment", "ProvisionNurseryId", "dbo.LookupProvisionNursery");
            DropForeignKey("dbo.Establishment", "ProvisionBoardingId", "dbo.LookupProvisionBoarding");
            DropForeignKey("dbo.Establishment", "LocalAuthorityId", "dbo.LocalAuthority");
            DropForeignKey("dbo.Establishment", "HeadTitleId", "dbo.LookupHeadTitle");
            DropForeignKey("dbo.Establishment", "GenderId", "dbo.LookupGender");
            DropForeignKey("dbo.Establishment", "TypeId", "dbo.LookupEstablishmentType");
            DropForeignKey("dbo.Establishment", "EducationPhaseId", "dbo.LookupEducationPhase");
            DropForeignKey("dbo.Establishment", "DioceseId", "dbo.LookupDiocese");
            DropForeignKey("dbo.Establishment", "AdmissionsPolicyId", "dbo.LookupAdmissionsPolicy");
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.Governor", new[] { "AppointingBodyId" });
            DropIndex("dbo.Governor", new[] { "RoleId" });
            DropIndex("dbo.Governor", new[] { "EstablishmentUrn" });
            DropIndex("dbo.Trust", new[] { "StatusId" });
            DropIndex("dbo.Trust", new[] { "GroupTypeId" });
            DropIndex("dbo.EstablishmentTrust", new[] { "EstablishmentUrn" });
            DropIndex("dbo.EstablishmentTrust", new[] { "TrustGroupUID" });
            DropIndex("dbo.EstablishmentLink", new[] { "LinkTypeId" });
            DropIndex("dbo.EstablishmentLink", new[] { "LinkedEstablishmentUrn" });
            DropIndex("dbo.EstablishmentLink", new[] { "EstablishmentUrn" });
            DropIndex("dbo.EstablishmentChangeHistory", new[] { "ApproverUserId" });
            DropIndex("dbo.EstablishmentChangeHistory", new[] { "OriginatorUserId" });
            DropIndex("dbo.EstablishmentChangeHistory", new[] { "Urn" });
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.Establishment", new[] { "TypeId" });
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
            DropIndex("dbo.EstablishmentApprovalQueue", new[] { "ProcessorUserId" });
            DropIndex("dbo.EstablishmentApprovalQueue", new[] { "OriginatorUserId" });
            DropIndex("dbo.EstablishmentApprovalQueue", new[] { "Urn" });
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.EstablishmentPermission");
            DropTable("dbo.LookupTypeOfResourcedProvision");
            DropTable("dbo.LookupTeenageMothersProvision");
            DropTable("dbo.LookupSpecialEducationNeeds");
            DropTable("dbo.LookupSection41Approved");
            DropTable("dbo.LookupResourcedProvision");
            DropTable("dbo.LookupPRUSEN");
            DropTable("dbo.LookupPruFulltimeProvision");
            DropTable("dbo.LookupPruEducatedByOthers");
            DropTable("dbo.LookupPRUEBD");
            DropTable("dbo.LookupNationality");
            DropTable("dbo.LookupLocalGovernors");
            DropTable("dbo.LookupInspectorate");
            DropTable("dbo.LookupInspectorateName");
            DropTable("dbo.LookupIndependentSchoolType");
            DropTable("dbo.LookupFurtherEducationType");
            DropTable("dbo.LookupDirectProvisionOfEarlyYears");
            DropTable("dbo.LookupChildcareFacilities");
            DropTable("dbo.LookupCCPhaseType");
            DropTable("dbo.LookupCCOperationalHours");
            DropTable("dbo.LookupCCGovernance");
            DropTable("dbo.LookupBoardingEstablishment");
            DropTable("dbo.LookupAccommodationChanged");
            DropTable("dbo.LookupGovernorRole");
            DropTable("dbo.LookupGovernorAppointingBody");
            DropTable("dbo.Governor");
            DropTable("dbo.LookupGroupType");
            DropTable("dbo.Trust");
            DropTable("dbo.EstablishmentTrust");
            DropTable("dbo.LookupEstablishmentLinkType");
            DropTable("dbo.EstablishmentLink");
            DropTable("dbo.EstablishmentChangeHistory");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.LookupEstablishmentStatus");
            DropTable("dbo.LookupReligiousEthos");
            DropTable("dbo.LookupReligiousCharacter");
            DropTable("dbo.LookupReasonEstablishmentOpened");
            DropTable("dbo.LookupReasonEstablishmentClosed");
            DropTable("dbo.LookupProvisionSpecialClasses");
            DropTable("dbo.LookupProvisionOfficialSixthForm");
            DropTable("dbo.LookupProvisionNursery");
            DropTable("dbo.LookupProvisionBoarding");
            DropTable("dbo.LocalAuthority");
            DropTable("dbo.LookupHeadTitle");
            DropTable("dbo.LookupGender");
            DropTable("dbo.LookupEstablishmentType");
            DropTable("dbo.LookupEducationPhase");
            DropTable("dbo.LookupDiocese");
            DropTable("dbo.LookupAdmissionsPolicy");
            DropTable("dbo.Establishment");
            DropTable("dbo.EstablishmentApprovalQueue");
        }
    }
}
