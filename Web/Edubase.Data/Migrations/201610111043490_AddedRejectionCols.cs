namespace Edubase.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedRejectionCols : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.EstablishmentApprovalQueue", name: "ApproverUserId", newName: "ProcessorUserId");
            RenameIndex(table: "dbo.EstablishmentApprovalQueue", name: "IX_ApproverUserId", newName: "IX_ProcessorUserId");
            AddColumn("dbo.EstablishmentApprovalQueue", "IsRejected", c => c.Boolean(nullable: false));
            AddColumn("dbo.EstablishmentApprovalQueue", "RejectionReason", c => c.String());
            AddColumn("dbo.EstablishmentApprovalQueue", "ProcessedDateUtc", c => c.DateTime(precision: 7, storeType: "datetime2"));
            DropColumn("dbo.EstablishmentApprovalQueue", "ApprovalDate");
        }
        
        public override void Down()
        {
            AddColumn("dbo.EstablishmentApprovalQueue", "ApprovalDate", c => c.DateTime(precision: 7, storeType: "datetime2"));
            DropColumn("dbo.EstablishmentApprovalQueue", "ProcessedDateUtc");
            DropColumn("dbo.EstablishmentApprovalQueue", "RejectionReason");
            DropColumn("dbo.EstablishmentApprovalQueue", "IsRejected");
            RenameIndex(table: "dbo.EstablishmentApprovalQueue", name: "IX_ProcessorUserId", newName: "IX_ApproverUserId");
            RenameColumn(table: "dbo.EstablishmentApprovalQueue", name: "ProcessorUserId", newName: "ApproverUserId");
        }
    }
}
