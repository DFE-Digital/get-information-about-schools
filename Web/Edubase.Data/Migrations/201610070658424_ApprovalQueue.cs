namespace Edubase.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ApprovalQueue : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.EstablishmentApprovalQueue",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Urn = c.Int(nullable: false),
                        Name = c.String(),
                        Value = c.String(),
                        IsApproved = c.Boolean(nullable: false),
                        OriginatorUserId = c.String(maxLength: 128),
                        ApprovalDate = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        ApproverUserId = c.String(maxLength: 128),
                        CreatedUtc = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        LastUpdatedUtc = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.ApproverUserId)
                .ForeignKey("dbo.Establishment", t => t.Urn, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.OriginatorUserId)
                .Index(t => t.Urn)
                .Index(t => t.OriginatorUserId)
                .Index(t => t.ApproverUserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.EstablishmentApprovalQueue", "OriginatorUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.EstablishmentApprovalQueue", "Urn", "dbo.Establishment");
            DropForeignKey("dbo.EstablishmentApprovalQueue", "ApproverUserId", "dbo.AspNetUsers");
            DropIndex("dbo.EstablishmentApprovalQueue", new[] { "ApproverUserId" });
            DropIndex("dbo.EstablishmentApprovalQueue", new[] { "OriginatorUserId" });
            DropIndex("dbo.EstablishmentApprovalQueue", new[] { "Urn" });
            DropTable("dbo.EstablishmentApprovalQueue");
        }
    }
}
