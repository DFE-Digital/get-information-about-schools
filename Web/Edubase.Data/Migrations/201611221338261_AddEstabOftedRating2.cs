namespace Edubase.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddEstabOftedRating2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Establishment", "OfstedInspectionDate", c => c.DateTime(precision: 7, storeType: "datetime2"));
            DropColumn("dbo.Establishment", "OftedInspectionDate");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Establishment", "OftedInspectionDate", c => c.DateTime(precision: 7, storeType: "datetime2"));
            DropColumn("dbo.Establishment", "OfstedInspectionDate");
        }
    }
}
