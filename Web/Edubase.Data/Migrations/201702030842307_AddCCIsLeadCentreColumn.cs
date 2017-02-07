namespace Edubase.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddCCIsLeadCentreColumn : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.EstablishmentGroup", "CCIsLeadCentre", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.EstablishmentGroup", "CCIsLeadCentre");
        }
    }
}
