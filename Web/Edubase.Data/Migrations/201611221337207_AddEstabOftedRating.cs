namespace Edubase.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddEstabOftedRating : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Establishment", "OfstedRating", c => c.Byte());
            AddColumn("dbo.Establishment", "OftedInspectionDate", c => c.DateTime(precision: 7, storeType: "datetime2"));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Establishment", "OftedInspectionDate");
            DropColumn("dbo.Establishment", "OfstedRating");
        }
    }
}
