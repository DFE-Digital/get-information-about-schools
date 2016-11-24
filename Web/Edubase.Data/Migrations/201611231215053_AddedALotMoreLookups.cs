namespace Edubase.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedALotMoreLookups : DbMigration
    {
        public override void Up()
        {
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
            
            AddColumn("dbo.Establishment", "Contact_FaxNumber", c => c.String());
            AddColumn("dbo.Establishment", "ContactAlt_FaxNumber", c => c.String());
            AddColumn("dbo.Establishment", "Section41ApprovedId", c => c.Int());
            AddColumn("dbo.Establishment", "ProprietorName", c => c.String());
            AddColumn("dbo.Establishment", "SENStat", c => c.Int());
            AddColumn("dbo.Establishment", "SENNoStat", c => c.Int());
            AddColumn("dbo.Establishment", "SEN1Id", c => c.Int());
            AddColumn("dbo.Establishment", "SEN2Id", c => c.Int());
            AddColumn("dbo.Establishment", "SEN3Id", c => c.Int());
            AddColumn("dbo.Establishment", "SEN4Id", c => c.Int());
            AddColumn("dbo.Establishment", "TeenageMothersProvisionId", c => c.Int());
            AddColumn("dbo.Establishment", "TeenageMothersCapacity", c => c.Int());
            AddColumn("dbo.Establishment", "ChildcareFacilitiesId", c => c.Int());
            AddColumn("dbo.Establishment", "PRUSENId", c => c.Int());
            AddColumn("dbo.Establishment", "PRUEBDId", c => c.Int());
            AddColumn("dbo.Establishment", "PlacesPRU", c => c.Int());
            AddColumn("dbo.Establishment", "PruFulltimeProvisionId", c => c.Int());
            AddColumn("dbo.Establishment", "PruEducatedByOthersId", c => c.Int());
            AddColumn("dbo.Establishment", "TypeOfResourcedProvisionId", c => c.Int());
            AddColumn("dbo.Establishment", "ResourcedProvisionOnRoll", c => c.Int());
            AddColumn("dbo.Establishment", "ResourcedProvisionCapacity", c => c.Int());
            AddColumn("dbo.Establishment", "GovernmentOfficeRegionId", c => c.Int());
            AddColumn("dbo.Establishment", "AdministrativeDistrictId", c => c.Int());
            AddColumn("dbo.Establishment", "AdministrativeWardId", c => c.Int());
            AddColumn("dbo.Establishment", "ParliamentaryConstituencyId", c => c.Int());
            AddColumn("dbo.Establishment", "UrbanRuralId", c => c.Int());
            AddColumn("dbo.Establishment", "GSSLAId", c => c.Int());
            AddColumn("dbo.Establishment", "CASWardId", c => c.Int());
            AddColumn("dbo.Establishment", "MSOAId", c => c.Int());
            AddColumn("dbo.Establishment", "LSOAId", c => c.Int());
            CreateIndex("dbo.Establishment", "Section41ApprovedId");
            CreateIndex("dbo.Establishment", "SEN1Id");
            CreateIndex("dbo.Establishment", "SEN2Id");
            CreateIndex("dbo.Establishment", "SEN3Id");
            CreateIndex("dbo.Establishment", "SEN4Id");
            CreateIndex("dbo.Establishment", "TeenageMothersProvisionId");
            CreateIndex("dbo.Establishment", "ChildcareFacilitiesId");
            CreateIndex("dbo.Establishment", "PRUSENId");
            CreateIndex("dbo.Establishment", "PRUEBDId");
            CreateIndex("dbo.Establishment", "PruFulltimeProvisionId");
            CreateIndex("dbo.Establishment", "PruEducatedByOthersId");
            CreateIndex("dbo.Establishment", "TypeOfResourcedProvisionId");
            CreateIndex("dbo.Establishment", "GovernmentOfficeRegionId");
            CreateIndex("dbo.Establishment", "AdministrativeDistrictId");
            CreateIndex("dbo.Establishment", "AdministrativeWardId");
            CreateIndex("dbo.Establishment", "ParliamentaryConstituencyId");
            CreateIndex("dbo.Establishment", "UrbanRuralId");
            CreateIndex("dbo.Establishment", "GSSLAId");
            CreateIndex("dbo.Establishment", "CASWardId");
            CreateIndex("dbo.Establishment", "MSOAId");
            CreateIndex("dbo.Establishment", "LSOAId");
            AddForeignKey("dbo.Establishment", "AdministrativeDistrictId", "dbo.LookupDistrictAdministrative", "Id");
            AddForeignKey("dbo.Establishment", "AdministrativeWardId", "dbo.LookupAdministrativeWard", "Id");
            AddForeignKey("dbo.Establishment", "CASWardId", "dbo.LookupCASWard", "Id");
            AddForeignKey("dbo.Establishment", "ChildcareFacilitiesId", "dbo.LookupChildcareFacilities", "Id");
            AddForeignKey("dbo.Establishment", "GovernmentOfficeRegionId", "dbo.LookupGovernmentOfficeRegion", "Id");
            AddForeignKey("dbo.Establishment", "GSSLAId", "dbo.LookupGSSLA", "Id");
            AddForeignKey("dbo.Establishment", "LSOAId", "dbo.LookupLSOA", "Id");
            AddForeignKey("dbo.Establishment", "MSOAId", "dbo.LookupMSOA", "Id");
            AddForeignKey("dbo.Establishment", "ParliamentaryConstituencyId", "dbo.LookupParliamentaryConstituency", "Id");
            AddForeignKey("dbo.Establishment", "PRUEBDId", "dbo.LookupPRUEBD", "Id");
            AddForeignKey("dbo.Establishment", "PruEducatedByOthersId", "dbo.LookupPruEducatedByOthers", "Id");
            AddForeignKey("dbo.Establishment", "PruFulltimeProvisionId", "dbo.LookupPruFulltimeProvision", "Id");
            AddForeignKey("dbo.Establishment", "PRUSENId", "dbo.LookupPRUSEN", "Id");
            AddForeignKey("dbo.Establishment", "Section41ApprovedId", "dbo.LookupSection41Approved", "Id");
            AddForeignKey("dbo.Establishment", "SEN1Id", "dbo.LookupSpecialEducationNeeds", "Id");
            AddForeignKey("dbo.Establishment", "SEN2Id", "dbo.LookupSpecialEducationNeeds", "Id");
            AddForeignKey("dbo.Establishment", "SEN3Id", "dbo.LookupSpecialEducationNeeds", "Id");
            AddForeignKey("dbo.Establishment", "SEN4Id", "dbo.LookupSpecialEducationNeeds", "Id");
            AddForeignKey("dbo.Establishment", "TeenageMothersProvisionId", "dbo.LookupTeenageMothersProvision", "Id");
            AddForeignKey("dbo.Establishment", "TypeOfResourcedProvisionId", "dbo.LookupTypeOfResourcedProvision", "Id");
            AddForeignKey("dbo.Establishment", "UrbanRuralId", "dbo.LookupUrbanRural", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Establishment", "UrbanRuralId", "dbo.LookupUrbanRural");
            DropForeignKey("dbo.Establishment", "TypeOfResourcedProvisionId", "dbo.LookupTypeOfResourcedProvision");
            DropForeignKey("dbo.Establishment", "TeenageMothersProvisionId", "dbo.LookupTeenageMothersProvision");
            DropForeignKey("dbo.Establishment", "SEN4Id", "dbo.LookupSpecialEducationNeeds");
            DropForeignKey("dbo.Establishment", "SEN3Id", "dbo.LookupSpecialEducationNeeds");
            DropForeignKey("dbo.Establishment", "SEN2Id", "dbo.LookupSpecialEducationNeeds");
            DropForeignKey("dbo.Establishment", "SEN1Id", "dbo.LookupSpecialEducationNeeds");
            DropForeignKey("dbo.Establishment", "Section41ApprovedId", "dbo.LookupSection41Approved");
            DropForeignKey("dbo.Establishment", "PRUSENId", "dbo.LookupPRUSEN");
            DropForeignKey("dbo.Establishment", "PruFulltimeProvisionId", "dbo.LookupPruFulltimeProvision");
            DropForeignKey("dbo.Establishment", "PruEducatedByOthersId", "dbo.LookupPruEducatedByOthers");
            DropForeignKey("dbo.Establishment", "PRUEBDId", "dbo.LookupPRUEBD");
            DropForeignKey("dbo.Establishment", "ParliamentaryConstituencyId", "dbo.LookupParliamentaryConstituency");
            DropForeignKey("dbo.Establishment", "MSOAId", "dbo.LookupMSOA");
            DropForeignKey("dbo.Establishment", "LSOAId", "dbo.LookupLSOA");
            DropForeignKey("dbo.Establishment", "GSSLAId", "dbo.LookupGSSLA");
            DropForeignKey("dbo.Establishment", "GovernmentOfficeRegionId", "dbo.LookupGovernmentOfficeRegion");
            DropForeignKey("dbo.Establishment", "ChildcareFacilitiesId", "dbo.LookupChildcareFacilities");
            DropForeignKey("dbo.Establishment", "CASWardId", "dbo.LookupCASWard");
            DropForeignKey("dbo.Establishment", "AdministrativeWardId", "dbo.LookupAdministrativeWard");
            DropForeignKey("dbo.Establishment", "AdministrativeDistrictId", "dbo.LookupDistrictAdministrative");
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
            DropColumn("dbo.Establishment", "LSOAId");
            DropColumn("dbo.Establishment", "MSOAId");
            DropColumn("dbo.Establishment", "CASWardId");
            DropColumn("dbo.Establishment", "GSSLAId");
            DropColumn("dbo.Establishment", "UrbanRuralId");
            DropColumn("dbo.Establishment", "ParliamentaryConstituencyId");
            DropColumn("dbo.Establishment", "AdministrativeWardId");
            DropColumn("dbo.Establishment", "AdministrativeDistrictId");
            DropColumn("dbo.Establishment", "GovernmentOfficeRegionId");
            DropColumn("dbo.Establishment", "ResourcedProvisionCapacity");
            DropColumn("dbo.Establishment", "ResourcedProvisionOnRoll");
            DropColumn("dbo.Establishment", "TypeOfResourcedProvisionId");
            DropColumn("dbo.Establishment", "PruEducatedByOthersId");
            DropColumn("dbo.Establishment", "PruFulltimeProvisionId");
            DropColumn("dbo.Establishment", "PlacesPRU");
            DropColumn("dbo.Establishment", "PRUEBDId");
            DropColumn("dbo.Establishment", "PRUSENId");
            DropColumn("dbo.Establishment", "ChildcareFacilitiesId");
            DropColumn("dbo.Establishment", "TeenageMothersCapacity");
            DropColumn("dbo.Establishment", "TeenageMothersProvisionId");
            DropColumn("dbo.Establishment", "SEN4Id");
            DropColumn("dbo.Establishment", "SEN3Id");
            DropColumn("dbo.Establishment", "SEN2Id");
            DropColumn("dbo.Establishment", "SEN1Id");
            DropColumn("dbo.Establishment", "SENNoStat");
            DropColumn("dbo.Establishment", "SENStat");
            DropColumn("dbo.Establishment", "ProprietorName");
            DropColumn("dbo.Establishment", "Section41ApprovedId");
            DropColumn("dbo.Establishment", "ContactAlt_FaxNumber");
            DropColumn("dbo.Establishment", "Contact_FaxNumber");
            DropTable("dbo.LookupUrbanRural");
            DropTable("dbo.LookupParliamentaryConstituency");
            DropTable("dbo.LookupMSOA");
            DropTable("dbo.LookupLSOA");
            DropTable("dbo.LookupGSSLA");
            DropTable("dbo.LookupGovernmentOfficeRegion");
            DropTable("dbo.LookupCASWard");
            DropTable("dbo.LookupAdministrativeWard");
            DropTable("dbo.LookupDistrictAdministrative");
        }
    }
}
