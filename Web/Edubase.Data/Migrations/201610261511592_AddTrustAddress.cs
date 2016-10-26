namespace Edubase.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTrustAddress : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Company", "Address", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Company", "Address");
        }
    }
}
