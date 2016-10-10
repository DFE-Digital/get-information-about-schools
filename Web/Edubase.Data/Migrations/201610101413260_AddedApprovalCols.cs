namespace Edubase.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedApprovalCols : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.EstablishmentApprovalQueue", "NewValue", c => c.String());
            AddColumn("dbo.EstablishmentApprovalQueue", "OldValue", c => c.String());
            AlterColumn("dbo.EstablishmentApprovalQueue", "ApprovalDate", c => c.DateTime(precision: 7, storeType: "datetime2"));
            DropColumn("dbo.EstablishmentApprovalQueue", "Value");
        }
        
        public override void Down()
        {
            AddColumn("dbo.EstablishmentApprovalQueue", "Value", c => c.String());
            AlterColumn("dbo.EstablishmentApprovalQueue", "ApprovalDate", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            DropColumn("dbo.EstablishmentApprovalQueue", "OldValue");
            DropColumn("dbo.EstablishmentApprovalQueue", "NewValue");
        }
    }
}
