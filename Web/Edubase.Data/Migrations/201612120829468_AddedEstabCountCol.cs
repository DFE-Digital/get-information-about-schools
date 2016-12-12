namespace Edubase.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedEstabCountCol : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.GroupCollection", "EstablishmentCount", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.GroupCollection", "EstablishmentCount");
        }
    }
}
