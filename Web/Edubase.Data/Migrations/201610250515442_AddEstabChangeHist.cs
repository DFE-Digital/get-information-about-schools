namespace Edubase.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddEstabChangeHist : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.EstablishmentChangeHistory",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Urn = c.Int(nullable: false),
                        Name = c.String(),
                        NewValue = c.String(),
                        OriginatorUserId = c.String(maxLength: 128),
                        ApproverUserId = c.String(maxLength: 128),
                        OldValue = c.String(),
                        EffectiveDateUtc = c.DateTime(precision: 7, storeType: "datetime2"),
                        RequestedDateUtc = c.DateTime(precision: 7, storeType: "datetime2"),
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
            DropForeignKey("dbo.EstablishmentChangeHistory", "OriginatorUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.EstablishmentChangeHistory", "Urn", "dbo.Establishment");
            DropForeignKey("dbo.EstablishmentChangeHistory", "ApproverUserId", "dbo.AspNetUsers");
            DropIndex("dbo.EstablishmentChangeHistory", new[] { "ApproverUserId" });
            DropIndex("dbo.EstablishmentChangeHistory", new[] { "OriginatorUserId" });
            DropIndex("dbo.EstablishmentChangeHistory", new[] { "Urn" });
            DropTable("dbo.EstablishmentChangeHistory");
        }
    }
}
