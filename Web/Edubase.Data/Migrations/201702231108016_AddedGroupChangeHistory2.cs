namespace Edubase.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedGroupChangeHistory2 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.GroupChangeHistory",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        GroupUId = c.Int(nullable: false),
                        Name = c.String(),
                        NewValue = c.String(),
                        OriginatorUserId = c.String(maxLength: 128),
                        ApproverUserId = c.String(maxLength: 128),
                        OldValue = c.String(),
                        Reason = c.String(),
                        EffectiveDateUtc = c.DateTime(precision: 7, storeType: "datetime2"),
                        RequestedDateUtc = c.DateTime(precision: 7, storeType: "datetime2"),
                        CreatedUtc = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        LastUpdatedUtc = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        IsDeleted = c.Boolean(nullable: false),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.ApproverUserId)
                .ForeignKey("dbo.GroupCollection", t => t.GroupUId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.OriginatorUserId)
                .Index(t => t.GroupUId)
                .Index(t => t.OriginatorUserId)
                .Index(t => t.ApproverUserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.GroupChangeHistory", "OriginatorUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.GroupChangeHistory", "GroupUId", "dbo.GroupCollection");
            DropForeignKey("dbo.GroupChangeHistory", "ApproverUserId", "dbo.AspNetUsers");
            DropIndex("dbo.GroupChangeHistory", new[] { "ApproverUserId" });
            DropIndex("dbo.GroupChangeHistory", new[] { "OriginatorUserId" });
            DropIndex("dbo.GroupChangeHistory", new[] { "GroupUId" });
            DropTable("dbo.GroupChangeHistory");
        }
    }
}
