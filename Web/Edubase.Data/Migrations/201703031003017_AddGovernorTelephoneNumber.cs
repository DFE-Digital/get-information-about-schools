namespace Edubase.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddGovernorTelephoneNumber : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Governor", "TelephoneNumber", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Governor", "TelephoneNumber");
        }
    }
}
