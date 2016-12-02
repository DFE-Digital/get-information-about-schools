namespace Edubase.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
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
                        Contact_FaxNumber = c.String(),
                        ContactAlt_TelephoneNumber = c.String(),
                        ContactAlt_EmailAddress = c.String(),
                        ContactAlt_WebsiteAddress = c.String(),
                        ContactAlt_FaxNumber = c.String(),
                        TypeId = c.Int(),
                        Easting = c.Int(),
                        Northing = c.Int(),
                        Location = c.Geography(),
                        EstablishmentTypeGroupId = c.Int(),
                        OfstedRating = c.Byte(),
                        OfstedInspectionDate = c.DateTime(precision: 7, storeType: "datetime2"),
                        InspectorateId = c.Int(),
                        Section41ApprovedId = c.Int(),
                        ProprietorName = c.String(),
                        SENStat = c.Int(),
                        SENNoStat = c.Int(),
                        SEN1Id = c.Int(),
                        SEN2Id = c.Int(),
                        SEN3Id = c.Int(),
                        SEN4Id = c.Int(),
                        TeenageMothersProvisionId = c.Int(),
                        TeenageMothersCapacity = c.Int(),
                        ChildcareFacilitiesId = c.Int(),
                        PRUSENId = c.Int(),
                        PRUEBDId = c.Int(),
                        PlacesPRU = c.Int(),
                        PruFulltimeProvisionId = c.Int(),
                        PruEducatedByOthersId = c.Int(),
                        TypeOfResourcedProvisionId = c.Int(),
                        ResourcedProvisionOnRoll = c.Int(),
                        ResourcedProvisionCapacity = c.Int(),
                        GovernmentOfficeRegionId = c.Int(),
                        AdministrativeDistrictId = c.Int(),
                        AdministrativeWardId = c.Int(),
                        ParliamentaryConstituencyId = c.Int(),
                        UrbanRuralId = c.Int(),
                        GSSLAId = c.Int(),
                        CASWardId = c.Int(),
                        MSOAId = c.Int(),
                        LSOAId = c.Int(),
                        FurtherEducationTypeId = c.Int(),
                        SenUnitOnRoll = c.Int(),
                        SenUnitCapacity = c.Int(),
                        RSCRegionId = c.Int(),
                        BSOInspectorateId = c.Int(),
                        BSOInspectorateReportUrl = c.String(),
                        BSODateOfLastInspectionVisit = c.DateTime(precision: 7, storeType: "datetime2"),
                        BSODateOfNextInspectionVisit = c.DateTime(precision: 7, storeType: "datetime2"),
                        CreatedUtc = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        LastUpdatedUtc = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        IsDeleted = c.Boolean(nullable: false),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Urn)
                .ForeignKey("dbo.LookupDistrictAdministrative", t => t.AdministrativeDistrictId)
                .ForeignKey("dbo.LookupAdministrativeWard", t => t.AdministrativeWardId)
                .ForeignKey("dbo.LookupAdmissionsPolicy", t => t.AdmissionsPolicyId)
                .ForeignKey("dbo.LookupInspectorateName", t => t.BSOInspectorateId)
                .ForeignKey("dbo.LookupCASWard", t => t.CASWardId)
                .ForeignKey("dbo.LookupChildcareFacilities", t => t.ChildcareFacilitiesId)
                .ForeignKey("dbo.LookupDiocese", t => t.DioceseId)
                .ForeignKey("dbo.LookupEducationPhase", t => t.EducationPhaseId)
                .ForeignKey("dbo.LookupEstablishmentType", t => t.TypeId)
                .ForeignKey("dbo.LookupEstablishmentTypeGroup", t => t.EstablishmentTypeGroupId)
                .ForeignKey("dbo.LookupFurtherEducationType", t => t.FurtherEducationTypeId)
                .ForeignKey("dbo.LookupGender", t => t.GenderId)
                .ForeignKey("dbo.LookupGovernmentOfficeRegion", t => t.GovernmentOfficeRegionId)
                .ForeignKey("dbo.LookupGSSLA", t => t.GSSLAId)
                .ForeignKey("dbo.LookupHeadTitle", t => t.HeadTitleId)
                .ForeignKey("dbo.LookupInspectorate", t => t.InspectorateId)
                .ForeignKey("dbo.LocalAuthority", t => t.LocalAuthorityId)
                .ForeignKey("dbo.LookupLSOA", t => t.LSOAId)
                .ForeignKey("dbo.LookupMSOA", t => t.MSOAId)
                .ForeignKey("dbo.LookupParliamentaryConstituency", t => t.ParliamentaryConstituencyId)
                .ForeignKey("dbo.LookupProvisionBoarding", t => t.ProvisionBoardingId)
                .ForeignKey("dbo.LookupProvisionNursery", t => t.ProvisionNurseryId)
                .ForeignKey("dbo.LookupProvisionOfficialSixthForm", t => t.ProvisionOfficialSixthFormId)
                .ForeignKey("dbo.LookupProvisionSpecialClasses", t => t.ProvisionSpecialClassesId)
                .ForeignKey("dbo.LookupPRUEBD", t => t.PRUEBDId)
                .ForeignKey("dbo.LookupPruEducatedByOthers", t => t.PruEducatedByOthersId)
                .ForeignKey("dbo.LookupPruFulltimeProvision", t => t.PruFulltimeProvisionId)
                .ForeignKey("dbo.LookupPRUSEN", t => t.PRUSENId)
                .ForeignKey("dbo.LookupReasonEstablishmentClosed", t => t.ReasonEstablishmentClosedId)
                .ForeignKey("dbo.LookupReasonEstablishmentOpened", t => t.ReasonEstablishmentOpenedId)
                .ForeignKey("dbo.LookupReligiousCharacter", t => t.ReligiousCharacterId)
                .ForeignKey("dbo.LookupReligiousEthos", t => t.ReligiousEthosId)
                .ForeignKey("dbo.LocalAuthority", t => t.RSCRegionId)
                .ForeignKey("dbo.LookupSection41Approved", t => t.Section41ApprovedId)
                .ForeignKey("dbo.LookupSpecialEducationNeeds", t => t.SEN1Id)
                .ForeignKey("dbo.LookupSpecialEducationNeeds", t => t.SEN2Id)
                .ForeignKey("dbo.LookupSpecialEducationNeeds", t => t.SEN3Id)
                .ForeignKey("dbo.LookupSpecialEducationNeeds", t => t.SEN4Id)
                .ForeignKey("dbo.LookupEstablishmentStatus", t => t.StatusId)
                .ForeignKey("dbo.LookupTeenageMothersProvision", t => t.TeenageMothersProvisionId)
                .ForeignKey("dbo.LookupTypeOfResourcedProvision", t => t.TypeOfResourcedProvisionId)
                .ForeignKey("dbo.LookupUrbanRural", t => t.UrbanRuralId)
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
                .Index(t => t.TypeId)
                .Index(t => t.EstablishmentTypeGroupId)
                .Index(t => t.InspectorateId)
                .Index(t => t.Section41ApprovedId)
                .Index(t => t.SEN1Id)
                .Index(t => t.SEN2Id)
                .Index(t => t.SEN3Id)
                .Index(t => t.SEN4Id)
                .Index(t => t.TeenageMothersProvisionId)
                .Index(t => t.ChildcareFacilitiesId)
                .Index(t => t.PRUSENId)
                .Index(t => t.PRUEBDId)
                .Index(t => t.PruFulltimeProvisionId)
                .Index(t => t.PruEducatedByOthersId)
                .Index(t => t.TypeOfResourcedProvisionId)
                .Index(t => t.GovernmentOfficeRegionId)
                .Index(t => t.AdministrativeDistrictId)
                .Index(t => t.AdministrativeWardId)
                .Index(t => t.ParliamentaryConstituencyId)
                .Index(t => t.UrbanRuralId)
                .Index(t => t.GSSLAId)
                .Index(t => t.CASWardId)
                .Index(t => t.MSOAId)
                .Index(t => t.LSOAId)
                .Index(t => t.FurtherEducationTypeId)
                .Index(t => t.RSCRegionId)
                .Index(t => t.BSOInspectorateId);
            
            CreateTable(
                "dbo.LookupDistrictAdministrative",
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
                "dbo.LookupAdministrativeWard",
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
                "dbo.LookupCASWard",
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
                "dbo.LookupEstablishmentTypeGroup",
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
                "dbo.LookupGovernmentOfficeRegion",
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
                "dbo.LookupGSSLA",
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
                "dbo.LookupLSOA",
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
                "dbo.LookupMSOA",
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
                "dbo.LookupParliamentaryConstituency",
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
                "dbo.LookupUrbanRural",
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
                "dbo.EstablishmentGroup",
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
                .ForeignKey("dbo.GroupCollection", t => t.TrustGroupUID, cascadeDelete: true)
                .Index(t => t.TrustGroupUID)
                .Index(t => t.EstablishmentUrn);
            
            CreateTable(
                "dbo.GroupCollection",
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
                        ManagerEmailAddress = c.String(),
                        GroupId = c.String(maxLength: 400),
                        CreatedUtc = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        LastUpdatedUtc = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        IsDeleted = c.Boolean(nullable: false),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.GroupUID)
                .ForeignKey("dbo.LookupGroupType", t => t.GroupTypeId)
                .ForeignKey("dbo.LookupGroupStatus", t => t.StatusId)
                .Index(t => t.GroupTypeId)
                .Index(t => t.StatusId)
                .Index(t => t.GroupId);
            
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
                "dbo.LookupGroupStatus",
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
                        EstablishmentUrn = c.Int(),
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
                        GroupUID = c.Int(),
                        CreatedUtc = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        LastUpdatedUtc = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        IsDeleted = c.Boolean(nullable: false),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.LookupGovernorAppointingBody", t => t.AppointingBodyId)
                .ForeignKey("dbo.Establishment", t => t.EstablishmentUrn)
                .ForeignKey("dbo.GroupCollection", t => t.GroupUID)
                .ForeignKey("dbo.LookupGovernorRole", t => t.RoleId)
                .Index(t => t.EstablishmentUrn)
                .Index(t => t.RoleId)
                .Index(t => t.AppointingBodyId)
                .Index(t => t.GroupUID);
            
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
            DropForeignKey("dbo.Governor", "GroupUID", "dbo.GroupCollection");
            DropForeignKey("dbo.Governor", "EstablishmentUrn", "dbo.Establishment");
            DropForeignKey("dbo.Governor", "AppointingBodyId", "dbo.LookupGovernorAppointingBody");
            DropForeignKey("dbo.EstablishmentGroup", "TrustGroupUID", "dbo.GroupCollection");
            DropForeignKey("dbo.GroupCollection", "StatusId", "dbo.LookupGroupStatus");
            DropForeignKey("dbo.GroupCollection", "GroupTypeId", "dbo.LookupGroupType");
            DropForeignKey("dbo.EstablishmentGroup", "EstablishmentUrn", "dbo.Establishment");
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
            DropForeignKey("dbo.Establishment", "UrbanRuralId", "dbo.LookupUrbanRural");
            DropForeignKey("dbo.Establishment", "TypeOfResourcedProvisionId", "dbo.LookupTypeOfResourcedProvision");
            DropForeignKey("dbo.Establishment", "TeenageMothersProvisionId", "dbo.LookupTeenageMothersProvision");
            DropForeignKey("dbo.Establishment", "StatusId", "dbo.LookupEstablishmentStatus");
            DropForeignKey("dbo.Establishment", "SEN4Id", "dbo.LookupSpecialEducationNeeds");
            DropForeignKey("dbo.Establishment", "SEN3Id", "dbo.LookupSpecialEducationNeeds");
            DropForeignKey("dbo.Establishment", "SEN2Id", "dbo.LookupSpecialEducationNeeds");
            DropForeignKey("dbo.Establishment", "SEN1Id", "dbo.LookupSpecialEducationNeeds");
            DropForeignKey("dbo.Establishment", "Section41ApprovedId", "dbo.LookupSection41Approved");
            DropForeignKey("dbo.Establishment", "RSCRegionId", "dbo.LocalAuthority");
            DropForeignKey("dbo.Establishment", "ReligiousEthosId", "dbo.LookupReligiousEthos");
            DropForeignKey("dbo.Establishment", "ReligiousCharacterId", "dbo.LookupReligiousCharacter");
            DropForeignKey("dbo.Establishment", "ReasonEstablishmentOpenedId", "dbo.LookupReasonEstablishmentOpened");
            DropForeignKey("dbo.Establishment", "ReasonEstablishmentClosedId", "dbo.LookupReasonEstablishmentClosed");
            DropForeignKey("dbo.Establishment", "PRUSENId", "dbo.LookupPRUSEN");
            DropForeignKey("dbo.Establishment", "PruFulltimeProvisionId", "dbo.LookupPruFulltimeProvision");
            DropForeignKey("dbo.Establishment", "PruEducatedByOthersId", "dbo.LookupPruEducatedByOthers");
            DropForeignKey("dbo.Establishment", "PRUEBDId", "dbo.LookupPRUEBD");
            DropForeignKey("dbo.Establishment", "ProvisionSpecialClassesId", "dbo.LookupProvisionSpecialClasses");
            DropForeignKey("dbo.Establishment", "ProvisionOfficialSixthFormId", "dbo.LookupProvisionOfficialSixthForm");
            DropForeignKey("dbo.Establishment", "ProvisionNurseryId", "dbo.LookupProvisionNursery");
            DropForeignKey("dbo.Establishment", "ProvisionBoardingId", "dbo.LookupProvisionBoarding");
            DropForeignKey("dbo.Establishment", "ParliamentaryConstituencyId", "dbo.LookupParliamentaryConstituency");
            DropForeignKey("dbo.Establishment", "MSOAId", "dbo.LookupMSOA");
            DropForeignKey("dbo.Establishment", "LSOAId", "dbo.LookupLSOA");
            DropForeignKey("dbo.Establishment", "LocalAuthorityId", "dbo.LocalAuthority");
            DropForeignKey("dbo.Establishment", "InspectorateId", "dbo.LookupInspectorate");
            DropForeignKey("dbo.Establishment", "HeadTitleId", "dbo.LookupHeadTitle");
            DropForeignKey("dbo.Establishment", "GSSLAId", "dbo.LookupGSSLA");
            DropForeignKey("dbo.Establishment", "GovernmentOfficeRegionId", "dbo.LookupGovernmentOfficeRegion");
            DropForeignKey("dbo.Establishment", "GenderId", "dbo.LookupGender");
            DropForeignKey("dbo.Establishment", "FurtherEducationTypeId", "dbo.LookupFurtherEducationType");
            DropForeignKey("dbo.Establishment", "EstablishmentTypeGroupId", "dbo.LookupEstablishmentTypeGroup");
            DropForeignKey("dbo.Establishment", "TypeId", "dbo.LookupEstablishmentType");
            DropForeignKey("dbo.Establishment", "EducationPhaseId", "dbo.LookupEducationPhase");
            DropForeignKey("dbo.Establishment", "DioceseId", "dbo.LookupDiocese");
            DropForeignKey("dbo.Establishment", "ChildcareFacilitiesId", "dbo.LookupChildcareFacilities");
            DropForeignKey("dbo.Establishment", "CASWardId", "dbo.LookupCASWard");
            DropForeignKey("dbo.Establishment", "BSOInspectorateId", "dbo.LookupInspectorateName");
            DropForeignKey("dbo.Establishment", "AdmissionsPolicyId", "dbo.LookupAdmissionsPolicy");
            DropForeignKey("dbo.Establishment", "AdministrativeWardId", "dbo.LookupAdministrativeWard");
            DropForeignKey("dbo.Establishment", "AdministrativeDistrictId", "dbo.LookupDistrictAdministrative");
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.Governor", new[] { "GroupUID" });
            DropIndex("dbo.Governor", new[] { "AppointingBodyId" });
            DropIndex("dbo.Governor", new[] { "RoleId" });
            DropIndex("dbo.Governor", new[] { "EstablishmentUrn" });
            DropIndex("dbo.GroupCollection", new[] { "GroupId" });
            DropIndex("dbo.GroupCollection", new[] { "StatusId" });
            DropIndex("dbo.GroupCollection", new[] { "GroupTypeId" });
            DropIndex("dbo.EstablishmentGroup", new[] { "EstablishmentUrn" });
            DropIndex("dbo.EstablishmentGroup", new[] { "TrustGroupUID" });
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
            DropIndex("dbo.Establishment", new[] { "BSOInspectorateId" });
            DropIndex("dbo.Establishment", new[] { "RSCRegionId" });
            DropIndex("dbo.Establishment", new[] { "FurtherEducationTypeId" });
            DropIndex("dbo.Establishment", new[] { "LSOAId" });
            DropIndex("dbo.Establishment", new[] { "MSOAId" });
            DropIndex("dbo.Establishment", new[] { "CASWardId" });
            DropIndex("dbo.Establishment", new[] { "GSSLAId" });
            DropIndex("dbo.Establishment", new[] { "UrbanRuralId" });
            DropIndex("dbo.Establishment", new[] { "ParliamentaryConstituencyId" });
            DropIndex("dbo.Establishment", new[] { "AdministrativeWardId" });
            DropIndex("dbo.Establishment", new[] { "AdministrativeDistrictId" });
            DropIndex("dbo.Establishment", new[] { "GovernmentOfficeRegionId" });
            DropIndex("dbo.Establishment", new[] { "TypeOfResourcedProvisionId" });
            DropIndex("dbo.Establishment", new[] { "PruEducatedByOthersId" });
            DropIndex("dbo.Establishment", new[] { "PruFulltimeProvisionId" });
            DropIndex("dbo.Establishment", new[] { "PRUEBDId" });
            DropIndex("dbo.Establishment", new[] { "PRUSENId" });
            DropIndex("dbo.Establishment", new[] { "ChildcareFacilitiesId" });
            DropIndex("dbo.Establishment", new[] { "TeenageMothersProvisionId" });
            DropIndex("dbo.Establishment", new[] { "SEN4Id" });
            DropIndex("dbo.Establishment", new[] { "SEN3Id" });
            DropIndex("dbo.Establishment", new[] { "SEN2Id" });
            DropIndex("dbo.Establishment", new[] { "SEN1Id" });
            DropIndex("dbo.Establishment", new[] { "Section41ApprovedId" });
            DropIndex("dbo.Establishment", new[] { "InspectorateId" });
            DropIndex("dbo.Establishment", new[] { "EstablishmentTypeGroupId" });
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
            DropTable("dbo.LookupResourcedProvision");
            DropTable("dbo.LookupNationality");
            DropTable("dbo.LookupLocalGovernors");
            DropTable("dbo.LookupIndependentSchoolType");
            DropTable("dbo.LookupDirectProvisionOfEarlyYears");
            DropTable("dbo.LookupCCPhaseType");
            DropTable("dbo.LookupCCOperationalHours");
            DropTable("dbo.LookupCCGovernance");
            DropTable("dbo.LookupBoardingEstablishment");
            DropTable("dbo.LookupAccommodationChanged");
            DropTable("dbo.LookupGovernorRole");
            DropTable("dbo.LookupGovernorAppointingBody");
            DropTable("dbo.Governor");
            DropTable("dbo.LookupGroupStatus");
            DropTable("dbo.LookupGroupType");
            DropTable("dbo.GroupCollection");
            DropTable("dbo.EstablishmentGroup");
            DropTable("dbo.LookupEstablishmentLinkType");
            DropTable("dbo.EstablishmentLink");
            DropTable("dbo.EstablishmentChangeHistory");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.LookupUrbanRural");
            DropTable("dbo.LookupTypeOfResourcedProvision");
            DropTable("dbo.LookupTeenageMothersProvision");
            DropTable("dbo.LookupEstablishmentStatus");
            DropTable("dbo.LookupSpecialEducationNeeds");
            DropTable("dbo.LookupSection41Approved");
            DropTable("dbo.LookupReligiousEthos");
            DropTable("dbo.LookupReligiousCharacter");
            DropTable("dbo.LookupReasonEstablishmentOpened");
            DropTable("dbo.LookupReasonEstablishmentClosed");
            DropTable("dbo.LookupPRUSEN");
            DropTable("dbo.LookupPruFulltimeProvision");
            DropTable("dbo.LookupPruEducatedByOthers");
            DropTable("dbo.LookupPRUEBD");
            DropTable("dbo.LookupProvisionSpecialClasses");
            DropTable("dbo.LookupProvisionOfficialSixthForm");
            DropTable("dbo.LookupProvisionNursery");
            DropTable("dbo.LookupProvisionBoarding");
            DropTable("dbo.LookupParliamentaryConstituency");
            DropTable("dbo.LookupMSOA");
            DropTable("dbo.LookupLSOA");
            DropTable("dbo.LocalAuthority");
            DropTable("dbo.LookupInspectorate");
            DropTable("dbo.LookupHeadTitle");
            DropTable("dbo.LookupGSSLA");
            DropTable("dbo.LookupGovernmentOfficeRegion");
            DropTable("dbo.LookupGender");
            DropTable("dbo.LookupFurtherEducationType");
            DropTable("dbo.LookupEstablishmentTypeGroup");
            DropTable("dbo.LookupEstablishmentType");
            DropTable("dbo.LookupEducationPhase");
            DropTable("dbo.LookupDiocese");
            DropTable("dbo.LookupChildcareFacilities");
            DropTable("dbo.LookupCASWard");
            DropTable("dbo.LookupInspectorateName");
            DropTable("dbo.LookupAdmissionsPolicy");
            DropTable("dbo.LookupAdministrativeWard");
            DropTable("dbo.LookupDistrictAdministrative");
            DropTable("dbo.Establishment");
            DropTable("dbo.EstablishmentApprovalQueue");
        }
    }
}
