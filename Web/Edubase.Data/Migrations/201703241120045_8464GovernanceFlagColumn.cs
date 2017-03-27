namespace Edubase.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _8464GovernanceFlagColumn : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Establishment", "GovernanceMode", c => c.Byte());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Establishment", "GovernanceMode");
        }
    }
}
