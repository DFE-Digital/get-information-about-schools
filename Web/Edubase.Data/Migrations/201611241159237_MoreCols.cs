namespace Edubase.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MoreCols : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Establishment", "SenUnitOnRoll", c => c.Int());
            AddColumn("dbo.Establishment", "SenUnitCapacity", c => c.Int());
            AddColumn("dbo.Establishment", "RSCRegionId", c => c.Int());
            AddColumn("dbo.Establishment", "BSOInspectorateId", c => c.Int());
            AddColumn("dbo.Establishment", "BSOInspectorateReportUrl", c => c.String());
            AddColumn("dbo.Establishment", "BSODateOfLastInspectionVisit", c => c.DateTime(precision: 7, storeType: "datetime2"));
            AddColumn("dbo.Establishment", "BSODateOfNextInspectionVisit", c => c.DateTime(precision: 7, storeType: "datetime2"));
            CreateIndex("dbo.Establishment", "RSCRegionId");
            CreateIndex("dbo.Establishment", "BSOInspectorateId");
            AddForeignKey("dbo.Establishment", "BSOInspectorateId", "dbo.LookupInspectorateName", "Id");
            AddForeignKey("dbo.Establishment", "RSCRegionId", "dbo.LocalAuthority", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Establishment", "RSCRegionId", "dbo.LocalAuthority");
            DropForeignKey("dbo.Establishment", "BSOInspectorateId", "dbo.LookupInspectorateName");
            DropIndex("dbo.Establishment", new[] { "BSOInspectorateId" });
            DropIndex("dbo.Establishment", new[] { "RSCRegionId" });
            DropColumn("dbo.Establishment", "BSODateOfNextInspectionVisit");
            DropColumn("dbo.Establishment", "BSODateOfLastInspectionVisit");
            DropColumn("dbo.Establishment", "BSOInspectorateReportUrl");
            DropColumn("dbo.Establishment", "BSOInspectorateId");
            DropColumn("dbo.Establishment", "RSCRegionId");
            DropColumn("dbo.Establishment", "SenUnitCapacity");
            DropColumn("dbo.Establishment", "SenUnitOnRoll");
        }
    }
}
