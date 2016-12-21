namespace Edubase.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EstabAdditionalAddresses : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Establishment", "AdditionalAddresses", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Establishment", "AdditionalAddresses");
        }
    }
}
