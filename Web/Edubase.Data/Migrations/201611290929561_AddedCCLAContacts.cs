namespace Edubase.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedCCLAContacts : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.LocalAuthority", "FirstName", c => c.String());
            AddColumn("dbo.LocalAuthority", "LastName", c => c.String());
            AddColumn("dbo.LocalAuthority", "EmailAddress", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.LocalAuthority", "EmailAddress");
            DropColumn("dbo.LocalAuthority", "LastName");
            DropColumn("dbo.LocalAuthority", "FirstName");
        }
    }
}
