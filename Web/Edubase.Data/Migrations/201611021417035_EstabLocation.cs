namespace Edubase.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    using System.Data.Entity.Spatial;
    
    public partial class EstabLocation : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Establishment", "Easting", c => c.Int());
            AddColumn("dbo.Establishment", "Northing", c => c.Int());
            AddColumn("dbo.Establishment", "Location", c => c.Geography());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Establishment", "Location");
            DropColumn("dbo.Establishment", "Northing");
            DropColumn("dbo.Establishment", "Easting");
        }
    }
}
